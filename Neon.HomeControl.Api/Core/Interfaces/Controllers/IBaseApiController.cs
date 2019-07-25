using Microsoft.AspNetCore.Mvc;
using Neon.HomeControl.Api.Core.Interfaces.Dao;
using Neon.HomeControl.Api.Core.Interfaces.Dto;
using Neon.HomeControl.Api.Core.Interfaces.Entities;
using System.Collections.Generic;

namespace Neon.HomeControl.Api.Core.Interfaces.Controllers
{
	public interface IBaseApiController<TEntity, TDto, TDataAccess>
		where TEntity : IBaseEntity
		where TDto : IBaseDto
		where TDataAccess : IDataAccess<TEntity>
	{
		ActionResult<List<TDto>> List();

		ActionResult<long> Count();
	}
}