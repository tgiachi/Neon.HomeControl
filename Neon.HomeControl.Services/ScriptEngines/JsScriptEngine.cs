using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Jint;
using Jint.Parser.Ast;
using Jint.Runtime.Environments;
using Jint.Runtime.Interop;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.ScriptEngine;
using Neon.HomeControl.Api.Core.Attributes.ScriptService;
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

		private readonly List<string> _scriptsClassLoaded = new List<string>();

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

			var scriptObjAttribute = obj.GetType().GetCustomAttribute<ScriptObjectAttribute>();

			if (_scriptsClassLoaded.Contains(scriptObjAttribute.ObjName))
				return;

			_jsEngine.SetValue(scriptObjAttribute.ObjName, DynamicCast(obj, obj.GetType()));

			_scriptsClassLoaded.Add(scriptObjAttribute.ObjName);
		}

		public object ExecuteCode(string code)
		{
			throw new NotImplementedException();
		}


		static T Cast<T>(object entity) where T : class
		{
			return entity as T;
		}

		dynamic DynamicCast(object entity, Type to)
		{
			var openCast = this.GetType().GetMethod("Cast", BindingFlags.Static | BindingFlags.NonPublic);
			var closeCast = openCast.MakeGenericMethod(to);
			return closeCast.Invoke(entity, new[] { entity });
		}

		public Task<bool> Build()
		{
			return Task.FromResult(true);
		}
	}
}
