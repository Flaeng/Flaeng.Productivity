using NuGet.Versioning;

partial class Build
{
    [Parameter("NuGet API Key"), Secret] readonly string NuGetApiKey;
    const string DefaultNuGetSource = "https://api.nuget.org/v3/index.json";
    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "artifacts";

    Target Pack => _ => _
        .Requires(() => VersionParameter)
        .DependsOn(Test)
        .Produces(ArtifactsDirectory)
        .Executes(() =>
        {
            var version = VersionParameter;
            var fileversion = version.Split('-').First();

            Projects
                .Where(x => x.IsTestProject() == false)
                .Where(x => x.IsSampleProject() == false)
                .ForEach(proj =>
                {
                    DotNetTasks.DotNetPack(opts => opts
                        .SetProject(proj)
                        .SetVersion(version)
                        .SetFileVersion(version)
                        .SetAssemblyVersion(fileversion)
                        .SetInformationalVersion(version)
                        .SetIncludeSymbols(true)
                        .SetIncludeSource(true)
                        .SetConfiguration(Configuration.Release)
                        );
                });

            ArtifactsDirectory.DeleteDirectory();
            ArtifactsDirectory.CreateDirectory();
            RootDirectory.GlobFiles($"**/*.{version}.nupkg")
                .ForEach(file => file.MoveToDirectory(ArtifactsDirectory));
        });

    Target Publish => _ => _
        .OnlyWhenDynamic(() => IsLocalBuild && GitRepository.IsOnMainBranch())
        .DependsOn(Pack)
        .Requires(() => NuGetApiKey)
        .Executes(() =>
            ArtifactsDirectory.GlobFiles("*.nupkg")
                .ForEach(file =>
                    NuGetTasks.NuGetPush(opts => opts
                        .SetSource(DefaultNuGetSource)
                        .SetApiKey(NuGetApiKey)
                        .SetTargetPath(file)
                        )
                    )
            );
}
