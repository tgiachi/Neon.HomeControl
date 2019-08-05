using System;
using System.Collections.Generic;
using System.Text;

namespace Neon.HomeControl.Api.Core.Attributes.ScriptEngine
{
	/// <summary>
	/// Script Engine Attribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public class ScriptEngineAttribute : Attribute
	{


		/// <summary>
		/// Name of Script Engine
		/// </summary>
		public string Name { get;set; }

		/// <summary>
		/// Version
		/// </summary>
		public string Version { get; set; }


		/// <summary>
		/// Extension of file to read
		/// </summary>
		public string FileExtension { get; set; }
	
		/// <summary>
		/// Ctor
		/// </summary>
		public ScriptEngineAttribute(string name, string extension, string version)
		{
			Name = name;
			FileExtension = extension;
			Version = version;
		}
	}
}
