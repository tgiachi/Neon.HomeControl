using Neon.HomeControl.Api.Core.Attributes.ScriptService;
using NLua;
using System;

namespace Neon.HomeControl.Web.LuaScripts
{
	[ScriptObject("testclass")]
	public class TestLuaObject
	{
		[ScriptFunction("TEST", "test", "Display test")]
		public void Test(string param)
		{
			Console.WriteLine("OK");
			//	throw new Exception("This is test");
		}

		[ScriptFunction("TEST", "bind_function", "Display test")]
		public void ExecuteFunction(LuaFunction function)
		{
			Console.WriteLine($"Executing {function.GetType().Name}");
			function.Call();
		}
	}
}