using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using GoogleCast;
using GoogleCast.Channels;
using GoogleCast.Models.Media;
using Neon.HomeControl.Api.Core.Attributes.Components;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Neon.HomeControl.Components.Config;
using Neon.HomeControl.Components.EventsDb;
using Neon.HomeControl.Components.Interfaces;

namespace Neon.HomeControl.Components.Components
{
	/// <summary>
	/// Component for connect to chromecast and play media
	/// </summary>
	[Component("chromecast","Chromecast connector", "1.0", "STREAMING", "Control chromecast", typeof(ChromecastConfig))]
	public class ChromecastComponent : IChromecastComponent
	{

		private ChromecastConfig _config;
		private readonly IIoTService _ioTService;
		public ChromecastComponent(IIoTService ioTService)
		{
			_ioTService = ioTService;
		}

		public async Task<bool> Start()
		{
			if (_config.EnableDiscovery)
			{
				var sender = new Sender();
				var deviceLocator = new DeviceLocator();
				var receivers = await deviceLocator.FindReceiversAsync();

				receivers.ToList().ForEach(c =>
				{
					_ioTService.InsertEvent(new ChromecastEd()
					{
						DeviceId =  c.Id,
						Address = c.IPEndPoint.Address.ToString(),
						EntityName = c.Id,
						FriendlyName = c.FriendlyName
					});
				});
				
				//await sender.ConnectAsync(chrome);
				//var mediaChannel = sender.GetChannel<IMediaChannel>();
				//await sender.LaunchAsync(mediaChannel);
				//// Load and play Big Buck Bunny video
				//var mediaStatus = await mediaChannel.LoadAsync(
				//	new MediaInformation() { ContentId = "https://open.spotify.com/track/2iGcN8KNk58rsXLo1yubR7?si=MJ7RReqUQBWa03AfQ2UI2w" });
			}

			return true;
		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}

		public Task InitConfiguration(object config)
		{
			_config = (ChromecastConfig)config;

			return Task.CompletedTask;
		}

		public object GetDefaultConfig()
		{
			return new ChromecastConfig() { EnableDiscovery = true, Enabled = true };
		}
	}
}
