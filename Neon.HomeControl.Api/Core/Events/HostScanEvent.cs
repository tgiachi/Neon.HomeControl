using MediatR;

namespace Neon.HomeControl.Api.Core.Events
{
	public class HostScanEvent : INotification
	{
		public string Host { get; set; }
		public string DnsName { get; set; }

		public bool IsOnline { get; set; }
	}
}