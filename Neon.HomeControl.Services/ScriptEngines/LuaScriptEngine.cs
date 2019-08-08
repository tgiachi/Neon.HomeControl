using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.ScriptEngine;
using Neon.HomeControl.Api.Core.Data.Config;
using Neon.HomeControl.Api.Core.Interfaces.ScriptEngine;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Neon.HomeControl.Api.Core.Utils;
using NLua;
using NLua.Exceptions;

namespace Neon.HomeControl.Services.ScriptEngines
{
	/// <summary>
	/// Implementation of LUA script Engine
	/// </summary>
	[ScriptEngine("lua", ".lua", "v1.0")]
	public class LuaScriptEngine : IScriptEngine
	{
		private readonly Lua _luaEngine;
		private readonly ILogger _logger;
		private string _modulesPath;
		private readonly IFileSystemManager _fileSystemManager;
		private readonly NeonConfig _neonConfig;
		private readonly List<LuaFunction> _functions = new List<LuaFunction>();



		public LuaScriptEngine(ILogger<LuaScriptEngine> logger, IFileSystemManager fileSystemManager, NeonConfig neonConfig)
		{
			_logger = logger;
			_fileSystemManager = fileSystemManager;
			_neonConfig = neonConfig;
			_luaEngine = new Lua();
			_luaEngine.State.Encoding = Encoding.UTF8;
			_luaEngine.LoadCLRPackage();

			CheckModulesDirectory();

			_luaEngine.HookException += (sender, args) =>
			{
				if (args.Exception is LuaException luaException)
				{
					_logger.LogError($"Error during execute LUA =>\n {FormatException(luaException)}");
				}
				else
				{
					_logger.LogError($"Error during execute LUA =>\n {args.Exception.FlattenException()}");
				}
			};

			_logger.LogInformation($"LUA script engine ready");
		}

		private string FormatException(LuaException e)
		{
			var source = (string.IsNullOrEmpty(e.Source)) ? "<no source>" : e.Source.Substring(0, e.Source.Length - 2);
			return string.Format("{0}\nLua (at {2})", e.Message, string.Empty, source);
		}

		private void CheckModulesDirectory()
		{
			_logger.LogInformation($"Check Modules directory");

			_fileSystemManager.CreateDirectory(Path.Join(_neonConfig.Scripts.Directory, "Modules" + Path.DirectorySeparatorChar));

			_modulesPath = _fileSystemManager.BuildFilePath(Path.Join(_neonConfig.Scripts.Directory, "Modules"  + Path.DirectorySeparatorChar));

			_logger.LogInformation($"LUA Modules path {_modulesPath}");

			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				_modulesPath = _modulesPath.Replace(@"\", @"\\");
			}

			_luaEngine.DoString($@"
			-- Update the search path
			local module_folder = '{_modulesPath}'
			package.path = module_folder .. '?.lua;' .. package.path");
		}


		public void LoadFile(string fileName, bool immediateExecute)
		{
			try
			{

				_logger.LogInformation($"Loading file {fileName}...");
				var func = _luaEngine.LoadFile(fileName);
				_functions.Add(func);
				if (immediateExecute)
					func.Call();
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error during load file {fileName} => {ex.FlattenException()}");
			}
		}

		public void RegisterFunction(string functionName, object obj, MethodInfo method)
		{
			_luaEngine.RegisterFunction(functionName, obj, method);
		}

		public object ExecuteCode(string code)
		{
			return _luaEngine.DoString(code);
		}

		public Task<bool> Build()
		{
			_functions.ForEach(f => { f.Call(); });

			return Task.FromResult(true);
		}

		public void Dispose()
		{
			_functions.ForEach(f => f.Dispose());
			_luaEngine?.Dispose();
		}


	}
}
