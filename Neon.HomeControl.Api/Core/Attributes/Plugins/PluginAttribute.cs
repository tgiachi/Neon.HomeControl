using System;

namespace Neon.HomeControl.Api.Core.Attributes.Plugins
{
	/// <summary>
	///     Attribute for creating plugins
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public class PluginAttribute : Attribute
	{
		/// <summary>
		///     Attribute for creating plugins
		/// </summary>
		/// <param name="name">Ex: My Plugin</param>
		/// <param name="category">Ex: MISC</param>
		/// <param name="version">Ex: v1.0</param>
		/// <param name="author">Ex: tgiachi</param>
		public PluginAttribute(string name, string category, string version, string author)
		{
			Name = name;
			Category = category;
			Version = version;
			Author = author;
		}

		/// <summary>
		///     Name of plugin
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///     Category of plugin
		/// </summary>
		public string Category { get; set; }

		/// <summary>
		///     Version of plugin
		/// </summary>
		public string Version { get; set; }

		/// <summary>
		///     Author of plugin
		/// </summary>
		public string Author { get; set; }

		/// <summary>
		///     If Plugin have dependencies of plugins
		/// </summary>
		public string[] Dependencies { get; set; }
	}
}