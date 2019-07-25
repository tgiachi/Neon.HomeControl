using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Data.Config;
using Neon.HomeControl.Api.Core.Interfaces.Managers;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Neon.HomeControl.Api.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Neon.HomeControl.Api.Core.Managers
{
	public class PluginsManager : IPluginsManager
	{

		private readonly List<Assembly> _pluginsAssemblies = new List<Assembly>();

		private readonly IFileSystemManager _fileSystemManager;
		private readonly IServicesManager _servicesManager;
		private readonly NeonConfig _neonConfig;

		private readonly ILogger _logger;

		public PluginsManager(ILogger logger, IFileSystemManager fileSystemManager, IServicesManager servicesManager,
			NeonConfig neonConfig)
		{
			_neonConfig = neonConfig;
			_logger = logger;
			_servicesManager = servicesManager;
			_fileSystemManager = fileSystemManager;
		}

		public Task<bool> Start()
		{
			_logger.LogInformation($"Plugins directory is: {_neonConfig.Plugins.Directory}");
			_fileSystemManager.CreateDirectory(_neonConfig.Plugins.Directory);

			ScanPlugins();
			return Task.FromResult(true);
		}

		private void ScanPlugins()
		{
			_logger.LogInformation($"Scanning {_fileSystemManager.BuildFilePath(_neonConfig.Plugins.Directory)} for plugins");

			var plugins =
				new DirectoryInfo(_fileSystemManager.BuildFilePath(_neonConfig.Plugins.Directory)).GetFiles("*.dll",
					SearchOption.AllDirectories);


			plugins.ToList().ForEach(LoadPlugin);

			_logger.LogInformation($"Update container");

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