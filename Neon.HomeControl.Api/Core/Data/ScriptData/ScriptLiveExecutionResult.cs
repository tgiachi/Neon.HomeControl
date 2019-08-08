using System;
using System.Collections.Generic;
using System.Text;

namespace Neon.HomeControl.Api.Core.Data.ScriptData
{
	public class ScriptLiveExecutionResult
	{
		public bool Success { get; set; }

		public string Output { get; set; }

		public string Source { get;set; }

		public Exception Exception { get; set; }
	}
}
