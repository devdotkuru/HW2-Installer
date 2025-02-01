using Newtonsoft.Json;
using System;
using System.IO.Compression;
using System.Net;
using System.Resources;
using System.Security.Policy;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace MaethrillianInstallerWin
{
    public partial class MaethrillianInstallerWin : Form
    {
        class ReleaseAsset
        {
            public string name;
            public string browser_download_url;
        }
        class Release
        {
            public string tag_name;
            public List<ReleaseAsset> assets;
        }
        public class ModInfo
        {
            public string name { get; set; }
            public string uri { get; set; }

            public override string ToString()
            {
                return name;
            }
        }

        public Dictionary<string, string> mods = new Dictionary<string, string>()
        {
            { "Vanilla HW2",        "" },
            { "The Yappening",      "https://github.com/Blackandfan/TheYappening/releases/latest/download/package.zip" },
            { "Fight Club",         "https://github.com/Blackandfan/HW2-Yoda/releases/latest/download/package.zip" },
            { "Fight Club (Dev)",   "https://github.com/Blackandfan/HW2-Yoda/releases/download/0.1/package.zip" },
            { "Maethrillian",       "https://github.com/eitener/maethrillian/releases/latest/download/maethrillian.zip" },
            { "Flood Onslaught",    "https://github.com/Blackandfan/FloodOnslaught/releases/latest/download/package.zip" },
            { "Project Nuphillion", "https://github.com/CutesyThrower12/Nuphillion/releases/download/vInDev/blacksNuphillion.zip" },
            { "No Limits",          "https://github.com/Blackandfan/FloodOnslaught/releases/download/0.0.1/package.zip" }
        };
        public Dictionary<string, Image> images = new Dictionary<string, Image>()
        {
            { "Vanilla HW2",        Properties.Resources.hw2 },
            { "The Yappening",      Properties.Resources.yap },
            { "Fight Club",         Properties.Resources.color },
            { "Fight Club (Dev)",   Properties.Resources.color },
            { "Maethrillian",       Properties.Resources.maethrillian },
            { "Flood Onslaught",    Properties.Resources.flood },
            { "Project Nuphillion", Properties.Resources.nuphillion },
            { "No Limits",          Properties.Resources.nolimit }
        };

        const string VersionVanilla = "1_11_2931_2";
        const string VersionPTR = "1_11_2931_10";

        public MaethrillianInstallerWin()
        {
            InitializeComponent();
            imageBox.SizeMode = PictureBoxSizeMode.StretchImage;
            status.Text = "Ready.";


            string defaultMod = "Vanilla HW2";
            string selectedMod = defaultMod;

            int offset = 26;
            foreach (var v in mods)
            {
                var cur = new RadioButton() { Text = v.Key };
                if (v.Key == defaultMod)
                {
                    cur.Checked = true;
                    imageBox.Image = Properties.Resources.hw2;
                }
                cur.Location = new Point(6, offset);
                cur.Size = new Size(150, 24);
                cur.AutoSize = true;
                cur.CheckedChanged += (s, e) =>
                {
                    imageBox.Image = images[v.Key];
                    selectedMod = v.Key;
                };
                offset += 26;
                group.Controls.Add(cur);
            }
            group.Height = offset + 6;

            installButton.Click += (s, e) =>
            {
                try
                {
                    var version = buttonPTR.Checked ? VersionPTR : VersionVanilla;
                    var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    var localStateDir = Path.Combine(appData, "Packages\\Microsoft.HoganThreshold_8wekyb3d8bbwe\\LocalState");
                    var localPkgDir = Path.Combine(localStateDir, String.Format("GTS\\{0}_active", version));
                    var localPkgPath = Path.Combine(localPkgDir, "maethrillian.pkg");
                    var localManifestPath = Path.Combine(localPkgDir, String.Format("{0}_file_manifest.xml", version));

                    if (Directory.Exists(localPkgDir))
                    {
                        if (File.Exists(localPkgPath))
                        {
                            File.Delete(localPkgPath);
                            status.Text = "Removing local package...";
                        }
                        if (File.Exists(localManifestPath))
                        {
                            File.Delete(localManifestPath);
                            status.Text = "Removing local manifest...";
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(localPkgDir);
                    }

                    if (selectedMod == defaultMod)
                    {
                        status.Text = "Reverted to vanilla.";
                        return;
                    }


                    status.Text = "Installing " + selectedMod + "...";

                    // Get the patch file
                    string patchFileName;
                    using (var client = new WebClient())
                    {
                        string patchURI = mods[selectedMod];

                        client.Headers.Add("user-agent", "Halo Wars 2 Mod Installer");

                        // Download patch file
                        patchFileName = Path.GetTempFileName();
                        client.DownloadFile(patchURI, patchFileName);
                    }

                    // Extract patch
                    using (ZipArchive archive = ZipFile.OpenRead(patchFileName))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            string extension = Path.GetExtension(entry.Name);
                            switch (extension)
                            {
                                case ".xml":
                                    entry.ExtractToFile(localManifestPath);
                                    break;
                                case ".pkg":
                                    entry.ExtractToFile(localPkgPath);
                                    break;
                                default: break;
                            }
                        }
                    }

                    status.Text = selectedMod + " installed successfully.";
                    return;
                }
                catch (Exception exception)
                {
                    status.Text = "Error! Failed to install mod.";
                    File.WriteAllText("error.log", exception.Message);
                }
            };
        }

    }
}
