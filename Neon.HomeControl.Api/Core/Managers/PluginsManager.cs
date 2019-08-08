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
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Neon.HomeControl.Api.Core.Managers
{
	public class PluginsManager : IPluginsManager
	{

		private readonly List<Assembly> _pluginsAssemblies = new List<Assembly>();

		private readonly IFileSystemManager _fileSystemManager;
		private readonly IServicesManager _servicesManager;
		private readonly NeonConfig _neonConfig;
		private readonly ILogger _logger;
		private ISettings _nugetSettings;
		private ISourceRepositoryProvider _nugetSourceRepositoryProvider;
		private NuGetFramework _nuGetFramework;



		public PluginsManager(ILogger logger, IFileSystemManager fileSystemManager, IServicesManager servicesManager,
			NeonConfig neonConfig)
		{
			
			_neonConfig = neonConfig;
			_logger = logger;
			_servicesManager = servicesManager;
			_fileSystemManager = fileSystemManager;
		}

		private void InitNuGet()
		{
			_nugetSettings = Settings.LoadDefaultSettings(root: null);
			_nugetSourceRepositoryProvider = new SourceRepositoryProvider(_nugetSettings, Repository.Provider.GetCoreV3());
			_nuGetFramework = NuGetFramework.ParseFolder("netstandard2.0");
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

			var pluginConfFilename = Path.Combine(file.DirectoryName, "plugin.conf");
			if (File.Exists(pluginConfFilename))
			{
				var pluginConf = JsonUtils.FromJson<PluginConfig>(File.ReadAllText(pluginConfFilename));
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
			else
			{
				_logger.LogWarning($"Can't load plugin {file.Name}. File plugin.conf is missing!");
			}

		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}
	}
}