/// Support plugins are available for:
///   - JetBrains ReSharper        https://nuke.build/resharper
///   - JetBrains Rider            https://nuke.build/rider
///   - Microsoft VisualStudio     https://nuke.build/visualstudio
///   - Microsoft VSCode           https://nuke.build/vscode
partial class Build : NukeBuild
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution(GenerateProjects = true)] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;

    public static int Main() => Execute<Build>(x => x.Compile);

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            DotNetTasks.DotNetClean(opts => opts
                .SetProcessWorkingDirectory(Solution.Directory)
            );

            var folders = Glob.Directories(Solution.Directory, "**/obj")
                .Concat(Glob.Directories(Solution.Directory, "**/bin"))
                .Select(path => Solution.Directory / path);
            Log.Information($"Deleting folders:{Environment.NewLine}{String.Join(Environment.NewLine, folders)}");
            folders.DeleteDirectories();
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetTasks.DotNetRestore(opts => opts
                .SetProcessWorkingDirectory(Solution.Directory)
            );
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetTasks.DotNetBuild(opts => opts
                .SetProcessWorkingDirectory(Solution.Directory)
                .SetTreatWarningsAsErrors(true)
                .SetNoRestore(true)
            );
        });

}
