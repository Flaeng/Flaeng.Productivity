partial class Build
{
    [Parameter("NuGet API Key"), Secret] readonly string NuGetApiKey;
    const string DefaultNuGetSource = "https://api.nuget.org/v3/index.json";
    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "artifacts";
    readonly AbsolutePath ReadmeSourceFile = RootDirectory / "README.md";
    readonly AbsolutePath LicenseSourceFile = RootDirectory / "LICENSE";
    readonly AbsolutePath ReadmeTargetFile = RootDirectory / "src" / "Flaeng.Productivity" / "README.md";
    readonly AbsolutePath LicenseTargetFile = RootDirectory / "src" / "Flaeng.Productivity" / "LICENSE";

    Target Pack => _ => _
        .Requires(() => VersionParameter)
        .DependsOn(Test)
        .Produces(ArtifactsDirectory)
        .Executes(() =>
        {
            var version = VersionParameter;
            var fileversion = version.Split('-').First();

            new Dictionary<AbsolutePath, AbsolutePath>
            {
                { ReadmeSourceFile, ReadmeTargetFile },
                { LicenseSourceFile, LicenseTargetFile }
            }.ForEach(x =>
            {
                x.Value.DeleteFile();
                x.Value.WriteAllBytes(x.Key.ReadAllBytes());
            });

            Projects
                .Where(x => x.IsTestProject() == false)
                .Where(x => x.IsSampleProject() == false)
                .ForEach(proj =>
                {
                    DotNetTasks.DotNetPack(opts => opts
                        .SetProcessWorkingDirectory("src")
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
                        .SetProcessWorkingDirectory("src")
                        .SetSource(DefaultNuGetSource)
                        .SetApiKey(NuGetApiKey)
                        .SetTargetPath(file)
                        )
                    )
            );
}
