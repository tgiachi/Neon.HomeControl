using Autofac;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.Components;
using Neon.HomeControl.Api.Core.Attributes.Database;
using Neon.HomeControl.Api.Core.Attributes.SchedulerJob;
using Neon.HomeControl.Api.Core.Attributes.ScriptService;
using Neon.HomeControl.Api.Core.Attributes.Services;
using Neon.HomeControl.Api.Core.Data.Config;
using Neon.HomeControl.Api.Core.Data.Services;
using Neon.HomeControl.Api.Core.Enums;
using Neon.HomeControl.Api.Core.Events.System;
using Neon.HomeControl.Api.Core.Impl.Dao;
using Neon.HomeControl.Api.Core.Impl.Dto;
using Neon.HomeControl.Api.Core.Interfaces;
using Neon.HomeControl.Api.Core.Interfaces.Dao;
using Neon.HomeControl.Api.Core.Interfaces.Database;
using Neon.HomeControl.Api.Core.Interfaces.Managers;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Neon.HomeControl.Api.Core.Modules;
using Neon.HomeControl.Api.Core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Neon.HomeControl.Api.Core.Attributes.ScriptEngine;

namespace Neon.HomeControl.Api.Core.Managers
{
	public class ServicesManager : IServicesManager
	{
		private readonly ILogger _logger;
		private List<Type> _availableServices = new List<Type>();

		private NeonConfig _neonConfig;

		public ServicesManager(ILogger<IServicesManager> logger, NeonConfig neonConfig)
		{
			_logger = logger;
			_neonConfig = neonConfig;
		}

		public ContainerBuilder ContainerBuilder { get; set; }

		public IContainer Container { get; set; }


		public ObservableCollection<ServiceInfo> ServicesInfo { get; set; }

