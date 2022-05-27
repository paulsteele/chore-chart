using System;
using System.Linq;
using System.Threading.Tasks;
using hub.Server.Configuration;
using Microsoft.AspNetCore.Identity;

namespace hub.Server.Database {
	public class EnsureUserHelper {
		private readonly IDb _db;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IEnvironmentVariableConfiguration _configuration;

		public EnsureUserHelper(IDb db, UserManager<IdentityUser> userManager, IEnvironmentVariableConfiguration configuration) {
			_db = db;
			_userManager = userManager;
			_configuration = configuration;
		}

		public Task EnsureUser()
		{
			return CreateUser(_configuration.DefaultUserName, _configuration.DefaultUserPass);
		}

		private async Task CreateUser(string username, string password) {
			var existingUser = _userManager.Users.FirstOrDefault(user => user.UserName == username);
			if (existingUser != null) {
				Console.WriteLine("Default user already exists, not creating.");
				return;
			}

			var newUser = new IdentityUser { UserName = username };

			await _userManager.CreateAsync(newUser, password);
		}
	}
}
