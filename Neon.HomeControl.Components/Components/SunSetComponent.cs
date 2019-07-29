using CoordinateSharp;
using MediatR;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.Components;
using Neon.HomeControl.Api.Core.Data.Commands;
using Neon.HomeControl.Api.Core.Data.Config;
using Neon.HomeControl.Api.Core.Enums;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Neon.HomeControl.Components.Config;
using Neon.HomeControl.Components.EventsDb;
using Neon.HomeControl.Components.Interfaces;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Neon.HomeControl.Components.Components
{
	[Component("sunset", "Sun Set Component", "1.0", "WEATHER", "Get sun set information", typeof(SunSetConfig))]
	public class SunSetComponent : ISunSetComponent, INotificationHandler<IotCommand<SunSetEd>>
	{
		private readonly IEventDatabaseService _eventDatabaseService;
		private readonly HttpClient _httpClient;
		private readonly IIoTService _ioTService;
		private readonly NeonConfig _neonConfig;
		private readonly ILogger _logger;
		private readonly ISchedulerService _schedulerService;

		public SunSetComponent(ILogger<SunSetComponent> logger, NeonConfig config, ISchedulerService schedulerService,
			IEventDatabaseService eventDatabaseService, IIoTService ioTService, IHttpClientFactory httpClientFactory)
		{
			_httpClient = httpClientFactory.CreateClient();
			_logger = logger;
			_neonConfig = config;
			_schedulerService = schedulerService;
			_eventDatabaseService = eventDatabaseService;
			_ioTService = ioTService;
		}

		public Task<bool> Start()
		{
			if (!string.IsNullOrEmpty(_neonConfig.Home.Name))
			{
				_schedulerService.AddPolling(GetSunSetJob, "SunSetUpdate",
					SchedulerServicePollingEnum.VERY_LONG_POLLING);
				GetSunSetJob();
			}


			return Task.FromResult(true);
		}

		public Task<bool> Stop()
		{
			_httpClient.Dispose();
			return Task.FromResult(true);
		}

		public Task InitConfiguration(object config)
		{
			return Task.CompletedTask;
		}

		public object GetDefaultConfig()
		{
			return new SunSetConfig { Enabled = true };
		}

		private async void GetSunSetJob()
		{
			var coordinates = new Coordinate(_neonConfig.Home.Latitude, _neonConfig.Home.Longitude,
				DateTime.Now.AddDays(1));
			var ed = new SunSetEd
			{
				EntityName = "SUNSET",

				Sunrise = coordinates.CelestialInfo.SunRise.Value.ToLocalTime(),
				Sunset = coordinates.CelestialInfo.SunSet.Value.ToLocalTime()
			};

			_ioTService.InsertEvent(ed);
		}

		public Task Handle(IotCommand<SunSetEd> notification, CancellationToken cancellationToken)
		{
			//throw new NotImplementedException();

			return Task.CompletedTask;
		}
	}
}