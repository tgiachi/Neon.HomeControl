using Neon.HomeControl.Api.Core.Impl.Components;

namespace Neon.HomeControl.Components.Config
{
	public class SonoffTasmodaConfig : BaseComponentConfig
	{
		public bool EnabledDiscovery { get; set; } = true;

		public string BaseTopic { get; set; }

		public string StateTopic { get; set; }

		public string UptimeTopic { get; set; }
	}
}