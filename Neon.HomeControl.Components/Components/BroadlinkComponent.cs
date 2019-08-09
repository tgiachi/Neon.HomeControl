using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.Components;
using Neon.HomeControl.Components.Config;
using Neon.HomeControl.Components.Interfaces;
using SharpBroadlink;
using SharpBroadlink.Devices;

namespace Neon.HomeControl.Components.Components
{
	[Component("broadlink", "Broadlink connector", "1.0", "UNIVERSAL_REMOTE", "Control Broadlink", typeof(BroadlinkConfig))]
	public class BroadlinkComponent : IBroadlinkComponent
	{
		private readonly ILogger _logger;
		private BroadlinkConfig _config;

		private readonly List<Device> _devices = new List<Device>();

		public async Task<bool> Start()
		{
			_config.Devices.ForEach(ConnectDevice);
			return true;
		}

		private void ConnectDevice(BroadlinkDeviceConfig device)
		{
			//Broadlink.Create(DeviceType.Rm, device, new)

		}

		public Task<bool> Stop()
		{

			return Task.FromResult(true);
		}

		public Task InitConfiguration(object config)
		{
			_config = (BroadlinkConfig) config;

			return Task.CompletedTask;
		}

		public object GetDefaultConfig()
		{
			return new BroadlinkConfig();
		}
	}
}
