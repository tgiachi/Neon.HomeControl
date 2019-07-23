using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Neon.HomeControl.Api.Core.Attributes.Components;
using Neon.HomeControl.Api.Core.Attributes.Services;
using Neon.HomeControl.Api.Core.Data.Components;
using Neon.HomeControl.Api.Core.Data.Config;
using Neon.HomeControl.Api.Core.Enums;
using Neon.HomeControl.Api.Core.Interfaces.Components;
using Neon.HomeControl.Api.Core.Interfaces.Managers;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Neon.HomeControl.Api.Core.Utils;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Events.System;

namespace Neon.HomeControl.Api.Core.Managers
{
	[Service(typeof(IComponentsService), LoadAtStartup = true, Name = "Components Service manager", Order = 3)]
	public class ComponentsService : IComponentsService, INotificationHandler<ServiceLoadedEvent>
	{
		private readonly Dictionary<ComponentInfo, Type> _componentsTypes;
		private readonly NeonConfig _config;
		private readonly IFileSystemService _fileSystemService;
		private readonly ILogger _logger;
		private readonly IServicesManager _servicesManager;

		public List<ComponentInfo> AvailableComponents { get; set; }
		public ObservableCollection<RunningComponentInfo> RunningComponents { get; set; }

		public ComponentsService(ILogger<ComponentsService> logger, IServicesManager servicesManager, NeonConfig config,
			IFileSystemService fileSystemService)
		{
			_logger = logger;
			_config = config;
			_servicesManager = servicesManager;
			_fileSystemService = fileSystemService;
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
			_fileSystemService.CreateDirectory(_config.Components.ConfigDirectory);
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
				var obj = _servicesManager.Resolve(type) as IComponent;
				var attr = type.GetCustomAttribute<ComponentAttribute>();


				RunningComponents.Add(runningComponent);
				runningComponent.Component = obj;
				var componentConfig = (IComponentConfig)LoadComponentConfig(attr.ComponentConfigType);

				if (componentConfig != null)
					await obj.InitConfiguration(componentConfig);
				else
				{
					componentConfig = (IComponentConfig)obj.GetDefaultConfig();
					SaveComponentConfig(componentConfig, attr.ComponentConfigType);
					await obj.InitConfiguration(componentConfig);
				}

				if (componentConfig != null && componentConfig.Enabled)
				{
					await obj.Start();
					runningComponent.Status = ComponentStatusEnum.STARTED;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error during start component {componentInfo.Name} => {ex.Message}");
				runningComponent.Error = ex;
				runningComponent.Status = ComponentStatusEnum.ERROR;
			}

		}

		private void SaveComponentConfig(object obj, Type configType)
		{
			_fileSystemService.SaveFile($"{_config.Components.ConfigDirectory}\\{configType.Name}.json",
				obj);
		}

		private object LoadComponentConfig(Type configType)
		{
			try
			{
				var config =
					_fileSystemService.LoadFile(
						$"{_config.Components.ConfigDirectory}\\{configType.Name.ToLower()}.json", configType);

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