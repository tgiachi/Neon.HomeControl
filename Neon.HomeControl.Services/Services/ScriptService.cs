using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.ScriptService;
using Neon.HomeControl.Api.Core.Attributes.Services;
using Neon.HomeControl.Api.Core.Data.Config;
using Neon.HomeControl.Api.Core.Data.LuaScript;
using Neon.HomeControl.Api.Core.Interfaces.Managers;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Neon.HomeControl.Api.Core.Utils;
using NLua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neon.HomeControl.Services.Services
{

	[Service(typeof(IScriptService), Name = "Universal Script service", LoadAtStartup = true, Order = 100)]
	public class ScriptService : IScriptService
	{
		private readonly IFileSystemManager _fileSystemManager;
		private readonly FileSystemWatcher _fileSystemWatcher = new FileSystemWatcher();
		private readonly List<LuaFunction> _functions = new List<LuaFunction>();
		private readonly NeonConfig _neonConfig;
		private readonly ILogger _logger;
		private readonly Lua _luaEngine;

		private readonly IServicesManager _servicesManager;

		public List<ScriptFunctionData> GlobalFunctions { get; set; }

		
		private string _bootstrapFile = "";

		public ScriptService(ILogger<ScriptService> logger, NeonConfig neonConfig, IServicesManager servicesManager,
			IFileSystemManager fileSystemManager)
		{
			GlobalFunctions = new List<ScriptFunctionData>();
			_logger = logger;
			_neonConfig = neonConfig;
			_fileSystemManager = fileSystemManager;
			_servicesManager = servicesManager;
			_luaEngine = new Lua();
			_luaEngine.State.Encoding = Encoding.UTF8;
	

			_luaEngine.HookException += (sender, args) =>
			{
				_logger.LogError($"Error during execute LUA =>\n {args.Exception.FlattenException()}");
			};
		}

		public Task<bool> Start()
		{
			_bootstrapFile = _fileSystemManager.BuildFilePath(_neonConfig.Scripts.Directory + Path.DirectorySeparatorChar + "bootstrap.lua");
			_fileSystemManager.CreateDirectory(_neonConfig.Scripts.Directory);
			CheckBootstrapFile();
			//StartMonitorDirectory();

			_logger.LogInformation("Initializing LUA script manager");
			_luaEngine.LoadCLRPackage();
			ScanForScriptClasses();

			_logger.LogInformation($"Loading bootstrap file");
			LoadLuaFile(_bootstrapFile, true);

			_logger.LogInformation($"Scanning files in directory {_neonConfig.Scripts.Directory}");
			LoadLuaFiles();

			_functions.ForEach(f => { _logger.LogInformation($"{f.Call()}"); });

			_logger.LogInformation("LUA Script manager initialized");
			return Task.FromResult(true);
		}

		private void CheckBootstrapFile()
		{
			if (!File.Exists(_bootstrapFile))
			{
				File.WriteAllText(_bootstrapFile, "");
			}

		}

		public Task<bool> Stop()
		{
			_luaEngine.Dispose();
			_fileSystemWatcher.Dispose();
			return Task.FromResult(true);
		}

		private void ScanForScriptClasses()
		{
			AssemblyUtils.ScanAllAssembliesFromAttribute(typeof(ScriptObjectAttribute)).ForEach(t =>
			{
				_logger.LogInformation($"Registering {t.Name} in LUA Objects");
				var obj = _servicesManager.Resolve(t);
				obj.GetType().GetMethods().ToList().ForEach(m =>
				{
					try

					{
						var scriptFuncAttr = m.GetCustomAttribute<ScriptFunctionAttribute>();


						if (scriptFuncAttr == null) return;

						_logger.LogInformation(
							$"{obj.GetType().Name} - {scriptFuncAttr.FunctionName} [{scriptFuncAttr.Help}]");

						_luaEngine.RegisterFunction(scriptFuncAttr.FunctionName, obj,
							obj.GetType().GetMethod(m.Name));
						

						
						GlobalFunctions.Add(new ScriptFunctionData
						{
							Category = scriptFuncAttr.FunctionCategory,
							Help = scriptFuncAttr.Help,
							Name = scriptFuncAttr.FunctionName,
							Args = m.GetParameters().GetMethodParamStrings()
						});
					}
					catch (Exception ex)
					{

					}

				});
			});
		}

		private void StartMonitorDirectory()
		{
			_logger.LogInformation($"Monitoring directory {_neonConfig.Scripts.Directory}");
			_fileSystemWatcher.IncludeSubdirectories = true;
			_fileSystemWatcher.Path = _fileSystemManager.BuildFilePath(_neonConfig.Scripts.Directory);
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
			var files = Directory.GetFiles(_fileSystemManager.BuildFilePath(_neonConfig.Scripts.Directory), "*.lua",
					SearchOption.AllDirectories)
				.ToList();

			files.ForEach(f =>
			{
				if (!f.ToLower().Contains("bootstrap.lua"))
					LoadLuaFile(f, false);
			});
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
				_logger.LogError($"Error during load file {filename} => {ex.FlattenException()}");
			}
		}
	}
}