using System;

namespace hub.Server.Configuration {
	public interface IEnvironmentVariableConfiguration {
		string JwtSecurityKey { get; }
		string JwtIssuer { get; }
		string JwtAudience { get; }
		int JwtExpiryHours { get; }
		string DatabaseUrl { get; }
		string DatabasePort { get; }
		string DatabaseUser { get; }
		string DatabasePassword { get; }
		string DatabaseName { get; }
	}

	public class EnvironmentVariableConfiguration : IEnvironmentVariableConfiguration {

		public EnvironmentVariableConfiguration() {

			JwtSecurityKey = GetVar(nameof(JwtSecurityKey), "default-signing-key", ConvertString);
			JwtIssuer = GetVar(nameof(JwtIssuer), "http://localhost", ConvertString);
			JwtAudience = GetVar(nameof(JwtAudience), "http://localhost", ConvertString);
			JwtExpiryHours = GetVar(nameof(JwtExpiryHours), 10, ConvertInt);
			DatabaseUrl = GetVar(nameof(DatabaseUrl), "localhost", ConvertString);
			DatabasePort = GetVar(nameof(DatabasePort), "3306", ConvertString);
			DatabaseUser = GetVar(nameof(DatabaseUser), "root", ConvertString);
			DatabasePassword = GetVar(nameof(DatabasePassword), "pass", ConvertString);
			DatabaseName = GetVar(nameof(DatabaseName), "hub", ConvertString);
		}

		private static T GetVar<T>(string name, T defaultValue, Func<string, T> converter) {
			var envVar = Environment.GetEnvironmentVariable(name);
			return envVar != null ? converter(envVar) : defaultValue;
		}

		private static string ConvertString(string s) {
			return s;
		}

		private static int ConvertInt(string s) {
			return int.Parse(s);
		}

		public string JwtSecurityKey { get; }
		public string JwtIssuer { get; }
		public string JwtAudience { get; }
		public int JwtExpiryHours { get; }
		
		public string DatabaseUrl { get; }
		public string DatabasePort { get; }
		public string DatabaseUser { get; }
		public string DatabasePassword { get; }
		public string DatabaseName { get; }
	}
}
