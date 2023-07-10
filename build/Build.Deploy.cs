partial class Build
{
    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "artifacts" / "*.*";

    Target Pack => _ => _
        .DependsOn(Compile)
        .Produces(ArtifactsDirectory)
        .Executes(() =>
        {
            // if (Configuration != Configuration.Release)
            //     throw new InvalidOperationException("Cannot pack in release configuration");

            var version = GetVersionNo();

            Projects
                .Where(x => x.IsTestProject() == false)
                .ForEach(proj =>
                {
                    DotNetTasks.DotNetPack(opts => opts
                        .SetVersion(version)
                        .SetFileVersion(version)
                        .SetAssemblyVersion(version)
                        .SetInformationalVersion(version)
                        .SetIncludeSymbols(true)
                        .SetIncludeSource(true)
                        );

                    var nuspec = proj.Directory.GlobFiles("*.nuspec")
                        .SingleOrDefault();

                    if (nuspec == null)
                        return;

                    NuGetTasks.NuGetPack(opts => opts
                        .SetOutputDirectory(ArtifactsDirectory)
                        .SetConfiguration(Configuration)
                        .SetVersion(version)
                        .SetTargetPath(nuspec)
                        .SetSymbols(true)
                        );
                });

            RootDirectory.GlobFiles("*.nupkg")
                .ForEach(file => file
                    .MoveToDirectory(ArtifactsDirectory)
                    );
        });

    [Parameter("NuGet API Key"), Secret] readonly string NuGetApiKey;

    Target Publish => _ => _
        .OnlyWhenDynamic(() => IsTaggedBuild)
        .DependsOn(Pack)
        .Requires(() => NuGetApiKey)
        .Executes(() =>
        {
            NuGetTasks.NuGetPush(opts => opts
                );
            DotNetTasks.DotNetNuGetPush(opts => opts
                );
        });
}
