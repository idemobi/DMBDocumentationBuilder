#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationMarkdownContentScanner.cs create at 2026/05/18 22:05:00
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using System.Text.RegularExpressions;

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Scans Markdown content folders used by DocumentationBuilder.
    /// </summary>
    public static class DocumentationMarkdownContentScanner
    {
        #region Static fields and properties

        private static readonly Regex WordBoundaryRegex = new(@"(?<=[a-z0-9])(?=[A-Z])", RegexOptions.Compiled);

        #endregion

        #region Static methods

        /// <summary>
        /// Reads Markdown content items from the configured project folders.
        /// </summary>
        /// <param name="project">The project whose Markdown folders should be scanned.</param>
        /// <returns>The Markdown content items found in configured folders.</returns>
        public static IReadOnlyList<DocumentationMarkdownContentItem> Scan(DocumentationProjectDescriptor project)
        {
            List<DocumentationMarkdownContentItem> items = [];

            foreach (DocumentationMarkdownContentDescriptor descriptor in project.MarkdownContents)
            {
                if (string.IsNullOrWhiteSpace(descriptor.RootDirectoryPath) ||
                    !Directory.Exists(descriptor.RootDirectoryPath))
                {
                    continue;
                }

                string root = Path.GetFullPath(descriptor.RootDirectoryPath);

                foreach (string filePath in Directory.EnumerateFiles(root, "*.md", SearchOption.AllDirectories))
                {
                    string relativePath = Path.GetRelativePath(root, filePath);
                    string[] parts = relativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                    if (parts.Length is < 1 or > 2)
                    {
                        continue;
                    }

                    string title = ReadTitle(filePath);
                    string slug = BuildSlug(parts);

                    items.Add(new DocumentationMarkdownContentItem
                    {
                        FolderTitle = parts.Length == 2 ? ToTitle(parts[0]) : string.Empty,
                        Icon = descriptor.Icon,
                        ObjectType = descriptor.ObjectType,
                        SectionTitle = descriptor.SectionTitle,
                        Slug = slug,
                        SourceFilePath = filePath,
                        Title = string.IsNullOrWhiteSpace(title)
                            ? ToTitle(Path.GetFileNameWithoutExtension(filePath))
                            : title
                    });
                }
            }

            return items
                .OrderBy(item => item.SectionTitle, StringComparer.Ordinal)
                .ThenBy(item => item.FolderTitle, StringComparer.Ordinal)
                .ThenBy(item => item.Title, StringComparer.Ordinal)
                .ToArray();
        }

        private static string BuildSlug(IReadOnlyList<string> pathParts)
        {
            return string.Join(
                "/",
                pathParts.Select(part => Slugify(Path.GetFileNameWithoutExtension(part))));
        }

        private static string ReadTitle(string filePath)
        {
            foreach (string line in File.ReadLines(filePath))
            {
                string trimmed = line.Trim();

                if (trimmed.StartsWith("# ", StringComparison.Ordinal))
                {
                    return trimmed[2..].Trim();
                }
            }

            return string.Empty;
        }

        private static string Slugify(string value)
        {
            string spaced = WordBoundaryRegex.Replace(value, "-");
            string lower = spaced.Trim().ToLowerInvariant();
            string result = Regex.Replace(lower, @"[^a-z0-9]+", "-").Trim('-');

            return string.IsNullOrWhiteSpace(result) ? "page" : result;
        }

        private static string ToTitle(string value)
        {
            string spaced = WordBoundaryRegex.Replace(value, " ");
            spaced = Regex.Replace(spaced, @"[_\-]+", " ").Trim();

            if (string.IsNullOrWhiteSpace(spaced))
            {
                return "Content";
            }

            return char.ToUpperInvariant(spaced[0]) + spaced[1..];
        }

        #endregion
    }
}
