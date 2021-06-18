using Autofac;
using Microsoft.Extensions.Logging;

namespace home.Shared.Registration {
	public class CommonContainer {

		public static void Register(ContainerBuilder containerBuilder) {
			containerBuilder.Register(context => context.Resolve<ILoggerFactory>().CreateLogger<CommonContainer>()).As<ILogger>();
		}
	}
}
