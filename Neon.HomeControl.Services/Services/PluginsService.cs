using System;
using System.Threading.Tasks;
using Neon.HomeControl.Api.Core.Attributes.Services;
using Neon.HomeControl.Api.Core.Data.Config;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Neon.HomeControl.Services.Services
{
	[Service(typeof(IPluginsService), LoadAtStartup = true, Name = "Plugins Manager service")]
	public class PluginsService : IPluginsService
	{
		private readonly IFileSystemService _fileSystemService;
		private readonly NeonConfig _neonConfig;

		private readonly ILogger _logger;

		public PluginsService(ILogger<PluginsService> logger, IFileSystemService fileSystemService,
			NeonConfig neonConfig)
		{
			_neonConfig = neonConfig;
			_logger = logger;
			_fileSystemService = fileSystemService;
		}

		public Task<bool> Start()
		{
			_logger.LogInformation($"Plugins directory is: {_neonConfig.Plugins.Directory}");
			_fileSystemService.CreateDirectory(_neonConfig.Plugins.Directory);

			return Task.FromResult(true);
		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}
	}
}