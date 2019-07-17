using System;
using System.Collections.Generic;
using Neon.HomeControl.Api.Core.Data.Scheduler;
using Neon.HomeControl.Api.Core.Enums;

namespace Neon.HomeControl.Api.Core.Interfaces.Services
{
	/// <summary>
	///     Interface for create Scheduler service
	/// </summary>
	public interface ISchedulerService : IService
	{
		/// <summary>
		///     Return all job information
		/// </summary>
		List<JobInfo> JobsInfo { get; set; }

		/// <summary>
		///     Add job with seconds
		/// </summary>
		/// <param name="job"></param>
		/// <param name="seconds"></param>
		/// <param name="startNow"></param>
		void AddJob(Action job, int seconds, bool startNow);

		void AddJob(Action job, string name, int seconds, bool startNow);

		void AddJob(Action job, string name, int hours, int minutes);

		void AddPolling(Action job, string name, SchedulerServicePollingEnum pollingType);
	}
}