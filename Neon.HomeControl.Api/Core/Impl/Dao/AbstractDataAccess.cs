using System;
using System.Collections.Generic;
using System.Linq;
using Neon.HomeControl.Api.Core.Interfaces.Dao;
using Neon.HomeControl.Api.Core.Interfaces.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Neon.HomeControl.Api.Core.Impl.Dao
{
	public class AbstractDataAccess<TEntity> : IDataAccess<TEntity> where TEntity : class, IBaseEntity
	{
		private readonly DbContext _dbContext;
		private readonly ILogger _logger;

		public AbstractDataAccess(DbContext dbContext, ILogger<AbstractDataAccess<TEntity>> logger)
		{
			_logger = logger;
			_dbContext = dbContext;
		}


		public List<TEntity> List()
		{
			return _dbContext.Set<TEntity>().ToList();
		}

		public long Count()
		{
			return _dbContext.Set<TEntity>().Count();
		}

		public TEntity Insert(TEntity entity)
		{
			if (entity is IBaseCodeEntity codeEntity) codeEntity.Code = Guid.NewGuid();

			entity.CreatedDateTime = DateTime.Now;

			_dbContext.Set<TEntity>().Add(entity);
			_dbContext.SaveChanges();
			return entity;
		}

		public TEntity Update(TEntity entity)
		{
			entity.UpdateDateTime = DateTime.Now;
			_dbContext.Entry(entity).State = EntityState.Modified;
			_dbContext.SaveChanges();
			return entity;
		}

		public bool Delete(TEntity entity)
		{
			_dbContext.Entry(entity).State = EntityState.Deleted;
			_dbContext.SaveChanges();

			return true;
		}

		public IQueryable<TEntity> Query()
		{
			return _dbContext.Set<TEntity>().AsQueryable();
		}
	}
}