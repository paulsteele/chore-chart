using hub.Shared.Models;

namespace hub.Client.Services {
	public interface ISessionService {
		Session Session { get; set; }
	}

	public class SessionService : ISessionService {
		public Session Session { get; set; }

	}
}
