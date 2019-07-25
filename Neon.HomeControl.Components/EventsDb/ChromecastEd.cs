using Neon.HomeControl.Api.Core.Attributes.EventDatabase;
using Neon.HomeControl.Api.Core.Impl.EventsDatabase;

namespace Neon.HomeControl.Components.EventsDb
{
	[EventDatabaseEntity("chromecast")]
	public class ChromecastEd : BaseEventDatabaseEntity
	{
		public string DeviceId { get; set; }

		public string Address { get; set; }

		public string FriendlyName { get; set; }
	}
}
