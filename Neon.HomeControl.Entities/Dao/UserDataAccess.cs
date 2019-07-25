using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Neon.HomeControl.Api.Core.Attributes.Services;
using Neon.HomeControl.Api.Core.Impl.Dao;
using Neon.HomeControl.Entities.Entities;

namespace Neon.HomeControl.Entities.Dao
{
	[DataAccess]
	public class UserDataAccess : AbstractDataAccess<UserEntity>
	{
		public UserDataAccess(DbContext dbContext, ILogger<UserDataAccess> logger) : base(dbContext, logger)
		{
		}
	}
}