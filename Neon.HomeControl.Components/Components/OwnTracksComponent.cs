using System;
using System.Threading.Tasks;
using CoordinateSharp;
using Neon.HomeControl.Api.Core.Attributes.Components;
using Neon.HomeControl.Api.Core.Data.Config;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Neon.HomeControl.Api.Core.Utils;
using Neon.HomeControl.Components.Config;
using Neon.HomeControl.Components.Data;
using Neon.HomeControl.Components.EventsDb;
using Neon.HomeControl.Components.Interfaces;
using Microsoft.Extensions.Logging;

namespace Neon.HomeControl.Components.Components
{
	[Component("owntracks","OwnTrack component", "1.0", "Presence", "Presence", typeof(OwnTracksConfig))]
	public class OwnTracksComponent : IOwnTracksComponent
	{
		private readonly IEventDatabaseService _eventDatabaseService;
		private readonly IIoTService _ioTService;
		private readonly NeonConfig _neonConfig;
		private readonly ILogger _logger;
		private readonly IMqttService _mqttService;
		private OwnTracksConfig _config;

		public OwnTracksComponent(ILogger<OwnTracksComponent> logger, IMqttService mqttService,
			IEventDatabaseService eventDatabaseService, IIoTService ioTService, NeonConfig neonConfig)
		{
			_logger = logger;
			_mqttService = mqttService;
			_eventDatabaseService = eventDatabaseService;
			_ioTService = ioTService;
			_neonConfig = neonConfig;
		}

		public Task<bool> Start()
		{
			_mqttService.SubscribeTopic(_config.DefaultOwnTrackTopic);
			_mqttService.OnMqttMessage.Subscribe(message =>
			{
				if (message.Topic.Contains("owntracks/user/"))
				{
					var homeCoordinate = new Coordinate(_neonConfig.Home.Latitude, _neonConfig.Home.Longitude);

					var ownTrackData = message.Message.FromJson<OwnTracksData>();
					var deviceCoordinate = new Coordinate(ownTrackData.Latitude, ownTrackData.Longitude);

					var ev = new OwnTracksEd
					{
						Longitude = ownTrackData.Longitude,
						Latitude = ownTrackData.Latitude,
						EntityName = ownTrackData.IdName,
						BatteryLevel = ownTrackData.Battery,
						Altitude = ownTrackData.Altitude,
						DistanceFromHome = new Distance(homeCoordinate, deviceCoordinate).Kilometers
					};


					_ioTService.InsertEvent(ev);
					_logger.LogInformation("OwnTracks!");
				}
			});

			return Task.FromResult(true);
		}

		public Task<bool> Stop()
		{
			throw new NotImplementedException();
		}

		public Task InitConfiguration(object config)
		{
			_config = (OwnTracksConfig) config;

			return Task.CompletedTask;
		}

		public object GetDefaultConfig()
		{
			return new OwnTracksConfig
			{
				DefaultOwnTrackTopic = "owntracks/user/$"
			};
		}
	}
}