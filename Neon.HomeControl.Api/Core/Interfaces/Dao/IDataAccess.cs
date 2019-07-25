using Neon.HomeControl.Api.Core.Interfaces.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Neon.HomeControl.Api.Core.Interfaces.Dao
{
	public interface IDataAccess<TEntity> where TEntity : IBaseEntity
	{
		List<TEntity> List();

		long Count();

		TEntity Insert(TEntity entity);

		TEntity Update(TEntity entity);

		bool Delete(TEntity entity);

		IQueryable<TEntity> Query();
	}
}