using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neon.HomeControl.Api.Core.Attributes.Components;
using Neon.HomeControl.Components.Config;
using Neon.HomeControl.Components.Interfaces;
using UPnP;

namespace Neon.HomeControl.Components.Components
{
	[Component("Ssdp connector", "1.0", "STREAMING", "Control ssdp", typeof(SsdpConfig))]
	public class SsdpComponent : ISsdpComponent
	{

		private SsdpConfig _config;
		public async Task<bool> Start()
		{
			var devices = await new Ssdp().SearchUPnPDevicesAsync("MediaRenderer");

			devices.ToList().ForEach(d =>
			{

			});
			return true;
		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}

		public Task InitConfiguration(object config)
		{
			_config = (SsdpConfig) config;
			return Task.CompletedTask;
		}

		public object GetDefaultConfig()
		{
			return new SsdpConfig()
			{
				EnableDiscovery = true,
				Enabled = true
			};
		}
	}
}
