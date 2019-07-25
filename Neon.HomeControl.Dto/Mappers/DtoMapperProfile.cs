using AutoMapper;
using Neon.HomeControl.Dto.Dto;
using Neon.HomeControl.Entities.Entities;

namespace Neon.HomeControl.Dto.Mappers
{
	public class DtoMapperProfile : Profile
	{
		public DtoMapperProfile()
		{
			CreateMap<UserEntity, UserDto>().ReverseMap();
			CreateMap<RoleEntity, RoleDto>().ReverseMap();
		}
	}
}
