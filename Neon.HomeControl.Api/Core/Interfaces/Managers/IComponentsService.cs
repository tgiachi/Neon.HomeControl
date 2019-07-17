using System.Collections.Generic;
using Neon.HomeControl.Api.Core.Data.Components;
using Neon.HomeControl.Api.Core.Interfaces.Components;
using Neon.HomeControl.Api.Core.Interfaces.Services;

namespace Neon.HomeControl.Api.Core.Interfaces.Managers
{
	public interface IComponentsService : IService
	{
		List<ComponentInfo> AvailableComponents { get; set; }

		List<RunningComponentInfo> RunningComponents { get; set; }


		void SaveComponentConfig<T>(T config) where T : IComponentConfig;
	}
}