using System.Reflection;
using Autofac;
using Microsoft.Extensions.Logging;

namespace hub.Shared.Registration {
	public class CommonContainer {

		public static void Register(ContainerBuilder containerBuilder) {
			containerBuilder.Register(context => context.Resolve<ILoggerFactory>().CreateLogger<CommonContainer>()).As<ILogger>();

			var assembly = Assembly.GetExecutingAssembly();
			containerBuilder.RegisterAssemblyTypes(assembly);
			containerBuilder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
		}
	}
}
