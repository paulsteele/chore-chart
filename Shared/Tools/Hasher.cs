namespace chores.Shared.Tools {
	public interface IHasher {
		string Hash(string input);
	}

	public class Hasher : IHasher {
		public string Hash(string input) {
			return BCrypt.Net.BCrypt.HashPassword(input);
		}
	}
}
