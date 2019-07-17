using Neon.HomeControl.Api.Core.Attributes.EventDatabase;
using Neon.HomeControl.Api.Core.Impl.EventsDatabase;

namespace Neon.HomeControl.Components.EventsDb

{
	[EventDatabaseEntity("lights")]
	public class PhilipsHueEd : BaseEventDatabaseEntity
	{
		public bool IsOn { get; set; }

		public byte Brightness { get; set; }

		public int Hue { get; set; }

		public bool IsReachable { get; set; }

		public PhilipsHueTypeEnum Type { get; set; }
	}

	public enum PhilipsHueTypeEnum
	{
		LIGHT,
		GROUP
	}
}