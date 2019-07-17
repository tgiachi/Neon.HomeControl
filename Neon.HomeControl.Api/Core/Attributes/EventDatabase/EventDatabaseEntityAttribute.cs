using System;

namespace Neon.HomeControl.Api.Core.Attributes.EventDatabase
{
	/// <summary>
	///     Attribute for mapping event entities. It is necessary to specify the name of the collection to be saved on the
	///     database
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class EventDatabaseEntityAttribute : Attribute
	{
		/// <summary>
		///     Initialize new attribute with name of collection
		/// </summary>
		/// <param name="collectionName">Example: weather</param>
		public EventDatabaseEntityAttribute(string collectionName)
		{
			CollectionName = collectionName;
		}

		/// <summary>
		///     Name of collection in database
		/// </summary>
		public string CollectionName { get; set; }
	}
}