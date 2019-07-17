using System;

namespace Neon.HomeControl.Api.Core.Interfaces.Entities
{
	public interface IBaseEntity
	{
		long Id { get; set; }

		DateTime CreatedDateTime { get; set; }

		DateTime UpdateDateTime { get; set; }
	}
}