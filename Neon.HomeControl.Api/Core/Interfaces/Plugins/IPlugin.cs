using System.Threading.Tasks;

namespace Neon.HomeControl.Api.Core.Interfaces.Plugins
{
	public interface IPlugin
	{
		Task<bool> Start();

		Task<bool> Stop();
	}
}