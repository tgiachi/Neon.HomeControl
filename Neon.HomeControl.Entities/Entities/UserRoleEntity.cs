using System;
using System.ComponentModel.DataAnnotations.Schema;
using Neon.HomeControl.Api.Core.Interfaces.Entities;

namespace Neon.HomeControl.Entities.Entities
{
	[Table("users_roles")]
	public class UserRoleEntity : IBaseEntity
	{
		[NotMapped]
		public long Id { get; set; }

		public long UserId { get; set; }
		public UserEntity User { get; set; }

		public long RoleId { get; set; }
		public RoleEntity Role { get; set; }
		

		public DateTime CreatedDateTime { get; set; }
		public DateTime UpdateDateTime { get; set; }
	}
}