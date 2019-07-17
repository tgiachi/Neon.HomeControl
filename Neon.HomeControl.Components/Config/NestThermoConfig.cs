using Neon.HomeControl.Api.Core.Impl.Components;

namespace Neon.HomeControl.Components.Config
{
	public class NestThermoConfig : BaseComponentConfig
	{
		public string ClientId { get; set; }

		public string ClientSecret { get; set; }
		public string Token { get; set; }
	}
}