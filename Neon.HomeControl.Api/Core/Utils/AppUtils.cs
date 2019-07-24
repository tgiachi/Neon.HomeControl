using System.Collections.Generic;
using Neon.HomeControl.Api.Core.Data.Logger;

namespace Neon.HomeControl.Api.Core.Utils
{
	public static class AppUtils
	{
		public static string AppName = "Neon.HomeControl";

		public static string AppVersion = "1.0.0.0";

		public static string AppFullVersion => $"{AppName} v{AppVersion}";

		public static List<LoggerEntry> LoggerEntries { get; set; } = new List<LoggerEntry>(); 

	}
}