using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Neon.HomeControl.Api.Core.Utils
{
	/// <summary>
	///     Threadpool class that help you to execute async task in pool
	/// </summary>
	/// <remarks>
	///     https://gist.github.com/OmidID/da234a6cfbacebbd46defdb71c6cf95e
	/// </remarks>
	public class TaskPool
	{
		private readonly object checkMutex = new object();
		private readonly ConcurrentQueue<IInternalTask> queue = new ConcurrentQueue<IInternalTask>();
		private readonly object tasksMutex = new object();

		private readonly int threadsMaxCount;
		private readonly HashSet<IInternalTask> workingTasks = new HashSet<IInternalTask>();

		/// <summary>
		///     Creates a new thread queue with a maximum number of threads
		/// </summary>
		/// <param name="threadsMaxCount">The maximum number of currently threads</param>
		public TaskPool(int threadsMaxCount)
		{
			this.threadsMaxCount = threadsMaxCount;
		}

		/// <summary>
		///     Creates a new thread queue with a maximum number of threads and the tasks that should be executed.
		/// </summary>
		/// <param name="threadsMaxCount">The maximum number of currently threads.</param>
		/// <param name="tasks">The tasks that will be execut in pool.</param>
		public TaskPool(int threadsMaxCount, Func<Task>[] tasks) : this(threadsMaxCount)
		{
			foreach (var task in tasks) queue.Enqueue(new InternalTaskHolder {Task = task});
		}


		/// <summary>
		///     Raised when all tasks have been completed.
		/// </summary>
		public event EventHandler Completed;


		/// <summary>
		///     Adds a task and runs it if free thread exists. Otherwise enqueue.
		/// </summary>
		/// <param name="task">The task that will be execut</param>
		public Task Enqueue(Func<Task> task)
		{
			lock (tasksMutex)
			{
				var holder = new InternalTaskHolder {Task = task, Waiter = new TaskCompletionSource<IDisposable>()};

				if (workingTasks.Count >= threadsMaxCount)
					queue.Enqueue(holder);
				else
					StartTask(holder);

				return holder.Waiter.Task;
			}
		}

		/// <summary>
		///     Adds a task and runs it if free thread exists. Otherwise enqueue.
		/// </summary>
		/// <param name="task">The task that will be executed</param>
		public Task<T> Enqueue<T>(Func<Task<T>> task)
		{
			lock (tasksMutex)
			{
				var holder = new InternalTaskHolderGeneric<T> {Task = task, Waiter = new TaskCompletionSource<T>()};

				if (workingTasks.Count >= threadsMaxCount)
					queue.Enqueue(holder);
				else
					StartTask(holder);

				return holder.Waiter.Task;
			}
		}

		/// <summary>
		///     Starts the execution of a task.
		/// </summary>
		/// <param name="task">The task that should be executed.</param>
		private async void StartTask(IInternalTask task)
		{
			await task.Execute();
			TaskCompleted(task);
		}

		private void TaskCompleted(IInternalTask task)
		{
			lock (tasksMutex)
			{
				workingTasks.Remove(task);
				CheckQueue();

				if (queue.Count == 0 && workingTasks.Count == 0)
					OnCompleted();
			}
		}

		/// <summary>
		///     Checks if the queue contains tasks and runs as many as there are free execution slots.
		/// </summary>
		private void CheckQueue()
		{
			lock (checkMutex)
			{
				while (queue.Count > 0 && workingTasks.Count < threadsMaxCount)
					if (queue.TryDequeue(out var task))
						StartTask(task);
			}
		}

		/// <summary>
		///     Raises the Completed event.
		/// </summary>
		protected void OnCompleted()
		{
			Completed?.Invoke(this, null);
		}

		private interface IInternalTask
		{
			Task Execute();
		}

		private class InternalTaskHolder : IInternalTask
		{
			public Func<Task> Task { get; set; }
			public TaskCompletionSource<IDisposable> Waiter { get; set; }

			public async Task Execute()
			{
				await Task();
				Waiter.SetResult(null);
			}
		}

		private class InternalTaskHolderGeneric<T> : IInternalTask
		{
			public Func<Task<T>> Task { get; set; }
			public TaskCompletionSource<T> Waiter { get; set; }

			public async Task Execute()
			{
				var result = await Task();
				Waiter.SetResult(result);
			}
		}
	}
}