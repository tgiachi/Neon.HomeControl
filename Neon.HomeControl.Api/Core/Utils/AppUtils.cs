using Neon.HomeControl.Api.Core.Data.Logger;
using System.Collections.Generic;

namespace Neon.HomeControl.Api.Core.Utils
{
	public static class AppUtils
	{
		public static string AppName = "Neon.HomeControl";

		public static string AppVersion = "0.0.5.5";

		public static string AppFullVersion => $"{AppName} v{AppVersion}";

		public static List<LoggerEntry> LoggerEntries { get; set; } = new List<LoggerEntry>();

	}
}