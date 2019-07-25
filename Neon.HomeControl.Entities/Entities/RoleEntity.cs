using Neon.HomeControl.Api.Core.Impl.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Neon.HomeControl.Entities.Entities
{
	[Table("roles")]
	public class RoleEntity : BaseEntity
	{
		public RoleEntity()
		{
			UsersRoles = new List<UserRoleEntity>();
		}

		[Column("name")] [MaxLength(100)] public string Name { get; set; }

		public List<UserRoleEntity> UsersRoles { get; set; }
	}
}