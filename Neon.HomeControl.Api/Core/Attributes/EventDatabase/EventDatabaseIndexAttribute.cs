using System;

namespace Neon.HomeControl.Api.Core.Attributes.EventDatabase
{
	/// <summary>
	///     Attribute for creating indexes on the event database
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class EventDatabaseIndexAttribute : Attribute
	{
		/// <summary>
		///     Create new attribute
		/// </summary>
		/// <param name="unique"></param>
		public EventDatabaseIndexAttribute(bool unique)
		{
			Unique = unique;
		}

		/// <summary>
		///     Is True if the Index if unique
		/// </summary>
		public bool Unique { get; set; }
	}
}