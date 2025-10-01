using System;
using System.ComponentModel;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using MaethrillianInstaller.Configuration;

namespace MaethrillianInstaller
{
    public class InstallationContext
    {
        public InstallationContext(string version, string localStateDirectory)
        {
            Version = version;
            LocalStateDirectory = localStateDirectory;
            LocalPackageDirectory = Path.Combine(localStateDirectory, $"GTS\\{version}_active");
            LocalManifestPath = Path.Combine(LocalPackageDirectory, $"{version}_file_manifest.xml");
        }

        public string Version { get; }
        public string LocalStateDirectory { get; }
        public string LocalPackageDirectory { get; }
        public string LocalManifestPath { get; }

        public string? LocalPackageFileName { get; private set; }

        public string? LocalPackagePath => LocalPackageFileName == null
            ? null
            : Path.Combine(LocalPackageDirectory, LocalPackageFileName);

        public void UpdateLocalPackageFileName(string? fileName)
        {
            LocalPackageFileName = fileName;
        }
    }

    public enum InstallerStage
    {
        ResetStarted,
        ResetCompleted,
        DownloadStarted,
        DownloadProgress,
        DownloadCompleted,
        ExtractStarted,
        ExtractProgress,
        ExtractCompleted,
        InstallCompleted
    }

    public readonly struct InstallerProgress
    {
        public InstallerProgress(InstallerStage stage, int percentage = 0, string? detail = null, long bytesReceived = 0, long totalBytes = 0)
        {
            Stage = stage;
            Percentage = percentage;
            Detail = detail;
            BytesReceived = bytesReceived;
            TotalBytes = totalBytes;
        }

        public InstallerStage Stage { get; }

        public int Percentage { get; }

        public string? Detail { get; }

        public long BytesReceived { get; }

        public long TotalBytes { get; }

        public static InstallerProgress FromStage(InstallerStage stage, int percentage = 0, string? detail = null, long bytesReceived = 0, long totalBytes = 0)
        {
            return new InstallerProgress(stage, percentage, detail, bytesReceived, totalBytes);
        }
    }

    public class InstallerService
    {
        private readonly InstallerConfiguration configuration;

        public InstallerService(InstallerConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string VanillaVersion => configuration.VersionVanilla;

        public string PtrVersion => configuration.VersionPtr;

        public InstallationContext CreateContext(string version)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var localState = Path.Combine(appData, configuration.LocalStatePackagePath);
            return new InstallationContext(version, localState);
        }

        public void ResetLocalState(InstallationContext context, IProgress<InstallerProgress>? progress = null)
        {
            progress?.Report(InstallerProgress.FromStage(InstallerStage.ResetStarted));
            if (Directory.Exists(context.LocalPackageDirectory))
            {
                foreach (var pkgFile in Directory.GetFiles(context.LocalPackageDirectory, "*.pkg", SearchOption.TopDirectoryOnly))
                {
                    File.Delete(pkgFile);
                }
                if (File.Exists(context.LocalManifestPath))
                {
                    File.Delete(context.LocalManifestPath);
                }
            }
            else
            {
                Directory.CreateDirectory(context.LocalPackageDirectory);
            }
            context.UpdateLocalPackageFileName(null);
            progress?.Report(InstallerProgress.FromStage(InstallerStage.ResetCompleted, 100));
        }

        public void InstallPatch(InstallationContext context, Uri patchUri, IProgress<InstallerProgress>? progress = null)
        {
            if (patchUri == null)
            {
                throw new ArgumentNullException(nameof(patchUri));
            }

            string? patchFileName = null;
            try
            {
                progress?.Report(InstallerProgress.FromStage(InstallerStage.DownloadStarted));
                patchFileName = DownloadPatch(patchUri, progress);
                progress?.Report(InstallerProgress.FromStage(InstallerStage.DownloadCompleted, 100));
                progress?.Report(InstallerProgress.FromStage(InstallerStage.ExtractStarted));
                var packageFileName = ExtractPatch(patchFileName, context.LocalManifestPath, context.LocalPackageDirectory, progress);
                context.UpdateLocalPackageFileName(packageFileName);
                progress?.Report(InstallerProgress.FromStage(InstallerStage.ExtractCompleted, 100));
                progress?.Report(InstallerProgress.FromStage(InstallerStage.InstallCompleted, 100));
            }
            finally
            {
                if (patchFileName != null && File.Exists(patchFileName))
                {
                    File.Delete(patchFileName);
                }
            }
        }

