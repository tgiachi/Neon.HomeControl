using System;

namespace Neon.HomeControl.Api.Core.Interfaces.Dto
{
	public interface IBaseDto
	{
		long Id { get; set; }

		DateTime CreatedDateTime { get; set; }

		DateTime UpdateDateTime { get; set; }
	}
}