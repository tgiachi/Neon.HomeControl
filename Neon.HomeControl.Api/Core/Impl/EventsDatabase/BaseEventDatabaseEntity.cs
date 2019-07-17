using System;
using Neon.HomeControl.Api.Core.Attributes.IoT;
using Neon.HomeControl.Api.Core.Interfaces.IoTEntities;
namespace Neon.HomeControl.Api.Core.Impl.EventsDatabase
{
	public class BaseEventDatabaseEntity : IIotEntity
	{
		public BaseEventDatabaseEntity()
		{
			EventDateTime = DateTime.Now;
		}

		[IgnorePropertyCompare] public Guid Id { get; set; }

		public string EntityName { get; set; }

		public string EntityType { get; set; }

		[IgnorePropertyCompare] public DateTime EventDateTime { get; set; }
	}
}