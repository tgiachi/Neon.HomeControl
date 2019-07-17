using Neon.HomeControl.Api.Core.Attributes.EventDatabase;
using Neon.HomeControl.Api.Core.Impl.EventsDatabase;

namespace Neon.HomeControl.Components.EventsDb
{
	[EventDatabaseEntity("weather")]
	public class WeatherEd : BaseEventDatabaseEntity
	{
		public double Humidity { get; set; }

		public double Temperature { get; set; }

		public string Status { get; set; }
	}
}