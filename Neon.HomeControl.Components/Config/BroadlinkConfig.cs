using System;
using System.Collections.Generic;
using System.Text;
using Neon.HomeControl.Api.Core.Impl.Components;

namespace Neon.HomeControl.Components.Config
{
	public class BroadlinkConfig : BaseComponentConfig
	{
		public List<BroadlinkDeviceConfig> Devices { get; set; }

		public BroadlinkConfig()
		{
			Devices = new List<BroadlinkDeviceConfig>();
		}
	}


	public class BroadlinkDeviceConfig
	{
		public string MacAddress { get; set; }

		public string IpAddress { get; set; }
	}
}
