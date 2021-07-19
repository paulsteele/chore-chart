using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace hub.Server.Database {
	public interface IDb {
		void Init();
		DatabaseContext DatabaseContext { get; }
	}

	public class Db : IDb {
		private readonly ILogger _logger;
		public DatabaseContext DatabaseContext { get; }

		public Db(ILogger logger, DatabaseContext databaseContext) {
			_logger = logger;
			DatabaseContext = databaseContext;
		}

		public void Init() {
			_logger.LogInformation("Initializing Database");
			DatabaseContext.Database.Migrate();
		}
	}
}
