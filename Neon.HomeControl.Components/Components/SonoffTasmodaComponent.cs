using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.Components;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Neon.HomeControl.Api.Core.Utils;
using Neon.HomeControl.Components.Config;
using Neon.HomeControl.Components.EventsDb;
using Neon.HomeControl.Components.Interfaces;
using Neon.HomeControl.Components.Mqtt;
using System;
using System.Globalization;
using System.Threading.Tasks;
using MediatR;
using Neon.HomeControl.Api.Core.Attributes.Commands;

namespace Neon.HomeControl.Components.Components
{
	[Component("sonoff_tasmoda", "Sonoff-Tasmoda", "1.0", "IOT", "Connect and control Sonoff Tasmoda IoT", typeof(SonoffTasmodaConfig))]
	public class SonoffTasmodaComponent : ISonoffTasmodaComponent
	{
		private readonly IEventDatabaseService _eventDatabaseService;
		private readonly IIoTService _ioTService;
		private readonly ILogger _logger;
		private readonly IMqttService _mqttService;
		private SonoffTasmodaConfig _config;

		public SonoffTasmodaComponent(ILogger<SonoffTasmodaComponent> logger, IMqttService mqttService,
			IIoTService ioTService, IEventDatabaseService eventDatabaseService)
		{
			_logger = logger;
			_mqttService = mqttService;
			_ioTService = ioTService;
			_eventDatabaseService = eventDatabaseService;
		}

		public Task<bool> Start()
		{
			if (_config.EnabledDiscovery)
			{
				_logger.LogInformation("Enabled discovery for network");
				_mqttService.SubscribeTopic(_config.BaseTopic);
				_mqttService.OnMqttMessage.Subscribe(message =>
				{
					if (message.Topic.StartsWith("tele/"))
						ParseMqttMessage(message.Topic, message.Message);
				});
			}

			return Task.FromResult(true);
		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}

		public Task InitConfiguration(object config)
		{
			_config = config as SonoffTasmodaConfig;

			return Task.CompletedTask;
		}

		public object GetDefaultConfig()
		{
			return new SonoffTasmodaConfig
			{
				Enabled = true,
				EnabledDiscovery = true,
				BaseTopic = "tele/+/#",
				StateTopic = "STATE",
				UptimeTopic = "UPTIME"
			};
		}


		private void ParseMqttMessage(string topic, string message)
		{
			topic = topic.Replace("tele/", "");
			var entityName = topic.Split('/')[0];
			var fromTopic = topic.Split('/')[1];

			if (fromTopic == _config.StateTopic)
			{
				var status = message.FromJson<SonoffStateMessage>();
				var ed = new SonoffStatusEd
				{
					EntityName = entityName,
					LoadAvg = status.LoadAvg,
					Power1 = status.Power1,
					SleepMode = status.SleepMode,
					Time = status.Time,
					Uptime = TimeSpan.ParseExact(status.Uptime.Split('T')[1], "g", CultureInfo.InvariantCulture)
				};

				_ioTService.InsertEvent(ed);
			}
		}

		[IotCommand("POWER", typeof(SonoffStatusEd), "Send message to Tasmoda-Sonoff")]
		[IotCommandParam("Device", true)]
		[IotCommandParam("Channel", true)]
		[IotCommandParam("Power_status", true)]
		public void SendPower(SonoffStatusEd entity, string commandName, params object[] args)
		{
			var device = args[0] as string;
			var channel = args[1] as string;
			var powerStatus = args[2] as string;
			_mqttService.SendMessage($"/cmnd/{device}/power{channel}", powerStatus);
		}

	}
}