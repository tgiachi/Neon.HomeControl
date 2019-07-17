using System;

namespace Neon.HomeControl.Api.Core.Interfaces.Entities
{
	public interface IBaseCodeEntity : IBaseEntity
	{
		Guid Code { get; set; }
	}
}