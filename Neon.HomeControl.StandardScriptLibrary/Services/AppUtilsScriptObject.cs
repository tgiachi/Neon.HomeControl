using System;
using System.Collections.Generic;
using System.Text;
using Neon.HomeControl.Api.Core.Attributes.ScriptService;
using Neon.HomeControl.Api.Core.Utils;

namespace Neon.HomeControl.StandardScriptLibrary.Services
{

	[ScriptObject("app")]
	public class AppUtilsScriptObject
	{

		[ScriptFunction("APP", "app_name", "Get app name")]
		public string AppName()
		{
			return AppUtils.AppName;
		}

		[ScriptFunction("APP", "app_version", "Get app version")]

		public string AppVersion()
		{
			return AppUtils.AppVersion;
		}
	}
}
