using Neon.HomeControl.Api.Core.Data.Components;
using Neon.HomeControl.Api.Core.Interfaces.Components;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Neon.HomeControl.Api.Core.Interfaces.Managers
{
	public interface IComponentsService : IService
	{
		List<ComponentInfo> AvailableComponents { get; set; }

		ObservableCollection<RunningComponentInfo> RunningComponents { get; set; }

		void SaveComponentConfig<T>(T config) where T : IComponentConfig;

		Task<bool> StartComponent(string componentId);

	}
}