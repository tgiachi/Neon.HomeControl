using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Neon.HomeControl.Api.Core.Attributes.Services;
using Neon.HomeControl.Api.Core.Data.Network;
using Neon.HomeControl.Api.Core.Events;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Neon.HomeControl.Api.Core.Utils;
using Microsoft.Extensions.Logging;

namespace Neon.HomeControl.Services.Services
{
	[Service(typeof(IDiscoveryService), LoadAtStartup = false, Name = "Discovery service")]
	public class DiscoveryService : IDiscoveryService
	{
		private readonly ILogger _logger;
		private readonly INotificationService _notificationService;
		private readonly ISchedulerService _schedulerService;

		public DiscoveryService(ILogger<DiscoveryService> logger, ISchedulerService schedulerService,
			INotificationService notificationService)
		{
			PortsOpened = new Dictionary<string, List<int>>();
			_logger = logger;
			_schedulerService = schedulerService;
			_notificationService = notificationService;
		}

		public Dictionary<string, List<int>> PortsOpened { get; set; }

		public Task<bool> Start()
		{
			//_schedulerService.AddJob(StartNetworkScan, 300, true);
			return Task.FromResult(true);
		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}

		private void StartNetworkScan()
		{
			GetLocalIps().ToList().ForEach(ip =>
			{
				_logger.LogInformation($"Starting network scan network Ip {ip}");

				var networkScanner = new NetworkScanner();
				networkScanner.NetworkResultObservable.Subscribe(OnNetworkScan);
				networkScanner.ScanNetwork(ip);
			});
		}

		private static IEnumerable<string> GetLocalIps()
		{
			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var ip in host.AddressList)
				if (ip.AddressFamily == AddressFamily.InterNetwork)
					yield return ip.ToString();
		}

		private async void OnNetworkScan(NetworkResult result)
		{
			if (result.Online)
			{
				_logger.LogDebug($"Host: {result.IpAddress} online");
				if (!PortsOpened.ContainsKey(result.IpAddress))
				{
					_notificationService.BroadcastMessage(new HostScanEvent
					{
						Host = result.IpAddress,
						DnsName = result.DnsName,
						IsOnline = result.Online
					});

					PortsOpened.Add(result.IpAddress, new List<int>());
				}

				var portScanner = new PortScanner(result.IpAddress, 20, 30000);
				await portScanner.ScanAsync(new Progress<PortScanner.PortScanResult>(scanResult =>
				{
					if (scanResult.IsPortOpen)
					{
						_logger.LogDebug($"Host: {result.IpAddress} {scanResult.PortNum} Opened");
						PortsOpened[result.IpAddress].Add(scanResult.PortNum);
						_notificationService.BroadcastMessage(new PortScanEvent
						{
							Host = result.IpAddress,
							Port = scanResult.PortNum
						});
					}
				}));
			}
		}
	}
}