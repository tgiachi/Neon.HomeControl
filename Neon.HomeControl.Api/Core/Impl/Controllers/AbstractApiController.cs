using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Interfaces;
using Neon.HomeControl.Api.Core.Interfaces.Controllers;
using Neon.HomeControl.Api.Core.Interfaces.Dao;
using Neon.HomeControl.Api.Core.Interfaces.Dto;
using Neon.HomeControl.Api.Core.Interfaces.Entities;
using System.Collections.Generic;

namespace Neon.HomeControl.Api.Core.Impl.Controllers
{
	public class AbstractApiController<TEntity, TDto> : IBaseApiController<TEntity, TDto, IDataAccess<TEntity>>
		where TEntity : IBaseEntity
		where TDto : IBaseDto

	{
		private readonly IDataAccess<TEntity> _dataAccess;
		private readonly ILogger _logger;
		private readonly IDtoMapper<TEntity, TDto> _mapper;

		public AbstractApiController(ILogger<IBaseApiController<TEntity, TDto, IDataAccess<TEntity>>> logger,
			IDataAccess<TEntity> dataAccess, IDtoMapper<TEntity, TDto> mapper)
		{
			_logger = logger;
			_dataAccess = dataAccess;
			_mapper = mapper;
		}


		[HttpGet]
		public ActionResult<List<TDto>> List()
		{
			return new ActionResult<List<TDto>>(_mapper.ToDto(_dataAccess.List()));
		}

		[HttpGet]
		public ActionResult<long> Count()
		{
			return new ActionResult<long>(_dataAccess.Count());
		}
	}
}