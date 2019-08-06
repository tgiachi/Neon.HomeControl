using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Jint;
using Jint.Parser.Ast;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.ScriptEngine;
using Neon.HomeControl.Api.Core.Interfaces.ScriptEngine;

namespace Neon.HomeControl.Services.ScriptEngines
{
	/// <summary>
	/// Implementation of Javascript Engine
	/// </summary>

	[ScriptEngine("js", ".js", "0.1")]
	public class JsScriptEngine : IScriptEngine
	{
		private readonly ILogger _logger;
		private Engine _jsEngine;

		public JsScriptEngine(ILogger<JsScriptEngine> logger)
		{
			_logger = logger;
			_jsEngine = new Engine(options =>
				options.AllowClr());

			
		}

		public void Dispose()
		{
			_jsEngine = null;
		}

		public void LoadFile(string fileName, bool immediateExecute)
		{
			_jsEngine.Execute(File.ReadAllText(fileName));

		}

		public void RegisterFunction(string functionName, object obj, MethodInfo method)
		{
			_jsEngine.SetValue(functionName, new Action<object[]>(
				objects => method.Invoke(obj, objects))
			);
		}

		public Task<bool> Build()
		{
			return Task.FromResult(true);
		}
	}
}
