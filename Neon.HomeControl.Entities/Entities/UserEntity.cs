using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Neon.HomeControl.Api.Core.Impl.Entities;
using Neon.HomeControl.Api.Core.Utils;

namespace Neon.HomeControl.Entities.Entities
{
	[Table("users")]
	public class UserEntity : BaseEntity
	{

		[Column("password")] 
		[MaxLength(255)] 
		private string _password;

		[Column("first_name")]
		[MaxLength(100)]
		public string FirstName { get; set; }

		[Column("last_name")] 
		[MaxLength(100)] 
		public string LastName { get; set; }

		[Column("email")]
		[MaxLength(150)] 
		public string Email { get; set; }

		[Column("is_enabled")] 
		public bool IsEnabled { get; set; }

		[Column("user_name")]
		[Required]
		public string UserName { get; set; }

		public List<UserRoleEntity> UsersRoles { get; set; }

		public string Password
		{
			get => _password;
			set => _password = value.HashSha1();
		}

		public UserEntity()
		{
			IsEnabled = true;
			UsersRoles = new List<UserRoleEntity>();
		}


	}
}