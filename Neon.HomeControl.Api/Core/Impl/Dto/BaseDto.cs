using Neon.HomeControl.Api.Core.Interfaces.Dto;
using System;

namespace Neon.HomeControl.Api.Core.Impl.Dto
{
	public class BaseDto : IBaseDto
	{
		public long Id { get; set; }
		public DateTime CreatedDateTime { get; set; }
		public DateTime UpdateDateTime { get; set; }
	}
}