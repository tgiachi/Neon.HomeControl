using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.ScriptService;

namespace Neon.HomeControl.StandardScriptLibrary.Logging
{
	[ScriptObject]
	public class LoggerScriptObject
	{
		private readonly ILogger _logger;

		public LoggerScriptObject(ILogger<LoggerScriptObject> logger)
		{
			_logger = logger;
		}

		[ScriptFunction("LOGGER", "log_info", "Log info message")]
		public void LogInfo(string category, string text, params object[] args)
		{
			_logger.LogInformation($"[{category}] - {string.Format(text, args)}");
		}

		[ScriptFunction("LOGGER", "log_warn", "Log warning message")]
		public void LogWarn(string category, string text, params object[] args)
		{
			_logger.LogWarning($"[{category}] - {string.Format(text, args)}");
		}

		[ScriptFunction("LOGGER", "log_error", "Log error message")]
		public void LogError(string category, string text, params object[] args)
		{
			_logger.LogError($"[{category}] - {string.Format(text, args)}");
		}

		[ScriptFunction("LOGGER", "log_debug", "Log debug message")]
		public void LogDebug(string category, string text, params object[] args)
		{
			_logger.LogDebug($"[{category}] - {string.Format(text, args)}");
		}
	}
}