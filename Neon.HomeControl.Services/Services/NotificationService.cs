using System.Threading.Tasks;
using Neon.HomeControl.Api.Core.Attributes.Services;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Neon.HomeControl.Services.Services
{
	[Service(typeof(INotificationService), Name = "Notification Service", LoadAtStartup = true, Order = 2)]
	public class NotificationService : INotificationService
	{
		private readonly ILogger _logger;
		private readonly IMediator _mediator;

		public NotificationService(IMediator mediator, ILogger<NotificationService> logger)
		{
			_mediator = mediator;
			_logger = logger;
		}

		public Task<bool> Start()
		{
			return Task.FromResult(true);
		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}

		public async void BroadcastMessage<TMessage>(TMessage message) where TMessage : INotification
		{
			try
			{
				await _mediator.Publish(message);
			}
			catch
			{
				//_logger.Error($"Error during publishing message {message.GetType().Name} => {ex}");
			}
		}
	}
}