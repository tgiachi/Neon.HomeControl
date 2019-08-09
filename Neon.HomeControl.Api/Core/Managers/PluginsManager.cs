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
using System.Threading;
using System.Threading.Tasks;
using Neon.HomeControl.Api.Core.Data.Plugins;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Packaging.Signing;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using NuGet.Versioning;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using PluginConfig = Neon.HomeControl.Api.Core.Data.Config.PluginConfig;

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
		private PackagePathResolver _packagePathResolver;
		private PackageExtractionContext _packageExtractionContext;
		private string _packagesDirectory;

		private readonly List<string> _assembliesToLoad = new List<string>();


		public PluginsManager(ILogger logger, IFileSystemManager fileSystemManager, IServicesManager servicesManager,
			NeonConfig neonConfig)
		{

			_neonConfig = neonConfig;
			_logger = logger;
			_servicesManager = servicesManager;
			_fileSystemManager = fileSystemManager;


			InitNuGet();
		}

		private void InitNuGet()
		{
			_nugetSettings = Settings.LoadDefaultSettings(root: null);
			_nugetSourceRepositoryProvider = new SourceRepositoryProvider(_nugetSettings, Repository.Provider.GetCoreV3());
			_nuGetFramework = NuGetFramework.ParseFolder("netstandard2.0");
			_packagesDirectory = _fileSystemManager.BuildFilePath(Path.Combine(_neonConfig.Plugins.Directory, "packages"));

			_fileSystemManager.CreateDirectory(Path.Combine(_neonConfig.Plugins.Directory, "packages"));
			_packagePathResolver = new PackagePathResolver(_packagesDirectory);

			_packageExtractionContext = new PackageExtractionContext(
				PackageSaveMode.Defaultv3,
				XmlDocFileSaveMode.None,
				ClientPolicyContext.GetClientPolicy(_nugetSettings, NullLogger.Instance), NullLogger.Instance);

		}

		public async Task<bool> Start()
		{
			_logger.LogInformation($"Plugins directory is: {_neonConfig.Plugins.Directory}");
			_fileSystemManager.CreateDirectory(_neonConfig.Plugins.Directory);

			await ScanPlugins();

			return true;
		}

		private async Task ScanPlugins()
		{
			_logger.LogInformation($"Scanning {_fileSystemManager.BuildFilePath(_neonConfig.Plugins.Directory)} for plugins");

			var plugins =
				new DirectoryInfo(_fileSystemManager.BuildFilePath(_neonConfig.Plugins.Directory)).GetFiles("*.dll",
					SearchOption.AllDirectories);


			plugins.ToList().ForEach(LoadPlugin);

			//<PackageReference Include="Castle.Core" Version="4.4.0" />
			//<PackageReference Include="Newtonsoft.Json" Version="12.0.2" />
			//<PackageReference Include="MySql.Data" Version="8.0.17" />
			await DownloadDependecies(new List<PluginDependencyConfig>()
				{new PluginDependencyConfig() {PackageName = "MySql.Data", PackageVersion = "8.0.17"}});

			_logger.LogInformation($"Updating Assemblies");

			_assembliesToLoad.ForEach(a => AssemblyUtils.AddAssemblyToCache(Assembly.LoadFile(a)));

		}

		private PackageIdentity GetPackageIdentity(string packageName, string packageVersion)
		{
			return new PackageIdentity(packageName, NuGetVersion.Parse(packageVersion));
		}

		private async Task GetPackageDependencies(PackageIdentity package,

			SourceCacheContext cacheContext,

			IEnumerable<SourceRepository> repositories,
			ISet<SourcePackageDependencyInfo> availablePackages)
		{
			if (availablePackages.Contains(package)) return;

			foreach (var sourceRepository in repositories)
			{
				var dependencyInfoResource = await sourceRepository.GetResourceAsync<DependencyInfoResource>();
				var dependencyInfo = await dependencyInfoResource.ResolvePackage(
					package, _nuGetFramework, cacheContext, NullLogger.Instance, CancellationToken.None);

				if (dependencyInfo == null) continue;

				availablePackages.Add(dependencyInfo);
				foreach (var dependency in dependencyInfo.Dependencies)
				{
					await GetPackageDependencies(
						new PackageIdentity(dependency.Id, dependency.VersionRange.MinVersion),
						cacheContext,
						 repositories, availablePackages);
				}
			}
		}

		private async Task DownloadDependecies(List<PluginDependencyConfig> packages)
		{
			var repositories = _nugetSourceRepositoryProvider.GetRepositories();
			var availablePackages = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);
			var resolver = new PackageResolver();
			var frameworkReducer = new FrameworkReducer();
			PackageReaderBase packageReader;
			using (var context = new SourceCacheContext())
			{
				foreach (var package in packages)
				{
					_logger.LogInformation(
						$"Getting Dependency for package {package.PackageName} v{package.PackageVersion}");
					await GetPackageDependencies(GetPackageIdentity(package.PackageName, package.PackageVersion), context,
						repositories, availablePackages);

					var resolverContext = new PackageResolverContext(
						DependencyBehavior.Lowest,
						new[] { package.PackageName },
						Enumerable.Empty<string>(),
						Enumerable.Empty<PackageReference>(),
						Enumerable.Empty<PackageIdentity>(),
						availablePackages,
						_nugetSourceRepositoryProvider.GetRepositories().Select(s => s.PackageSource),
						NullLogger.Instance);

					var packagesToInstall = resolver.Resolve(resolverContext, CancellationToken.None)
						.Select(p => availablePackages.Single(x => PackageIdentityComparer.Default.Equals(x, p)));


					foreach (var packageToInstall in packagesToInstall)
					{
						var basePath = "";
						var installedPath = _packagePathResolver.GetInstalledPath(packageToInstall);
						if (installedPath == null)
						{
							_logger.LogInformation(
								$"Download package {packageToInstall.Id} {packageToInstall.Version}");
							var downloadResource =
								await packageToInstall.Source
									.GetResourceAsync<DownloadResource>(CancellationToken.None);
							var downloadResult = await downloadResource.GetDownloadResourceResultAsync(
								packageToInstall,
								new PackageDownloadContext(context),
								SettingsUtility.GetGlobalPackagesFolder(_nugetSettings),
								NullLogger.Instance, CancellationToken.None);

							_logger.LogInformation(
								$"Extracting package {packageToInstall.Id} v{packageToInstall.Version}");
							var extractedFiles = await PackageExtractor.ExtractPackageAsync(downloadResult.PackageSource,
								   downloadResult.PackageStream,
								   _packagePathResolver,
								   _packageExtractionContext,
								   CancellationToken.None);

							var pathList = extractedFiles.ToArray()[0].Split(Path.DirectorySeparatorChar).ToList();
							pathList.RemoveAt(pathList.Count - 1);
							basePath = string.Join($"{Path.DirectorySeparatorChar}", pathList);
							packageReader = downloadResult.PackageReader;
						}
						else
						{
							packageReader = new PackageFolderReader(installedPath);
							var pathList = packageReader.GetNuspecFile().Split(Path.DirectorySeparatorChar).ToList();
							pathList.RemoveAt(pathList.Count - 1);
							basePath = string.Join($"{Path.DirectorySeparatorChar}", pathList);
						}
							
						

						var libItems = packageReader.GetLibItems();
						var nearest =
							frameworkReducer.GetNearest(_nuGetFramework, libItems.Select(x => x.TargetFramework));


						var items = libItems
							.Where(x => x.TargetFramework.Equals(nearest))
							.SelectMany(x => x.Items).ToList();


						//var frameworkItems = packageReader.GetFrameworkItems();
						//nearest = frameworkReducer.GetNearest(_nuGetFramework,
						//	frameworkItems.Select(x => x.TargetFramework));

						//items.AddRange(frameworkItems
						//	.Where(x => x.TargetFramework.Equals(nearest))
						//	.SelectMany(x => x.Items));

						items.ForEach(lib =>
						{
							if (!string.IsNullOrEmpty(lib))
							{
								if (lib.EndsWith(".dll"))
								{
									if (!lib.Contains("System."))
									{
										var libName = Path.Combine(_packagesDirectory,
											$"{basePath}", lib.Replace('/', Path.DirectorySeparatorChar));


										_assembliesToLoad.Add(libName);
									}

								}
							};
						});

					}
				}
			}
		}




		private void LoadPlugin(FileInfo file)
		{

			if (file.DirectoryName.Contains("packages"))
				return;

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