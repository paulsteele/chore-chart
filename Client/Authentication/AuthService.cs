using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using hub.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace hub.Client.Authentication {
	public interface IAuthService {
		Task<LoginResult> Login(LoginModel loginModel);
		Task Logout();
	}

	public class AuthService : AuthenticationStateProvider, IAuthService {
		private readonly HttpClient _httpClient;
		private readonly ILogger _logger;
		private readonly ILocalStorageService _localStorageService;

		public AuthService(
			HttpClient httpClient,
			ILogger logger,
			ILocalStorageService localStorageService
		)
		{
			_httpClient = httpClient;
			_logger = logger;
			_localStorageService = localStorageService;
		}

		public async Task<LoginResult> Login(LoginModel loginModel)
		{
			var response = await _httpClient.PostAsJsonAsync("login", loginModel);

			var loginResult = await response.Content.ReadFromJsonAsync<LoginResult>();

			if (response.IsSuccessStatusCode && loginResult is {Token: { }}) {
				await _localStorageService.SetItemAsync("authToken", loginResult.Token);

				var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, loginModel.Username) }, "apiauth"));
				var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
				NotifyAuthenticationStateChanged(authState);
			}

			//_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.Token);
			return loginResult;
		}

		private IEnumerable<Claim> ParseClaimsFromJwt(string jwt) {
			var claims = new List<Claim>();
			var payload = jwt.Split('.')[1];
			var jsonBytes = ParseBase64WithoutPadding(payload);
			Dictionary<string, object>? keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

			keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles);

			if (roles != null) {
				if (roles.ToString().Trim().StartsWith("[")) {
					var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

					foreach (var parsedRole in parsedRoles) claims.Add(new Claim(ClaimTypes.Role, parsedRole));
				}
				else {
					claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
				}

				keyValuePairs.Remove(ClaimTypes.Role);
			}

			claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

			return claims;
		}

		private byte[] ParseBase64WithoutPadding(string base64) {
			switch (base64.Length % 4) {
				case 2:
					base64 += "==";
					break;
				case 3:
					base64 += "=";
					break;
			}

			return Convert.FromBase64String(base64);
		}

		public async Task Logout()
		{
			await _localStorageService.RemoveItemAsync("authToken");
			var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
			var authState = Task.FromResult(new AuthenticationState(anonymousUser));
			NotifyAuthenticationStateChanged(authState);
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
			var savedToken = await _localStorageService.GetItemAsync<string>("authToken");
			var claimsIdentity = !string.IsNullOrWhiteSpace(savedToken) ? new ClaimsIdentity(ParseClaimsFromJwt(savedToken), "jwt") : new ClaimsIdentity();
			return new AuthenticationState(new ClaimsPrincipal(claimsIdentity));
		}
	}
}
