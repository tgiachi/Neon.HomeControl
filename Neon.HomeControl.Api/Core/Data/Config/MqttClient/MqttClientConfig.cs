using System;
using System.Collections.Generic;
using System.Text;

namespace Neon.HomeControl.Api.Core.Data.Config.MqttClient
{
	public class MqttClientConfig
	{
		public string HostName { get; set; }

		public int Port { get; set; }

		public bool IsAuth { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }
	}
}
