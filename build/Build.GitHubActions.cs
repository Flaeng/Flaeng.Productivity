[GitHubActions(
    "PR",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = new[] { /*nameof(Format),*/ nameof(Test) },
    OnPullRequestBranches = new[] { "main" },
    PublishArtifacts = false,
    EnableGitHubToken = true,
    ImportSecrets = new[] { nameof(NuGetApiKey) }
)]
[GitHubActions(
    "Build",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = new[] { nameof(Stryker) },
    OnPushBranches = new[] { "main" },
    OnPushIncludePaths = new[] { "src/**" },
    PublishArtifacts = true,
    EnableGitHubToken = true,
    ImportSecrets = new[] { nameof(NuGetApiKey) }
)]
[GitHubActions(
    "Deploy",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = new[] { nameof(Publish) },
    OnPushBranches = new[] { "main" },
    OnPushIncludePaths = new[] { "src/**" },
    PublishArtifacts = true,
    EnableGitHubToken = true,
    ImportSecrets = new[] { nameof(NuGetApiKey) }
)]
partial class Build
{
    GitHubActions GitHubActions => GitHubActions.Instance;

    [Parameter("NuGet API Key")][Secret] readonly string NuGetApiKey;
}
