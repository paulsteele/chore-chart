using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace dotvote.Client
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("app");

			builder.ConfigureContainer(new AutofacServiceProviderFactory(Register));

			await builder.Build().RunAsync();
		}

		private static void Register(ContainerBuilder builder)
		{
			// add any registrations here
			builder.RegisterType<HttpClient>().As<HttpClient>();
		}
	}
}