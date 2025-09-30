using System;
using System.Collections.Generic;

namespace MaethrillianInstaller.Configuration
{
    public sealed class InstallerConfiguration
    {
        public InstallerConfiguration(string versionVanilla, string versionPtr, string localStatePackagePath, IReadOnlyList<ModDefinition> mods)
        {
            VersionVanilla = versionVanilla ?? throw new ArgumentNullException(nameof(versionVanilla));
            VersionPtr = versionPtr ?? throw new ArgumentNullException(nameof(versionPtr));
            LocalStatePackagePath = localStatePackagePath ?? throw new ArgumentNullException(nameof(localStatePackagePath));
            Mods = mods ?? throw new ArgumentNullException(nameof(mods));
        }

        public string VersionVanilla { get; }

        public string VersionPtr { get; }

        public string LocalStatePackagePath { get; }

        public IReadOnlyList<ModDefinition> Mods { get; }
    }

    public sealed class ModDefinition
    {
        public ModDefinition(string name, string? packageUrl, string? imageUrl, string? info)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Mod name must be provided.", nameof(name));
            }

            Name = name;
            PackageUrl = string.IsNullOrWhiteSpace(packageUrl) ? null : packageUrl;
            ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl;
            Info = string.IsNullOrWhiteSpace(info) ? null : info;
        }

        public string Name { get; }

        public string? PackageUrl { get; }

        public string? ImageUrl { get; }

        public string? Info { get; }

        public bool IsVanilla => string.IsNullOrEmpty(PackageUrl);
    }
}
