using System;
using System.Linq;
using hub.Server.Database;
using hub.Shared.Models;
using hub.Shared.Tools;

namespace hub.Server.Commands {
	public class AddUserCommand {
		private readonly IDb _db;
		private readonly IHasher _hasher;

		public AddUserCommand(IDb db, IHasher hasher) {
			_db = db;
			_hasher = hasher;
		}

		public void StartCommand() {
			Console.WriteLine("Enter the desired email address:");
			var email = Console.ReadLine();
			using var dbDatabaseContext = _db.DatabaseContext;
			var existingUser = dbDatabaseContext.Users.FirstOrDefault(user => user.Email == email);
			if (existingUser != null) {
				Console.WriteLine("User already exists");
			}
			Console.WriteLine("Enter the desired password:");
			var pass = Console.ReadLine();
			var userToAdd = new User {
				Email = email,
				PasswordHash = _hasher.Hash(pass)
			};

			dbDatabaseContext.Users.Add(userToAdd);
			dbDatabaseContext.SaveChanges();
		}
	}
}
