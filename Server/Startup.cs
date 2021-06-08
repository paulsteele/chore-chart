using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using chores.Server.Database;
using chores.Shared.Registration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace chores.Server
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public ILifetimeScope AutofacContainer { get; private set; }

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			AutofacContainer = app.ApplicationServices.GetAutofacRoot();
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseWebAssemblyDebugging();
			}
			else
			{
				app.UseExceptionHandler("/Error");
			}

			app.UseBlazorFrameworkFiles();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseEndpoints(endpoints => {
					endpoints.MapRazorPages();
					endpoints.MapControllers();
					endpoints.MapFallbackToFile("index.html");
			});

			using ILifetimeScope setupScope = AutofacContainer.BeginLifetimeScope();
			setupScope.Resolve<IDb>().Init();
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews();
			services.AddRazorPages();
		}

		public void ConfigureContainer(ContainerBuilder builder)
		{
			var assembly = Assembly.GetExecutingAssembly();

			builder.RegisterAssemblyTypes(assembly);
			builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();

			builder.Register(context => LoggerFactory.Create(logBuilder => logBuilder.AddConsole())).As<ILoggerFactory>();

			builder.RegisterType<Db>().As<IDb>().SingleInstance();
			CommonContainer.Register(builder);
		}
	}
}
