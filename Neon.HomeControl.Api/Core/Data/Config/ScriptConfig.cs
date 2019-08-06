using Neon.HomeControl.Api.Core.Interfaces.Services;

namespace Neon.HomeControl.Api.Core.Data.Config
{

	/// <summary>
	/// Config for <see cref="IScriptService"/> Script Service
	/// </summary>
	public class ScriptConfig
	{

		public string EngineName { get; set; }

		/// <summary>
		/// Directory where script is located
		/// </summary>
		public string Directory { get; set; }

		public ScriptConfig()
		{
			EngineName = "lua";
			Directory = "Scripts";
		}
	}
}