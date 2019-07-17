using System.Threading.Tasks;

namespace Neon.HomeControl.Api.Core.Interfaces.Database
{
	public interface IDatabaseSeed
	{
		Task Seed();
	}
}