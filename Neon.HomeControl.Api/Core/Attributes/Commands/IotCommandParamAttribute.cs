using System;
using System.Collections.Generic;
using System.Text;

namespace Neon.HomeControl.Api.Core.Attributes.Commands
{

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class IotCommandParamAttribute :Attribute
	{
		/// <summary>
		/// Name of param
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// If is required
		/// </summary>
		public bool IsRequired { get; set; }

	

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="isRequired"></param>
		public IotCommandParamAttribute(string name, bool isRequired)
		{
			Name = name;
			IsRequired = isRequired;
		}
	}
}
