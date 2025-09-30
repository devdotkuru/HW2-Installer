using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.NetworkInformation;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaethrillianInstaller;
using MaethrillianInstaller.Configuration;
using MaethrillianInstaller.Desktop.Logging;

#nullable enable

namespace MaethrillianInstaller.GUI
{
    public partial class MainForm : Form
    {
        private readonly InstallerIndexProvider indexProvider = new InstallerIndexProvider(fallbackPath: Path.Combine(AppContext.BaseDirectory, "index.xml"));
        private readonly Logger logger = new Logger();
        private readonly ConcurrentDictionary<string, Image> imageCache = new ConcurrentDictionary<string, Image>(StringComparer.OrdinalIgnoreCase);
        private static readonly HttpClient ImageClient = CreateImageClient();
        private InstallerService? installer;
        private IReadOnlyList<ModDefinition> mods = Array.Empty<ModDefinition>();
        private ModDefinition? selectedMod;
        private ModDefinition? vanillaMod;
        private bool isNetworkAvailable;
        private bool hasShownConnectivityWarning;

        private const int ResetStartedProgress = 5;
        private const int ResetCompletedProgress = 15;
        private const int DownloadStartProgress = 20;
        private const int DownloadCompletedProgress = 85;
        private const int ExtractCompletedProgress = 95;

        public MainForm()
        {
            InitializeComponent();
            installButton.Enabled = false;
            connectivityTimer.Tick += ConnectivityTimerOnTick;
            isNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
            modInfoTextBox.Text = "Loading mod details...";

            if (isNetworkAvailable)
            {
                SetStatus("Ready.");
            }
            else
            {
                NotifyConnectivityLost(initial: true);
            }

            connectivityTimer.Start();
            logger.Log(LogLevel.Information, "Main window initialized.");

            linkDiscord.Links.Clear();
            linkDiscord.Links.Add(0, linkDiscord.Text.Length, "https://discord.gg/CWbugEvu9N");
            linkDiscord.LinkClicked += LinkDiscordOnLinkClicked;
            installButton.Click += InstallButtonOnClick;
            Shown += async (_, _) => await InitializeIndexAsync();
            imageBox.Image = Properties.Resources.hw2;
        }

