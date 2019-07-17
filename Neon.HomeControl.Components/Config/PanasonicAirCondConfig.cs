using Neon.HomeControl.Api.Core.Impl.Components;

namespace Neon.HomeControl.Components.Config
{
	public class PanasonicAirCondConfig : BaseComponentConfig
	{
		public string Username { get; set; }

		public string Password { get; set; }


		public string AuthCode { get; set; }
	}
}