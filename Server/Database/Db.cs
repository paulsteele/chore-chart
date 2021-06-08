using System;
using Microsoft.Extensions.Logging;

namespace chores.Server.Database {
	public interface IDb {
		void Init();
	}

	public class Db : IDb {
		private readonly ILogger _logger;

		public Db(ILogger logger) {
			_logger = logger;
		}
		public void Init() {
			_logger.LogInformation("Initializing Database");
		}
	}
}
