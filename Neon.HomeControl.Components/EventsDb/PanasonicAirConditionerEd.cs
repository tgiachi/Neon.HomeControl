using Neon.HomeControl.Api.Core.Attributes.EventDatabase;
using Neon.HomeControl.Api.Core.Impl.EventsDatabase;

namespace Neon.HomeControl.Components.EventsDb
{
	[EventDatabaseEntity("panasonic_air_conditioners")]
	public class PanasonicAirConditioner : BaseEventDatabaseEntity
	{
		public string GroupName { get; set; }

		public string DeviceId { get; set; }

		public string OperationMode { get; set; }

		public double InsideTemperature { get; set; }

		public double OutTemperature { get; set; }

		public decimal TemperatureSet { get; set; }
	}
}