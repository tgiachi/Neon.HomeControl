using System;
using System.Collections.Generic;
using System.Text;

namespace Neon.HomeControl.Api.Core.Data.Config.MqttClient
{
	public class MqttMirrorConfig
	{
		public MqttClientConfig ClientConfig { get; set; }

		public bool SendToMirror { get; set; }

		public bool ReceiveFromMirror { get; set; }

		public bool IsEnabled { get; set; }
	}
}
