using Neon.HomeControl.Api.Core.Attributes.ScriptService;

namespace Neon.Plugin.Test.Plugin
{
	[LuaScriptObject]
	public class TestLuaCommand
	{
		[LuaScriptFunction("EXAMPLE", "sum", "sum two numbers")]
		public string Sum(int firstNumber, int secondNumber)
		{
			return (firstNumber + secondNumber).ToString();
		}
	}
}
