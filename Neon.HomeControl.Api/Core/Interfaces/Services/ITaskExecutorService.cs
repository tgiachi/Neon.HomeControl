using System;
using System.Threading.Tasks;

namespace Neon.HomeControl.Api.Core.Interfaces.Services
{
	public interface ITaskExecutorService : IService
	{
		Task Enqueue(Func<Task> task);
	}
}