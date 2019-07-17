using AutoMapper;
using Neon.HomeControl.Api.Core.Impl.Dto;
using Neon.HomeControl.Dto.Dto;
using Neon.HomeControl.Entities.Entities;

namespace Neon.HomeControl.Dto.Mappers
{
	public class UserDtoMapper : AbstractDtoMapper<UserEntity, UserDto>
	{
		public UserDtoMapper(IMapper mapper) : base(mapper)
		{
		}
	}
}