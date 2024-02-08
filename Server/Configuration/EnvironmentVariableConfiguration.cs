using System;

namespace hub.Server.Configuration;

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
	string DefaultUserName { get;  }
	string DefaultUserPass { get;  }
}

public class EnvironmentVariableConfiguration : IEnvironmentVariableConfiguration {
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

	public string JwtSecurityKey { get; } = GetVar(nameof(JwtSecurityKey), "default-signing-key-that-is-large-enough-to-hit-256-bytes", ConvertString);
	public string JwtIssuer { get; } = GetVar(nameof(JwtIssuer), "http://localhost", ConvertString);
	public string JwtAudience { get; } = GetVar(nameof(JwtAudience), "http://localhost", ConvertString);
	public int JwtExpiryHours { get; } = GetVar(nameof(JwtExpiryHours), 10, ConvertInt);

	public string DatabaseUrl { get; } = GetVar(nameof(DatabaseUrl), "localhost", ConvertString);
	public string DatabasePort { get; } = GetVar(nameof(DatabasePort), "3306", ConvertString);
	public string DatabaseUser { get; } = GetVar(nameof(DatabaseUser), "root", ConvertString);
	public string DatabasePassword { get; } = GetVar(nameof(DatabasePassword), "pass", ConvertString);
	public string DatabaseName { get; } = GetVar(nameof(DatabaseName), "hub", ConvertString);
	public string DefaultUserName { get; } = GetVar(nameof(DefaultUserName), "user", ConvertString);
	public string DefaultUserPass { get; } = GetVar(nameof(DefaultUserPass), "pass", ConvertString);
}