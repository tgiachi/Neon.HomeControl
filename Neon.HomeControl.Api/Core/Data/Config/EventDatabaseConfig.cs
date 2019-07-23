namespace Neon.HomeControl.Api.Core.Data.Config
{
	public class EventDatabaseConfig
	{
		public string DatabaseDirectory { get; set; }


		public EventDatabaseConfig()
		{
			DatabaseDirectory = "Database";
		}
	}
}