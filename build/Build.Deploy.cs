using NuGet.Versioning;

partial class Build
{
    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "artifacts";

    Target Pack => _ => _
        .DependsOn(Test)
        .Produces(ArtifactsDirectory)
        .Executes(() =>
        {
            var version = GetVersionNo();
            DoPack(version);
        });

    [LatestNuGetVersion(
        packageId: "Flaeng.Productivity",
        IncludePrerelease = true)]
    readonly NuGetVersion LatestVersionOnNuGet;

    Target PackRC => _ => _
        .DependsOn(Test)
        .Requires(() => LatestVersionOnNuGet)
        .Requires(() => VersionParameter)
        .Produces(ArtifactsDirectory)
        .Executes(() =>
        {
            var version = GetVersionNo();
            version = NextVersionNo(version);
            DoPack(version);
        });

    private string NextVersionNo(string version)
    {
        if (LatestVersionOnNuGet.IsPrerelease == false)
        {
            return new Version(
                LatestVersionOnNuGet.Major,
                LatestVersionOnNuGet.Minor,
                LatestVersionOnNuGet.Patch + 1
            ).ToString() + "-rc.1";
        }
        else
        {
            int num = int.Parse(LatestVersionOnNuGet.ToString().Split("rc").Last());
            num++;
            return new Version(
                LatestVersionOnNuGet.Major,
                LatestVersionOnNuGet.Minor,
                LatestVersionOnNuGet.Patch
            ).ToString() + "-rc." + num;
        }
    }

    private void DoPack(string version)
    {
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
    }

    [Parameter("NuGet API Key"), Secret] readonly string NuGetApiKey;

    Target Publish => _ => _
        .OnlyWhenDynamic(() => IsTaggedBuild || IsServerBuild == false)
        .OnlyWhenDynamic(() => GitRepository.IsOnMainBranch())
        .DependsOn(Pack)
        .Requires(() => NuGetApiKey)
        .Executes(PublishArtificatsToNuGet);

    Target PublishRC => _ => _
        .OnlyWhenDynamic(() => IsTaggedBuild || IsServerBuild == false)
        .OnlyWhenDynamic(() => GitRepository.IsOnMainBranch())
        .DependsOn(PackRC)
        .Requires(() => NuGetApiKey)
        .Executes(PublishArtificatsToNuGet);

    private void PublishArtificatsToNuGet() => 
        ArtifactsDirectory.GlobFiles("*.nupkg")
            .ForEach(file =>
                NuGetTasks.NuGetPush(opts => opts
                    .SetApiKey(NuGetApiKey)
                    .SetSource(DefaultNuGetSource)
                    .SetTargetPath(file)
                    )
            );
    
    const string DefaultNuGetSource = "https://api.nuget.org/v3/index.json";
}
