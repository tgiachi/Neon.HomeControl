namespace Neon.HomeControl.Api.Core.Data.Config
{
	public class TaskConfig
	{
		public int MaxNumThreads { get; set; }


		public TaskConfig()
		{
			MaxNumThreads = 10;
		}
	}
}