using System;
using Neon.HomeControl.Api.Core.Attributes.EventDatabase;
using Neon.HomeControl.Api.Core.Impl.EventsDatabase;

namespace Neon.HomeControl.Components.EventsDb
{
	[EventDatabaseEntity("sunset")]
	public class SunSetEd : BaseEventDatabaseEntity
	{
		public DateTime Sunrise { get; set; }

		public DateTime Sunset { get; set; }
	}
}