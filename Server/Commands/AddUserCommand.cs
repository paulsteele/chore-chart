using System;
using System.Linq;
using System.Threading.Tasks;
using hub.Server.Database;
using Microsoft.AspNetCore.Identity;

namespace hub.Server.Commands {
	public class AddUserCommand {
		private readonly IDb _db;
		private readonly UserManager<IdentityUser> _userManager;

		public AddUserCommand(IDb db, UserManager<IdentityUser> userManager) {
			_db = db;
			_userManager = userManager;
		}

		public async Task StartCommand() {
			Console.WriteLine("Enter the desired username:");
			var username = Console.ReadLine();
			var existingUser = _userManager.Users.FirstOrDefault(user => user.UserName == username);
			if (existingUser != null) {
				Console.WriteLine("User already exists");
				return;
			}

			Console.WriteLine("Enter the desired password:");
			var pass = Console.ReadLine();

			var newUser = new IdentityUser { UserName = username };

			await _userManager.CreateAsync(newUser, pass);
		}
	}
}
