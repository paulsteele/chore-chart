using System;

namespace hub.Shared.Tools {
	public interface INowTimeProvider {
		DateTime Now { get; }
	}

	public class NowTimeProvider : INowTimeProvider {
		public DateTime Now => DateTime.Now;
	}
}
