using System;
using MediatR;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.ScriptService;
using Neon.HomeControl.Api.Core.Events;
using Neon.HomeControl.Api.Core.Interfaces.Services;

namespace Neon.HomeControl.StandardScriptLibrary.Logging
{
	[ScriptObject("logger")]
	public class LoggerScriptObject
	{
		private readonly ILogger _logger;
		private readonly INotificationService _mediator;

		public LoggerScriptObject(ILogger<LoggerScriptObject> logger, INotificationService mediator)
		{
			_logger = logger;
			_mediator = mediator;
		}

		[ScriptFunction("LOGGER", "log_info", "Log info message")]
		public void LogInfo(string category, string text, params object[] args)
		{
			_logger.LogInformation($"[{category}] - {string.Format(text, args)}");
			BroadcastScriptEvent(category, string.Format(text, args), "INFO");
		
		}

		[ScriptFunction("LOGGER", "log_warn", "Log warning message")]
		public void LogWarn(string category, string text, params object[] args)
		{
			_logger.LogWarning($"[{category}] - {string.Format(text, args)}");
			BroadcastScriptEvent(category, string.Format(text, args), "WARNING");

		}

		[ScriptFunction("LOGGER", "log_error", "Log error message")]
		public void LogError(string category, string text, params object[] args)
		{
			_logger.LogError($"[{category}] - {string.Format(text, args)}");
			BroadcastScriptEvent(category, string.Format(text, args), "ERROR");

		}

		[ScriptFunction("LOGGER", "log_debug", "Log debug message")]
		public void LogDebug(string category, string text, params object[] args)
		{
			_logger.LogDebug($"[{category}] - {string.Format(text, args)}");
			BroadcastScriptEvent(category, string.Format(text, args), "DEBUG");

		}

		private void BroadcastScriptEvent(string category, string text, string level)
		{
			_mediator.BroadcastMessage(new ScriptOutputEvent()
			{

				EventDate = DateTime.Now,
				Message = text,
				Level = level,
				Category = category
			});	
		}
	}
}