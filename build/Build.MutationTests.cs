partial class Build
{
    readonly AbsolutePath StrykerOutput = RootDirectory / "src" / "StrykerOutput" / "**" / "mutation-report.html";

    Target Stryker => _ => _
        .DependsOn(Compile)
        .Produces(StrykerOutput)
        .Executes(() =>
        {
            DotNetTasks.DotNetToolInstall(opts => opts
                .SetPackageName("dotnet-stryker")
                .SetGlobal(true)
                .SetProcessExitHandler(process => { })
            );

            DotNetTasks.DotNet(
                arguments: "stryker -l Advanced",
                workingDirectory: Solution.Directory,
                logger: StrykerLogger);
        });

    private static void StrykerLogger(OutputType outputType, string arg2)
    {
        if (String.IsNullOrWhiteSpace(arg2))
            return;

        var type = arg2.Substring(10, 3);
        Action<string> logger = type switch
        {
            "ERR" => Log.Error,
            "WRN" => Log.Warning,
            "INF" => Log.Information,
            _ => Log.Information
        };
        string output = arg2.StartsWith("[") ? arg2[15..] : arg2;
        logger(output);
    }
}
