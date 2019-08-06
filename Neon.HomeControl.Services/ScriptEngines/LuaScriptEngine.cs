using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.ScriptEngine;
using Neon.HomeControl.Api.Core.Interfaces.ScriptEngine;
using Neon.HomeControl.Api.Core.Utils;
using NLua;

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
		private readonly List<LuaFunction> _functions = new List<LuaFunction>();

		public LuaScriptEngine(ILogger<LuaScriptEngine> logger)
		{
			_logger = logger;
			_luaEngine = new Lua();
			_luaEngine.State.Encoding = Encoding.UTF8;
			_luaEngine.LoadCLRPackage();

			_luaEngine.HookException += (sender, args) =>
			{
				_logger.LogError($"Error during execute LUA =>\n {args.Exception.FlattenException()}");
			};

			_logger.LogInformation($"LUA script engine ready");
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

		public Task<bool> Build()
		{
			_functions.ForEach(f => { _logger.LogInformation($"{f.Call()}"); });

			return Task.FromResult(true);
		}

		public void Dispose() => _luaEngine?.Dispose();
	}
}
