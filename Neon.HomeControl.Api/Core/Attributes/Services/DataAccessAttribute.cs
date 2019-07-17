using System;

namespace Neon.HomeControl.Api.Core.Attributes.Services
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public class DataAccessAttribute : Attribute
	{
	}
}