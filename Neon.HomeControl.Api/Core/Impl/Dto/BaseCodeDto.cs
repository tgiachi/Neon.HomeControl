using Neon.HomeControl.Api.Core.Interfaces.Dto;
using System;

namespace Neon.HomeControl.Api.Core.Impl.Dto
{
	public class BaseCodeDto : BaseDto, IBaseCodeDto
	{
		public Guid Code { get; set; }
	}
}