using home.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace home.Server.Database {

	public class DatabaseContext : DbContext{
		private string url = "localhost";
		private string port = "3306";
		private string user = "root";
		private string pass = "pass";
		private string database = "home";

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			optionsBuilder.UseMySQL($"Server={url};Port={port};Database={database};Uid={user};Pwd={pass};");
		}

		private DbSet<User> Users { get; set; }
	}
}
