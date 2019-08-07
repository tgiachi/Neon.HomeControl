using System;
using System.Collections.Generic;
using System.Text;
using Neon.HomeControl.Api.Core.Attributes.ScriptService;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using NLua;

namespace Neon.HomeControl.StandardScriptLibrary.Services
{
	[ScriptObject("routines")]
	public class RoutinesScriptObject
	{

		private readonly IRoutineService _routineService;

		public RoutinesScriptObject(IRoutineService routineService)
		{
			_routineService = routineService;
		}

		[ScriptFunction("ROUTINES", "add_routine","Add routine")]
		public bool AddRoutines(string name, LuaFunction function, params object[] args)
		{
			return _routineService.AddRoutine(name, () => { function.Call(args); });
		}

		[ScriptFunction("ROUTINES", "execute_routine","Add routine")]
		public void ExecuteRoutine(string name)
		{
			_routineService.ExecuteRoutine(name);
		}
	}
}
