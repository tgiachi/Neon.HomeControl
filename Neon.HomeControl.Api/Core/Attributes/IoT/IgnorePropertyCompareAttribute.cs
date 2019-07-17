using System;

namespace Neon.HomeControl.Api.Core.Attributes.IoT
{
	/// <summary>
	///     Attribute for ignore
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class IgnorePropertyCompareAttribute : Attribute
	{
	}
}