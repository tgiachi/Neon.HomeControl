using System;
using System.Collections.Generic;
using System.Text;
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
		
	}
}
