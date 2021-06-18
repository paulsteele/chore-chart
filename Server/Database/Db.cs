using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace home.Server.Database {
	public interface IDb {
		void Init();
	}

	public class Db : IDb {
		private readonly ILogger _logger;
		private readonly DatabaseContext _databaseContext;

		public Db(ILogger logger, DatabaseContext databaseContext) {
			_logger = logger;
			_databaseContext = databaseContext;
		}

		public void Init() {
			_logger.LogInformation("Initializing Database");
			_databaseContext.Database.Migrate();
		}
	}
}
