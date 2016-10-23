var configuration = Argument("configuration", "Debug");

Task("Clean")
.Does(() =>
{
    CleanDirectory("./artifacts/");
});

Task("Restore")
.Does(() => 
{
    DotNetCoreRestore();
});

Task("Build")
.Does(() =>
{
    DotNetCoreBuild("./src/**/project.json", new DotNetCoreBuildSettings
    {
        Configuration = configuration
    });
});

Task("Pack")
.Does(() =>
{
    var projects = GetFiles("./src/**/project.json");
    Console.WriteLine("Packing {0} projects", projects.Count());

    foreach (var project in projects)
    {
        DotNetCorePack(project.FullPath, new DotNetCorePackSettings
        {
            Configuration = configuration,
            OutputDirectory = "./artifacts/"
        });
    }

    Console.WriteLine("Creating meta package");
    NuGetPack("./Stormpath.SDK.nuspec", new NuGetPackSettings
    {
        OutputDirectory = "./artifacts",
        ArgumentCustomization = args => args.Append("-Exclude **\\*")
    });
});

Task("Test")
.IsDependentOn("Restore")
.IsDependentOn("Build")
.Does(() =>
{
    new List<string>
    {
        "Stormpath.SDK.JsonNetSerializer.Tests",
        "Stormpath.SDK.RestSharpClient.Tests",
        "Stormpath.SDK.Tests",
        "Stormpath.SDK.Tests.Unit",
        "Stormpath.SDK.Tests.Sanity"
    }.ForEach(name =>
    {
        DotNetCoreTest("./test/" + name + "/project.json");
    });
});

Task("IntegrationTest")
.Does(() =>
{
    new List<string>
    {
        "Stormpath.SDK.Tests.CoreClr",
        "Stormpath.SDK.Tests.Integration",
    }.ForEach(name =>
    {
        DotNetCoreTest("./test/" + name + "/project.json");
    });
});

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("Pack");


var target = Argument("target", "Default");
RunTarget(target);
