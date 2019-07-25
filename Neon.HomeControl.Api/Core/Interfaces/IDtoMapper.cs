using Neon.HomeControl.Api.Core.Interfaces.Dto;
using Neon.HomeControl.Api.Core.Interfaces.Entities;
using System.Collections.Generic;

namespace Neon.HomeControl.Api.Core.Interfaces
{
	/// <summary>
	/// Interface for create DTO Mapper
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	/// <typeparam name="TDto"></typeparam>
	public interface IDtoMapper<TEntity, TDto> where TEntity : IBaseEntity where TDto : IBaseDto
	{
		/// <summary>
		/// Transform list of entities in DTOs
		/// </summary>
		/// <param name="entities"></param>
		/// <returns></returns>
		List<TDto> ToDto(List<TEntity> entities);
		/// <summary>
		/// Transform single entity in DTO
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		TDto ToDto(TEntity entity);
		/// <summary>
		/// Transform List of DTOs in Entities
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		List<TEntity> ToEntity(List<TDto> dto);
		/// <summary>
		/// Transform DTO in Entity
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		TEntity ToEntity(TDto dto);
	}
}