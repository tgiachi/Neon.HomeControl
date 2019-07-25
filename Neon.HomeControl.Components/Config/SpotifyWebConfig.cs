using Neon.HomeControl.Api.Core.Impl.Components;
using System;

namespace Neon.HomeControl.Components.Config
{
	public class SpotifyWebConfig : BaseComponentConfig
	{
		public string ClientId { get; set; }

		public string ClientSecret { get; set; }

		public string TokenType { get; set; }

		public string AccessToken { get; set; }

		public string RefreshToken { get; set; }
		public DateTime ExpireOn { get; set; }
	}
}