using System.Threading.Tasks;
using System;
using Amg.Build;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using Amg.FileSystem;
using Amg.Extensions;
using System.Text.RegularExpressions;

namespace Build;

public partial class Program
{
    private static readonly Serilog.ILogger Logger = Serilog.Log.Logger.ForContext(System.Reflection.MethodBase.GetCurrentMethod()!.DeclaringType);

    static int Main(string[] args) => Amg.Build.Runner.Run<Program>(args);

    string Company => "AMG";
    string Name => "Amg.GetOpt";
    string productName => Name;
    string year => DateTime.UtcNow.ToString("yyyy");
    string copyright => $"Copyright (c) {Company} {year}";

    [Once, Description("Release or Debug. Default: Release")]
    public virtual string Configuration { get; set; } = ConfigurationRelease;

    const string ConfigurationRelease = "Release";
    const string ConfigurationDebug = "Debug";

    string Root => Runner.RootDirectory();
    string OutDir => Root.Combine("out", Configuration.ToString());
    string PackagesDir => OutDir.Combine("packages");
    string SrcDir => Root;
    string CommonAssemblyInfoFile => OutDir.Combine("CommonAssemblyInfo.cs");
    string VersionPropsFile => OutDir.Combine("Version.props");

    string SlnFile => SrcDir.Combine($"{Name}.sln");
    string LibDir => SrcDir.Combine(Name);

    [Once]
    protected virtual Dotnet Dotnet => Once.Create<Dotnet>();

    [Once]
    protected virtual Git Git => Git.Create(Runner.RootDirectory());

    [Once]
    [Description("Build")]
    public virtual async Task Build()
    {
        await (await Dotnet.Tool()).Run("build", SlnFile,
            "--configuration", this.Configuration);
    }

    [Once, Description("run unit tests")]
    public virtual async Task Test()
    {
        await Build();
        await (await Dotnet.Tool()).Run("test",
            SlnFile,
            "--no-build",
            "--configuration", Configuration
            );
    }

    [Once, Description("measure code coverage")]
    public virtual async Task CodeCoverage()
    {
        await Build();
        await (await Dotnet.Tool()).Run("test", SlnFile, "--no-build",
            "--collect:Code Coverage"
            );
    }

    [Once]
    protected virtual ITool DotnetTool => Dotnet.Tool().Result;

    [Once, Description("pack nuget package")]
    public virtual async Task<IEnumerable<string>> Pack()
    {
        await Build();
        var r = await DotnetTool.Run("pack", "--nologo",
            SlnFile,
            "--configuration", Configuration,
            "--no-build",
            "--include-source",
            "--include-symbols",
            "--output", PackagesDir.EnsureDirectoryExists()
            );

        var createdPackages = Match(r.Output.SplitLines(), @"Successfully created package '([^']+)'\.");
        return createdPackages;
    }

    IEnumerable<string> Match(IEnumerable<string> lines, string regexPattern)
    {
        return Match(lines, new Regex(regexPattern));
    }

    IEnumerable<string> Match(IEnumerable<string> lines, Regex regularExpression)
    {
        return lines
            .Select(_ => regularExpression.Match(_))
            .Where(_ => _.Success)
            .Select(_ => _.Groups[1].Value);
    }

    ITool Nuget => Tools.Default.WithFileName("nuget.exe");

    [Once]
    protected virtual async Task Push(string nugetPushSource)
    {
        await Git.EnsureNoPendingChanges();
        await Task.WhenAll(Test(), Pack());
        var nupkgFiles = await Pack();
        var push = Nuget.WithArguments("push", "-NonInteractive");

        if (nugetPushSource != null)
        {
            push = push.WithArguments("-Source", nugetPushSource);
        }

        foreach (var nupkgFile in nupkgFiles)
        {
            await push.Run(nupkgFile);
        }
    }

    [Once, Description("push all nuget packages")]
    protected virtual async Task Push()
    {
        await this.DotnetTool.Run("nuget", "push");
    }

    static string IncreasePatchVersion(string version)
    {
        var i = version.Split('.').Select(_ => Int32.Parse(_)).ToArray();
        ++i[i.Length - 1];
        return i.Select(_ => _.ToString()).Join(".");
    }

    [Once]
    [Description("Build a release version and push to nuget.org")]
    public virtual async Task<string> Release()
    {
        await Git.EnsureNoPendingChanges();
        var version = await GetVersion();
        Logger.Information("Tagging with {version}", version);
        var gitTool = Git.GitTool;
        try
        {
            await gitTool.Run("tag", version);
        }
        catch (ToolException te)
        {
            if (te.Result.Error.Contains("already exists"))
            {
                await gitTool.Run("tag", IncreasePatchVersion(version));
            }
        }
        await gitTool.Run("push", "--tags");
        await Push();
        return version;
    }

    [Once]
    protected virtual async Task<string> GetVersion()
    {
        return await Task.FromResult("0.1.0");
    }

    [Once, Description("Deletes all output files")]
    public virtual async Task Clean()
    {
        await this.OutDir.EnsureNotExists();
    }
}
