#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationInterfaceTechnicalKeywordExtractor.cs create at 2026/04/13 17:04:46
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using System.Text.RegularExpressions;

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents the DocumentationInterfaceTechnicalKeywordExtractor type used by DocumentationBuilder generation.
    /// </summary>
    public static class DocumentationInterfaceTechnicalKeywordExtractor
    {
        #region Fields

        private static readonly Regex SplitRegex =
            new(@"[^A-Za-z0-9_]+", RegexOptions.Compiled);

        private static readonly Regex PascalSplitRegex =
            new(@"(?<!^)([A-Z])", RegexOptions.Compiled);

        private static readonly HashSet<string> UselessWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "interface", "method", "property", "properties",
            "event", "events",
            "get", "set", "init",
            "true", "false", "null"
        };

        #endregion

        #region Public methods

        /// <summary>
        /// Extracts documentation keywords and returns them as a comma-separated string.
        /// </summary>
        /// <param name="model">The model value used by the documentation generation operation.</param>
        /// <returns>The ExtractKeywordsAsString result produced by DocumentationBuilder generation.</returns>
        public static string ExtractKeywordsAsString(DocumentationInterfacePageModel model)
        {
            return string.Join(' ', ExtractKeywords(model));
        }

        /// <summary>
        /// Extracts ordered documentation keywords from the supplied model or HTML content.
        /// </summary>
        /// <param name="model">The model value used by the documentation generation operation.</param>
        /// <returns>The ExtractKeywords result produced by DocumentationBuilder generation.</returns>
        public static IReadOnlyList<string> ExtractKeywords(DocumentationInterfacePageModel model)
        {
            if (model is null) throw new ArgumentNullException(nameof(model));

            HashSet<string> keywords = new(StringComparer.OrdinalIgnoreCase);

            Add(keywords, model.InterfaceName);
            Add(keywords, model.NamespaceName);
            Add(keywords, model.PackageId);
            Add(keywords, model.Version);
            Add(keywords, model.Accessibility);
            Add(keywords, model.AssemblyName);

            foreach (var method in model.Methods)
            {
                Add(keywords, method.MethodName);
                Add(keywords, method.Signature);
            }

            foreach (var property in model.Properties)
            {
                Add(keywords, property.PropertyName);
                Add(keywords, property.Signature);
            }

            foreach (var ev in model.Events)
            {
                Add(keywords, ev.EventName);
                Add(keywords, ev.Signature);
            }
            foreach (var ext in model.ExtensionMethods)
            {
                Add(keywords, ext.MethodName);
                Add(keywords, ext.ExtensionTypeName);
                Add(keywords, ext.ExtensionNamespaceName);
                Add(keywords, ext.Signature);
            }
            if (model.IsObsolete)
            {
                keywords.Add("obsolete");
                Add(keywords, model.ObsoleteMessage);
            }

            return keywords
                .Where(static x => !string.IsNullOrWhiteSpace(x))
                .Select(static x => x.Trim().ToLowerInvariant())
                .Where(static x => x.Length >= 2)
                .Where(static x => !UselessWords.Contains(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(static x => x, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        #endregion

        #region Private methods

        private static void Add(HashSet<string> keywords, string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;

            foreach (string token in Tokenize(value))
            {
                keywords.Add(token);
            }
        }

        private static IEnumerable<string> Tokenize(string input)
        {
            foreach (string part in SplitRegex.Split(input))
            {
                if (string.IsNullOrWhiteSpace(part)) continue;

                yield return part;

                // split PascalCase
                string split = PascalSplitRegex.Replace(part, " $1");

                foreach (string sub in split.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                {
                    yield return sub;
                }
            }
        }

        #endregion
    }
}