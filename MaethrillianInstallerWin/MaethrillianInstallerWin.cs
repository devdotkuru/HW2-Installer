using Newtonsoft.Json;
using System;
using System.IO.Compression;
using System.Net;
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
            { "Vanilla HW2",     "" },
            { "The Yappening",   "https://github.com/Blackandfan/TheYappening/releases/latest/download/package.zip" },
            { "Color Mod",       "https://github.com/Blackandfan/HW2-Yoda/releases/latest/download/package.zip" },
            { "Flood Onslaught", "https://github.com/Blackandfan/FloodOnslaught/releases/latest/download/package.zip" }
        };
        public Dictionary<string, Image> images = new Dictionary<string, Image>()
        {
            { "Vanilla HW2",     Image.FromFile("hw2.jpg") },
            { "The Yappening",   Image.FromFile("yap.jpg") },
            { "Color Mod",       Image.FromFile("color.jpg") },
            { "Flood Onslaught", Image.FromFile("flood.jpg") }
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
            foreach(var v in mods)
            {
                var cur = new RadioButton() { Text = v.Key };
                if (v.Key == defaultMod)
                {
                    cur.Checked = true;
                    imageBox.Image = images[v.Key];
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

                status.Text = selectedMod + " installed sucessfully.";
            };
        }
    }
}
