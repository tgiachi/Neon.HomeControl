using System.IO;

namespace Neon.HomeControl.Api.Core.Data.Config
{
	public class EventDatabaseConfig
	{
		public string ConnectorName { get; set; }

		public string ConnectionString { get; set; }


		public EventDatabaseConfig()
		{
			ConnectorName = "litedb";
			ConnectionString = "Database" + Path.DirectorySeparatorChar + "Neon.HomeControl.Events.db";
		}
	}
}