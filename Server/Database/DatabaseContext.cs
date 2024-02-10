using hub.Server.Configuration;
using hub.Shared.Converters;
using hub.Shared.Models.Finance;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace hub.Server.Database;

public class DatabaseContext(EnvironmentVariableConfiguration configuration) : IdentityDbContext {
	
	public DbSet<Category> Categories { get; set; }
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		var connectionString = $"Server={configuration.DatabaseUrl};Port={configuration.DatabasePort};Database={configuration.DatabaseName};Uid={configuration.DatabaseUser};Pwd={configuration.DatabasePassword};";
		var serverVersion = ServerVersion.AutoDetect(connectionString);
		optionsBuilder
			.UseMySql(connectionString, serverVersion)
			.EnableDetailedErrors();
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

		builder.Entity<Category>()
			.Property(nameof(Category.Color))
			.HasConversion<ColorValueConverter>();
	}
}

public class MigrationContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
	public DatabaseContext CreateDbContext(string[] args)
	{
		return new(new EnvironmentVariableConfiguration());
	}
}