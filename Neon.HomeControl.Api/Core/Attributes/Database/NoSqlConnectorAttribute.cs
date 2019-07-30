using System;
using System.Collections.Generic;
using System.Text;

namespace Neon.HomeControl.Api.Core.Attributes.Database
{
	/// <summary>
	/// Attribute for NoSQL connector
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class NoSqlConnectorAttribute : Attribute
	{
		/// <summary>
		/// Name of connector
		/// </summary>
		public string Name { get;set; }

		/// <summary>
		/// ctor	
		/// </summary>
		/// <param name="connectorName"></param>
		public NoSqlConnectorAttribute(string connectorName)
		{
			Name = connectorName;
		}
	}
}
