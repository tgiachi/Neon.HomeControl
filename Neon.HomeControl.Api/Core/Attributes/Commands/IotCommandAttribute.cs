using System;
using System.Collections.Generic;
using System.Text;

namespace Neon.HomeControl.Api.Core.Attributes.Commands
{
	/// <summary>
	/// Command attribute for dispatch to components commands
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public class IotCommandAttribute : Attribute
	{
		/// <summary>
		/// Command name
		/// </summary>
		public string CommandName { get; set; }

		/// <summary>
		/// Entity Type
		/// </summary>
		public Type EntityType { get; set; }
	
		public IotCommandAttribute(string commandName, Type entityType)
		{
			CommandName = commandName;
			EntityType = entityType;
		}
	}
}
