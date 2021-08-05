using System;
using Microsoft.Extensions.Logging;

namespace hub.Client.Logging {
	public class WebLoggerFactory : ILoggerFactory {
		public void Dispose() {
			throw new System.NotImplementedException();
		}

		public ILogger CreateLogger(string categoryName) {
			return new WebLogger();
		}

		public void AddProvider(ILoggerProvider provider) {
			throw new System.NotImplementedException();
		}
	}

	public class WebLogger : ILogger {
		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
			Console.WriteLine(formatter(state, exception));
		}

		public bool IsEnabled(LogLevel logLevel) {
			return true;
		}

		public IDisposable BeginScope<TState>(TState state) {
			throw new NotImplementedException();
		}
	}
}
