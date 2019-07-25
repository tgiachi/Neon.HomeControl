using MediatR;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.Services;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace Neon.HomeControl.Services.Services
{
	/// <summary>
	/// Service bridge for notify 
	/// </summary>
	[Service(typeof(INotificationService), Name = "Notification Service", LoadAtStartup = true, Order = 3)]
	public class NotificationService : INotificationService
	{
		private readonly ILogger _logger;
		private readonly IMediator _mediator;

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="mediator"></param>
		/// <param name="logger"></param>
		public NotificationService(IMediator mediator, ILogger<NotificationService> logger)
		{
			_mediator = mediator;
			_logger = logger;
		}


		/// <summary>
		/// Start notification service
		/// </summary>
		/// <returns></returns>
		public Task<bool> Start()
		{
			return Task.FromResult(true);
		}

		/// <summary>
		/// Stop notification service
		/// </summary>
		/// <returns></returns>
		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}

		/// <summary>
		/// Broadcast to listener
		/// </summary>
		/// <typeparam name="TMessage"></typeparam>
		/// <param name="message"></param>
		public async void BroadcastMessage<TMessage>(TMessage message) where TMessage : INotification
		{
			try
			{
				_logger.LogDebug($"Sending notification type {message.GetType().Name}");
				await _mediator.Publish(message);
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error during publishing message {message.GetType().Name} => {ex}");
			}
		}
	}
}