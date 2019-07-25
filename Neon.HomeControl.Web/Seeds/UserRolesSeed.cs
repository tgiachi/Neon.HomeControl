using Neon.HomeControl.Api.Core.Attributes.Database;
using Neon.HomeControl.Api.Core.Interfaces.Dao;
using Neon.HomeControl.Api.Core.Interfaces.Database;
using Neon.HomeControl.Entities.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Neon.HomeControl.Web.Seeds
{
	[DatabaseSeed]
	public class UserRoleSeed : IDatabaseSeed
	{
		private readonly IDataAccess<RoleEntity> _roleDataAccess;
		private readonly IDataAccess<UserEntity> _userDataAccess;
		private readonly IDataAccess<UserRoleEntity> _userRoleDataAccess;

		public UserRoleSeed(
			IDataAccess<UserEntity> userDataAccess, IDataAccess<RoleEntity> roleDataAccess,
			IDataAccess<UserRoleEntity> userRoleDataAccess)
		{
			_roleDataAccess = roleDataAccess;
			_userDataAccess = userDataAccess;
			_userRoleDataAccess = userRoleDataAccess;
		}

		public Task Seed()
		{
			if (_roleDataAccess.Count() == 0)
			{
				_roleDataAccess.Insert(new RoleEntity
				{
					Name = "ROLE_ADMIN"
				});

				_roleDataAccess.Insert(new RoleEntity
				{
					Name = "ROLE_USER"
				});
			}

			if (_userDataAccess.Count() == 0)
			{
				var user = new UserEntity
				{
					Email = "squid@stormwind.it",
					UserName = "admin",
					FirstName = "Admin",
					LastName = "Admin",
					Password = "admin"
				};


				_userDataAccess.Insert(user);


				_userRoleDataAccess.Insert(new UserRoleEntity
				{
					RoleId = _roleDataAccess.Query().FirstOrDefault(s => s.Name == "ROLE_ADMIN").Id,
					UserId = _userDataAccess.Query().FirstOrDefault(s => s.UserName == "admin").Id
				});

				_userRoleDataAccess.Insert(new UserRoleEntity
				{
					RoleId = _roleDataAccess.Query().FirstOrDefault(s => s.Name == "ROLE_USER").Id,
					UserId = _userDataAccess.Query().FirstOrDefault(s => s.UserName == "admin").Id
				});
			}

			return Task.CompletedTask;
		}
	}
}