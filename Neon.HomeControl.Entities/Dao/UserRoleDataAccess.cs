using Neon.HomeControl.Api.Core.Impl.Dao;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Entities.Entities;

namespace Neon.HomeControl.Entities.Dao
{
	public class UserRoleDataAccess : AbstractDataAccess<UserRoleEntity>
	{
		public UserRoleDataAccess(DbContext dbContext, ILogger<UserRoleDataAccess> logger) : base(dbContext, logger)
		{
		}
	}
}