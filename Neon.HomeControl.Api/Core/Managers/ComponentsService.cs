using MediatR;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.Components;
using Neon.HomeControl.Api.Core.Attributes.Services;
using Neon.HomeControl.Api.Core.Data.Components;
using Neon.HomeControl.Api.Core.Data.Config;
using Neon.HomeControl.Api.Core.Enums;
using Neon.HomeControl.Api.Core.Events.System;
using Neon.HomeControl.Api.Core.Interfaces.Components;
using Neon.HomeControl.Api.Core.Interfaces.Managers;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Neon.HomeControl.Api.Core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Neon.HomeControl.Api.Core.Managers
{
	[Service(typeof(IComponentsService), LoadAtStartup = true, Name = "Components Service manager", Order = 3)]
	public class ComponentsService : IComponentsService, INotificationHandler<ServiceLoadedEvent>
	{
		private readonly Dictionary<ComponentInfo, Type> _componentsTypes;
		private readonly ITaskExecutorService _taskExecutorService;
		private readonly NeonConfig _config;
		private readonly IFileSystemManager _fileSystemManager;
		private readonly ILogger _logger;
		private readonly IServicesManager _servicesManager;

		public List<ComponentInfo> AvailableComponents { get; set; }
		public ObservableCollection<RunningComponentInfo> RunningComponents { get; set; }

		public ComponentsService(ILogger<ComponentsService> logger,
			ITaskExecutorService taskExecutorService,
			IServicesManager servicesManager,
			NeonConfig config,
			IFileSystemManager fileSystemManager)
		{
			_logger = logger;
			_config = config;
			_servicesManager = servicesManager;
			_taskExecutorService = taskExecutorService;
			_fileSystemManager = fileSystemManager;
			AvailableComponents = new List<ComponentInfo>();
			RunningComponents = new ObservableCollection<RunningComponentInfo>();
			_componentsTypes = new Dictionary<ComponentInfo, Type>();
		}


		public void SaveComponentConfig<T>(T config) where T : IComponentConfig
		{
			SaveComponentConfig(config, typeof(T));
		}

		/// <summary>
		/// Start component
		/// </summary>
		/// <param name="componentId"></param>
		/// <returns></returns>
		public async Task<bool> StartComponent(string componentId)
		{
			var component = AvailableComponents.FirstOrDefault(c =>
				string.Equals(c.Id, componentId, StringComparison.CurrentCultureIgnoreCase));

			if (component == null)
				return false;

			if (RunningComponents.FirstOrDefault(c => c.Id == componentId) == null)
				await StartComponent(component, _componentsTypes[component]);


			return true;
		}

		public async Task<bool> Start()
		{
			_fileSystemManager.CreateDirectory(_config.Components.ConfigDirectory);
			ScanComponents();
			//await StartComponents();
			return true;
		}

		public async Task<bool> Stop()
		{
			foreach (var component in RunningComponents)
				if (component.Status == ComponentStatusEnum.STARTED)
					await component.Component.Stop();

			return true;
		}


		private void ScanComponents()
		{
			_logger.LogInformation("Scanning for components");
			AssemblyUtils.ScanAllAssembliesFromAttribute(typeof(ComponentAttribute)).ForEach(t =>
			{
				try
				{
					var attr = t.GetCustomAttribute<ComponentAttribute>();
					var componentInfo = new ComponentInfo
					{
						Id = attr.Id,
						Name = attr.Name,
						Version = attr.Version,
						Description = attr.Description
					};
					AvailableComponents.Add(componentInfo);
					_componentsTypes.Add(componentInfo, t);
				}
				catch (Exception ex)
				{
					_logger.LogError($"Error during initialize component {t.Name} => {ex}");
				}
			});
		}

		private async Task StartComponents()
		{
			foreach (var keyValuePair in _componentsTypes)
			{

				await StartComponent(keyValuePair.Key, keyValuePair.Value);
			}
		}

		private async Task StartComponent(ComponentInfo componentInfo, Type type)
		{
			await _taskExecutorService.Enqueue(async () =>
			{
				var sw = new Stopwatch();
				sw.Start();

				var runningComponent = new RunningComponentInfo
				{
					Id = componentInfo.Id,
					Name = componentInfo.Name,
					Version = componentInfo.Version,
					Description = componentInfo.Description,
					Status = ComponentStatusEnum.STOPPED
				};
				try
				{

					_logger.LogInformation($"Initialize component {componentInfo.Name} v{componentInfo.Version}");
					var obj = _servicesManager.Resolve(AssemblyUtils.GetInterfaceOfType(type)) as IComponent;
					var attr = type.GetCustomAttribute<ComponentAttribute>();



					runningComponent.Component = obj;
					RunningComponents.Add(runningComponent);
					var componentConfig = (IComponentConfig)LoadComponentConfig(attr.ComponentConfigType);


					if (componentConfig != null)
						await obj.InitConfiguration(componentConfig);
					else
					{
						_logger.LogWarning($"Component {obj.GetType().Name} don't have configuration");
						componentConfig = (IComponentConfig)obj.GetDefaultConfig();
						SaveComponentConfig(componentConfig, attr.ComponentConfigType);
						await obj.InitConfiguration(componentConfig);
					}

					if (componentConfig != null && componentConfig.Enabled)
					{
						await obj.Start();
						runningComponent.Status = ComponentStatusEnum.STARTED;
					}

					sw.Stop();

					_logger.LogInformation($"Component {componentInfo.Name} loaded id {sw.Elapsed}");
				}
				catch (Exception ex)
				{
					_logger.LogError($"Error during start component {componentInfo.Name} => {ex.Message}");
					runningComponent.Error = ex;
					runningComponent.Status = ComponentStatusEnum.ERROR;
				}
			});

		}

		private void SaveComponentConfig(object obj, Type configType)
		{
			_fileSystemManager.SaveFile($"{_config.Components.ConfigDirectory}{Path.DirectorySeparatorChar}{configType.Name}.json",
				obj);
		}

		private object LoadComponentConfig(Type configType)
		{
			try
			{
				var config =
					_fileSystemManager.LoadFile(
						$"{_config.Components.ConfigDirectory}{Path.DirectorySeparatorChar}{configType.Name}.json", configType);

				return config;
			}

			catch (Exception ex)
			{
				_logger.LogError($"Error during load config {configType.Name} => {ex}");
				return null;
			}
		}

		public async Task Handle(ServiceLoadedEvent notification, CancellationToken cancellationToken)
		{
			if (AvailableComponents.Count > 0)
				if (_config.AutoLoadComponents)
				{
					_logger.LogInformation($"Load components");
					await StartComponents();
					_logger.LogInformation($"Loaded {_componentsTypes.Count} components");
				}

		}

	}
}