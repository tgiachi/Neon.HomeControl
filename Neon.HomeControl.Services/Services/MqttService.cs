using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Neon.HomeControl.Api.Core.Attributes.Services;
using Neon.HomeControl.Api.Core.Data.Config;
using Neon.HomeControl.Api.Core.Data.Mqtt;
using Neon.HomeControl.Api.Core.Events;
using Neon.HomeControl.Api.Core.Interfaces.Managers;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Neon.HomeControl.Api.Core.Utils;
using System;
using System.Net;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Neon.HomeControl.Services.Services
{
	[Service(typeof(IMqttService), Name = "Mqtt Service", LoadAtStartup = true, Order = 4)]
	public class MqttService : IMqttService
	{
		private readonly NeonConfig _neonConfig;
		private readonly ILogger _logger;
		private readonly IServicesManager _servicesManager;

		private IMqttServer _mqttServer;
		private IMqttClient _mqttClient;
		private INotificationService _notificationService;

		private int _reconnectTry;

		public MqttService(ILogger<MqttService> logger, NeonConfig neonConfig, IServicesManager servicesManager)
		{
			_logger = logger;
			_servicesManager = servicesManager;
			_neonConfig = neonConfig;
			OnMqttMessage = new ReplaySubject<MqttMessage>();
		}

		public IObservable<MqttMessage> OnMqttMessage { get; set; }

		public async Task<bool> SubscribeTopic(string topic)
		{
			if (_mqttClient.IsConnected)
			{
				var result = await _mqttClient.SubscribeAsync(topic, MqttQualityOfServiceLevel.ExactlyOnce);

				_logger.LogDebug($"Topic {topic} subcribed {result.Items[0].ResultCode}");

				return true;
			}

			_logger.LogWarning($"MQTT Queue not connected, can't subscribe to {topic}");
			return false;
		}


		public async Task<bool> SendMessage(string topic, object message)
		{
			if (_mqttClient.IsConnected)
			{
				await _mqttClient.PublishAsync(topic, message.ToJson());
				return true;
			}

			throw new Exception("MQTT not connected!");
		}

		public async Task<bool> Start()
		{
			if (_neonConfig.Mqtt.RunEmbedded)
			{
				var mqttOptionBuilder = new MqttServerOptionsBuilder()
					.WithConnectionBacklog(100)
					.WithDefaultEndpointBoundIPAddress(IPAddress.Parse("0.0.0.0"))
					.WithDefaultEndpointPort(1883);
				_mqttServer = new MqttFactory().CreateMqttServer();
				_logger.LogInformation($"Starting embedded MQTT Server");
				await _mqttServer.StartAsync(mqttOptionBuilder.Build());
				_logger.LogInformation($"Embedded MQTT Server started");
				_neonConfig.Mqtt.Host = "127.0.0.1";
			}


			_logger.LogInformation($"Connecting to {_neonConfig.Mqtt.Host} with clientId: {_neonConfig.Mqtt.ClientId}");

			var options = new MqttClientOptionsBuilder()
				.WithClientId(_neonConfig.Mqtt.ClientId)
				.WithTcpServer(_neonConfig.Mqtt.Host)
				.WithCleanSession()
				.Build();

			_mqttClient = new MqttFactory().CreateMqttClient();
			_mqttClient.UseDisconnectedHandler(async e =>
			{
				if (_reconnectTry <= 5)
				{
					_logger.LogWarning("Disconnected from MQTT Server, reconnecting in 10 seconds");
					await Task.Delay(10000);
					await Start();

					_reconnectTry++;
				}
				else
				{
					_logger.LogWarning("Max number reconnect try to MQTT Server, abort");
				}
			});

			_mqttClient.UseConnectedHandler(async e =>
			{
				await _mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("Neon.iot").Build());
				await _mqttClient.PublishAsync(new MqttApplicationMessageBuilder().WithTopic("Neon.iot")
					.WithPayload("connected!").WithExactlyOnceQoS().WithRetainFlag().Build());
			});
			_mqttClient.UseApplicationMessageReceivedHandler(args => { OnMessageReceived(args.ApplicationMessage); });
			await _mqttClient.ConnectAsync(options);

			_logger.LogInformation("Connected");
			return true;
		}

		public async Task<bool> Stop()
		{
			_logger.LogInformation("Disconnecting from MQTT queue");
			await _mqttClient.DisconnectAsync();
			return true;
		}

		private void OnMessageReceived(MqttApplicationMessage message)
		{
			_logger.LogDebug($"Received message from topic {message.Topic} {Encoding.UTF8.GetString(message.Payload)}");
			((ReplaySubject<MqttMessage>)OnMqttMessage).OnNext(new MqttMessage
			{ Message = Encoding.UTF8.GetString(message.Payload), Topic = message.Topic });

			_notificationService = _servicesManager.Resolve<INotificationService>();
			_notificationService.BroadcastMessage(new MqttMessageEvent
			{
				Topic = message.Topic,
				Message = Encoding.UTF8.GetString(message.Payload)
			});
		}
	}
}