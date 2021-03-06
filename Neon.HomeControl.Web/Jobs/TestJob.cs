﻿using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Interfaces.JobScheduler;
using System.Threading.Tasks;

namespace Neon.HomeControl.Web.Jobs
{
	//[SchedulerJobTask(true, 10)]
	public class TestJob : IJobSchedulerTask
	{
		private readonly ILogger _logger;

		public TestJob(ILogger<TestJob> logger)
		{
			_logger = logger;
		}

		public Task Execute(params object[] args)
		{
			_logger.LogInformation("Log from Job");

			return Task.CompletedTask;
		}

		public void Dispose()
		{
		}
	}
}