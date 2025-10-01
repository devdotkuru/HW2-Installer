using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using MaethrillianInstaller;
using MaethrillianInstaller.Configuration;

namespace MaethrillianInstaller.CLI
{
    internal static class Program
    {
        private static readonly InstallerIndexProvider IndexProvider = new InstallerIndexProvider(fallbackPath: Path.Combine(AppContext.BaseDirectory, "index.xml"));

        private static InstallerService? installer;
        private static IReadOnlyList<ModDefinition> mods = Array.Empty<ModDefinition>();

        private static void Main()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            if (!TryInitializeInstaller())
            {
                Return(-1);
                return;
            }

            try
            {
                var usePtr = false;
                Uri? patchUri = null;
                var isInstall = false;
                ModDefinition? selectedMod = null;
                var selectionName = string.Empty;

                while (true)
                {
                    WriteLine();
                    WriteLine("Available mods:");
                    for (var i = 0; i < mods.Count; i++)
                    {
                        var mod = mods[i];
                        var descriptor = mod.IsVanilla ? " (vanilla)" : string.Empty;
                        WriteLine($"({i + 1}) {mod.Name}{descriptor}");
                    }

                    WriteLine();
                    WriteLine("Special commands: U=Uninstall, B=Custom build, P=Toggle PTR");
                    Write($"Current mode: {(usePtr ? "PTR" : "Retail")}\nEnter selection: ");
                    var input = Console.ReadLine()?.Trim();

                    if (string.IsNullOrEmpty(input))
                    {
                        WriteLine("Invalid input");
                        continue;
                    }

                    if (string.Equals(input, "P", StringComparison.OrdinalIgnoreCase))
                    {
                        usePtr = !usePtr;
                        WriteLine($"Switched to {(usePtr ? "PTR" : "Retail")} mode.");
                        continue;
                    }

                    if (string.Equals(input, "U", StringComparison.OrdinalIgnoreCase))
                    {
                        selectedMod = mods.FirstOrDefault(mod => mod.IsVanilla);
                        if (selectedMod == null)
                        {
                            WriteLine("No vanilla entry available.");
                            continue;
                        }

                        isInstall = false;
                        selectionName = selectedMod.Name;
                        break;
                    }

                    if (string.Equals(input, "B", StringComparison.OrdinalIgnoreCase))
                    {
                        Write("Enter build URL: ");
                        var artifactUrl = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(artifactUrl) && Uri.TryCreate(artifactUrl, UriKind.Absolute, out var customUri))
                        {
                            patchUri = customUri;
                            isInstall = true;
                            selectionName = "Custom Build";
                            break;
                        }

                        WriteLine("Invalid build URL");
                        continue;
                    }

                    if (int.TryParse(input, out var selection) && selection >= 1 && selection <= mods.Count)
                    {
                        selectedMod = mods[selection - 1];
                        selectionName = selectedMod.Name;
                        isInstall = !selectedMod.IsVanilla;

                        if (isInstall && !TryGetModUri(selectedMod, out patchUri))
                        {
                            WriteLine("Selected mod does not have a valid download link.");
                            continue;
                        }

                        break;
                    }

                    WriteLine("Invalid input");
                }

                if (selectedMod == null && string.IsNullOrEmpty(selectionName))
                {
                    WriteLine("No selection made.");
                    Return(-1);
                    return;
                }

                var version = usePtr ? installer!.PtrVersion : installer!.VanillaVersion;
                var context = installer!.CreateContext(version);
                installer.ResetLocalState(context, CreateProgressReporter());

                if (!isInstall)
                {
                    WriteLine($"Reverted to {selectionName}.");
                    Return(0);
                    return;
                }

                if (patchUri == null)
                {
                    WriteLine("Missing download location.");
                    Return(-1);
                    return;
                }

                installer.InstallPatch(context, patchUri, CreateProgressReporter());
            }
            catch (Exception e)
            {
                WriteLine();
                WriteLine(e.ToString());
            }

            Return(0);
        }

        private static bool TryInitializeInstaller()
        {
            try
            {
                var configuration = IndexProvider.LoadAsync().GetAwaiter().GetResult();
                installer = new InstallerService(configuration);
                mods = configuration.Mods;
                return mods.Count > 0;
            }
            catch (Exception exception)
            {
                WriteLine();
                WriteLine("Failed to load installer index:");
                WriteLine(exception.Message);
                return false;
            }
        }

        private static bool TryGetModUri(ModDefinition mod, out Uri? uri)
        {
            uri = null;

            if (string.IsNullOrWhiteSpace(mod.PackageUrl))
            {
                return false;
            }

            if (Uri.TryCreate(mod.PackageUrl, UriKind.Absolute, out var parsed))
            {
                uri = parsed;
                return true;
            }

            return false;
        }

        private static Progress<InstallerProgress> CreateProgressReporter()
        {
            var lastDownloadAnnouncement = -10;
            var lastExtractAnnouncement = -10;

            return new Progress<InstallerProgress>(progress =>
            {
                switch (progress.Stage)
                {
                    case InstallerStage.ResetStarted:
                        WriteLine();
                        WriteLine("[ RUN  ] Reset local state");
                        break;
                    case InstallerStage.ResetCompleted:
                        WriteLine("[ OK   ] Reset local state");
                        break;
                    case InstallerStage.DownloadStarted:
                        WriteLine("[ RUN  ] Download patch file");
                        break;
                    case InstallerStage.DownloadProgress:
                        var downloadPercent = ClampPercentage(progress.Percentage);
                        if (downloadPercent >= lastDownloadAnnouncement + 10 || downloadPercent == 100)
                        {
                            lastDownloadAnnouncement = downloadPercent - downloadPercent % 10;
                            WriteLine($"[ .... ] Download {downloadPercent}%");
                        }
                        break;
                    case InstallerStage.DownloadCompleted:
                        WriteLine("[ OK   ] Download patch file");
                        break;
                    case InstallerStage.ExtractStarted:
                        WriteLine("[ RUN  ] Extract patch");
                        break;
                    case InstallerStage.ExtractProgress:
                        var extractPercent = ClampPercentage(progress.Percentage);
                        if (extractPercent >= lastExtractAnnouncement + 25 || extractPercent == 100)
                        {
                            lastExtractAnnouncement = extractPercent;
                            WriteLine($"[ .... ] Extract {extractPercent}%");
                        }
                        break;
                    case InstallerStage.ExtractCompleted:
                        WriteLine("[ OK   ] Extract patch");
                        break;
                    case InstallerStage.InstallCompleted:
                        WriteLine("[ OK   ] Install complete");
                        break;
                }
            });
        }

        private static int ClampPercentage(int percent)
        {
            if (percent < 0)
            {
                return 0;
            }

            if (percent > 100)
            {
                return 100;
            }

            return percent;
        }

        private static void Write(string text = "") => Console.Write("  " + text);

        private static void WriteLine(string text = "") => Console.WriteLine("  " + text);

        private static void Return(int exitCode)
        {
            WriteLine();
            Write("Press any key to exit... ");
            Console.ReadKey();
            Environment.Exit(exitCode);
        }
    }
}
