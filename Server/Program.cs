using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using hub.Server.Commands;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace hub.Server
{
	public static class Program {
		private const string AddUserCommand = "addUser";

		public static void Main(string[] args)
		{
			IHost host = Host.CreateDefaultBuilder(args)
				.UseServiceProviderFactory(new AutofacServiceProviderFactory())
				.ConfigureWebHostDefaults(webHostBuilder => {
					webHostBuilder.UseStartup<Startup>();
				})
				.Build();

			if (args.Contains(AddUserCommand)) {
				host.Services.GetAutofacRoot().Resolve<AddUserCommand>().StartCommand();

				return;
			}

			host.Run();
		}
	}
}
