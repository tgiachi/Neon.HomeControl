using Neon.HomeControl.Api.Core.Attributes.ScriptService;
using NLua;
using System;

namespace Neon.HomeControl.Web.LuaScripts
{
	[LuaScriptObject]
	public class TestLuaObject
	{
		[LuaScriptFunction("TEST", "test", "Display test")]
		public void Test(string param)
		{
			Console.WriteLine("OK");
			//	throw new Exception("This is test");
		}

		[LuaScriptFunction("TEST", "bind_function", "Display test")]
		public void ExecuteFunction(LuaFunction function)
		{
			Console.WriteLine($"Executing {function.GetType().Name}");
			function.Call();
		}
	}
}