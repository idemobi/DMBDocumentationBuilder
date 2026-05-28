#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationKeywordExtractor.cs create at 2026/04/13 17:04:54
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using System.Net;
using System.Text.RegularExpressions;

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents the DocumentationKeywordExtractor type used by DocumentationBuilder generation.
    /// </summary>
    public static class DocumentationKeywordExtractor
    {
        #region Fields

        private static readonly Regex HtmlTagRegex = new("<.*?>", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex WordSplitRegex = new(@"[^A-Za-zÀ-ÖØ-öø-ÿ0-9_]+", RegexOptions.Compiled);

        private static readonly HashSet<string> UselessWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "a", "an", "the", "and", "or", "of", "to", "in", "on", "for", "with",
            "is", "are", "be", "this", "that", "these", "those", "it", "its",
            "as", "at", "by", "from", "into", "about", "over", "after", "before",
            "le", "la", "les", "un", "une", "des", "de", "du", "et", "ou",
            "à", "au", "aux", "en", "dans", "sur", "pour", "par", "avec",
            "est", "sont", "ce", "cet", "cette", "ces", "il", "elle", "ils", "elles",
            "html", "div", "span", "class", "card", "body", "section"
        };

        #endregion

        #region Public methods

        /// <summary>
        /// Extracts documentation keywords and returns them as a comma-separated string.
        /// </summary>
        /// <param name="html">The html value used by the documentation generation operation.</param>
        /// <returns>The ExtractKeywordsAsString result produced by DocumentationBuilder generation.</returns>
        public static string ExtractKeywordsAsString(string html)
        {
            return string.Join(' ', ExtractKeywords(html));
        }

        /// <summary>
        /// Extracts ordered documentation keywords from the supplied model or HTML content.
        /// </summary>
        /// <param name="html">The html value used by the documentation generation operation.</param>
        /// <returns>The ExtractKeywords result produced by DocumentationBuilder generation.</returns>
        public static IReadOnlyList<string> ExtractKeywords(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return Array.Empty<string>();

            string text = HtmlTagRegex.Replace(html, " ");
            text = WebUtility.HtmlDecode(text) ?? string.Empty;

            string[] rawWords = WordSplitRegex.Split(text);

            string[] keywords = rawWords
                .Where(static word => !string.IsNullOrWhiteSpace(word))
                .Select(static word => word.Trim().ToLowerInvariant())
                .Where(static word => word.Length >= 2)
                .Where(static word => !UselessWords.Contains(word))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(static word => word, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return keywords;
        }

        #endregion
    }
}