		private void PrintHeader()
		{
			_logger.LogInformation(@"
 ____     ___   ___   ____  
|    \   /  _] /   \ |    \ 
|  _  | /  [_ |     ||  _  |
|  |  ||    _]|  O  ||  |  |
|  |  ||   [_ |     ||  |  |
|  |  ||     ||     ||  |  |
|__|__||_____| \___/ |__|__|
                            ");

			_logger.LogInformation($"v {AppUtils.AppVersion}");
		}



		public ContainerBuilder InitContainer()
		{
			var assemblies = AssemblyUtils.GetAppAssemblies();
			PrintHeader();
			EnhancedStackTrace.Current();
			InitPolly();
			ServicesInfo = new ObservableCollection<ServiceInfo>();
			ContainerBuilder = new ContainerBuilder();



			ContainerBuilder.RegisterModule(new LogRequestModule());
			ContainerBuilder.RegisterInstance(_neonConfig);
			ContainerBuilder.RegisterInstance<IServicesManager>(this);
			InitManagers();


			ContainerBuilder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();
			AssemblyUtils.GetAppAssemblies().ForEach(a =>
			{
				ContainerBuilder
					.RegisterAssemblyTypes(a)
					.AsClosedTypesOf(typeof(IRequestHandler<,>))
					.AsImplementedInterfaces().SingleInstance(); ;

				ContainerBuilder
					.RegisterAssemblyTypes(a)
					.AsClosedTypesOf(typeof(INotificationHandler<>))
					.AsImplementedInterfaces().SingleInstance();
			});

			// It appears Autofac returns the last registered types first
			ContainerBuilder.RegisterGeneric(typeof(RequestPostProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
			ContainerBuilder.RegisterGeneric(typeof(RequestPreProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));

			ContainerBuilder.Register<ServiceFactory>(ctx =>
			{
				var c = ctx.Resolve<IComponentContext>();
				return t => c.Resolve(t);
			});

			var singletonServices = AssemblyUtils.ScanAllAssembliesFromAttribute(typeof(SingletonAttribute));
			var transientServices = AssemblyUtils.ScanAllAssembliesFromAttribute(typeof(TransientAttribute));
			var scopedServices = AssemblyUtils.ScanAllAssembliesFromAttribute(typeof(ScopedAttribute));
			var dataAccess = AssemblyUtils.ScanAllAssembliesFromAttribute(typeof(DataAccessAttribute));
			var luaObjects = AssemblyUtils.ScanAllAssembliesFromAttribute(typeof(ScriptObjectAttribute));
			var jobObjects = AssemblyUtils.ScanAllAssembliesFromAttribute(typeof(SchedulerJobTaskAttribute));
			var dbSeedsObject = AssemblyUtils.ScanAllAssembliesFromAttribute(typeof(DatabaseSeedAttribute));
			var componentsObject = AssemblyUtils.ScanAllAssembliesFromAttribute(typeof(ComponentAttribute));
			var noSqlConnectors = AssemblyUtils.ScanAllAssembliesFromAttribute(typeof(NoSqlConnectorAttribute));
			var scriptEngines = AssemblyUtils.ScanAllAssembliesFromAttribute(typeof(ScriptEngineAttribute));

			_availableServices = AssemblyUtils.ScanAllAssembliesFromAttribute(typeof(ServiceAttribute));


			_logger.LogDebug($"Services  {_availableServices.Count}");
			_logger.LogDebug($"Singleton services {singletonServices.Count}");
			_logger.LogDebug($"Transient services {transientServices.Count}");
			_logger.LogDebug($"Scoped services {scopedServices.Count}");
			_logger.LogDebug($"DataAccesses  {dataAccess.Count}");


			luaObjects.ForEach(l => { ContainerBuilder.RegisterType(l).AsSelf().SingleInstance(); });
			singletonServices.ForEach(t => RegisterService(LifeScopeTypeEnum.SINGLETON, t));
			componentsObject.ForEach(t => RegisterService(LifeScopeTypeEnum.SINGLETON, t));
			scriptEngines.ForEach(s => RegisterService(LifeScopeTypeEnum.SINGLETON, s));

			ContainerBuilder.RegisterAssemblyTypes(AssemblyUtils.GetAppAssemblies().ToArray())
				.Where(t => t == typeof(IDatabaseSeed))
				.AsImplementedInterfaces()
				.SingleInstance();

			ContainerBuilder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
				.Where(t => t == typeof(IDataAccess<>))
				.AsImplementedInterfaces()
				.SingleInstance();

			ContainerBuilder.RegisterAssemblyTypes(AssemblyUtils.GetAppAssemblies().ToArray())
				.AsClosedTypesOf(typeof(IDataAccess<>)).AsImplementedInterfaces();

			ContainerBuilder.RegisterAssemblyTypes(AssemblyUtils.GetAppAssemblies().ToArray())
				.AsClosedTypesOf(typeof(AbstractDataAccess<>)).AsImplementedInterfaces();

			ContainerBuilder.RegisterAssemblyTypes(AssemblyUtils.GetAppAssemblies().ToArray())
				.AsClosedTypesOf(typeof(IDtoMapper<,>)).AsImplementedInterfaces();

			ContainerBuilder.RegisterAssemblyTypes(AssemblyUtils.GetAppAssemblies().ToArray())
				.AsClosedTypesOf(typeof(AbstractDtoMapper<,>)).AsImplementedInterfaces();


			dataAccess.ForEach(d =>
			{
				ContainerBuilder.RegisterType(d).As(AssemblyUtils.GetInterfaceOfType(d)).InstancePerLifetimeScope();
			});

			noSqlConnectors.ForEach(t => { ContainerBuilder.RegisterType(t).InstancePerDependency(); });

			transientServices.ForEach(t => RegisterService(LifeScopeTypeEnum.TRANSIENT, t));

			dbSeedsObject.ForEach(t => { ContainerBuilder.RegisterType(t).AsSelf().InstancePerLifetimeScope(); });

			scopedServices.ForEach(t => RegisterService(LifeScopeTypeEnum.SCOPED, t));

			jobObjects.ForEach(t => RegisterService(LifeScopeTypeEnum.SCOPED, t));

			ContainerBuilder.RegisterAssemblyTypes(AssemblyUtils.GetAppAssemblies().ToArray())
				.Where(t => t.Name.ToLower().EndsWith("service"))
				.AsImplementedInterfaces().SingleInstance();



			return ContainerBuilder;
		}

		private void InitPolly()
		{
			// TODO
		}

		private async void InitManagers()
		{
			var fileSystemManager = new FileSystemManager(_neonConfig, _logger);
			await fileSystemManager.Start();
			ContainerBuilder.RegisterInstance<IFileSystemManager>(fileSystemManager);

			var pluginsManager = new PluginsManager(_logger, fileSystemManager, this, _neonConfig);
			await pluginsManager.Start();
			ContainerBuilder.RegisterInstance<IPluginsManager>(pluginsManager);
		}

		public async Task<bool> Start()
		{
			var orderList = new Dictionary<int, List<Type>>();

			_availableServices.ToList().ForEach(s =>
			{
				var attribute = s.GetCustomAttribute<ServiceAttribute>();
				if (!orderList.ContainsKey(attribute.Order))
					orderList[attribute.Order] = new List<Type>();
				orderList[attribute.Order].Add(s);
			});

			var list = orderList.OrderBy(pair => pair.Key).ToList();


#if DEBUG
			foreach (var keyValuePair in list)
				keyValuePair.Value.ForEach(t =>
				{
					_logger.LogDebug($"ORDER {keyValuePair.Key} => {t.Name}");
				});
#endif

			foreach (var keyValuePair in list)
				foreach (var service in keyValuePair.Value)
				{
					var attribute = service.GetCustomAttribute<ServiceAttribute>();
					await StartService(service);

				}

			var notificationService = Resolve<INotificationService>();

			notificationService.BroadcastMessage(new ServiceLoadedEvent());

			return true;
		}

		public async Task<bool> Stop()
		{
			_logger.LogInformation("Stopping services...");
			foreach (var serviceInfo in ServicesInfo)
				try
				{
					await serviceInfo.Service.Stop();
				}
				catch (Exception ex)
				{
					_logger.LogError($"Error during shutdown service {serviceInfo.Name} => {ex.Message}");
				}

			return true;
		}

		public async Task StartService(Type serviceType)
		{
			var logger = Container.Resolve<ILogger<IServicesManager>>();
			var attribute = serviceType.GetCustomAttribute<ServiceAttribute>();
			logger.LogDebug($"Resolving service {attribute.ServiceInterface}");
			var service = Container.Resolve(attribute.ServiceInterface) as IService;
			var generatedId = Guid.NewGuid();
			ServicesInfo.Add(new ServiceInfo
			{
				Name = attribute.Name,
				ServiceId = generatedId,
				Status = ServiceStatusEnum.STARTING
			});

			try
			{
				logger.LogInformation($"Starting service {service.GetType().Name}");
				await service.Start();

				var _service = ServicesInfo.FirstOrDefault(s => s.ServiceId == generatedId);
				_service.Service = service;
			}
			catch (Exception ex)
			{
				var _service = ServicesInfo.FirstOrDefault(s => s.ServiceId == generatedId);
				logger.LogInformation($"Error during start service {service.GetType().Name} => {ex}");
				_service.Exception = ex;
				_service.Status = ServiceStatusEnum.ERROR;
			}
		}

		public async Task StartService<TService>()
		{
			await StartService(typeof(TService));
		}

		public void AddConfiguration(NeonConfig config)
		{
			_neonConfig = config;
		}


		public async void StopService(Guid serviceId)
		{
			var _service = ServicesInfo.FirstOrDefault(s => s.ServiceId == serviceId);

			await _service.Service.Stop();
			_service.Status = ServiceStatusEnum.STOPPED;
		}

		public T Resolve<T>()
		{
			return (T)Resolve(typeof(T));
		}

		public object Resolve(Type type)
		{
			if (Container == null)
				throw new Exception("Container is null, are you built container?");

			_logger.LogDebug($"Requesting service {type.Name}");
			//	using (var scope = Container.BeginLifetimeScope())
			//	{
			return Container.Resolve(type);
			//	}
		}


		public bool RegisterService(LifeScopeTypeEnum lifeScope, Type type)
		{
			var regType = ContainerBuilder.RegisterType(type);
			var haveInterface = false;
			if (AssemblyUtils.GetInterfaceOfType(type) != null)
			{
				regType = regType.As(AssemblyUtils.GetInterfaceOfType(type));
				haveInterface = true;
			}

			switch (lifeScope)
			{
				case LifeScopeTypeEnum.SINGLETON:
					regType.SingleInstance();

					if (haveInterface)
						ContainerBuilder.RegisterType(type).SingleInstance();
					break;
				case LifeScopeTypeEnum.SCOPED:
					regType.InstancePerLifetimeScope();
					if (haveInterface)
						ContainerBuilder.RegisterType(type).InstancePerLifetimeScope();

					break;
				case LifeScopeTypeEnum.TRANSIENT:
					regType.InstancePerRequest();
					if (haveInterface)
						ContainerBuilder.RegisterType(type).InstancePerRequest();

					break;
			}

			if (haveInterface)
				_logger.LogDebug($"Registering type {type.Name} => {AssemblyUtils.GetInterfaceOfType(type).Name}");
			else
				_logger.LogDebug($"Registering type {type.Name}");


			return true;
		}

		public bool RegisterService<T>(LifeScopeTypeEnum lifeScope)
		{
			return RegisterService(lifeScope, typeof(T));
		}

		public IContainer Build()
		{
			Container = ContainerBuilder.Build();

			return Container;
		}

		//private void BuildAutoMapper()
		//{

		//	var mappers = AssemblyUtils.ScanAllAssembliesFromAttribute(typeof(DtoMapAttribute));

		//	ContainerBuilder.Register(ctx =>
		//	{
		//		return new MapperConfiguration(cfg =>
		//		{
		//			mappers.ForEach(t =>
		//			{
		//				var attr = t.GetCustomAttribute<DtoMapAttribute>();
		//				cfg.CreateMap(t, attr.EntityType).ReverseMap();
		//				//cfg.CreateMap(t, attr.EntityType);
		//				//cfg.CreateMap(attr.EntityType, t);
		//			});
		//		});
		//	}); 

		//	ContainerBuilder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper()).As<IMapper>().InstancePerLifetimeScope();
		//}
	}
}