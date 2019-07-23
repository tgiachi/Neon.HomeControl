using System;
using System.Threading.Tasks;
using Neon.HomeControl.Api.Core.Attributes.Database;
using Neon.HomeControl.Api.Core.Attributes.Services;
using Neon.HomeControl.Api.Core.Interfaces.Database;
using Neon.HomeControl.Api.Core.Interfaces.Managers;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Neon.HomeControl.Api.Core.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Neon.HomeControl.Entities.Services
{
	[Service(typeof(IDatabaseService), Name = "Database Service", LoadAtStartup = true, Order = 1)]
	public class DatabaseService : IDatabaseService
	{
		private readonly ILogger _logger;
		private readonly IServicesManager _servicesManager;
		private readonly NeonDbContext _neonDbContext;

		public DatabaseService(ILogger<DatabaseService> logger, IServicesManager servicesManager,
			NeonDbContext neonDbContext)
		{
			_logger = logger;
			_servicesManager = servicesManager;
			GetDbContextForContainer = typeof(NeonDbContext);
			_neonDbContext = neonDbContext;
		}

		public async Task<bool> Start()
		{
			//_neonDbContext = _servicesManager.Resolve<NeonDbContext>();
			_logger.LogInformation("Applying migrations");
			await _neonDbContext.Database.EnsureCreatedAsync();
			await _neonDbContext.Database.MigrateAsync();
			_logger.LogInformation("Migrations completed");

			_logger.LogInformation("Starting seeds");
			await ExecuteSeeds();
			return true;
		}

		public Task<bool> Stop()
		{
			//throw new NotImplementedException();
			return Task.FromResult(true);
		}

		public Type GetDbContextForContainer { get; set; }

		private async Task ExecuteSeeds()
		{
			var seeds = AssemblyUtils.ScanAllAssembliesFromAttribute(typeof(DatabaseSeedAttribute));

			foreach (var t in seeds)
				try
				{
					var seedObj = _servicesManager.Resolve(t) as IDatabaseSeed;

					_logger.LogInformation($"Executing seed {t.Name}");
					await seedObj.Seed();
				}
				catch (Exception ex)
				{
					_logger.LogInformation($"Error during execute seed {t.Name} => {ex}");
				}
		}
	}
}