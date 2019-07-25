using Neon.HomeControl.Api.Core.Data.Network;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reactive.Subjects;
using System.Threading;

namespace Neon.HomeControl.Api.Core.Utils
{
	public class NetworkScanner
	{
		private static CountdownEvent _countdown;
		private static int _upCount;
		private static readonly object LockObj = new object();
		public IObservable<NetworkResult> NetworkResultObservable = new ReplaySubject<NetworkResult>();


		public void ScanNetwork(string networkAddress)
		{
			_countdown = new CountdownEvent(1);
			var sw = new Stopwatch();
			sw.Start();

			var startAddress = networkAddress.Split('.')[0] + "." + networkAddress.Split('.')[1] + "." +
							   networkAddress.Split('.')[2] + ".";

			for (var i = 1; i < 255; i++)
			{
				var address = startAddress + i;
				var p = new Ping();

				p.PingCompleted += P_PingCompleted;
				_countdown.AddCount();
				p.SendAsync(address, 100, address);
			}

			_countdown.Signal();
			_countdown.Wait();
			sw.Stop();
			var span = new TimeSpan(sw.ElapsedTicks);
		}

		private void P_PingCompleted(object sender, PingCompletedEventArgs e)
		{
			var ip = (string)e.UserState;
			if (e.Reply != null && e.Reply.Status == IPStatus.Success)
			{
				string name;
				try
				{
					var hostEntry = Dns.GetHostEntry(ip);
					name = hostEntry.HostName;
				}
				catch (SocketException ex)
				{
					name = "?";
				}

				((ReplaySubject<NetworkResult>)NetworkResultObservable).OnNext(new NetworkResult
				{ IpAddress = ip, DnsName = name, Online = true, RoundTripTime = e.Reply.RoundtripTime });


				lock (LockObj)
				{
					_upCount++;
				}
			}
			else if (e.Reply == null)
			{
				((ReplaySubject<NetworkResult>)NetworkResultObservable).OnNext(new NetworkResult
				{ IpAddress = ip, Online = false });
			}

			_countdown.Signal();
		}
	}
}