#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Text.RegularExpressions;

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents the DocumentationStructTechnicalKeywordExtractor type used by DocumentationBuilder generation.
    /// </summary>
    public static class DocumentationStructTechnicalKeywordExtractor
    {
        #region Fields

        private static readonly Regex SplitRegex =
            new(@"[^A-Za-z0-9_]+", RegexOptions.Compiled);

        private static readonly Regex PascalSplitRegex =
            new(@"(?<!^)([A-Z])", RegexOptions.Compiled);

        private static readonly HashSet<string> UselessWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "struct", "field", "fields", "method", "methods",
            "property", "properties",
            "get", "set", "init",
            "true", "false", "null"
        };

        #endregion

        #region Public methods

        /// <summary>
        ///     Extracts documentation keywords and returns them as a comma-separated string.
        /// </summary>
        /// <param name="model">The model value used by the documentation generation operation.</param>
        /// <returns>The ExtractKeywordsAsString result produced by DocumentationBuilder generation.</returns>
        public static string ExtractKeywordsAsString(DocumentationStructPageModel model)
        {
            return string.Join(' ', ExtractKeywords(model));
        }

        /// <summary>
        ///     Extracts ordered documentation keywords from the supplied model or HTML content.
        /// </summary>
        /// <param name="model">The model value used by the documentation generation operation.</param>
        /// <returns>The ExtractKeywords result produced by DocumentationBuilder generation.</returns>
        public static IReadOnlyList<string> ExtractKeywords(DocumentationStructPageModel model)
        {
            if (model is null) throw new ArgumentNullException(nameof(model));

            HashSet<string> keywords = new(StringComparer.OrdinalIgnoreCase);
            Add(keywords, model.StructName);
            Add(keywords, model.NamespaceName);
            Add(keywords, model.PackageId);
            Add(keywords, model.Version);
            Add(keywords, model.Accessibility);
            Add(keywords, model.AssemblyName);

            keywords.Add("struct");

            if (model.IsReadOnly) keywords.Add("readonly");

            if (model.IsRefLike) keywords.Add("reflike");

            foreach (string implementedInterface in model.ImplementedInterfaces)
            {
                Add(keywords, implementedInterface);
            }

            foreach (DocumentationDependencyEdgeItem dependencyEdge in model.DependencyEdges)
            {
                Add(keywords, dependencyEdge.RelationshipKind);
                Add(keywords, dependencyEdge.SourceName);
                Add(keywords, dependencyEdge.SourceKindLabel);
                Add(keywords, dependencyEdge.SourceNamespaceName);
                Add(keywords, dependencyEdge.TargetName);
                Add(keywords, dependencyEdge.TargetKindLabel);
                Add(keywords, dependencyEdge.TargetNamespaceName);
            }

            foreach (DocumentationStructFieldItem field in model.Fields)
            {
                Add(keywords, field.FieldName);
                Add(keywords, field.Signature);
            }

            foreach (DocumentationStructPropertyItem property in model.Properties)
            {
                Add(keywords, property.PropertyName);
                Add(keywords, property.Signature);
            }

            foreach (DocumentationStructMethodItem method in model.Methods)
            {
                Add(keywords, method.MethodName);
                Add(keywords, method.Signature);
            }

            foreach (DocumentationExtensionMethodItem extensionMethod in model.ExtensionMethods)
            {
                Add(keywords, extensionMethod.MethodName);
                Add(keywords, extensionMethod.ExtensionTypeName);
                Add(keywords, extensionMethod.ExtensionNamespaceName);
                Add(keywords, extensionMethod.Signature);
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
