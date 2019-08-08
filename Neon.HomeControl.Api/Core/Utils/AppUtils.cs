﻿using Neon.HomeControl.Api.Core.Data.Logger;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Neon.HomeControl.Api.Core.Utils
{
	public static class AppUtils
	{
		public static string AppName = "Neon.HomeControl";

		public static string AppVersion = typeof(AppUtils).Assembly.GetName().Version.ToString();

		public static string AppFullVersion => $"{AppName} v{AppVersion}";

		public static ObservableCollection<LoggerEntry> LoggerEntries { get; set; } = new ObservableCollection<LoggerEntry>();

	}
}