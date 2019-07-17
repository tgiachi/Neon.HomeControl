using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
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

namespace Neon.HomeControl.Api.Core.Managers
{
	[Service(typeof(IComponentsService), LoadAtStartup = true, Name = "Components Service manager")]
	public class ComponentsService : IComponentsService
	{
		private readonly Dictionary<ComponentInfo, Type> _componentsTypes;
		private readonly NeonConfig _config;
		private readonly IFileSystemService _fileSystemService;
		private readonly ILogger _logger;
		private readonly IServicesManager _servicesManager;


		public ComponentsService(ILogger<ComponentsService> logger, IServicesManager servicesManager, NeonConfig config,
			IFileSystemService fileSystemService)
		{
			_logger = logger;
			_config = config;
			_servicesManager = servicesManager;
			_fileSystemService = fileSystemService;
			AvailableComponents = new List<ComponentInfo>();
			RunningComponents = new List<RunningComponentInfo>();
			_componentsTypes = new Dictionary<ComponentInfo, Type>();
		}

		public List<ComponentInfo> AvailableComponents { get; set; }

		public List<RunningComponentInfo> RunningComponents { get; set; }

		public void SaveComponentConfig<T>(T config) where T : IComponentConfig
		{
			SaveComponentConfig(config, typeof(T));
		}

		public async Task<bool> Start()
		{
			_fileSystemService.CreateDirectory(_config.Components.ConfigDirectory);
			ScanComponents();
			await StartComponents();
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
				var runningComponent = new RunningComponentInfo
				{
					Name = keyValuePair.Key.Name,
					Version = keyValuePair.Key.Version,
					Description = keyValuePair.Key.Description,
					Status = ComponentStatusEnum.STOPPED
				};
				try
				{
					_logger.LogInformation($"Initialize component {keyValuePair.Key.Name} v{keyValuePair.Key.Version}");
					var obj = _servicesManager.Resolve(keyValuePair.Value) as IComponent;
					var attr = keyValuePair.Value.GetCustomAttribute<ComponentAttribute>();


					RunningComponents.Add(runningComponent);
					runningComponent.Component = obj;
					var componentConfig = (IComponentConfig) LoadComponentConfig(attr.ComponentConfigType);

					if (componentConfig != null)
						await obj.InitConfiguration(componentConfig);
					else
						SaveComponentConfig(obj.GetDefaultConfig(), attr.ComponentConfigType);

					if (componentConfig != null && componentConfig.Enabled)
					{
						await obj.Start();
						runningComponent.Status = ComponentStatusEnum.STARTED;
					}
				}
				catch (Exception ex)
				{
					_logger.LogError($"Error during start component {keyValuePair.Value.Name} => {ex.Message}");
					runningComponent.Error = ex;
					runningComponent.Status = ComponentStatusEnum.ERROR;
				}
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
	}
}