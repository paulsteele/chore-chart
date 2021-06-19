using System;
using System.Collections.Generic;
using System.Linq;
using hub.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace hub.Server.Controllers {

	[ApiController]
	[Route("[controller]")]
	public class LoginController {
		private readonly ILogger _logger;

		public LoginController(ILogger logger) {
			_logger = logger;
		}

		[HttpPost]
		[Route("login")]
		public User Login([FromBody]User user) {
			_logger.LogInformation("login post");
			user.PasswordHash = "test";
			return user;
		}
	}
}
