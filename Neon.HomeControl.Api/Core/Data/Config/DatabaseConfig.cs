namespace Neon.HomeControl.Api.Core.Data.Config
{
	/// <summary>
	/// Internal Database config
	/// </summary>
	public class DatabaseConfig
	{
		/// <summary>
		/// SqLite file 
		/// </summary>
		public string ConnectionString { get; set; }


		/// <summary>
		/// Default Connection string is Neon.HomeControl.sqlite
		/// </summary>
		public DatabaseConfig()
		{
			ConnectionString = "Neon.HomeControl.sqlite";
		}
	}
}