namespace hub.Shared.Tools {
	public interface IHasher {
		string Hash(string input);
		bool Validate(string input, string hash);
	}

	public class Hasher : IHasher {
		public string Hash(string input) {
			return BCrypt.Net.BCrypt.HashPassword(input);
		}

		public bool Validate(string input, string hash) {
			return BCrypt.Net.BCrypt.Verify(input, hash);
		}
	}
}
