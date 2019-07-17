using System.Threading.Tasks;

namespace Neon.HomeControl.Api.Core.Interfaces.Components
{
	public interface IComponent
	{
		Task<bool> Start();
		Task<bool> Stop();

		Task InitConfiguration(object config);

		object GetDefaultConfig();
	}
}