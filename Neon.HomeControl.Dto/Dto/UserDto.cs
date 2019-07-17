using Neon.HomeControl.Api.Core.Attributes.Dto;
using Neon.HomeControl.Api.Core.Impl.Dto;
using Neon.HomeControl.Entities.Entities;

namespace Neon.HomeControl.Dto.Dto
{
	[DtoMap(typeof(UserEntity))]
	public class UserDto : BaseDto
	{
		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Email { get; set; }
	}
}