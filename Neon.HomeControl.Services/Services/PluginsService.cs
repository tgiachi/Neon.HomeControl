using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Neon.HomeControl.Api.Core.Attributes.Services;
using Neon.HomeControl.Api.Core.Data.Config;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.ScriptService;
using Neon.HomeControl.Api.Core.Interfaces.Managers;
using Neon.HomeControl.Api.Core.Utils;

namespace Neon.HomeControl.Services.Services
{
	[Service(typeof(IPluginsService), LoadAtStartup = true, Name = "Plugins Manager service", Order = 2)]
	public class PluginsService : IPluginsService
	{

		private readonly List<Assembly> _pluginsAssemblies = new List<Assembly>();

		private readonly IFileSystemService _fileSystemService;
		private readonly IServicesManager _servicesManager;
		private readonly NeonConfig _neonConfig;

		private readonly ILogger _logger;

		public PluginsService(ILogger<PluginsService> logger, IFileSystemService fileSystemService, IServicesManager servicesManager,
			NeonConfig neonConfig)
		{
			_neonConfig = neonConfig;
			_logger = logger;
			_servicesManager = servicesManager;
			_fileSystemService = fileSystemService;
		}

		public Task<bool> Start()
		{
			_logger.LogInformation($"Plugins directory is: {_neonConfig.Plugins.Directory}");
			_fileSystemService.CreateDirectory(_neonConfig.Plugins.Directory);

			ScanPlugins();
			return Task.FromResult(true);
		}

		private void ScanPlugins()
		{
			_logger.LogInformation($"Scanning {_fileSystemService.BuildFilePath(_neonConfig.Plugins.Directory)} for plugins");

			var plugins =
				new DirectoryInfo(_fileSystemService.BuildFilePath(_neonConfig.Plugins.Directory)).GetFiles("*.dll",
					SearchOption.AllDirectories);


			plugins.ToList().ForEach(LoadPlugin);

			_logger.LogInformation($"Update container");
			UpdateContainer();

		}

		private void UpdateContainer()
		{
			var containerUpdater = new ContainerBuilder();

			_pluginsAssemblies.ForEach(assembly =>
				{
					var luaObjects = AssemblyUtils.GetTypesWithCustomAttribute(assembly, typeof(LuaScriptObjectAttribute));

					luaObjects.ForEach(t => { containerUpdater.RegisterType(t).AsSelf().SingleInstance(); });
				});


			containerUpdater.Update(_servicesManager.Container);

		}

		private void LoadPlugin(FileInfo file)
		{
			_logger.LogInformation($"Loading plugin name {file.Name}");

			try
			{
				var assembly = Assembly.LoadFile(file.FullName);
				AssemblyUtils.AddAssemblyToCache(assembly);
				_pluginsAssemblies.Add(assembly);
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error during loading plugin {file.Name} => {ex}");
			}
		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}
	}
}