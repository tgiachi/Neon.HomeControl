using Makaretu.Dns;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.Services;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Neon.HomeControl.Services.Services
{
	[Service(typeof(IDiscoveryService), LoadAtStartup = true, Name = "Discovery service")]
	public class DiscoveryService : IDiscoveryService
	{
		private readonly ILogger _logger;
		private readonly INotificationService _notificationService;
		private readonly ISchedulerService _schedulerService;

		private MulticastService _multicastService;
		private ServiceDiscovery _serviceDiscovery;

		public DiscoveryService(ILogger<DiscoveryService> logger, ISchedulerService schedulerService,
			INotificationService notificationService)
		{
			_logger = logger;
			_schedulerService = schedulerService;
			_notificationService = notificationService;

		}


		public Task<bool> Start()
		{
			_multicastService = new MulticastService();
			_serviceDiscovery = new ServiceDiscovery();

			_multicastService.NetworkInterfaceDiscovered += (sender, args) =>
			{
				_serviceDiscovery.QueryAllServices();
			};
			_serviceDiscovery.ServiceDiscovered += (s, serviceName) =>
			{
				_logger.LogInformation($"service '{serviceName}'");

				// Ask for the name of instances of the service.
				_multicastService.SendQuery(serviceName, type: DnsType.PTR);
			};

			_serviceDiscovery.ServiceInstanceDiscovered += (s, e) =>
			{
				_logger.LogInformation($"service instance '{e.ServiceInstanceName}'");

				// Ask for the service instance details.
				_multicastService.SendQuery(e.ServiceInstanceName, type: DnsType.SRV);
			};

			_multicastService.Start();

			return Task.FromResult(true);
		}

		public Task<bool> Stop()
		{
			_multicastService.Stop();
			_serviceDiscovery.Dispose();
			return Task.FromResult(true);
		}

		private static IEnumerable<string> GetLocalIps()
		{
			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var ip in host.AddressList)
				if (ip.AddressFamily == AddressFamily.InterNetwork)
					yield return ip.ToString();
		}

	}
}