using System;
using System.Threading.Tasks;

namespace Neon.HomeControl.Api.Core.Interfaces.JobScheduler
{
	public interface IJobSchedulerTask : IDisposable
	{
		Task Execute(params object[] args);
	}
}