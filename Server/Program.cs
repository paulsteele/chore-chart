using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace home.Server
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			IHost host = Host.CreateDefaultBuilder(args)
				.UseServiceProviderFactory(new AutofacServiceProviderFactory())
				.ConfigureWebHostDefaults(webHostBuilder => {
					webHostBuilder.UseStartup<Startup>();
				})
				.Build();

			host.Run();
		}
	}
}
