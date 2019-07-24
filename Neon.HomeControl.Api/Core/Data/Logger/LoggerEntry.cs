using System;
using System.Collections.Generic;
using System.Text;

namespace Neon.HomeControl.Api.Core.Data.Logger
{
	public class LoggerEntry
	{
		public DateTime EventDateTime { get; set; }
		public string Source { get; set; }

		public string Severity { get; set; }
		public string Message { get; set; }
	}
}
