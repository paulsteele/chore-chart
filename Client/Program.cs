using System;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using hub.Client.Logging;
using hub.Shared.Registration;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;

namespace hub.Client
{
	public class Program {
		private static Uri BaseAddress;
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
			builder.ConfigureContainer(new AutofacServiceProviderFactory(Register));
			builder.RootComponents.Add<App>("#app");

			await builder.Build().RunAsync();
		}

		private static void Register(ContainerBuilder builder)
		{
			builder.Register(context => new HttpClient() {BaseAddress = BaseAddress});
			builder.Register(context => new WebLoggerFactory()).As<ILoggerFactory>();
			CommonContainer.Register(builder);
		}
	}
}
