using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.Components;
using Neon.HomeControl.Api.Core.Interfaces.Components;
using Neon.HomeControl.Components.Config;
using Neon.HomeControl.Components.Interfaces;

namespace Neon.HomeControl.Components.Components
{

	[Component("plex_hook", "Plex WebHook connector", "1.0", "MEDIA", "Get and parse Plex Web hook", typeof(PlexHookConfig))]
	public class PlexHookComponent : IPlexHookComponent
	{
		private PlexHookConfig _config;
		private readonly ILogger _logger;

		public PlexHookComponent(ILogger<PlexHookComponent> logger)
		{
			_logger = logger;
		}
		public Task<bool> Start()
		{
			_logger.LogInformation("listening in /components/plexhook");

			return Task.FromResult(true);

		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}

		public Task InitConfiguration(object config)
		{
			_config = (PlexHookConfig) config;
			return Task.CompletedTask;
		}

		public object GetDefaultConfig()
		{
			return new PlexHookConfig();
		}

		public void Hook(string jsonData)
		{
			
		}
	}
}
