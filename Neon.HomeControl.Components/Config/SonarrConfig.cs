using Neon.HomeControl.Api.Core.Impl.Components;

namespace Neon.HomeControl.Components.Config
{
	/// <summary>
	/// Config for Sonarr Component
	/// </summary>
	public class SonarrConfig : BaseComponentConfig
	{

		/// <summary>
		/// Host of Sonarr
		/// </summary>
		public string Host { get; set; }

		/// <summary>
		/// Port of Sonarr
		/// </summary>
		public int Port { get; set; }

		/// <summary>
		/// Api key of Sonarr
		/// </summary>
		public string ApiKey { get; set; }


	}
}
