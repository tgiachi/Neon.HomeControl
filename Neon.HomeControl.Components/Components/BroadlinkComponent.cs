using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.Components;
using Neon.HomeControl.Api.Core.Interfaces.Services;
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
		private ISchedulerService _schedulerService;

		public BroadlinkComponent(ISchedulerService schedulerService, ILogger<BroadlinkComponent> logger)
		{
			_logger = logger;
			_schedulerService = schedulerService;
		}

		private readonly List<Device> _devices = new List<Device>();

		public async Task<bool> Start()
		{
			//_config.Devices.ForEach(ConnectDevice);
			_schedulerService.AddJob(async () => { await ScanDevices(); }, 60, true);
			return true;
		}

		private async Task ScanDevices()
		{
			string mac = "78-0f-77-63-15-e1";
			byte[] arr = mac.Split('-').Select(x => Convert.ToByte(x, 16)).ToArray();
			IPAddress localIpAddress = null;
			IPAddress.TryParse("192.168.0.9", out localIpAddress);


		//			var devices = await Broadlink.Discover(10, localIpAddress);
		//Broadlink.Create(Rm, mac, new IPEndPoint("192.168.0.9", 80));

		var rmDevice = Broadlink.Create(0x2712, arr, new IPEndPoint(localIpAddress, 80));
		var auth = await rmDevice.Auth();
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
			_config = (BroadlinkConfig)config;

			return Task.CompletedTask;
		}

		public object GetDefaultConfig()
		{
			return new BroadlinkConfig();
		}
	}
}
