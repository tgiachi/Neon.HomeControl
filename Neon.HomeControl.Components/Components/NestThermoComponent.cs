using System.Threading.Tasks;
using Neon.HomeControl.Api.Core.Attributes.Components;
using Neon.HomeControl.Components.Config;
using Neon.HomeControl.Components.Interfaces;

namespace Neon.HomeControl.Components.Components
{
	[Component("nest","Nest Thermostat", "1.0", "COMFORT", "Control Nest Thermostat", typeof(NestThermoConfig))]
	public class NestThermoComponent : INestThermoComponent
	{
		private NestThermoConfig _config;

		public Task<bool> Start()
		{
			return Task.FromResult(true);
		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}

		public Task InitConfiguration(object config)
		{
			_config = config as NestThermoConfig;

			return Task.CompletedTask;
		}

		public object GetDefaultConfig()
		{
			return new NestThermoConfig();
		}
	}
}