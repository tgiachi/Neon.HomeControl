using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Neon.HomeControl.Api.Core.Utils
{
	/// <summary>
	///     Use Sockets to Scan the Ports on a Machine.
	/// </summary>
	/// <remarks>
	///     From the Oreilly C# 6.0 Cookbook
	///     https://github.com/oreillymedia/c_sharp_6_cookbook
	///     http://shop.oreilly.com/product/0636920037347.do
	/// </remarks>
	public class PortScanner
	{
		private const int PortMinValue = 1;
		private const int PortMaxValue = 65535;
		private List<int> _closedPorts;

		private List<int> _openPorts;

		public PortScanner()
		{
			// defaults are already set for ports & localhost
			SetupLists();
		}

		public PortScanner(string host, int minPort, int maxPort)
		{
			if (minPort > maxPort)
				throw new ArgumentException("Min port cannot be greater than max port");

			if (minPort < PortMinValue || minPort > PortMaxValue)
				throw new ArgumentOutOfRangeException(
					$"Min port cannot be less than {PortMinValue} " +
					$"or greater than {PortMaxValue}");

			if (maxPort < PortMinValue || maxPort > PortMaxValue)
				throw new ArgumentOutOfRangeException(
					$"Max port cannot be less than {PortMinValue} " +
					$"or greater than {PortMaxValue}");

			Host = host;
			MinPort = minPort;
			MaxPort = maxPort;

			SetupLists();
		}

		public ReadOnlyCollection<int> OpenPorts => new ReadOnlyCollection<int>(_openPorts);
		public ReadOnlyCollection<int> ClosedPorts => new ReadOnlyCollection<int>(_closedPorts);

		public int MinPort { get; } = PortMinValue;
		public int MaxPort { get; } = PortMaxValue;

		public string Host { get; } = "127.0.0.1"; // localhost

		private void SetupLists()
		{
			// set up lists with capacity to hold half of range
			// since we can't know how many ports are going to be open
			// so we compromise and allocate enough for half

			// rangeCount is max - min + 1
			var rangeCount = MaxPort - MinPort + 1;

			// if there are an odd number, bump by one to get one extra slot
			if (rangeCount % 2 != 0) rangeCount += 1;

			// reserve half the ports in the range for each
			_openPorts = new List<int>(rangeCount / 2);
			_closedPorts = new List<int>(rangeCount / 2);
		}

		private async Task CheckPortAsync(int port, IProgress<PortScanResult> progress)
		{
			if (await IsPortOpenAsync(port))
			{
				// if we got here it is open
				_openPorts.Add(port);

				// notify anyone paying attention
				progress?.Report(new PortScanResult {PortNum = port, IsPortOpen = true});
			}
			else
			{
				// server doesn't have that port open
				_closedPorts.Add(port);
				progress?.Report(new PortScanResult {PortNum = port, IsPortOpen = false});
			}
		}

		private async Task<bool> IsPortOpenAsync(int port)
		{
			Socket socket = null;

			try
			{
				// make a TCP based socket
				socket = new Socket(AddressFamily.InterNetwork, SocketType
					.Stream, ProtocolType.Tcp);

				// connect
				await socket.ConnectAsync(Host, port);

				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
			finally
			{
				if (socket?.Connected ?? false) socket?.Disconnect(false);
				socket?.Close();
			}
		}

		public async Task ScanAsync(IProgress<PortScanResult> progress)
		{
			for (var port = MinPort; port <= MaxPort; port++)
				await CheckPortAsync(port, progress);
		}

		public class PortScanResult
		{
			public int PortNum { get; set; }
			public bool IsPortOpen { get; set; }
		}
	}
}