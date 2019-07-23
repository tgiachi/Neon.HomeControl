using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neon.HomeControl.Api.Core.Attributes.Components;
using Neon.HomeControl.Api.Core.Data.UserInteraction;
using Neon.HomeControl.Api.Core.Enums;
using Neon.HomeControl.Api.Core.Interfaces.Managers;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Neon.HomeControl.Components.Config;
using Neon.HomeControl.Components.EventsDb;
using Neon.HomeControl.Components.Interfaces;
using Microsoft.Extensions.Logging;
using Q42.HueApi;
using Q42.HueApi.Interfaces;

namespace Neon.HomeControl.Components.Components
{
	[Component("philip_hue", "Philip Hue", "1.0", "LIGHTS", "Control Philip Hue lights", typeof(PhilipHueConfig))]
	public class PhilipHueComponent : IPhilipHueComponent
	{
		private readonly IBridgeLocator _bridgeLocator = new HttpBridgeLocator();
		private readonly IComponentsService _componentsService;
		private readonly IEventDatabaseService _eventDatabaseService;
		private readonly IIoTService _ioTService;
		private readonly ILogger _logger;
		private readonly ISchedulerService _schedulerService;
		private readonly IUserInteractionService _userInteractionService;
		private PhilipHueConfig _config;
		private ILocalHueClient _hueClient;

		public PhilipHueComponent(ILogger<PhilipHueComponent> logger,
			IComponentsService componentsService,
			IEventDatabaseService eventDatabaseService,
			IIoTService ioTService,
			ISchedulerService schedulerService,
			IUserInteractionService userInteractionService)
		{
			_logger = logger;
			_componentsService = componentsService;
			_userInteractionService = userInteractionService;
			_ioTService = ioTService;
			_eventDatabaseService = eventDatabaseService;
			_schedulerService = schedulerService;
		}


		public async Task<bool> Start()
		{
			if (string.IsNullOrEmpty(_config.BridgeIpAddress))
			{
				_logger.LogInformation("Searching from Philip Hue bridge...");

				var bridgeIps = await _bridgeLocator.LocateBridgesAsync(TimeSpan.FromSeconds(10));

				if (bridgeIps.Any())
				{
					_logger.LogInformation($"Found {bridgeIps.Count()} bridges");

					_hueClient = new LocalHueClient(bridgeIps.ToList()[0].IpAddress);

					_config.BridgeIpAddress = bridgeIps.ToList()[0].IpAddress;

					_logger.LogInformation("Button pressed");
					var appKey = await _hueClient.RegisterAsync("LeonHomeControl", "Leon");
					_config.ApiKey = appKey;

					_componentsService.SaveComponentConfig(_config);

					_hueClient.Initialize(appKey);

					_logger.LogInformation("Philip Hue Configured");

					_hueClient = new LocalHueClient(_config.BridgeIpAddress, _config.ApiKey);

					_logger.LogInformation("Connected to Philip Hue");

					var lights = await _hueClient.GetLightsAsync();

					lights.ToList().ForEach(s => { _logger.LogInformation($"{s.Name}"); });
					var groups = await _hueClient.GetGroupsAsync();
					groups.ToList().ForEach(g => { _logger.LogInformation($"{g.Name}"); });
				}
			}
			else
			{
				_hueClient = new LocalHueClient(_config.BridgeIpAddress);

				if (string.IsNullOrEmpty(_config.ApiKey))
				{
					_userInteractionService.AddUserInteractionData(new UserInteractionData
					{
						Name = "Philip hue component",
						Fields = new List<UserInteractionField>
						{
							new UserInteractionField().Build().SetIsRequired(true)
								.SetFieldType(UserInteractionFieldTypeEnum.BUTTON)
								.SetFieldName("PHILIP_HUE_BUTTON_PRESS")
								.SetDescription("Press Philip hue button for link bride")
						}
					}, async data => { });
				}
				else
				{
					_hueClient = new LocalHueClient(_config.BridgeIpAddress, _config.ApiKey);

					_logger.LogInformation("Connected to Philip Hue");

					_schedulerService.AddPolling(UpdateEntities, "PhiipHue_UpdateLightEntities",
						SchedulerServicePollingEnum.NORMAL_POLLING);
				}
			}

			return true;
		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}

		public Task InitConfiguration(object config)
		{
			_config = config as PhilipHueConfig;

			return Task.CompletedTask;
		}

		public object GetDefaultConfig()
		{
			return new PhilipHueConfig();
		}

		private void UpdateEntities()
		{
			try
			{
				UpdateLightsStatus();
				UpdateLightGroupsStatus();
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error during update lights status {ex}");
			}
		}

		private async void UpdateLightsStatus()
		{
			try
			{
				var lights = await _hueClient.GetLightsAsync();

				lights.ToList().ForEach(s =>
				{
					var entity = new PhilipsHueEd
					{
						EntityName = s.Name,
						Brightness = s.State.Brightness,
						Hue = s.State.Hue ?? -1,
						IsOn = s.State.On,
						IsReachable = s.State.IsReachable ?? false,
						Type = PhilipsHueTypeEnum.LIGHT
					};

					_ioTService.InsertEvent(entity);
				});
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error during update light status: {ex}");
			}

		}

		private async void UpdateLightGroupsStatus()
		{
			try
			{
				var groups = await _hueClient.GetGroupsAsync();
				groups.ToList().ForEach(g =>
				{
					var entity = new PhilipsHueEd
					{
						EntityName = g.Name,
						IsOn = g.State?.AllOn == null ? g.State.AllOn.Value : false,
						Type = PhilipsHueTypeEnum.GROUP
					};

					_ioTService.InsertEvent(entity);
				});
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error during update light group status: {ex}");
			}

		}
	}
}