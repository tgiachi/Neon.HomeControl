using System;

namespace Neon.HomeControl.Api.Core.Attributes.Services
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class TransientAttribute : Attribute
	{
	}
}