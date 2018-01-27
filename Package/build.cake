//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var sln = File("../EarlyPusher.sln");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("../EarlyPusher/bin/x64") + Directory(configuration);

private void OutputException(Exception ex)
{
    Error(ex.Message);
    Error(ex.StackTrace);
    Error("============");
}

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore(sln);
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
      MSBuild(sln, settings =>
        settings.SetConfiguration(configuration));
});

Task("Package")
    .IsDependentOn("Build")
    .Does(() =>
{
    CopyDirectory("../licenses", buildDir + Directory("license"));
    CopyFileToDirectory("../ReadMe.md", buildDir);
    Zip(buildDir, "EarlyPusher.zip");     
})
.ReportError(ex =>
{
    OutputException(ex);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Package");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
