using System.Threading.Tasks;

namespace Neon.HomeControl.Api.Core.Interfaces.Components
{
	public interface IComponent
	{
		/// <summary>
		/// Start component
		/// </summary>
		/// <returns></returns>
		Task<bool> Start();
		/// <summary>
		/// Stop component
		/// </summary>
		/// <returns></returns>
		Task<bool> Stop();

		/// <summary>
		/// Send to component the configuration
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		Task InitConfiguration(object config);

		/// <summary>
		/// If configuration is empty send default config
		/// </summary>
		/// <returns></returns>
		object GetDefaultConfig();
	}
}