        private async Task InitializeIndexAsync()
        {
            try
            {
                var configuration = await indexProvider.LoadAsync().ConfigureAwait(true);
                installer = new InstallerService(configuration);
                mods = configuration.Mods;
                vanillaMod = mods.FirstOrDefault(mod => mod.IsVanilla);
                selectedMod = vanillaMod ?? mods.First();
                PopulateModList();
                await ShowModImageAsync(selectedMod).ConfigureAwait(true);
                UpdateModDetails(selectedMod);
                installButton.Enabled = true;
                logger.Log(LogLevel.Information, "Installer index loaded successfully.");
            }
            catch (Exception exception)
            {
                logger.Log(LogLevel.Error, "Failed to load installer index.", exception);
                installButton.Enabled = false;
                SetStatus("Failed to load installer data.", StatusState.Error);
                modInfoGroup.Text = "Mod Details";
                modInfoTextBox.Text = "Unable to load mod descriptions.";
                MessageBox.Show(this, "The installer could not load the latest mod list. Please try again later.", "Installer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateModList()
        {
            modListPanel.SuspendLayout();
            modListPanel.Controls.Clear();

            foreach (var mod in mods)
            {
                var radio = new RadioButton
                {
                    Text = mod.Name,
                    AutoSize = true,
                    Margin = new Padding(3),
                    Tag = mod
                };

                if (ReferenceEquals(mod, selectedMod))
                {
                    radio.Checked = true;
                }

                radio.CheckedChanged += async (_, _) =>
                {
                    if (!radio.Checked)
                    {
                        return;
                    }

                    selectedMod = (ModDefinition)radio.Tag;
                    UpdateModDetails(selectedMod);
                    await ShowModImageAsync(selectedMod).ConfigureAwait(true);
                    logger.Log(LogLevel.Information, $"Selected mod '{selectedMod.Name}'.");
                };

                modListPanel.Controls.Add(radio);
            }

            modListPanel.ResumeLayout();
        }

        private async void InstallButtonOnClick(object? sender, EventArgs e)
        {
            if (selectedMod is null || installer is null)
            {
                SetStatus("Installer data is not ready.", StatusState.Warning);
                return;
            }

            var modName = selectedMod.Name;
            logger.Log(LogLevel.Information, $"Install requested for '{modName}' (PTR: {buttonPTR.Checked}).");

            if (!isNetworkAvailable)
            {
                NotifyConnectivityLost(forceDialog: true);
                return;
            }

            installButton.Enabled = false;
            ShowProgress();
            UpdateProgress(0);

            var installingVanilla = selectedMod.IsVanilla;
            SetStatus($"Preparing installation for {modName}...", StatusState.Information);

            var installationFailed = false;

            try
            {
                var version = buttonPTR.Checked ? installer.PtrVersion : installer.VanillaVersion;
                var context = installer.CreateContext(version);
                logger.Log(LogLevel.Debug, $"Created installation context for version {version}.");

                var progressReporter = new Progress<InstallerProgress>(progress => HandleInstallerProgress(progress, modName));
                Uri? patchUri = null;
                if (!installingVanilla)
                {
                    if (!TryGetModUri(selectedMod, out patchUri))
                    {
                        installationFailed = true;
                        SetStatus($"The download link for {modName} is invalid.", StatusState.Error);
                        logger.Log(LogLevel.Error, $"Mod '{modName}' has an invalid download URI: '{selectedMod.PackageUrl}'.");
                        HideProgress();
                        return;
                    }

                    logger.Log(LogLevel.Information, $"Installing mod '{modName}'.");
                    logger.Log(LogLevel.Debug, $"Resolved patch URI: {patchUri}");
                }

                await Task.Run(() =>
                {
                    installer.ResetLocalState(context, progressReporter);

                    if (patchUri != null)
                    {
                        installer.InstallPatch(context, patchUri, progressReporter);
                    }
                }).ConfigureAwait(true);

                if (installingVanilla)
                {
                    UpdateProgress(100);
                    var vanillaName = vanillaMod?.Name ?? "vanilla";
                    SetStatus($"Reverted to {vanillaName}.", StatusState.Success);
                    logger.Log(LogLevel.Information, "Reverted to vanilla game files.");
                }
            }
            catch (Exception exception)
            {
                installationFailed = true;
                HandleInstallationError(exception);
            }
            finally
            {
                if (!installationFailed)
                {
                    HideProgress();
                }

                installButton.Enabled = true;
            }
        }

        private void HandleInstallerProgress(InstallerProgress progress, string modName)
        {
            switch (progress.Stage)
            {
                case InstallerStage.ResetStarted:
                    SetStatus("Removing existing files...", StatusState.Information, logMessage: false);
                    UpdateProgress(ResetStartedProgress);
                    logger.Log(LogLevel.Debug, "Reset local state started.");
                    break;
                case InstallerStage.ResetCompleted:
                    SetStatus("Local files cleared.", StatusState.Success, logMessage: false);
                    UpdateProgress(ResetCompletedProgress);
                    logger.Log(LogLevel.Debug, "Reset local state completed.");
                    break;
                case InstallerStage.DownloadStarted:
                    SetStatus($"Downloading {modName}...", StatusState.Information, logMessage: false);
                    UpdateProgress(DownloadStartProgress);
                    logger.Log(LogLevel.Information, $"Downloading mod '{modName}'.");
                    break;
                case InstallerStage.DownloadProgress:
                    UpdateProgress(MapProgress(progress.Percentage, DownloadStartProgress, DownloadCompletedProgress));
                    SetStatus($"Downloading {modName}... {ClampPercentage(progress.Percentage)}%", StatusState.Information, logMessage: false);
                    break;
                case InstallerStage.DownloadCompleted:
                    SetStatus($"Download complete for {modName}.", StatusState.Success);
                    UpdateProgress(DownloadCompletedProgress);
                    break;
                case InstallerStage.ExtractStarted:
                    SetStatus($"Installing {modName} files...", StatusState.Information, logMessage: false);
                    UpdateProgress(DownloadCompletedProgress);
                    logger.Log(LogLevel.Debug, $"Extracting files for '{modName}'.");
                    break;
                case InstallerStage.ExtractProgress:
                    UpdateProgress(MapProgress(progress.Percentage, DownloadCompletedProgress, ExtractCompletedProgress));
                    SetStatus($"Installing {modName} files... {ClampPercentage(progress.Percentage)}%", StatusState.Information, logMessage: false);
                    break;
                case InstallerStage.ExtractCompleted:
                    SetStatus($"Finalizing {modName}...", StatusState.Information);
                    UpdateProgress(ExtractCompletedProgress);
                    break;
                case InstallerStage.InstallCompleted:
                    SetStatus($"Success. Installed {modName}.", StatusState.Success);
                    UpdateProgress(100);
                    break;
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

        private async Task ShowModImageAsync(ModDefinition? mod)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => imageBox.Image = Properties.Resources.hw2));
            }
            else
            {
                imageBox.Image = Properties.Resources.hw2;
            }

            if (mod?.ImageUrl is null)
            {
                return;
            }

            if (!Uri.TryCreate(mod.ImageUrl, UriKind.Absolute, out var imageUri))
            {
                logger.Log(LogLevel.Information, $"Invalid image URL for {mod.Name}: {mod.ImageUrl}");
                return;
            }

            if (imageCache.TryGetValue(mod.ImageUrl, out var cachedImage))
            {
                UpdateImage(cachedImage);
                return;
            }

            if (!isNetworkAvailable)
            {
                logger.Log(LogLevel.Information, "Network unavailable. Using fallback image.");
                return;
            }

            try
            {
                using var response = await ImageClient.GetAsync(imageUri).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                using var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                using var memory = new MemoryStream();
                await responseStream.CopyToAsync(memory).ConfigureAwait(false);
                var buffer = memory.ToArray();
                using var imageStream = new MemoryStream(buffer);
                using var loaded = Image.FromStream(imageStream);
                var cached = (Image)loaded.Clone();
                imageCache[mod.ImageUrl] = cached;
                UpdateImage(cached);
                logger.Log(LogLevel.Debug, $"Loaded remote image for {mod.Name}.");
            }
            catch (Exception exception)
            {
                logger.Log(LogLevel.Error, $"Failed to load image for {mod.Name} from {mod.ImageUrl}.", exception);
            }
        }

        private void UpdateImage(Image image)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => imageBox.Image = image));
            }
            else
            {
                imageBox.Image = image;
            }
        }

        private static HttpClient CreateImageClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Halo Wars 2 Mod Installer");
            return client;
        }

        private static int MapProgress(int percent, int start, int end)
        {
            var clamped = ClampPercentage(percent);
            var range = Math.Max(end - start, 0);
            var mapped = start + (int)Math.Round(range * (clamped / 100.0));
            if (mapped < start)
            {
                mapped = start;
            }
            else if (mapped > end)
            {
                mapped = end;
            }

            return mapped;
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

        private void LinkDiscordOnLinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            var inviteUrl = e.Link.LinkData as string ?? "https://discord.gg/CWbugEvu9N";
            try
            {
                Process.Start(new ProcessStartInfo(inviteUrl) { UseShellExecute = true });
                logger.Log(LogLevel.Information, "Opened Discord invite link.");
            }
            catch (Exception exception)
            {
                logger.Log(LogLevel.Error, "Failed to open Discord invite link.", exception);
                MessageBox.Show("Unable to open the Discord link. Please check your default browser settings.", "Link Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void HandleInstallationError(Exception exception)
        {
            HideProgress();
            logger.Log(LogLevel.Error, "Failed to install mod.", exception);
            SetStatus("Error! Failed to install mod.", StatusState.Error, logMessage: false);

            var dialogResult = MessageBox.Show(
                "An error occurred while installing the mod. Would you like to generate a diagnostic log?",
                "Installation Error",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Error);

            if (dialogResult != DialogResult.Yes)
            {
                return;
            }

            try
            {
                var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");
                logger.WriteToFile(logPath);
                logger.Log(LogLevel.Information, $"Diagnostic log written to {logPath}.");
                MessageBox.Show($"Log saved to {logPath}", "Log Generated", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception loggingException)
            {
                logger.Log(LogLevel.Error, "Failed to write diagnostic log.", loggingException);
                MessageBox.Show("Failed to write the diagnostic log.", "Logging Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetStatus(string message, StatusState state = StatusState.Information, bool logMessage = true)
        {
            statusLabel.Text = message;
            statusLabel.ForeColor = state switch
            {
                StatusState.Success => Color.FromArgb(34, 139, 34),
                StatusState.Warning => Color.FromArgb(184, 134, 11),
                StatusState.Error => Color.FromArgb(178, 34, 34),
                _ => SystemColors.ControlText
            };

            if (!logMessage)
            {
                return;
            }

            var level = state switch
            {
                StatusState.Error => LogLevel.Error,
                StatusState.Warning => LogLevel.Information,
                StatusState.Success => LogLevel.Information,
                _ => LogLevel.Information
            };

            logger.Log(level, message);
        }

        private void ConnectivityTimerOnTick(object? sender, EventArgs e)
        {
            EvaluateConnectivity();
        }

        private void EvaluateConnectivity()
        {
            var currentlyAvailable = NetworkInterface.GetIsNetworkAvailable();
            if (currentlyAvailable == isNetworkAvailable)
            {
                return;
            }

            isNetworkAvailable = currentlyAvailable;

            if (!currentlyAvailable)
            {
                NotifyConnectivityLost();
            }
            else
            {
                hasShownConnectivityWarning = false;
                SetStatus("Internet connection restored.", StatusState.Success);
            }
        }

        private void NotifyConnectivityLost(bool initial = false, bool forceDialog = false)
        {
            SetStatus("No internet connection detected. Please connect to download mods.", StatusState.Warning);

            if (forceDialog || !hasShownConnectivityWarning || initial)
            {
                hasShownConnectivityWarning = true;
                MessageBox.Show(
                    "An internet connection is required to download mods. Please reconnect and try again.",
                    "No Internet Connection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void ShowProgress()
        {
            installProgressBar.Visible = true;
            installProgressBar.Style = ProgressBarStyle.Blocks;
            installProgressBar.MarqueeAnimationSpeed = 0;
            installProgressBar.Value = installProgressBar.Minimum;
        }

        private void UpdateProgress(int percent)
        {
            if (!installProgressBar.Visible)
            {
                ShowProgress();
            }

            installProgressBar.Style = ProgressBarStyle.Blocks;
            installProgressBar.MarqueeAnimationSpeed = 0;

            if (percent < installProgressBar.Minimum)
            {
                percent = installProgressBar.Minimum;
            }
            else if (percent > installProgressBar.Maximum)
            {
                percent = installProgressBar.Maximum;
            }

            installProgressBar.Value = percent;
        }

        private void HideProgress()
        {
            installProgressBar.MarqueeAnimationSpeed = 0;
            installProgressBar.Style = ProgressBarStyle.Blocks;
            installProgressBar.Value = installProgressBar.Minimum;
            installProgressBar.Visible = false;
        }

        private void UpdateModDetails(ModDefinition? mod)
        {
            if (mod == null)
            {
                modInfoGroup.Text = "Mod Details";
                modInfoTextBox.Text = "Select a mod to view its description.";
                return;
            }

            modInfoGroup.Text = $"Mod Details - {mod.Name}";

            var infoLines = new List<string>();

            if (!string.IsNullOrWhiteSpace(mod.Info))
            {
                infoLines.Add(mod.Info!.Trim());
            }
            else if (mod.IsVanilla)
            {
                infoLines.Add("Restore the original Halo Wars 2 files without modifications.");
            }
            else
            {
                infoLines.Add("No description available for this mod.");
            }

            modInfoTextBox.Text = string.Join(Environment.NewLine, infoLines);
        }

        private enum StatusState
        {
            Information,
            Success,
            Warning,
            Error
        }

    }
}
