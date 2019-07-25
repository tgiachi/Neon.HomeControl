using Microsoft.EntityFrameworkCore;
using Neon.HomeControl.Entities.Entities;

namespace Neon.HomeControl.Entities.Services
{
	public class NeonDbContext : DbContext
	{
		public NeonDbContext(DbContextOptions<NeonDbContext> options) : base(options)
		{
		}

		/// <summary>
		/// Users
		/// </summary>
		public DbSet<UserEntity> Users { get; set; }
		/// <summary>
		/// Roles
		/// </summary>
		public DbSet<RoleEntity> Roles { get; set; }
		/// <summary>
		///  Many user have may roles
		/// </summary>
		public DbSet<UserRoleEntity> UserRoles { get; set; }

		/// <summary>
		/// Setup relations
		/// </summary>
		/// <param name="modelBuilder"></param>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

			modelBuilder.Entity<UserRoleEntity>().Ignore(sc => sc.Id)
				.HasKey(sc => new { sc.UserId, sc.RoleId });


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