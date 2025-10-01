using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MaethrillianInstaller.Configuration
{
    public sealed class InstallerIndexProvider
    {
        public static readonly Uri DefaultIndexUri = new Uri("https://raw.githubusercontent.com/Blackandfan/HW2-Installer/main/index.xml");

        private readonly Uri indexUri;
        private readonly string? fallbackPath;

        public InstallerIndexProvider(Uri? indexUri = null, string? fallbackPath = null)
        {
            this.indexUri = indexUri ?? DefaultIndexUri;
            this.fallbackPath = fallbackPath;
        }

        public async Task<InstallerConfiguration> LoadAsync(CancellationToken cancellationToken = default)
        {
            string indexContent;

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Halo Wars 2 Mod Installer");

                using var response = await client
                    .GetAsync(indexUri, HttpCompletionOption.ResponseContentRead, cancellationToken)
                    .ConfigureAwait(false);

                response.EnsureSuccessStatusCode();
                indexContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch (Exception) when (!string.IsNullOrWhiteSpace(fallbackPath))
            {
                indexContent = await ReadFallbackAsync(fallbackPath!, cancellationToken).ConfigureAwait(false);
            }

            return Parse(indexContent);
        }

        private static async Task<string> ReadFallbackAsync(string path, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync().ConfigureAwait(false);
        }

        private static InstallerConfiguration Parse(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                throw new InvalidDataException("Installer index payload is empty.");
            }

            var document = XDocument.Parse(xml, LoadOptions.None);
            var root = document.Root ?? throw new InvalidDataException("Installer index payload is missing a root element.");

            var versions = GetRequiredElement(root, "Versions");
            var vanillaVersion = GetRequiredValue(versions, "Vanilla");
            var ptrVersion = GetRequiredValue(versions, "Ptr");

            var localStatePath = GetRequiredValue(root, "LocalStatePackagePath");

            var modsParent = GetRequiredElement(root, "Mods");
            var modElements = GetElements(modsParent, "Mod");
            var mods = new List<ModDefinition>();

            foreach (var modElement in modElements)
            {
                var name = GetOptionalAttributeOrElement(modElement, "NAME");
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                var url = GetOptionalAttributeOrElement(modElement, "URL");
                var imageUrl = GetOptionalAttributeOrElement(modElement, "IMAGE");
                var info = GetOptionalAttributeOrElement(modElement, "INFO");

                mods.Add(new ModDefinition(name!, url, imageUrl, info));
            }

            if (mods.Count == 0)
            {
                throw new InvalidDataException("Installer index does not contain any mod definitions.");
            }

            return new InstallerConfiguration(vanillaVersion, ptrVersion, localStatePath, mods);
        }

        private static XElement GetRequiredElement(XElement parent, string name)
        {
            var element = FindElement(parent, name);
            if (element == null)
            {
                throw new InvalidDataException(FormattableString.Invariant($"Installer index missing '{name}' element."));
            }

            return element;
        }

        private static IEnumerable<XElement> GetElements(XElement parent, string name)
        {
            foreach (var element in parent.Elements())
            {
                if (string.Equals(element.Name.LocalName, name, StringComparison.OrdinalIgnoreCase))
                {
                    yield return element;
                }
            }
        }

        private static string GetRequiredValue(XElement parent, string name)
        {
            var value = GetOptionalValue(parent, name);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidDataException(FormattableString.Invariant($"Installer index missing '{name}' value."));
            }

            return value;
        }

        private static string? GetOptionalValue(XElement parent, string name)
        {
            var element = FindElement(parent, name);
            var value = element?.Value;
            return string.IsNullOrWhiteSpace(value) ? value : value.Trim();
        }

        private static XElement? FindElement(XElement parent, string name)
        {
            foreach (var element in parent.Elements())
            {
                if (string.Equals(element.Name.LocalName, name, StringComparison.OrdinalIgnoreCase))
                {
                    return element;
                }
            }

            return null;
        }

        private static string? GetOptionalAttributeOrElement(XElement element, string name)
        {
            foreach (var attribute in element.Attributes())
            {
                if (string.Equals(attribute.Name.LocalName, name, StringComparison.OrdinalIgnoreCase))
                {
                    var value = attribute.Value;
                    return string.IsNullOrWhiteSpace(value) ? value : value.Trim();
                }
            }

            foreach (var child in element.Elements())
            {
                if (string.Equals(child.Name.LocalName, name, StringComparison.OrdinalIgnoreCase))
                {
                    var value = child.Value;
                    return string.IsNullOrWhiteSpace(value) ? value : value.Trim();
                }
            }

            return null;
        }
    }
}
