using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace hub.Server;

public static class Program {
	public static async Task Main(string[] args)
	{
		var host = Host.CreateDefaultBuilder(args)
			.UseServiceProviderFactory(new AutofacServiceProviderFactory())
			.ConfigureWebHostDefaults(webHostBuilder => {
				webHostBuilder.UseStartup<Startup>();
			})
			.Build();

		await host.RunAsync();
	}
}