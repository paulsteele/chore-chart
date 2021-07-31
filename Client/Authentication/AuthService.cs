using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using hub.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace hub.Client.Authentication {
	public interface IAuthService {
		Task<LoginResult> Login(LoginModel loginModel);
		Task Logout();
		ReadOnlyCollection<Claim> Claims { get; }
	}

	public class AuthService : AuthenticationStateProvider, IAuthService, IAuthorizationPolicyProvider, IAuthorizationService{
		private readonly HttpClient _httpClient;

		public AuthService(HttpClient httpClient)
		{
			_httpClient = httpClient;
			InternalClaims = new List<Claim>();
			Claims = new ReadOnlyCollection<Claim>(InternalClaims);
		}

		public ReadOnlyCollection<Claim> Claims { get;}
		private List<Claim> InternalClaims { get; }

		public async Task<LoginResult> Login(LoginModel loginModel)
		{
			var response = await _httpClient.PostAsJsonAsync("login", loginModel);

			var loginResult = await response.Content.ReadFromJsonAsync<LoginResult>();

			if (response.IsSuccessStatusCode && loginResult is {Token: { }}) {
				InternalClaims.Clear();
				InternalClaims.AddRange(ParseClaimsFromJwt(loginResult.Token));
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

		public Task Logout()
		{
			//await _localStorage.RemoveItemAsync("authToken");
			//((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
			//_httpClient.DefaultRequestHeaders.Authorization = null;
			return Task.CompletedTask;
		}

		public override Task<AuthenticationState> GetAuthenticationStateAsync() {
			var tsc = new TaskCompletionSource<AuthenticationState>();
			tsc.SetResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(Claims, "jwt"))));
			return tsc.Task;
		}

		public Task<AuthorizationPolicy> GetPolicyAsync(string policyName) {
			return GetDefaultPolicyAsync();
		}

		public Task<AuthorizationPolicy> GetDefaultPolicyAsync() {
			var tsc = new TaskCompletionSource<AuthorizationPolicy>();

			var policy = new AuthorizationPolicyBuilder()
				.RequireAuthenticatedUser()
				.Build();

			tsc.SetResult(policy);

			return tsc.Task;
		}

		public Task<AuthorizationPolicy> GetFallbackPolicyAsync() {
			return GetDefaultPolicyAsync();
		}

		public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, IEnumerable<IAuthorizationRequirement> requirements) {
			var tsc = new TaskCompletionSource<AuthorizationResult>();

			tsc.SetResult(AuthorizationResult.Failed());

			return tsc.Task;
		}

		public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, string policyName) {
			var tsc = new TaskCompletionSource<AuthorizationResult>();

			tsc.SetResult(AuthorizationResult.Failed());

			return tsc.Task;
		}
	}
}
