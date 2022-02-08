using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.EntityFramework;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local)")]
    readonly Configuration Configuration = Configuration.Debug;
    [Parameter("Name of migration to modify")]
    readonly string MigrationName;

    [Solution] readonly Solution Solution;
    [PathExecutable]
    readonly Tool Podman;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            RootDirectory
                .GlobDirectories("**/bin", "**/obj")
                .Where(d => !d.ToString().Contains("NativeBle"))
                .Where(d => !d.ToString().Contains("Build"))
                .ForEach(DeleteDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target Run => _ => _
        .Executes(() =>
        {
            DotNetRun(s => s
                .SetProjectFile(Solution.GetProject("twitchDnd.Server"))
                .SetConfiguration(Configuration)
                .EnableNoRestore()
            );
        });

    Target Publish => _ => _
        .DependsOn(Restore)
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetPublish(s => s
                .SetProject(Solution)
                .SetConfiguration(Configuration.Release)
            );
        });

    Target ListMigrations => _ => _
        .Executes(() =>
        {
            EntityFrameworkTasks
                .EntityFrameworkMigrationsList(s => s
                    .SetProject(Solution.GetProject("hub.Server")));
        });
    Target AddMigration => _ => _
        .Requires(() => MigrationName)
        .Executes(() =>
        {
            EntityFrameworkTasks
                .EntityFrameworkMigrationsAdd(s => s
                    .SetProject(Solution.GetProject("hub.Server"))
                    .SetName(MigrationName)
                );
        });

    Target CreateDevContainers => _ => _
        .Executes(() =>
        {
            Podman("run -d --name hub-db -p 3306:3306 -e MYSQL_ROOT_PASSWORD=pass mysql --default-authentication-plugin=mysql_native_password");
            Podman("run -d --name hub-adminer -p 5200:8080 adminer");
        });
    
    Target StartDevContainers => _ => _
        .Executes(() =>
        {
            Podman("start hub-db");
            Podman("start hub-adminer");
        });
    
    Target StopDevContainers => _ => _
        .Before(RemoveDevContainers)
        .Executes(() =>
        {
            Podman("stop hub-db");
            Podman("stop hub-adminer");
        });
    
    Target RemoveDevContainers => _ => _
        .Executes(() =>
        {
            Podman("rm hub-db");
            Podman("rm hub-adminer");
        });
}
