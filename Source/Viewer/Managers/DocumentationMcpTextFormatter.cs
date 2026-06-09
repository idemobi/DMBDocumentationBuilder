#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Text;

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Formats documentation query results into compact text responses for MCP tools.
    /// </summary>
    public static class DocumentationMcpTextFormatter
    {
        #region Static methods

        /// <summary>
        ///     Formats one documentation query result with metadata and generated HTML content.
        /// </summary>
        /// <param name="result">The documentation result to format, or <see langword="null" /> when the object was not found.</param>
        /// <returns>An MCP-friendly text response describing the documentation object.</returns>
        public static string FormatDocumentation(DocumentationQueryResult? result)
        {
            if (result is null) return "Documentation object not found.";

            StringBuilder text = new();
            text.AppendLine($"Documentation found for {result.NamespaceName}.{result.ObjectName} ({result.ObjectType})");
            text.AppendLine();
            text.AppendLine($"Id: {result.Id}");
            text.AppendLine($"Package: {result.PackageId}");
            text.AppendLine($"Version: {result.Version}");
            text.AppendLine($"Namespace: {result.NamespaceName}");
            text.AppendLine($"Object: {result.ObjectName}");
            text.AppendLine($"Type: {result.ObjectType}");
            text.AppendLine($"Route: {result.RoutePath}");
            text.AppendLine($"Builder: {result.Builder}");

            if (!string.IsNullOrWhiteSpace(result.TechnicalKeywords))
            {
                text.AppendLine();
                text.AppendLine("Technical keywords:");
                text.AppendLine(result.TechnicalKeywords);
            }

            if (!string.IsNullOrWhiteSpace(result.Keywords))
            {
                text.AppendLine();
                text.AppendLine("Keywords:");
                text.AppendLine(result.Keywords);
            }

            if (!string.IsNullOrWhiteSpace(result.HtmlContent))
            {
                text.AppendLine();
                text.AppendLine("HTML content:");
                text.AppendLine(LimitText(result.HtmlContent, 12000));
            }

            return text.ToString();
        }

        /// <summary>
        ///     Formats documentation search results.
        /// </summary>
        /// <param name="query">Search text used to produce the results.</param>
        /// <param name="results">Documentation search results to format.</param>
        /// <returns>An MCP-friendly search response.</returns>
        public static string FormatDocumentationSearch(string query, IReadOnlyList<DocumentationSearchResultItem> results)
        {
            if (results.Count == 0) return $"No documentation result found for '{query}'.";

            List<string> lines = [];

            foreach (DocumentationSearchResultItem item in results)
            {
                lines.Add(
                    $"- {item.NamespaceName}.{item.ObjectName} ({item.ObjectType}) [{item.PackageId} {item.Version}]\n  {item.RoutePath}");
            }

            return $"{results.Count} result(s) found for '{query}':\n\n{string.Join("\n\n", lines)}";
        }

        /// <summary>
        ///     Formats the list of documented objects contained in a namespace.
        /// </summary>
        /// <param name="namespaceName">Namespace that was queried.</param>
        /// <param name="results">Namespace object results to format.</param>
        /// <returns>An MCP-friendly namespace listing.</returns>
        public static string FormatNamespaceObjects(
            string namespaceName,
            IReadOnlyList<DocumentationSearchResultItem> results
        )
        {
            if (results.Count == 0)
            {
                return $"No documented object found in namespace '{namespaceName}'.";
            }

            List<string> lines = [];

            foreach (DocumentationSearchResultItem item in results)
            {
                lines.Add(
                    $"- {item.ObjectName} ({item.ObjectType}) [{item.PackageId} {item.Version}]\n  {item.RoutePath}");
            }

            return $"Namespace '{namespaceName}' contains {results.Count} object(s):\n\n{string.Join("\n\n", lines)}";
        }

        /// <summary>
        ///     Formats documentation objects related to one documented object.
        /// </summary>
        /// <param name="objectName">Object name used to find related results.</param>
        /// <param name="results">Related documentation objects to format.</param>
        /// <returns>An MCP-friendly related-object listing.</returns>
        public static string FormatRelatedObjects(
            string objectName,
            IReadOnlyList<DocumentationSearchResultItem> results
        )
        {
            if (results.Count == 0)
            {
                return $"No related object found for '{objectName}'.";
            }

            List<string> lines = [];

            foreach (DocumentationSearchResultItem item in results)
            {
                lines.Add(
                    $"- {item.NamespaceName}.{item.ObjectName} ({item.ObjectType}) [{item.PackageId} {item.Version}]\n  {item.RoutePath}");
            }

            return $"Related object(s) found for '{objectName}':\n\n{string.Join("\n\n", lines)}";
        }

        /// <summary>
        ///     Limits a text block to a maximum length and appends a truncation marker when needed.
        /// </summary>
        /// <param name="value">Text value to limit.</param>
        /// <param name="maxLength">Maximum number of characters to keep before truncation.</param>
        /// <returns>The original text when it fits, an empty string for blank input, or a truncated text block.</returns>
        public static string LimitText(string value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            return value.Length <= maxLength
                ? value
                : value[..maxLength] + "\n\n[truncated]";
        }

        #endregion
    }
}