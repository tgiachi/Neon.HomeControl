using System;

namespace Neon.HomeControl.Api.Core.Attributes.Database
{
	/// <summary>
	///     Seeds are classes for initializing default data in the database
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class DatabaseSeedAttribute : Attribute
	{
	}
}