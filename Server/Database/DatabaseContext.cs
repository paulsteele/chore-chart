using hub.Server.Configuration;
using hub.Shared.Models.Todo;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace hub.Server.Database {

	public class DatabaseContext : IdentityDbContext {
		private readonly EnvironmentVariableConfiguration _configuration;

		public DatabaseContext(EnvironmentVariableConfiguration configuration)
		{
			_configuration = configuration;
		}
		
		public DbSet<TodoModel> Todos { get; set; }
		public DbSet<TodoCompletion> TodosCompletions { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			optionsBuilder.UseMySQL($"Server={_configuration.DatabaseUrl};Port={_configuration.DatabasePort};Database={_configuration.DatabaseName};Uid={_configuration.DatabaseUser};Pwd={_configuration.DatabasePassword};");
		}

		// needed due to using mysql. See https://decovar.dev/blog/2018/03/20/csharp-dotnet-core-identity-mysql/
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<IdentityUser>(entity => entity.Property(m => m.Id).HasMaxLength(85));
			builder.Entity<IdentityUser>(entity => entity.Property(m => m.NormalizedEmail).HasMaxLength(85));
			builder.Entity<IdentityUser>(entity => entity.Property(m => m.NormalizedUserName).HasMaxLength(85));

			builder.Entity<IdentityRole>(entity => entity.Property(m => m.Id).HasMaxLength(85));
			builder.Entity<IdentityRole>(entity => entity.Property(m => m.NormalizedName).HasMaxLength(85));

			builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(85));
			builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.ProviderKey).HasMaxLength(85));
			builder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
			builder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));

			builder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.RoleId).HasMaxLength(85));

			builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
			builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(85));
			builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.Name).HasMaxLength(85));

			builder.Entity<IdentityUserClaim<string>>(entity => entity.Property(m => m.Id).HasMaxLength(85));
			builder.Entity<IdentityUserClaim<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
			builder.Entity<IdentityRoleClaim<string>>(entity => entity.Property(m => m.Id).HasMaxLength(85));
			builder.Entity<IdentityRoleClaim<string>>(entity => entity.Property(m => m.RoleId).HasMaxLength(85));
		}
	}
	
	public class MigrationContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
	{
		public DatabaseContext CreateDbContext(string[] args)
		{
			return new(new EnvironmentVariableConfiguration());
		}
	}
}
