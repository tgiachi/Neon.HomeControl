using Newtonsoft.Json;

namespace Neon.HomeControl.Components.Data
{
	public class OwnTracksData
	{
		[JsonProperty("tid")] public string IdName { get; set; }

		[JsonProperty("batt")] public int Battery { get; set; }

		[JsonProperty("alt")] public int Altitude { get; set; }

		[JsonProperty("lon")] public double Longitude { get; set; }

		[JsonProperty("lat")] public double Latitude { get; set; }

		[JsonProperty("acc")] public int Accuracy { get; set; }
	}
}