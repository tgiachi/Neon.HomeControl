using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.Components;
using Neon.HomeControl.Api.Core.Enums;
using Neon.HomeControl.Api.Core.Interfaces.Managers;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Neon.HomeControl.Components.AirCo;
using Neon.HomeControl.Components.Config;
using Neon.HomeControl.Components.EventsDb;
using Neon.HomeControl.Components.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Neon.HomeControl.Components.Components
{
	[Component("panasonic_air", "Panasonic AirConditioner", "1.0", "AIR", "Connect and control Panasonic Airconditioner",
		typeof(PanasonicAirCondConfig))]
	public class PanasonicAirCondComponent : IPanasonicAirCondComponent
	{
		private readonly IComponentsService _componentsService;
		private readonly IEventDatabaseService _eventDatabaseService;
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IIoTService _ioTService;
		private readonly ILogger _logger;
		private readonly ISchedulerService _schedulerService;
		private AircoManager _aircoManager;
		private PanasonicAirCondConfig _config;

		public PanasonicAirCondComponent(ILogger<IPanasonicAirCondComponent> logger,
			IComponentsService componentsService, ISchedulerService schedulerService, IIoTService ioTService,
			IHttpClientFactory clientFactory,
			IEventDatabaseService eventDatabaseService)
		{
			_logger = logger;
			_componentsService = componentsService;
			_schedulerService = schedulerService;
			_ioTService = ioTService;
			_httpClientFactory = clientFactory;
			_eventDatabaseService = eventDatabaseService;
		}

		public async Task<bool> Start()
		{
			_aircoManager = new AircoManager(_httpClientFactory.CreateClient());

			if (!string.IsNullOrEmpty(_config.AuthCode))
			{
				_aircoManager.SetAuthorizationToken(_config.AuthCode);

				_schedulerService.AddPolling(PollingConds, "PANASONIC_AIR_CONDITIONER",
					SchedulerServicePollingEnum.NORMAL_POLLING);
			}
			else if (!string.IsNullOrEmpty(_config.Username))
			{
				var result = await _aircoManager.Login("0", _config.Username, _config.Password);

				if (result.Result == 0)
				{
					_config.AuthCode = result.UToken;

					_componentsService.SaveComponentConfig(_config);
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
			_config = (PanasonicAirCondConfig)config;

			return Task.CompletedTask;
		}

		public object GetDefaultConfig()
		{
			return new PanasonicAirCondConfig();
		}

		private async void PollingConds()
		{
			try
			{
				var deviceGroups = await _aircoManager.GetDeviceGroups();
				deviceGroups.GroupList.ForEach(g =>
				{
					g.DeviceIdList.ForEach(async d =>
					{
						try
						{
							var device = await _aircoManager.GetDeviceStatus(d.DeviceGuid);
							var deviceEd = new PanasonicAirConditioner
							{
								EntityName = d.DeviceName,
								DeviceId = d.DeviceGuid,
								GroupName = g.GroupName,
								InsideTemperature = device.Parameters.InsideTemperature,
								OutTemperature = device.Parameters.OutTemperature,
								TemperatureSet = device.Parameters.TemperatureSet,
								OperationMode = device.Parameters.OperationMode.ToString()
							};


							_ioTService.InsertEvent(deviceEd);
						}
						catch (Exception e)
						{
							_logger.LogError($"Error during air conditioner => {e.Message}");
						}
					});
				});
			}
			catch (Exception e)
			{
				if (e.Message.Contains("Token expires"))
				{
					_config.AuthCode = "";
					await Start();

				}
				else
					_logger.LogError($"Error during air conditioner");
				//throw;
			}
		}
	}
}