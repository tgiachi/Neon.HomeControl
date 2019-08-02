using Neon.HomeControl.Api.Core.Attributes.EventDatabase;
using Neon.HomeControl.Api.Core.Impl.EventsDatabase;

namespace Neon.HomeControl.Components.EventsDb
{
	[EventDatabaseEntity("spotify_devices")]
	public class SpotifyDeviceEd : BaseEventDatabaseEntity
    {
        public string DeviceId { get; set; }

        public string DeviceName { get; set; }

		public string DeviceType { get; set; }

		public int VolumePercent { get; set; }

		public bool IsRestricted { get; set; }

		public bool IsActive { get; set; }
	}
}