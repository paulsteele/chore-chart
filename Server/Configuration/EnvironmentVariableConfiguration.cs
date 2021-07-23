using System;

namespace hub.Server.Configuration {
	public interface IEnvironmentVariableConfiguration {
		string JwtSecurityKey { get; }
		string JwtIssuer { get; }
		string JwtAudience { get; }
		int JwtExpiryHours { get; }
	}

	public class EnvironmentVariableConfiguration : IEnvironmentVariableConfiguration {

		public EnvironmentVariableConfiguration() {

			JwtSecurityKey = GetVar(nameof(JwtSecurityKey), "default-signing-key", ConvertString);
			JwtIssuer = GetVar(nameof(JwtIssuer), "http://localhost", ConvertString);
			JwtAudience = GetVar(nameof(JwtAudience), "http://localhost", ConvertString);
			JwtExpiryHours = GetVar(nameof(JwtExpiryHours), 1, ConvertInt);
		}

		private T GetVar<T>(string name, T defaultValue, Func<string, T> converter) {
			var envVar = Environment.GetEnvironmentVariable(name);
			if (envVar != null) {
				return converter(envVar);
			}

			return defaultValue;
		}

		private string ConvertString(string s) {
			return s;
		}

		private int ConvertInt(string s) {
			return int.Parse(s);
		}

		public string JwtSecurityKey { get; }
		public string JwtIssuer { get; }
		public string JwtAudience { get; }
		public int JwtExpiryHours { get; }
	}
}
