using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Neon.HomeControl.Api.Core.Attributes.Database;

namespace Neon.HomeControl.Api.Core.Utils
{
	public static class NoSqlUtils
	{
		public static Type GetNoSqlConnector(string name)
		{
			var noConnectors = AssemblyUtils.ScanAllAssembliesFromAttribute(typeof(NoSqlConnectorAttribute));

			foreach (var connector in noConnectors)
			{
				var nosqlAttribute = connector.GetCustomAttribute<NoSqlConnectorAttribute>();

				if (nosqlAttribute.Name.ToLower() == name.ToLower())
					return connector;
			}

			return null;
		}
	}
}
