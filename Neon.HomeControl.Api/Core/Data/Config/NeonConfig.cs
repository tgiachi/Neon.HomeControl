namespace Neon.HomeControl.Api.Core.Data.Config
{

	/// <summary>
	/// Neon System config 
	/// </summary>
	public class NeonConfig
	{
		/// <summary>
		/// Information about home (GPS coordinates)
		/// </summary>
		public HomeConfig Home { get; set; }
		/// <summary>
		/// Plugins System Configuration
		/// </summary>
		public PluginConfig Plugins { get; set; }
		/// <summary>
		/// Jwt Secret key for generate JWT Token
		/// </summary>
		public string JwtToken { get; set; }
		/// <summary>
		/// MQTT Server information
		/// </summary>
		public MqttConfig Mqtt { get; set; }
		public DatabaseConfig Database { get; set; }
		public ScriptConfig Scripts { get; set; }
		public TaskConfig Tasks { get; set; }
		public FileSystemConfig FileSystem { get; set; }
		public ComponentConfig Components { get; set; }
		public EventDatabaseConfig EventsDatabase { get; set; }
		public IoTConfig IoT { get; set; }
	}
}