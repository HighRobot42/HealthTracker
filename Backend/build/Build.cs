using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using System.Linq;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    [Solution] readonly Solution Solution = null!;

    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Build configuration — Debug (local) or Release (CI)")]
    readonly string Configuration = IsLocalBuild ? "Debug" : "Release";

    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    Target Clean => _ => _
        .Executes(() => DotNetClean(s => s.SetProject(Solution)));

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() => DotNetRestore(s => s.SetProjectFile(Solution)));

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() => DotNetBuild(s => s
            .SetProjectFile(Solution)
            .SetConfiguration(Configuration)
            .EnableNoRestore()));

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() => DotNetTest(s => s
            .SetProjectFile(Solution)
            .SetConfiguration(Configuration)
            .EnableNoRestore()
            .EnableNoBuild()));

    Target Publish => _ => _
        .DependsOn(Test)
        .Executes(() =>
        {
            var apiProject = Solution.AllProjects.First(p => p.Name.EndsWith(".Api"));
            DotNetPublish(s => s
                .SetProject(apiProject)
                .SetConfiguration("Release")
                .SetOutput(ArtifactsDirectory / "api"));
        });

    Target ExportSwagger => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            var api = Solution.AllProjects.FirstOrDefault(p =>
                p.Name == "Api" || p.Name.EndsWith(".Api"));
            if (api is null) return;
            DotNetRun(s => s
                .SetProjectFile(api)
                .SetApplicationArguments($"--export-swagger {RootDirectory / "swagger.json"}")
                .SetConfiguration("Release")
                .EnableNoBuild()
                .EnableNoRestore());
        });
}
