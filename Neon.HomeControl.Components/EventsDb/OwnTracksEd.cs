using Neon.HomeControl.Api.Core.Attributes.EventDatabase;
using Neon.HomeControl.Api.Core.Impl.EventsDatabase;

namespace Neon.HomeControl.Components.EventsDb
{
	[EventDatabaseEntity("owntracks")]
	public class OwnTracksEd : BaseEventDatabaseEntity
	{
		public double Latitude { get; set; }

		public double Longitude { get; set; }

		public int Altitude { get; set; }

		public int BatteryLevel { get; set; }

		public double DistanceFromHome { get; set; }
	}
}