using Neon.HomeControl.Api.Core.Attributes.Dto;
using Neon.HomeControl.Api.Core.Impl.Dto;
using Neon.HomeControl.Entities.Entities;

namespace Neon.HomeControl.Dto.Dto
{
	[DtoMap(typeof(RoleEntity))]
	public class RoleDto : BaseDto
	{
		public string Name { get; set; }
	}
}