using Neon.HomeControl.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Neon.HomeControl.Api.Core.Interfaces.Entities;

namespace Neon.HomeControl.Entities.Services
{
	public class NeonDbContext : DbContext
	{
		public NeonDbContext(DbContextOptions<NeonDbContext> options) : base(options)
		{
		}

		public DbSet<UserEntity> Users { get; set; }
		public DbSet<RoleEntity> Roles { get; set; }

		public DbSet<UserRoleEntity> UserRoles { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			
			modelBuilder.Entity<UserRoleEntity>().Ignore(sc => sc.Id)
				.HasKey(sc => new {sc.UserId, sc.RoleId});


			modelBuilder.Entity<UserRoleEntity>()
				.HasOne(ur => ur.Role)
				.WithMany(u => u.UsersRoles)
				.HasForeignKey(s => s.RoleId);

			modelBuilder.Entity<UserRoleEntity>()
				.HasOne(u => u.User)
				.WithMany(s => s.UsersRoles)
				.HasForeignKey(s => s.UserId);
		}
	}
}