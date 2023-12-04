using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using hub.Server.Configuration;
using hub.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace hub.Server.Controllers;

[ApiController]
[AllowAnonymous]
[Route("login")]
public class LoginController(
	ILogger logger,
	SignInManager<IdentityUser> signInManager,
	EnvironmentVariableConfiguration configuration
) : ControllerBase {
	
	[HttpPost]
	public async Task<IActionResult> Login([FromBody]LoginModel loginModel) {

		var result = await signInManager.PasswordSignInAsync(loginModel.Username, loginModel.Password, false, false);

		if (!result.Succeeded) return BadRequest(new LoginResult { Success = false, Error = "Username or password are invalid." });

		var claims = new[]
		{
			new Claim(ClaimTypes.Name, loginModel.Username)
		};

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.JwtSecurityKey));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
		var expiry = DateTime.Now.AddHours(configuration.JwtExpiryHours);

		var token = new JwtSecurityToken(
			configuration.JwtIssuer,
			configuration.JwtAudience,
			claims,
			expires: expiry,
			signingCredentials: creds
		);

		return Ok(new LoginResult { Success = true, Token = new JwtSecurityTokenHandler().WriteToken(token) });
	}
}