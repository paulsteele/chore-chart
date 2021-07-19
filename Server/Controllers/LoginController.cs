using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using hub.Server.Database;
using hub.Shared.Models;
using hub.Shared.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace hub.Server.Controllers {

	[ApiController]
	[Route("login")]
	public class LoginController : ControllerBase {
		private readonly ILogger _logger;
		private readonly IDb _db;
		private readonly IHasher _hasher;
		private readonly INowTimeProvider _nowTimeProvider;

		public LoginController(
			ILogger logger,
			IDb db,
			IHasher hasher,
			INowTimeProvider nowTimeProvider
		) {
			_logger = logger;
			_db = db;
			_hasher = hasher;
			_nowTimeProvider = nowTimeProvider;
		}

		[HttpPost]
		public IActionResult Login([FromBody]User user) {
			var dbUser = _db.DatabaseContext.Users.FirstOrDefault(u => u.Email == user.Email);
			if (dbUser == null) {
				return NotFound();
			}

			if (!_hasher.Validate(user.PasswordHash, dbUser.PasswordHash)) {
				return NotFound();
			}

			var session = new Session {
				UserId = dbUser.Id,
				ExpirationTime = _nowTimeProvider.Now.AddDays(7)
			};

			_db.DatabaseContext.Sessions.Add(session);
			_db.DatabaseContext.SaveChanges();

			return Ok(session);
		}
	}
}
