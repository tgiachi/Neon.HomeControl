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
		/// <summary>
		/// Expose application metrics
		/// </summary>
		public bool EnableMetrics { get; set; }

		/// <summary>
		/// Expose API information with swagger
		/// </summary>
		public bool EnableSwagger { get; set; }

		/// <summary>
		/// If true, load all components
		/// </summary>
		public bool AutoLoadComponents { get; set; }

		public DatabaseConfig Database { get; set; }
		public ScriptConfig Scripts { get; set; }
		public TaskConfig Tasks { get; set; }
		public FileSystemConfig FileSystem { get; set; }
		public ComponentConfig Components { get; set; }
		public EventDatabaseConfig EventsDatabase { get; set; }
		public IoTConfig IoT { get; set; }

		/// <summary>
		/// Database directory
		/// </summary>
		public string DatabaseDirectory { get; set; }


		public NeonConfig()
		{
			Database = new DatabaseConfig();
			Scripts = new ScriptConfig();
			Mqtt = new MqttConfig();
			Plugins = new PluginConfig();
			Tasks = new TaskConfig();
			FileSystem = new FileSystemConfig();
			Components = new ComponentConfig();
			EventsDatabase = new EventDatabaseConfig();
			IoT = new IoTConfig();
			JwtToken = "password";
			EnableSwagger = true;
			DatabaseDirectory = "Database";
			AutoLoadComponents = true;
			Home = new HomeConfig();
		}
	}
}