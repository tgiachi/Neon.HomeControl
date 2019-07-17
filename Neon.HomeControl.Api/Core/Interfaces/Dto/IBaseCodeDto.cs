using System;

namespace Neon.HomeControl.Api.Core.Interfaces.Dto
{
	public interface IBaseCodeDto : IBaseDto
	{
		Guid Code { get; set; }
	}
}