using System;

namespace Neon.HomeControl.Api.Core.Attributes.Services
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class ServiceAttribute : Attribute
	{
		public ServiceAttribute(Type serviceInterface)
		{
			ServiceInterface = serviceInterface;
		}

		public string Name { get; set; }

		public Type ServiceInterface { get; set; }

		public int Order { get; set; } = 10;
		public bool LoadAtStartup { get; set; }
	}
}