using Neon.HomeControl.Api.Core.Impl.Components;

namespace Neon.HomeControl.Components.Config
{
	public class WeatherComponentConfig : BaseComponentConfig
	{
		public string ApiKey { get; set; } = "ChangeMe";

		public double Latitude { get; set; } = 0.0d;

		public double Longitude { get; set; } = 0.0d;
	}
}