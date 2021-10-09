using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using hub.Client.Services.Alerts;
using hub.Shared.Models;
using hub.Shared.Tools;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace hub.Client.Services.Authentication {
	public interface IAuthService {
		Task<LoginResult> Login(LoginModel loginModel);
		Task<AuthenticationHeaderValue> GetAuthHeader();
		Task Logout();
	}

	public class AuthService : AuthenticationStateProvider, IAuthService {
		private readonly HttpClient _httpClient;
		private readonly ILogger _logger;
		private readonly ILocalStorageService _localStorageService;
		private readonly IAlertService _alertService;
		private readonly INowTimeProvider _nowTimeProvider;

		public AuthService(
			HttpClient httpClient,
			ILogger logger,
			ILocalStorageService localStorageService,
			IAlertService alertService,
			INowTimeProvider nowTimeProvider
		)
		{
			_httpClient = httpClient;
			_logger = logger;
			_localStorageService = localStorageService;
			_alertService = alertService;
			_nowTimeProvider = nowTimeProvider;
		}

		public async Task<LoginResult> Login(LoginModel loginModel)
		{
			var response = await _httpClient.PostAsJsonAsync("login", loginModel);

			var loginResult = await response.Content.ReadFromJsonAsync<LoginResult>();

			if (response.IsSuccessStatusCode && loginResult is {Token: { }}) {
				await _localStorageService.SetItemAsync("authToken", loginResult.Token);

				var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, loginModel.Username) }, "apiauth"));
				var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
				_alertService.DismissAlert();
				NotifyAuthenticationStateChanged(authState);
			}
			else
			{
				_alertService.ShowAlert("Error.", "Login failed. Please check your credentials and try again.");
			}

			return loginResult;
		}

		private ClaimsIdentity ParseClaimIdentityFromJwt(string jwt) {
			var claims = new List<Claim>();
			var payload = jwt.Split('.')[1];
			var jsonBytes = ParseBase64WithoutPadding(payload);
			Dictionary<string, object>? keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

			keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles);

			if (roles != null) {
				if (roles.ToString().Trim().StartsWith("["))
				{
					var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

					claims.AddRange(parsedRoles.Select(parsedRole => new Claim(ClaimTypes.Role, parsedRole)));
				}
				else {
					claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
				}

				keyValuePairs.Remove(ClaimTypes.Role);
			}

			claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

			keyValuePairs.TryGetValue("exp", out var expiration);

			var identity = new ClaimsIdentity(claims);

			if (expiration == null) return identity;
			if (!long.TryParse(expiration.ToString(), out var utcExpiration)) return identity;

			var expirationDate = DateTimeOffset.FromUnixTimeSeconds(utcExpiration).UtcDateTime;
			
			if (expirationDate > _nowTimeProvider.Now)
			{
				identity = new ClaimsIdentity(claims, "jwt");
			}

			return identity;
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

		public async Task<AuthenticationHeaderValue> GetAuthHeader() {
			return new AuthenticationHeaderValue("bearer", await _localStorageService.GetItemAsync<string>("authToken"));
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
			var claimsIdentity = !string.IsNullOrWhiteSpace(savedToken) ? ParseClaimIdentityFromJwt(savedToken) : new ClaimsIdentity();
			return new AuthenticationState(new ClaimsPrincipal(claimsIdentity));
		}
	}
}
