using Neon.HomeControl.Api.Core.Data.OAuth;

namespace Neon.HomeControl.Api.Core.Interfaces.OAuth
{
	public interface IOAuthCallback
	{
		void OnOAuthResult(OAuthResult result);
	}
}