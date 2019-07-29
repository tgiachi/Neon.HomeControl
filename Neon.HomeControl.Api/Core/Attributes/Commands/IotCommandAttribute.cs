using System;
using System.Collections.Generic;

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
		/// Command description
		/// </summary>
		public string Description { get; set; }


		/// <summary>
		/// Entity Type
		/// </summary>
		public Type EntityType { get; set; }


		/// <summary>
		/// ctor
		/// </summary>
		/// <param name="commandName"></param>
		/// <param name="entityType"></param>
		/// <param name="description"></param>
		/// <param name="args"></param>
		public IotCommandAttribute(string commandName, Type entityType, string description)
		{
			CommandName = commandName;
			Description = description;
			EntityType = entityType;
		}
	}
}
