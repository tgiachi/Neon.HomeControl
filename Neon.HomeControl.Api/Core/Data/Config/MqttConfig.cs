using Neon.HomeControl.Api.Core.Data.Config.MqttClient;

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


		public MqttMirrorConfig MirrorConfig { get; set; }


		public MqttConfig()
		{
			RunEmbedded = true;
			MirrorConfig = new MqttMirrorConfig()
			{
				ClientConfig = new MqttClientConfig(),
				ReceiveFromMirror = false,
				SendToMirror = false,
				IsEnabled = false
			};
			ClientId = "Neon-server";
		}
	}
}