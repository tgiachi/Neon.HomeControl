using MediatR;

namespace Neon.HomeControl.Api.Core.Events
{
	public class PortScanEvent : INotification
	{
		public string Host { get; set; }

		public int Port { get; set; }
	}
}