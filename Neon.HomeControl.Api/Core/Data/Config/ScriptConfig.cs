using Neon.HomeControl.Api.Core.Interfaces.Services;

namespace Neon.HomeControl.Api.Core.Data.Config
{

	/// <summary>
	/// Config for <see cref="IScriptService"/> Script Service
	/// </summary>
	public class ScriptConfig
	{
		/// <summary>
		/// Directory where script is located
		/// </summary>
		public string Directory { get; set; }

		public ScriptConfig()
		{
			Directory = "Scripts";
		}
	}
}