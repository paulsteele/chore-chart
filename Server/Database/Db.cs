using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace hub.Server.Database;

public interface IDb {
	void Init();
	DatabaseContext DatabaseContext { get; }
}

public class Db(ILogger logger, DatabaseContext databaseContext) : IDb {
	public DatabaseContext DatabaseContext { get; } = databaseContext;

	public void Init() {
		logger.LogInformation("Initializing Database");
		DatabaseContext.Database.Migrate();
	}
}