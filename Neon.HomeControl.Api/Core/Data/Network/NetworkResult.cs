namespace Neon.HomeControl.Api.Core.Data.Network
{
	public class NetworkResult
	{
		public string IpAddress { get; set; }

		public string DnsName { get; set; }

		public bool Online { get; set; }

		public long RoundTripTime { get; set; }
	}
}