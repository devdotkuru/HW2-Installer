using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Collections.Generic;
using System.IO.Compression;
using Newtonsoft.Json;


namespace MaethrillianInstaller
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

    class Program
    {
        static void Write(string text = "")
        {
            Console.Write("  " + text);
        }

        static void WriteLine(string text = "")
        {
            Console.WriteLine("  " + text);
        }

        static void Return(int exitCode)
        {
            Console.WriteLine();
            Write("Press any key to exit... ");
            Console.ReadKey();
            Environment.Exit(exitCode);
        }

        static void Main()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            try
            {
                const string VersionVanilla = "1_11_2931_2";
                const string VersionPTR = "1_11_2931_10";
                const string Mod1URI = "https://github.com/Blackandfan/TheYappening/releases/latest/download/package.zip";
                const string Mod2URI = "https://github.com/Blackandfan/HW2-Yoda/releases/latest/download/package.zip";
                const string Mod3URI = "https://github.com/Blackandfan/FloodOnslaught/releases/latest/download/package.zip";

                var version = VersionVanilla;
                var release = new Uri(Mod3URI);
                Uri patchURI = null;
                bool isInstall = false;

                // Ask the user
                while (true) {
                    WriteLine();
                    WriteLine("Install one of the following mods using the following letter");
                    WriteLine("(Y)The Yappening, (C)Color Mod, (F)Flood Onslaught");
                    WriteLine();
                    WriteLine();
                    WriteLine("Special commands");
                    WriteLine("(U)ninstall, (B)uild, (P)TR");
                    Write("Enter key: ");
                    char key = Console.ReadKey().KeyChar;
                    WriteLine();
                    if (key == 'I' || key == 'i')
                    {
                        isInstall = true;
                        break;
                    }
                    else if (key == 'U' || key == 'u')
                    {
                        isInstall = false;
                        break;
                    } else if (key == 'P' || key == 'p')
                    {
                        WriteLine("Switch to PTR mode");
                        version = VersionPTR;
                        continue;
                    } else if (key == 'Y' || key == 'y')
                    {
                        isInstall = true;
                        patchURI = new Uri(Mod1URI);
                        break;
                    }
                    else if (key == 'C' || key == 'c')
                    {
                        isInstall = true;
                        patchURI = new Uri(Mod2URI);
                        break;
                    }
                    else if (key == 'F' || key == 'f')
                    {
                        isInstall = true;
                        patchURI = new Uri(Mod3URI);
                        break;
                    }
                    else if (key == 'B' || key == 'b')
                    {
                        Write("Enter build URL: ");
                        var artifactURL = Console.ReadLine();
                        patchURI = new Uri(artifactURL);
                        continue;
                    }
                    else
                    {
                        WriteLine("Invalid input");
                        Return(-1);
                    }
                }

                var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var localStateDir = Path.Combine(appData, "Packages\\Microsoft.HoganThreshold_8wekyb3d8bbwe\\LocalState");
                var localPkgDir = Path.Combine(localStateDir, String.Format("GTS\\{0}_active", version));
                var localPkgPath = Path.Combine(localPkgDir, "maethrillian.pkg");
                var localManifestPath = Path.Combine(localPkgDir, String.Format("{0}_file_manifest.xml", version));

                // Reset the local state
                WriteLine();
                WriteLine("[ RUN  ] Reset local state");
                if (Directory.Exists(localPkgDir))
                {
                    if (File.Exists(localPkgPath))
                    {
                        File.Delete(localPkgPath);
                    }
                    if (File.Exists(localManifestPath))
                    {
                        File.Delete(localManifestPath);
                    }  
                }
                else
                {
                    Directory.CreateDirectory(localPkgDir);
                }
                WriteLine("[ OK   ] Reset local state");

                // Return if not install
                if (!isInstall)
                {
                    Return(0);
                }

                // Get the patch file
                string patchFileName;
                using (var client = new WebClient())
                {
                    client.Headers.Add("user-agent", "Halo Wars 2 Mod Installer");

                    // Get the latest release if the user didnt provide us with a build artifact
                    if (patchURI == null)
                    {
                        WriteLine("[ RUN  ] Get release metadata");
                        var releaseResponse = client.DownloadString(release);
                        var releaseObject = JsonConvert.DeserializeObject<Release>(releaseResponse);
                        for (int i = 0; i < releaseObject.assets.Count; ++i)
                        {
                            if (releaseObject.assets[i].name == "package.zip")
                            {
                                patchURI = new Uri(releaseObject.assets[i].browser_download_url);
                                break;
                            }
                        }
                        if (patchURI == null)
                        {
                            Return(-1);
                        }
                        WriteLine("[ OK   ] Get release metadata");
                    }
                    

                    // Download patch file
                    WriteLine("[ RUN  ] Download patch file");
                    patchFileName = Path.GetTempFileName();
                    client.DownloadFile(patchURI, patchFileName);
                    WriteLine("[ OK   ] Download patch file");
                }

                // Extract patch
                WriteLine("[ RUN  ] Extract patch");
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
                WriteLine("[ OK   ] Extract patch");

            }
            catch (System.Net.WebException)
            {
                WriteLine();
                WriteLine("Failed to load release");
            }
            catch (Exception e)
            {
                WriteLine();
                WriteLine(e.ToString());
            }

            Return(0);
        }
    }
}