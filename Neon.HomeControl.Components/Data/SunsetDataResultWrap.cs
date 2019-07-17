using Neon.HomeControl.Components.EventsDb;
using Newtonsoft.Json;

namespace Neon.HomeControl.Components.Data
{
	public class SunsetDataResultWrap
	{
		[JsonProperty("results")] public SunsetDataResults Results { get; set; }
	}
}