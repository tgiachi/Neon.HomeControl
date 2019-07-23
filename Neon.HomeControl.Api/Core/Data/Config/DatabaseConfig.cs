namespace Neon.HomeControl.Api.Core.Data.Config
{
	public class DatabaseConfig
	{
		public string ConnectionString { get; set; }


		public DatabaseConfig()
		{
			ConnectionString = "Neon.HomeControl.sqlite";
		}
	}
}