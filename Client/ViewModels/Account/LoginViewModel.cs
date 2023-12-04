using System.Threading.Tasks;
using hub.Client.Services.Authentication;
using hub.Shared.Models;

namespace hub.Client.ViewModels.Account;

public interface ILoginViewModel {
	public Task<LoginResult> Login(LoginModel loginModel);
}

public class LoginViewModel(IAuthService authService) : ILoginViewModel 
{
	public Task<LoginResult> Login(LoginModel loginModel) {
		return authService.Login(loginModel);
	}
}