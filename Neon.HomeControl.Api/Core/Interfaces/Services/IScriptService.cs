using System.Collections.Generic;
using Neon.HomeControl.Api.Core.Data.LuaScript;

namespace Neon.HomeControl.Api.Core.Interfaces.Services
{
	public interface IScriptService : IService
	{
		List<LuaScriptFunctionData> GlobalFunctions { get; set; }
	}
}