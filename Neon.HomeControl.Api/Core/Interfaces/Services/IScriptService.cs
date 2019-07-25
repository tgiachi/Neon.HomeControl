using Neon.HomeControl.Api.Core.Data.LuaScript;
using System.Collections.Generic;

namespace Neon.HomeControl.Api.Core.Interfaces.Services
{
	public interface IScriptService : IService
	{
		List<LuaScriptFunctionData> GlobalFunctions { get; set; }
	}
}