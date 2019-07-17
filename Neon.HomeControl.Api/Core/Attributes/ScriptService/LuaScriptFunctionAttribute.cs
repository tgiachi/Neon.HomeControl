using System;

namespace Neon.HomeControl.Api.Core.Attributes.ScriptService
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public class LuaScriptFunctionAttribute : Attribute
	{
		public LuaScriptFunctionAttribute(string category, string functionName, string help)
		{
			FunctionCategory = category;
			FunctionName = functionName;
			Help = help;
		}

		public string FunctionName { get; set; }

		public string FunctionCategory { get; set; }
		public string Help { get; set; }
	}
}