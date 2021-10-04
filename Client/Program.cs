using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Blazored.LocalStorage;
using hub.Client.Logging;
using hub.Client.Services.Alerts;
using hub.Client.Services.Authentication;
using hub.Shared.Registration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace hub.Client
{
	public class Program {
		private static Uri _baseAddress;
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			_baseAddress = new Uri(builder.HostEnvironment.BaseAddress);
			builder.ConfigureContainer(new AutofacServiceProviderFactory(Register));
			builder.Services.AddAuthorizationCore();
			builder.Services.AddBlazoredLocalStorage();
			builder.RootComponents.Add<App>("#app");

			await builder.Build().RunAsync();
		}

		private static void Register(ContainerBuilder builder)
		{
			var assembly = Assembly.GetExecutingAssembly();
			builder.RegisterAssemblyTypes(assembly);
			builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();

			builder.Register(context => new HttpClient() {BaseAddress = _baseAddress});
			builder.Register(context => new WebLoggerFactory()).As<ILoggerFactory>();
			builder.RegisterType<AuthService>()
				.As<IAuthService>()
				.As<AuthenticationStateProvider>()
				.SingleInstance();

			builder.RegisterType<AlertService>()
				.As<IAlertService>()
				.SingleInstance();

			CommonContainer.Register(builder);
		}
	}
}
