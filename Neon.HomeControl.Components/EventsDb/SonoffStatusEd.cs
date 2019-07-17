using System;
using Neon.HomeControl.Api.Core.Attributes.EventDatabase;
using Neon.HomeControl.Api.Core.Impl.EventsDatabase;
using Newtonsoft.Json;

namespace Neon.HomeControl.Components.EventsDb
{
	[EventDatabaseEntity("sonoff")]
	public class SonoffStatusEd : BaseEventDatabaseEntity
	{
		public DateTime Time { get; set; }

		public TimeSpan Uptime { get; set; }

		public double Vcc { get; set; }

		public string SleepMode { get; set; }

		public double LoadAvg { get; set; }

		[JsonProperty("POWER1")] public string Power1 { get; set; }
	}
}