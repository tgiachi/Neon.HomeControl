using System.IO;

namespace Neon.HomeControl.Api.Core.Data.Config
{
	public class IoTConfig
	{
		public string ConnectorName { get; set; }

		public string ConnectionString { get; set; }

		public IoTConfig()
		{
			ConnectorName = "litedb";
			ConnectionString = "Database"+ Path.DirectorySeparatorChar + "Neon.HomeControl.IoT.db";
		}
	}
}