        public Uri ResolveReleaseAsset(Uri releaseUri, string assetName = "package.zip")
        {
            if (releaseUri == null)
            {
                throw new ArgumentNullException(nameof(releaseUri));
            }

            using var client = new WebClient();
            client.Headers.Add("user-agent", "Halo Wars 2 Mod Installer");

            var releaseResponse = client.DownloadString(releaseUri);
            var serializer = new DataContractJsonSerializer(typeof(Release));
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(releaseResponse));
            var release = (Release?)serializer.ReadObject(ms) ?? throw new SerializationException("Failed to deserialize release information.");

            foreach (var asset in release.assets)
            {
                if (asset != null && StringComparer.OrdinalIgnoreCase.Equals(asset.name, assetName))
                {
                    if (Uri.TryCreate(asset.browser_download_url, UriKind.Absolute, out var assetUri))
                    {
                        return assetUri;
                    }
                }
            }

            throw new InvalidOperationException($"Asset '{assetName}' was not found in the release metadata.");
        }

        private static string DownloadPatch(Uri patchUri, IProgress<InstallerProgress>? progress)
        {
            using var client = new WebClient();
            client.Headers.Add("user-agent", "Halo Wars 2 Mod Installer");

            var patchFileName = Path.GetTempFileName();
            var completionSource = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);

            DownloadProgressChangedEventHandler? progressHandler = null;
            AsyncCompletedEventHandler? completedHandler = null;

            progressHandler = (_, args) =>
            {
                progress?.Report(InstallerProgress.FromStage(
                    InstallerStage.DownloadProgress,
                    args.ProgressPercentage,
                    detail: null,
                    bytesReceived: args.BytesReceived,
                    totalBytes: args.TotalBytesToReceive));
            };

            completedHandler = (_, args) =>
            {
                client.DownloadProgressChanged -= progressHandler;
                client.DownloadFileCompleted -= completedHandler;

                if (args.Cancelled)
                {
                    completionSource.TrySetCanceled();
                    return;
                }

                if (args.Error != null)
                {
                    completionSource.TrySetException(args.Error);
                    return;
                }

                completionSource.TrySetResult(null);
            };

            client.DownloadProgressChanged += progressHandler;
            client.DownloadFileCompleted += completedHandler;

            client.DownloadFileAsync(patchUri, patchFileName);
            completionSource.Task.GetAwaiter().GetResult();

            return patchFileName;
        }

        private static string? ExtractPatch(string patchFileName, string manifestPath, string packageDirectory, IProgress<InstallerProgress>? progress)
        {
            using var archive = ZipFile.OpenRead(patchFileName);
            Directory.CreateDirectory(packageDirectory);
            var entries = archive.Entries
                .Where(entry =>
                {
                    var extension = Path.GetExtension(entry.Name);
                    return extension is ".xml" or ".pkg";
                })
                .ToList();

            var processed = 0;
            var total = entries.Count;
            string? extractedPackageFileName = null;
            foreach (ZipArchiveEntry entry in entries)
            {
                processed++;
                var extension = Path.GetExtension(entry.Name);
                var percent = total == 0 ? 100 : (int)Math.Round(processed / (double)total * 100);

                switch (extension)
                {
                    case ".xml":
                        entry.ExtractToFile(manifestPath, overwrite: true);
                        break;
                    case ".pkg":
                        var fileName = Path.GetFileName(entry.Name);
                        if (!string.IsNullOrWhiteSpace(fileName))
                        {
                            var destinationPath = Path.Combine(packageDirectory, fileName);
                            entry.ExtractToFile(destinationPath, overwrite: true);
                            extractedPackageFileName = fileName;
                        }
                        break;
                    default:
                        break;
                }

                progress?.Report(InstallerProgress.FromStage(InstallerStage.ExtractProgress, percent, entry.FullName));
            }
            return extractedPackageFileName;
        }

        [DataContract]
        private class Release
        {
            [DataMember]
            public string? tag_name { get; set; }

            [DataMember]
            public List<ReleaseAsset?> assets { get; set; } = new List<ReleaseAsset?>();
        }

        [DataContract]
        private class ReleaseAsset
        {
            [DataMember]
            public string? name { get; set; }

            [DataMember]
            public string? browser_download_url { get; set; }
        }
    }
}
