using hub.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace hub.Server.Database {

	public class DatabaseContext : DbContext{
		private string url = "localhost";
		private string port = "3306";
		private string user = "root";
		private string pass = "pass";
		private string database = "hub";

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			optionsBuilder.UseMySQL($"Server={url};Port={port};Database={database};Uid={user};Pwd={pass};");
		}

		public DbSet<User> Users { get; set; }
		public DbSet<Session> Sessions { get; set; }
	}
}
