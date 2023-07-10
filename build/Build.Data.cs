partial class Build
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;

    bool IsTaggedBuild => GitRepository.Tags.Any();

    [Parameter("Version of the package being built for NuGets")] readonly string VersionParameter;

    IEnumerable<Project> Projects => Solution
        .Projects.Where(x => x.Name != "_build");

    string GetVersionNo()
    {
        if (IsServerBuild)
        {
            var tags = GitRepository.Tags;
            return tags.FirstOrDefault();
        }
        else return VersionParameter;
    }
}
