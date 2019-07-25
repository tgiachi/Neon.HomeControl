using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.Components;
using Neon.HomeControl.Components.Config;
using Neon.HomeControl.Components.Interfaces;
using RadarrSharp;
using System.Threading.Tasks;

namespace Neon.HomeControl.Components.Components
{
	[Component("radarr", "Radarr connector", "1.0", "STREAMING", "Connect to Radarr", typeof(RadarrConfig))]

	public class RadarrComponent : IRadarrComponent
	{
		private RadarrConfig _config;
		private RadarrClient _radarrClient;
		private ILogger _logger;

		public RadarrComponent(ILogger<RadarrComponent> logger)
		{
			_logger = logger;
		}


		public async Task<bool> Start()
		{
			if (!string.IsNullOrEmpty(_config.Host) && !string.IsNullOrEmpty(_config.ApiKey))
			{
				_radarrClient = new RadarrClient(_config.Host, _config.Port, _config.ApiKey);

				var movies = await _radarrClient.Movie.GetMovies();

				_logger.LogInformation($"Totale movies {movies.Count}");

			}

			return true;

		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}

		public Task InitConfiguration(object config)
		{
			_config = (RadarrConfig)config;

			return Task.CompletedTask;

		}

		public object GetDefaultConfig()
		{
			return new RadarrConfig();
		}
	}
}
