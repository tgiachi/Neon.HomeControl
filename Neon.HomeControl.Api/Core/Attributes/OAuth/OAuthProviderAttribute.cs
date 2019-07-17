using System;

namespace Neon.HomeControl.Api.Core.Attributes.OAuth
{
	/// <summary>
	///     Attribute to create a callback during the oauth process
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class OAuthProviderAttribute : Attribute
	{
		/// <summary>
		///     ctor with name of OAuth provider
		/// </summary>
		/// <param name="name">Ex: Spotify, Facebook, ecc...</param>
		public OAuthProviderAttribute(string name)
		{
			Name = name;
		}

		/// <summary>
		///     Name of Provider
		/// </summary>
		public string Name { get; set; }
	}
}