using System.Collections.Generic;

namespace Neon.HomeControl.Api.Core.Interfaces.Services
{
	public interface IDiscoveryService : IService
	{
		Dictionary<string, List<int>> PortsOpened { get; set; }
	}
}