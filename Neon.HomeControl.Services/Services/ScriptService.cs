using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Neon.HomeControl.Api.Core.Attributes.ScriptService;
using Neon.HomeControl.Api.Core.Attributes.Services;
using Neon.HomeControl.Api.Core.Data.Config;
using Neon.HomeControl.Api.Core.Data.LuaScript;
using Neon.HomeControl.Api.Core.Interfaces.Managers;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Neon.HomeControl.Api.Core.Utils;
using Microsoft.Extensions.Logging;
using NLua;

namespace Neon.HomeControl.Services.Services
{
	[Service(typeof(IScriptService), Name = "LUA Script service", LoadAtStartup = true, Order = 100)]
	public class ScriptService : IScriptService
	{
		private readonly IFileSystemService _fileSystemService;
		private readonly FileSystemWatcher _fileSystemWatcher = new FileSystemWatcher();
		private readonly List<LuaFunction> _functions = new List<LuaFunction>();
		private readonly NeonConfig _neonConfig;
		private readonly ILogger _logger;
		private readonly Lua _luaEngine;
		private readonly IServicesManager _servicesManager;

		public ScriptService(ILogger<ScriptService> logger, NeonConfig neonConfig, IServicesManager servicesManager,
			IFileSystemService fileSystemService)
		{
			GlobalFunctions = new List<LuaScriptFunctionData>();
			_logger = logger;
			_neonConfig = neonConfig;
			_fileSystemService = fileSystemService;
			_servicesManager = servicesManager;
			_luaEngine = new Lua();
			_luaEngine.State.Encoding = Encoding.UTF8;

			_luaEngine.HookException += (sender, args) =>
			{
				_logger.LogError($"Error during execute LUA => {args.Exception}");
			};
		}

		public List<LuaScriptFunctionData> GlobalFunctions { get; set; }

		public Task<bool> Start()
		{
			_fileSystemService.CreateDirectory(_neonConfig.Scripts.Directory);
			StartMonitorDirectory();

			_logger.LogInformation("Initializing LUA script manager");
			_luaEngine.LoadCLRPackage();
			ScanForScriptClasses();

			_logger.LogInformation($"Scanning files in directory {_neonConfig.Scripts.Directory}");
			LoadLuaFiles();

			_functions.ForEach(f => { _logger.LogInformation($"{f.Call()}"); });

			_logger.LogInformation("LUA Script manager initialized");
			return Task.FromResult(true);
		}

		public Task<bool> Stop()
		{
			_luaEngine.Dispose();
			_fileSystemWatcher.Dispose();
			return Task.FromResult(true);
		}

		private void ScanForScriptClasses()
		{
			AssemblyUtils.ScanAllAssembliesFromAttribute(typeof(LuaScriptObjectAttribute)).ForEach(t =>
			{
				_logger.LogInformation($"Registering {t.Name} in LUA Objects");
				var obj = _servicesManager.Resolve(t);
				obj.GetType().GetMethods().ToList().ForEach(m =>
				{
					var scriptFuncAttr =
						(LuaScriptFunctionAttribute) m.GetCustomAttribute(typeof(LuaScriptFunctionAttribute));

					if (scriptFuncAttr == null) return;

					_logger.LogInformation(
						$"{obj.GetType().Name} - {scriptFuncAttr.FunctionName} [{scriptFuncAttr.Help}]");

					_luaEngine.RegisterFunction(scriptFuncAttr.FunctionName, obj,
						obj.GetType().GetMethod(m.Name));

					GlobalFunctions.Add(new LuaScriptFunctionData
					{
						Category = scriptFuncAttr.FunctionCategory,
						Help = scriptFuncAttr.Help,
						Name = scriptFuncAttr.FunctionName,
						Args = m.GetParameters().GetMethodParamStrings()
					});
				});
			});
		}

		private void StartMonitorDirectory()
		{
			_logger.LogInformation($"Monitoring directory {_neonConfig.Scripts.Directory}");
			_fileSystemWatcher.IncludeSubdirectories = true;
			_fileSystemWatcher.Path = _fileSystemService.BuildFilePath(_neonConfig.Scripts.Directory);
			_fileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess
			                                  | NotifyFilters.LastWrite
			                                  | NotifyFilters.FileName
			                                  | NotifyFilters.DirectoryName;

			_fileSystemWatcher.Created += (sender, args) => ProcessMonitorFile(args.FullPath);
			_fileSystemWatcher.Changed += (sender, args) => ProcessMonitorFile(args.FullPath);

			_fileSystemWatcher.EnableRaisingEvents = true;
		}

		private void ProcessMonitorFile(string filename)
		{
			if (File.GetAttributes(filename) == FileAttributes.Directory) return;

			if (Path.GetExtension(filename) != ".lua") return;

			_logger.LogInformation($"New File created: {filename}");
			LoadLuaFile(filename, true);
		}

		private void LoadLuaFiles()
		{
			var files = Directory.GetFiles(_fileSystemService.BuildFilePath(_neonConfig.Scripts.Directory), "*.lua",
					SearchOption.AllDirectories)
				.ToList();

			files.ForEach(f => LoadLuaFile(f, false));
		}

		public void LoadLuaFile(string filename, bool execute)
		{
			try
			{
				_logger.LogInformation($"Loading file {filename}...");
				var func = _luaEngine.LoadFile(filename);
				_functions.Add(func);
				if (execute)
					func.Call();
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error during load file {filename} => {ex}");
			}
		}
	}
}