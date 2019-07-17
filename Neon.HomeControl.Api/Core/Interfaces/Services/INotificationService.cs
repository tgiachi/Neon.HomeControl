using MediatR;

namespace Neon.HomeControl.Api.Core.Interfaces.Services
{
	public interface INotificationService : IService
	{
		void BroadcastMessage<TMessage>(TMessage message) where TMessage : INotification;
	}
}