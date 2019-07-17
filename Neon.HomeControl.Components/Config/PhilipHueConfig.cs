using Neon.HomeControl.Api.Core.Impl.Components;

namespace Neon.HomeControl.Components.Config
{
	public class PhilipHueConfig : BaseComponentConfig
	{
		public string BridgeIpAddress { get; set; }

		public string ApiKey { get; set; }
	}
}