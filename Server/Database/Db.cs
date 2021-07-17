using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace hub.Server.Database {
	public interface IDb {
		void Init();
	}

	public class Db : IDb {
		private readonly ILogger _logger;
		public readonly DatabaseContext DatabaseContext;

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
