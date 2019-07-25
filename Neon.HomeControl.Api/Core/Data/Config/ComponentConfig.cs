namespace Neon.HomeControl.Api.Core.Data.Config
{
	/// <summary>
	/// Components config
	/// </summary>
	public class ComponentConfig
	{
		/// <summary>
		/// Where Neon Load/Save component's config
		/// </summary>
		public string ConfigDirectory { get; set; }


		/// <summary>
		/// Default component config if Components
		/// </summary>
		public ComponentConfig()
		{
			ConfigDirectory = "Components";
		}
	}
}