using Neon.HomeControl.Api.Core.Interfaces.Components;

namespace Neon.HomeControl.Api.Core.Impl.Components
{
	public class BaseComponentConfig : IComponentConfig
	{
		public BaseComponentConfig()
		{
			Enabled = true;
		}

		public bool Enabled { get; set; }
	}
}