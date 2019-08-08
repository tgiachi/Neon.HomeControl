using System.Collections.Generic;
using Neon.HomeControl.Api.Core.Data.ScriptData;

namespace Neon.HomeControl.Api.Core.Interfaces.Services
{
	public interface IScriptService : IService
	{
		List<ScriptFunctionData> GlobalFunctions { get; set; }

		ScriptLiveExecutionResult ExecuteCode(string code);
	}
}