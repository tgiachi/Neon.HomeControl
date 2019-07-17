namespace Neon.HomeControl.Api.Core.Data.Config
{
	public class MqttConfig
	{
		/// <summary>
		/// If true start local Mqtt Server
		/// </summary>
		public bool RunEmbedded { get; set; }

		public string Host { get; set; }

		public string ClientId { get; set; }
	}
}