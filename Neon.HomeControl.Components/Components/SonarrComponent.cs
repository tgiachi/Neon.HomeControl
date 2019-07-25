using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.Components;
using Neon.HomeControl.Components.Config;
using Neon.HomeControl.Components.Interfaces;
using SonarrSharp;
using System;
using System.Threading.Tasks;

namespace Neon.HomeControl.Components.Components
{
	[Component("sonarr", "Sonarr connector", "1.0", "STREAMING", "Connect to Sonarr", typeof(SonarrConfig))]
	public class SonarrComponent : ISonarrComponent
	{

		private readonly ILogger _logger;
		private SonarrClient _sonarrClient;
		private SonarrConfig _sonarrConfig;


		public SonarrComponent(ILogger<SonarrComponent> logger)
		{
			_logger = logger;
		}

		public async Task<bool> Start()
		{
			if (!string.IsNullOrEmpty(_sonarrConfig.ApiKey) && !string.IsNullOrEmpty(_sonarrConfig.Host))
			{
				_sonarrClient = new SonarrClient(_sonarrConfig.Host, _sonarrConfig.Port, _sonarrConfig.ApiKey);

				var calendar = await _sonarrClient.Calendar.GetCalendar(DateTime.Now, DateTime.Now.AddDays(30));
				foreach (var item in calendar)
				{
					_logger.LogInformation($"{item.AirDate}: {item.Series.Title} - s{item.SeasonNumber}e{item.EpisodeNumber} - {item.Title}");
				}
			}

			return true;
		}

		public Task<bool> Stop()
		{
			_sonarrClient = null;

			return Task.FromResult(true);
		}

		public Task InitConfiguration(object config)
		{
			_sonarrConfig = (SonarrConfig)config;
			return Task.CompletedTask;
		}

		public object GetDefaultConfig()
		{
			return new SonarrConfig();
		}
	}
}
