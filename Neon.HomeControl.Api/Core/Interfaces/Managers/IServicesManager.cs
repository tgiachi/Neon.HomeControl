using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Autofac;
using Neon.HomeControl.Api.Core.Data.Config;
using Neon.HomeControl.Api.Core.Data.Services;
using Neon.HomeControl.Api.Core.Enums;

namespace Neon.HomeControl.Api.Core.Interfaces.Managers
{
	public interface IServicesManager
	{
		IContainer Container { get; set; }

		ObservableCollection<ServiceInfo> ServicesInfo { get; set; }
		ContainerBuilder InitContainer();

		bool RegisterService(LifeScopeTypeEnum lifeScope, Type type);

		bool RegisterService<T>(LifeScopeTypeEnum lifeScope);

		IContainer Build();

		Task<bool> Start();

		Task<bool> Stop();

		T Resolve<T>();

		object Resolve(Type type);

		void StopService(Guid serviceId);

		Task StartService(Type serviceType);

		Task StartService<TService>();

		void AddConfiguration(NeonConfig config);
	}
}