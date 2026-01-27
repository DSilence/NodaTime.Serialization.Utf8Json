using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.Ci);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly string Configuration = IsLocalBuild ? "Debug" : "Release";

    [Parameter("Explicit framework to build")] readonly string Framework = "";

    [Parameter("Collect code coverage. Default is 'true'")] readonly bool? Cover = true;

    [Parameter("Coverage threshold. Default is 80%")] readonly int Threshold = 70;

    [Solution("src/NodaTime.Serialization.Utf8Json/NodaTime.Serialization.Utf8Json.sln")] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion(Framework = "net10")] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    Target Clean => _ => _
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").DeleteDirectories();
            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .DependsOn(Clean)
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
                .SetAssemblyVersion($"{GitVersion.Major}.{GitVersion.Minor}.0")
                .SetFileVersion(GitVersion.MajorMinorPatch)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .SetFramework(Framework)
                .EnableNoRestore()

            );
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(Solution.GetProject("DS.NodaTime.Serialization.Utf8Json.Tests"))
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore()
                .ClearLoggers()
                .SetProcessOutputLogging(true)
                .AddLoggers("trx")
                .SetFramework(Framework)
                .SetProcessAdditionalArguments($"/p:CollectCoverage={Cover}",
                    $"/p:CoverletOutput={ArtifactsDirectory / "coverage"}/",
                    $"/p:Threshold={Threshold}",
                    $"/p:UseSourceLink={true}",
                    $"/p:CoverletOutputFormat=cobertura"
                    )
                .SetResultsDirectory(ArtifactsDirectory / "tests"));
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetPack(s => s
                .SetProject(Solution.GetProject("DS.NodaTime.Serialization.Utf8Json"))
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore()
                .SetVersion(GitVersion.FullSemVer)
                .SetOutputDirectory(ArtifactsDirectory / "nuget"));
        });

    Target CiNonWin => _ => _
        .DependsOn(Test);
    Target Ci => _ => _
        .DependsOn(Pack, Test);

}
