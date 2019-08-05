using System;
using System.Collections.Generic;
using System.Text;
using Neon.HomeControl.Api.Core.Attributes.ScriptEngine;
using Neon.HomeControl.Api.Core.Interfaces.ScriptEngine;

namespace Neon.HomeControl.Services.ScriptEngines
{
	/// <summary>
	/// Implementation of LUA script Engine
	/// </summary>
	[ScriptEngine("lua", ".lua", "v1.0")]
	public class LuaScriptEngine : IScriptEngine
	{
		
	}
}
