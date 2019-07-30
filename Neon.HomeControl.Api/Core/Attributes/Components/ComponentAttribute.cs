using System;

namespace Neon.HomeControl.Api.Core.Attributes.Components
{
	/// <summary>
	///     Attribute for the implementation of the components
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class ComponentAttribute : Attribute
	{
		/// <summary>
		///     Ctor
		/// </summary>
		/// <param name="name">Name of component</param>
		/// <param name="version">Version of component</param>
		/// <param name="category">Category (LIGHTS, MUSIC, HOME_AUTOMATION)</param>
		/// <param name="description">Describes what the component does</param>
		/// <param name="componentConfigType">Type of Config</param>
		public ComponentAttribute(string id, string name, string version, string category, string description,
			Type componentConfigType)
		{
			Id = id;
			Name = name;
			Version = version;
			Category = category;
			Description = description;
			ComponentConfigType = componentConfigType;
		}

		/// <summary>
		/// Id of component (philip_hue, ecc..)
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		///     Name of components
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///     Version of component
		/// </summary>
		public string Version { get; set; }

		/// <summary>
		///     Category of component
		/// </summary>
		public string Category { get; set; }

		/// <summary>
		///     Description of component
		/// </summary>
		public string Description { get; set; }
		
		/// <summary>
		///     Type of config
		/// </summary>
		public Type ComponentConfigType { get; set; }
	}
}