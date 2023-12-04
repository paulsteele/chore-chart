using System;
using System.Linq;
using System.Threading.Tasks;
using hub.Server.Configuration;
using Microsoft.AspNetCore.Identity;

namespace hub.Server.Database {
	public class EnsureUserHelper(
		IDb db, 
		UserManager<IdentityUser> userManager, 
		IEnvironmentVariableConfiguration configuration
	)
	{
		public Task EnsureUser()
		{
			return CreateUser(configuration.DefaultUserName, configuration.DefaultUserPass);
		}

		private async Task CreateUser(string username, string password) {
			var existingUser = userManager.Users.FirstOrDefault(user => user.UserName == username);
			if (existingUser != null) {
				Console.WriteLine("Default user already exists, not creating.");
				return;
			}

			var newUser = new IdentityUser { UserName = username };

			await userManager.CreateAsync(newUser, password);
		}
	}
}
