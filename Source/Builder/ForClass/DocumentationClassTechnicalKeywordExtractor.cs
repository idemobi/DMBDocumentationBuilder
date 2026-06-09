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
    ///     Represents the DocumentationClassTechnicalKeywordExtractor type used by DocumentationBuilder generation.
    /// </summary>
    public static class DocumentationClassTechnicalKeywordExtractor
    {
        #region Fields

        private static readonly Regex IdentifierSplitRegex =
            new(@"(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z]|\d+)", RegexOptions.Compiled);

        private static readonly HashSet<string> UselessWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "get", "set", "init",
            "class", "object", "item", "model",
            "data", "value", "values",
            "true", "false", "null"
        };

        #endregion

        #region Public methods

        /// <summary>
        ///     Extracts documentation keywords and returns them as a comma-separated string.
        /// </summary>
        /// <param name="model">The model value used by the documentation generation operation.</param>
        /// <returns>The ExtractKeywordsAsString result produced by DocumentationBuilder generation.</returns>
        public static string ExtractKeywordsAsString(DocumentationClassPageModel model)
        {
            return string.Join(' ', ExtractKeywords(model));
        }

        /// <summary>
        ///     Extracts ordered documentation keywords from the supplied model or HTML content.
        /// </summary>
        /// <param name="model">The model value used by the documentation generation operation.</param>
        /// <returns>The ExtractKeywords result produced by DocumentationBuilder generation.</returns>
        public static IReadOnlyList<string> ExtractKeywords(DocumentationClassPageModel model)
        {
            if (model is null) throw new ArgumentNullException(nameof(model));

            HashSet<string> keywords = new(StringComparer.OrdinalIgnoreCase);

            AddIdentifier(keywords, model.ClassName);
            AddIdentifier(keywords, model.NamespaceName);
            AddIdentifier(keywords, model.PackageId);
            AddIdentifier(keywords, model.Version);

            if (model.BaseType is not null)
            {
                AddIdentifier(keywords, model.BaseType.ObjectName);
                AddIdentifier(keywords, model.BaseType.DisplayName);
                AddIdentifier(keywords, model.BaseType.NamespaceName);
            }

            foreach (DocumentationTypeLinkItem implementedInterface in model.ImplementedInterfaces)
            {
                AddIdentifier(keywords, implementedInterface.ObjectName);
                AddIdentifier(keywords, implementedInterface.DisplayName);
                AddIdentifier(keywords, implementedInterface.NamespaceName);
            }

            foreach (DocumentationDependencyEdgeItem dependencyEdge in model.DependencyEdges)
            {
                AddIdentifier(keywords, dependencyEdge.RelationshipKind);
                AddIdentifier(keywords, dependencyEdge.TargetName);
                AddIdentifier(keywords, dependencyEdge.TargetKindLabel);
                AddIdentifier(keywords, dependencyEdge.TargetNamespaceName);
            }

            foreach (DocumentationClassConstructorItem constructor in model.Constructors)
            {
                AddIdentifier(keywords, constructor.ConstructorName);
                AddIdentifier(keywords, constructor.Signature);
            }

            foreach (DocumentationClassFieldItem field in model.Fields)
            {
                AddIdentifier(keywords, field.FieldName);
                AddIdentifier(keywords, field.Signature);
            }

            foreach (DocumentationClassPropertyItem property in model.Properties)
            {
                AddIdentifier(keywords, property.PropertyName);
                AddIdentifier(keywords, property.Signature);
            }

            foreach (DocumentationClassMethodItem method in model.Methods)
            {
                AddIdentifier(keywords, method.MethodName);
                AddIdentifier(keywords, method.Signature);
            }

            foreach (DocumentationExtensionMethodItem extensionMethod in model.ExtensionMethods)
            {
                AddIdentifier(keywords, extensionMethod.MethodName);
                AddIdentifier(keywords, extensionMethod.ExtensionTypeName);
                AddIdentifier(keywords, extensionMethod.ExtensionNamespaceName);
                AddIdentifier(keywords, extensionMethod.Signature);
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

        private static void AddIdentifier(ISet<string> keywords, string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;

            foreach (string token in Tokenize(value))
            {
                if (!string.IsNullOrWhiteSpace(token)) keywords.Add(token);
            }
        }

        private static IEnumerable<string> Tokenize(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) yield break;

            string normalized = value
                .Replace('<', ' ')
                .Replace('>', ' ')
                .Replace('(', ' ')
                .Replace(')', ' ')
                .Replace('[', ' ')
                .Replace(']', ' ')
                .Replace('{', ' ')
                .Replace('}', ' ')
                .Replace(',', ' ')
                .Replace('.', ' ')
                .Replace(':', ' ')
                .Replace(';', ' ')
                .Replace('/', ' ')
                .Replace('\\', ' ')
                .Replace('_', ' ')
                .Replace('-', ' ')
                .Replace('+', ' ')
                .Replace('=', ' ')
                .Replace('?', ' ')
                .Replace('!', ' ')
                .Replace('&', ' ')
                .Replace('|', ' ')
                .Replace('"', ' ')
                .Replace('\'', ' ');

            foreach (string part in normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                yield return part;

                foreach (string splitPart in SplitIdentifier(part))
                {
                    if (!string.Equals(splitPart, part, StringComparison.OrdinalIgnoreCase)) yield return splitPart;
                }
            }
        }

        private static IEnumerable<string> SplitIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier)) yield break;

            string spaced = IdentifierSplitRegex.Replace(identifier, " $1");

            foreach (string part in spaced.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                yield return part;
            }
        }

        #endregion
    }
}
