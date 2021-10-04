using System;
using System.Net.Http;
using System.Threading.Tasks;
using hub.Client.Services.Authentication;
using Microsoft.AspNetCore.Components;

namespace hub.Client.Services.Web
{

    public class AuthedHttpClient : HttpClient
    {
        private readonly IAuthService _authService;

        public AuthedHttpClient(IAuthService authService, NavigationManager navigationManager)
        {
            _authService = authService;
            BaseAddress = new Uri(navigationManager.BaseUri);
        }

        public async Task Init()
        {
            DefaultRequestHeaders.Authorization = await _authService.GetAuthHeader();
        }
    }
}