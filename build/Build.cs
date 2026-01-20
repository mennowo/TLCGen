using System;
using System.IO;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.SignTool;
using Nuke.Common.Utilities.Collections;
using Rebex.Net;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using File = System.IO.File;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Deploy);
    
    [Solution] readonly Solution Solution;

    private static AbsolutePath SourceDirectory => RootDirectory;
    private static AbsolutePath OutputDirectory => RootDirectory / "output";

    const string MsBuildPath = @"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe";
    
    const bool Dev = false;

    const bool DoClean = true;
    const bool DoSign = true;
    const bool DoDeploy = false;
    const bool DoArchiveOld = false;
    const string ArchiveOldVersion = "12.4.0.17";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            OutputDirectory.CreateOrCleanDirectory();

            if (DoClean) 
            { 
                SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(x =>
                {
                    Directory.EnumerateFiles(x).ForEach(File.Delete);
                });
            }
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(_ => _.SetProjectFile(Solution.GetProject("TLCGen")));
        });

    Target CompileAndSign => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            MSBuildTasks.MSBuild(_ => _
                .SetProcessToolPath(MsBuildPath)
                .SetProcessEnvironmentVariable("DevEnvDir", @"C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\")
                .SetProjectFile(Solution.GetProject("TLCGen"))
                .SetConfiguration("Release")
                .SetTargetPlatform(MSBuildTargetPlatform.x64)
                .SetMSBuildPlatform(MSBuildPlatform.x64));

            var project = Solution.GetProject("TLCGen");
            if (DoSign && project != null)
            {
                // get publish folder
                var publishDirectory = project.Directory / "bin" / "x64" / "Release" / "net8.0-windows";

                SignToolTasks.SignTool(_ => _
                    .SetFile("C:\\Users\\menno\\CodingConnected\\Various\\CodeCert\\cert-cc-2023-2026.cer")
                    .SetDescription("TLCGen")
                    .SetFileDigestAlgorithm(SignToolDigestAlgorithm.SHA256)
                    .SetFiles(publishDirectory / "TLCGen.exe"));
            }
        });

    Target CompileSetup => _ => _
        .DependsOn(CompileAndSign)
        .Executes(() =>
        {
            MSBuildTasks.MSBuild(_ => _
                .SetProcessToolPath(MsBuildPath)
                .SetProjectFile(Solution.GetProject("TLCGen.Setup"))
                .SetConfiguration("Release")
                .SetTargetPlatform(MSBuildTargetPlatform.x64)
                .SetMSBuildPlatform(MSBuildPlatform.x64));

            var setupProject = Solution.GetProject("TLCGen.Setup");
            var project = Solution.GetProject("TLCGen");
            if (setupProject == null || project == null) return;
            
            var setupPublishDirectory = setupProject.Directory / "bin" / "x64" / "Release";
            var versionLine = File.ReadAllLines(project.Directory / "tlcgenversioning")[0][7..];
            var version = Version.Parse(versionLine);

            if (DoSign)
            {
                SignToolTasks.SignTool(_ => _
                    .SetFile("C:\\Users\\menno\\CodingConnected\\Various\\CodeCert\\cert-cc-2023-2026.cer")
                    .SetDescription("TLCGen")
                    .SetFileDigestAlgorithm(SignToolDigestAlgorithm.SHA256)
                    .SetFiles(setupPublishDirectory / "TLCGen.Setup.msi"));
            }

            var file = OutputDirectory / $"{DateTime.Now:yyyy-MM-dd}-Setup_TLCGen_V{version.ToString(3).Replace('.', '_')}.msi";
            File.Copy(setupPublishDirectory / "TLCGen.Setup.msi", file);
        });

    Target PackPortable => _ => _
        .DependsOn(CompileSetup)
        .Executes(() =>
        {
            var project = Solution.GetProject("TLCGen");
            if (project != null)
            {
                // get publish folder
                var publishDirectory = project.Directory / "bin" / "x64" / "Release" / "net8.0-windows";

                var packDirectory = OutputDirectory / "PackTLCGen";
                packDirectory.CreateOrCleanDirectory();

                // version
                var versionLine = File.ReadAllLines(project.Directory / "tlcgenversioning")[0][7..];
                var version = Version.Parse(versionLine);

                // output file name
                var outputFileName = $"{DateTime.Now:yyyy-MM-dd}-TLCGen_V{version.ToString(3).Replace('.', '_')}.zip";
                
                // copy TLCGen files
                var files = Directory.EnumerateFiles(publishDirectory, "TLCGen*.dll").Concat<string>(
                [
                    publishDirectory + "/TLCGen.exe",
                    publishDirectory + "/TLCGen.deps.json",
                    publishDirectory + "/TLCGen.dll.config",
                    publishDirectory + "/TLCGen.runtimeconfig.json"
                ]);
                foreach (var f in files)
                {
                    File.Move(f, packDirectory / Path.GetFileName(f));
                }

                // copy all external dependencies
                files =
                [
                    "CodingConnected.Topology.dll",
                    "CommunityToolkit.Mvvm.dll",
                    "DocumentFormat.OpenXml.dll",
                    "DocumentFormat.OpenXml.Framework.dll",
                    "DotNetProjects.Wpf.Extended.Toolkit.dll",
                    "GongSolutions.WPF.DragDrop.dll",
                    "Gu.Wpf.DataGrid2D.dll",
                    "ICSharpCode.AvalonEdit.dll",
                    "MdXaml.dll",
                    "MdXaml.Plugins.dll",
                    "Microsoft.Win32.SystemEvents.dll",
                    "Microsoft.Xaml.Behaviors.dll",
                    "Newtonsoft.Json.dll",
                    "System.Drawing.Common.dll",
                    "System.IO.Packaging.dll",
                    "System.Private.Windows.Core.dll",
                    "WindowsInput.dll",
                ];
                foreach (var f in files)
                {
                    File.Copy(publishDirectory / f, packDirectory / f);
                }

                // other files
                (publishDirectory / "Docs").CopyToDirectory(packDirectory);
                (publishDirectory / "Licenses").CopyToDirectory(packDirectory);
                (publishDirectory / "Plugins").CopyToDirectory(packDirectory);
                (publishDirectory / "runtimes").CopyToDirectory(packDirectory);
                (publishDirectory / "Settings").CopyToDirectory(packDirectory);
                (publishDirectory / "SourceFiles").CopyToDirectory(packDirectory);
                (publishDirectory / "SourceFilesToCopy").CopyToDirectory(packDirectory);
                (publishDirectory / "Updater").CopyToDirectory(packDirectory);

                // compress
                (packDirectory).CompressTo(OutputDirectory / outputFileName);
            }
        });

    Target Deploy => _ => _
        .DependsOn(PackPortable)
        .Executes(() =>
        {
            if (!DoDeploy) return;

            var project = Solution.GetProject("TLCGen");
            if (project == null) return;

            var versionLine = File.ReadAllLines(project.Directory / "tlcgenversioning")[0][7..];
            var version = Version.Parse(versionLine);

            var outputNameSetup = OutputDirectory / $"{DateTime.Now:yyyy-MM-dd}-Setup_TLCGen_V{version.ToString(3).Replace('.', '_')}.msi";
            var outputNamePortable = OutputDirectory / $"{DateTime.Now:yyyy-MM-dd}-TLCGen_V{version.ToString(3).Replace('.', '_')}.zip";

            var remoteFileNameSetup = "TLCGen.Setup.msi";
            var remoteFileNamePortable = "TLCGen_portable_latest.zip";
            var remoteFileNameVersioning = "tlcgenversioning";

            var s = new CodingConnectedLocalSettings();
            using var client = new Sftp();
            client.Connect(s.HostName, s.HostPort);
            client.Login(s.UserName, s.Password);
            client.ChangeDirectory("/var/www/html/codingconnected.eu/tlcgen/deploy/" + (Dev ? "Dev" : "Release"));
            Console.WriteLine("Now connected via sftp...");
            
            // Setup
            Console.WriteLine($"Uploading TLCGen Setup to {client.GetCurrentDirectory() + '/' + remoteFileNameSetup}");
            try
            {
                if (client.FileExists(client.GetCurrentDirectory() + '/' + remoteFileNameSetup))
                {
                    if (DoArchiveOld &&
                        !client.FileExists(client.GetCurrentDirectory() + '/' + ArchiveOldVersion + "_" + remoteFileNameSetup))
                    {
                        Console.WriteLine("Found existing ZIP file, will archive...");
                        client.Rename(client.GetCurrentDirectory() + '/' + remoteFileNameSetup, client.GetCurrentDirectory() + '/' + ArchiveOldVersion + "_" + remoteFileNameSetup);
                    }
                    else
                    {
                        Console.WriteLine("Found existing setup file, will try removing...");
                        client.DeleteFile(client.GetCurrentDirectory() + '/' + remoteFileNameSetup);
                    }
                }
            }
            catch
            {
                // ignored
            }
            client.Upload(outputNameSetup, client.GetCurrentDirectory());
            client.Rename(Path.GetFileName(outputNameSetup), remoteFileNameSetup);

            // Portable
            Console.WriteLine($"Uploading TLCGen Portable to {client.GetCurrentDirectory() + '/' + remoteFileNamePortable}");
            try
            {
                if (client.FileExists(client.GetCurrentDirectory() + '/' + remoteFileNamePortable))
                {
                    if (DoArchiveOld &&
                        !client.FileExists(client.GetCurrentDirectory() + '/' + ArchiveOldVersion + "_" + remoteFileNamePortable))
                    {
                        Console.WriteLine("Found existing ZIP file, will archive...");
                        client.Rename(client.GetCurrentDirectory() + '/' + remoteFileNamePortable, client.GetCurrentDirectory() + '/' + ArchiveOldVersion + "_" + remoteFileNamePortable);
                    }
                    else
                    {
                        Console.WriteLine("Found existing ZIP file, will try removing...");
                        client.DeleteFile(client.GetCurrentDirectory() + '/' + remoteFileNamePortable);
                    }
                }
            }
            catch
            {
                // ignored
            }
            client.Upload(outputNamePortable, client.GetCurrentDirectory());
            client.Rename(Path.GetFileName(outputNamePortable), remoteFileNamePortable);

            // Versioning
            client.ChangeDirectory("/var/www/html/codingconnected.eu/tlcgen/deploy/");
            Console.WriteLine($"Uploading TLCGen VERSIONING file to {client.GetCurrentDirectory() + '/' + remoteFileNameVersioning}");
            try
            {
                if (client.FileExists(client.GetCurrentDirectory() + '/' + remoteFileNameVersioning))
                {
                    Console.WriteLine("Found existing VERSIONING file, will try removing...");
                    client.DeleteFile(client.GetCurrentDirectory() + '/' + remoteFileNameVersioning);
                }
            }
            catch
            {
                // ignored
            }
            client.Upload(project.Directory / remoteFileNameVersioning, client.GetCurrentDirectory());

            client.Disconnect();
        });

}
