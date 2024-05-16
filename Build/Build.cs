using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.EntityFramework;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[ShutdownDotNetAfterServerBuild]
internal class Build : NukeBuild
{
	[Parameter("Configuration to build - Default is 'Debug' (local)")]
	private readonly Configuration _configuration = Configuration.Debug;

	[Parameter("Name of migration to modify")]
	private readonly string _migrationName;

	[PathVariable("podman")] private readonly Tool _podman;

	// ReSharper disable once InconsistentNaming
	[Solution] private readonly Solution Solution;

	private Target Clean => _ => _
		.Before(Restore)
		.Executes(() =>
		{
			RootDirectory
				.GlobDirectories("**/bin", "**/obj")
				.Where(d => d.ToString()!.Contains("NativeBle"))
				.Where(d => d.ToString()!.Contains("Build"))
				.ForEach(s => s.DeleteDirectory());
		});

	private Target Restore => _ => _
		.Executes(() =>
		{
			DotNetRestore(s => s
				.SetProjectFile(Solution));
		});

	private Target Compile => _ => _
		.DependsOn(Restore)
		.Executes(() =>
		{
			DotNetBuild(s => s
				.SetProjectFile(Solution)
				.SetConfiguration(_configuration)
				.EnableNoRestore());
		});

	private Target Run => _ => _
		.Executes(() =>
		{
			DotNetRun(s => s
				.SetProjectFile(Solution.GetProject("hub.Server"))
				.SetConfiguration(_configuration)
				.EnableNoRestore()
			);
		});

	private Target Publish => _ => _
		.DependsOn(Restore)
		.DependsOn(Clean)
		.Executes(() =>
		{
			DotNetPublish(s => s
				.SetProject(Solution)
				.SetConfiguration(Configuration.Release)
			);
		});

	private Target ListMigrations => _ => _
		.Executes(() =>
		{
			EntityFrameworkTasks
				.EntityFrameworkMigrationsList(s => s
					.SetProject(Solution.GetProject("hub.Server")));
		});

	private Target AddMigration => _ => _
		.Requires(() => _migrationName)
		.Executes(() =>
		{
			EntityFrameworkTasks
				.EntityFrameworkMigrationsAdd(s => s
					.SetProject(Solution.GetProject("hub.Server"))
					.SetName(_migrationName)
				);
		});

	private Target CreateDevContainers => _ => _
		.Executes(() =>
		{
			_podman("run -d --name hub-db -p 3306:3306 -e MYSQL_ROOT_PASSWORD=pass mysql --mysql-native-password=ON");
			_podman("run -d --name hub-adminer -p 5001:8080 adminer");
		});

	private Target StartDevContainers => _ => _
		.Executes(() =>
		{
			_podman("start hub-db");
			_podman("start hub-adminer");
		});

	private Target StopDevContainers => _ => _
		.Before(RemoveDevContainers)
		.Executes(() =>
		{
			_podman("stop hub-db");
			_podman("stop hub-adminer");
		});

	private Target RemoveDevContainers => _ => _
		.Executes(() =>
		{
			_podman("rm hub-db");
			_podman("rm hub-adminer");
		});

	public static int Main()
	{
		return Execute<Build>(x => x.Compile);
	}
}