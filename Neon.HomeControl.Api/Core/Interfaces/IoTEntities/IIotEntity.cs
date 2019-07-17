using System;

namespace Neon.HomeControl.Api.Core.Interfaces.IoTEntities
{
	public interface IIotEntity
	{
		Guid Id { get; set; }

		DateTime EventDateTime { get; set; }

		string EntityName { get; set; }

		string EntityType { get; set; }
	}
}