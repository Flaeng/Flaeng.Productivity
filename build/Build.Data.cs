using System.Diagnostics;

partial class Build
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;

    bool IsTaggedBuild => GitRepository.Tags.Any();

    [Parameter(
        description: "Version of the package being built for NuGets",
        Name = "Version"
        )]
    readonly string VersionParameter;

    IEnumerable<Project> Projects => Solution
        .Projects.Where(x => x.Name != "_build");

    string GetVersionNo()
    {
        // if (Debugger.IsAttached)
        //     return "0.3.0-rc.1";
        if (IsServerBuild)
        {
            var tags = GitRepository.Tags;
            return tags.Last();
        }
        else return VersionParameter;
    }
}
