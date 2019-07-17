using Neon.HomeControl.Api.Core.Attributes.ScriptService;
using Microsoft.Extensions.Logging;

namespace Neon.HomeControl.StandardLuaLibrary.StandardLuaLibrary.Logging
{
	[LuaScriptObject]
	public class LoggerLuaObject
	{
		private readonly ILogger _logger;

		public LoggerLuaObject(ILogger<LoggerLuaObject> logger)
		{
			_logger = logger;
		}

		[LuaScriptFunction("LOGGER", "log_info", "Log info message")]
		public void LogInfo(string category, string text, params object[] args)
		{
			_logger.LogInformation($"[{category}] - {string.Format(text, args)}");
		}

		[LuaScriptFunction("LOGGER", "log_warn", "Log warning message")]
		public void LogWarn(string category, string text, params object[] args)
		{
			_logger.LogWarning($"[{category}] - {string.Format(text, args)}");
		}

		[LuaScriptFunction("LOGGER", "log_error", "Log error message")]
		public void LogError(string category, string text, params object[] args)
		{
			_logger.LogError($"[{category}] - {string.Format(text, args)}");
		}

		[LuaScriptFunction("LOGGER", "log_debug", "Log debug message")]
		public void LogDebug(string category, string text, params object[] args)
		{
			_logger.LogDebug($"[{category}] - {string.Format(text, args)}");
		}
	}
}