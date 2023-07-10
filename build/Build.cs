/// Support plugins are available for:
///   - JetBrains ReSharper        https://nuke.build/resharper
///   - JetBrains Rider            https://nuke.build/rider
///   - Microsoft VisualStudio     https://nuke.build/visualstudio
///   - Microsoft VSCode           https://nuke.build/vscode
partial class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            Projects
                .ForEach(proj =>
                    DotNetTasks.DotNetClean(opts => opts
                    .SetProject(proj)));
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            Projects
                .ForEach(proj =>
                    DotNetTasks.DotNetRestore(opts => opts
                    .SetProjectFile(proj)));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            Projects
                .ForEach(proj =>
                    DotNetTasks.DotNetBuild(opts => opts
                    .SetProjectFile(proj)
                    .SetTreatWarningsAsErrors(true)
                    ));
        });

}
