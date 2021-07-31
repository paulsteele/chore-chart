using System.Reflection;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using hub.Server.Configuration;
using hub.Server.Database;
using hub.Server.NativeBle;
using hub.Shared.Registration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace hub.Server
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
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints => {
					endpoints.MapRazorPages();
					endpoints.MapControllers();
					endpoints.MapFallbackToFile("index.html");
			});

			using ILifetimeScope setupScope = AutofacContainer.BeginLifetimeScope();
			setupScope.Resolve<IDb>().Init();
		}

		public void ConfigureServices(IServiceCollection services) {
			var configuration = new EnvironmentVariableConfiguration();
			services
				.AddDefaultIdentity<IdentityUser>((options => {
					options.Password.RequireDigit = false;
					options.Password.RequiredLength = 0;
					options.Password.RequireLowercase = false;
					options.Password.RequireUppercase = false;
					options.Password.RequiredUniqueChars = 0;
					options.Password.RequireNonAlphanumeric = false;
				}))
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<DatabaseContext>();

			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = configuration.JwtIssuer,
						ValidAudience = configuration.JwtAudience,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.JwtSecurityKey))
					};
				});
			services.AddControllersWithViews(options => {
				var policy = new AuthorizationPolicyBuilder()
					.RequireAuthenticatedUser()
					.Build();
				options.Filters.Add(new AuthorizeFilter(policy));
			});
			services.AddRazorPages();
		}

		public void ConfigureContainer(ContainerBuilder builder)
		{
			var assembly = Assembly.GetExecutingAssembly();

			builder.RegisterAssemblyTypes(assembly);
			builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();

			builder.Register(context => LoggerFactory.Create(logBuilder => logBuilder.AddConsole())).As<ILoggerFactory>();

			builder.RegisterType<Db>().As<IDb>().SingleInstance();
			builder.RegisterType<Bluetooth>().As<IBluetooth>().SingleInstance();
			builder.RegisterType<EnvironmentVariableConfiguration>().As<IEnvironmentVariableConfiguration>().SingleInstance();
			CommonContainer.Register(builder);
		}
	}
}
