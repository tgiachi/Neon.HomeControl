using MediatR;

namespace Neon.HomeControl.Api.Core.Events
{
	public class MqttMessageEvent : INotification
	{
		public string Topic { get; set; }

		public string Message { get; set; }
	}
}