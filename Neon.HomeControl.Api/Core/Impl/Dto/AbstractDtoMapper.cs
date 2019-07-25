using AutoMapper;
using Neon.HomeControl.Api.Core.Interfaces;
using Neon.HomeControl.Api.Core.Interfaces.Dto;
using Neon.HomeControl.Api.Core.Interfaces.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Neon.HomeControl.Api.Core.Impl.Dto
{
	public class AbstractDtoMapper<TEntity, TDto> : IDtoMapper<TEntity, TDto>
		where TEntity : IBaseEntity where TDto : IBaseDto
	{
		private readonly IMapper _mapper;

		public AbstractDtoMapper(IMapper mapper)
		{
			_mapper = mapper;
		}

		public List<TDto> ToDto(List<TEntity> entities)
		{
			return entities.Select(ToDto).ToList();
		}

		public TDto ToDto(TEntity entity)
		{
			return _mapper.Map<TEntity, TDto>(entity);
		}

		public List<TEntity> ToEntity(List<TDto> dto)
		{
			return dto.Select(ToEntity).ToList();
		}

		public TEntity ToEntity(TDto dto)
		{
			return _mapper.Map<TEntity>(dto);
		}
	}
}