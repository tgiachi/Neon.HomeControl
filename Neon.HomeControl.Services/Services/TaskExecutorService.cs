using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.Services;
using Neon.HomeControl.Api.Core.Data.Config;
using Neon.HomeControl.Api.Core.Interfaces.Services;
using Neon.HomeControl.Api.Core.Utils;
using System;
using System.Threading.Tasks;

namespace Neon.HomeControl.Services.Services
{
	[Service(typeof(ITaskExecutorService), Name = "Task Executor Service", LoadAtStartup = true)]
	public class TaskExecutorService : ITaskExecutorService
	{
		private readonly NeonConfig _neonConfig;
		private readonly ILogger _logger;
		private TaskPool _taskPool;

		public TaskExecutorService(NeonConfig neonConfig, ILogger<TaskExecutorService> logger)
		{
			_logger = logger;
			_neonConfig = neonConfig;
			_taskPool = new TaskPool(_neonConfig.Tasks.MaxNumThreads);
		}

		public Task Enqueue(Func<Task> task)
		{
			_logger.LogDebug($"Executing task {task.Method}");
			return _taskPool.Enqueue(task);
		}

		public Task<bool> Start()
		{
			_logger.LogInformation($"Task Executor pool: {_neonConfig.Tasks.MaxNumThreads} threads");

			return Task.FromResult(true);
		}

		public Task<bool> Stop()
		{
			_taskPool = null;

			return Task.FromResult(true);
		}
	}
}