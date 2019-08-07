using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.Services;
using Neon.HomeControl.Api.Core.Interfaces.Services;

namespace Neon.HomeControl.Services.Services
{
	[Service(typeof(IRoutineService), Name = "Routine Service", LoadAtStartup = true, Order = 3)]
	public class RoutineService : IRoutineService
	{
		private readonly ILogger _logger;
		private readonly Dictionary<string, Action> _routines = new Dictionary<string, Action>();

		public List<string> RoutineNames => _routines.Keys.ToList();

		public RoutineService(ILogger<IRoutineService> logger)
		{
			_logger = logger;
		}

		public Task<bool> Start()
		{

			_logger.LogInformation($"Routine service started");
			return Task.FromResult(true);
		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}

		public bool AddRoutine(string name, Action action)
		{
			if (_routines.ContainsKey(name)) return false;

			_routines.Add(name, action);
			return true;
		}

		public void ExecuteRoutine(string name)
		{

			if (!_routines.ContainsKey(name)) return;

			_routines[name].Invoke();


		}

		
	}
}
