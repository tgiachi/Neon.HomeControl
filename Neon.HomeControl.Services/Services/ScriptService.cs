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
using Neon.HomeControl.Api.Core.Attributes.ScriptEngine;
using Neon.HomeControl.Api.Core.Interfaces.ScriptEngine;

namespace Neon.HomeControl.Services.Services
{

	[Service(typeof(IScriptService), Name = "Universal Script service", LoadAtStartup = true, Order = 100)]
	public class ScriptService : IScriptService
	{
		private readonly IFileSystemManager _fileSystemManager;
		private readonly FileSystemWatcher _fileSystemWatcher = new FileSystemWatcher();
		
		private readonly Dictionary<string, Type> _scriptsEngines = new Dictionary<string, Type>();

		private readonly NeonConfig _neonConfig;
		private readonly ILogger _logger;
		
		private readonly IServicesManager _servicesManager;
		private IScriptEngine _scriptEngine;
		private ScriptEngineAttribute _scriptEngineAttribute;

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

			AssemblyUtils.ScanAllAssembliesFromAttribute(typeof(ScriptEngineAttribute)).ForEach(se =>
			{
				var attr = se.GetCustomAttribute<ScriptEngineAttribute>();
				_scriptsEngines.Add(attr.Name, se);

			});
			
		}

		public async Task<bool> Start()
		{
			if (_scriptsEngines.ContainsKey(_neonConfig.Scripts.EngineName))
			{
				_scriptEngine = (IScriptEngine)_servicesManager.Resolve(_scriptsEngines[_neonConfig.Scripts.EngineName]);
				_scriptEngineAttribute = _scriptEngine.GetType().GetCustomAttribute<ScriptEngineAttribute>();

				_logger.LogInformation($"Initializing script engine {_scriptEngineAttribute.Name} {_scriptEngineAttribute.Version}");

				_bootstrapFile = _fileSystemManager.BuildFilePath(_neonConfig.Scripts.Directory + Path.DirectorySeparatorChar + $"bootstrap{_scriptEngineAttribute.FileExtension}");
				_fileSystemManager.CreateDirectory(_neonConfig.Scripts.Directory);
				CheckBootstrapFile();
				//StartMonitorDirectory();

				_logger.LogInformation("Initializing Script manager");
				ScanForScriptClasses();

				_logger.LogInformation($"Loading bootstrap file");
				_scriptEngine.LoadFile(_bootstrapFile, true);

				_logger.LogInformation($"Scanning files in directory {_neonConfig.Scripts.Directory}");
				LoadScriptsFiles();

				await _scriptEngine.Build();

				_logger.LogInformation("Script manager initialized");
			}
			else
			{
				throw new Exception("No script engine found");
			}

			

			return true;
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
			_fileSystemWatcher.Dispose();
			return Task.FromResult(true);
		}

		private void ScanForScriptClasses()
		{
			AssemblyUtils.ScanAllAssembliesFromAttribute(typeof(ScriptObjectAttribute)).ForEach(t =>
			{
				_logger.LogInformation($"Registering {t.Name} in Scripts Objects");
				var obj = _servicesManager.Resolve(t);
				obj.GetType().GetMethods().ToList().ForEach(m =>
				{
					try

					{
						var scriptFuncAttr = m.GetCustomAttribute<ScriptFunctionAttribute>();


						if (scriptFuncAttr == null) return;

						_logger.LogInformation(
							$"{obj.GetType().Name} - {scriptFuncAttr.FunctionName} [{scriptFuncAttr.Help}]");

						_scriptEngine.RegisterFunction(scriptFuncAttr.FunctionName, obj,
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

			if (Path.GetExtension(filename) != "." + _scriptEngineAttribute.FileExtension) return;

			_logger.LogInformation($"New File created: {filename}");
			_scriptEngine.LoadFile(filename, true);
		}

		private void LoadScriptsFiles()
		{
			var files = Directory.GetFiles(_fileSystemManager.BuildFilePath(_neonConfig.Scripts.Directory), "*" + _scriptEngineAttribute.FileExtension,
					SearchOption.AllDirectories)
				.ToList();

			files.ForEach(f =>
			{
				if (!f.ToLower().Contains("bootstrap" + _scriptEngineAttribute.FileExtension))
					_scriptEngine.LoadFile(f, false);
			});
		}

	
	}
}