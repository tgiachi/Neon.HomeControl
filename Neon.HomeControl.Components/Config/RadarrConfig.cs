using Neon.HomeControl.Api.Core.Impl.Components;

namespace Neon.HomeControl.Components.Config
{
	public class RadarrConfig : BaseComponentConfig
	{
		/// <summary>
		/// Host of Radarr
		/// </summary>
		public string Host { get; set; }

		/// <summary>
		/// Port of Radarr
		/// </summary>
		public int Port { get; set; }

		/// <summary>
		/// Api key of Radarr
		/// </summary>
		public string ApiKey { get; set; }
	}
}
