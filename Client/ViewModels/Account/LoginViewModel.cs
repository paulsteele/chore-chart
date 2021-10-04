using System.Threading.Tasks;
using hub.Client.Services.Authentication;
using hub.Shared.Models;

namespace hub.Client.ViewModels.Account {
	public interface ILoginViewModel {
		public Task<LoginResult> Login(LoginModel loginModel);
	}

	public class LoginViewModel : ILoginViewModel {
		private readonly IAuthService _authService;
		public LoginViewModel(IAuthService authService) {
			_authService = authService;
		}

		public Task<LoginResult> Login(LoginModel loginModel) {
			return _authService.Login(loginModel);
		}
	}
}
