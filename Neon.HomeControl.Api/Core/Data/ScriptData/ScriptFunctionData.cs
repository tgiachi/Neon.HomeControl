namespace Neon.HomeControl.Api.Core.Data.ScriptData
{
	/// <summary>
	/// Class for describe C# to script engine function
	/// </summary>
	public class ScriptFunctionData
	{
		/// <summary>
		/// Name of function
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Parameters
		/// </summary>
		public string[] Args { get; set; }
		/// <summary>
		/// Category of function (Ex: Logger)
		/// </summary>
		public string Category { get; set; }
		/// <summary>
		/// Help text for describe function
		/// </summary>
		public string Help { get; set; }
	}
}