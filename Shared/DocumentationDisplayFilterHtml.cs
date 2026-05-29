#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    internal static class DocumentationDisplayFilterHtml
    {
        #region Static methods

        private static string Html(string? value)
        {
            return System.Net.WebUtility.HtmlEncode(value ?? string.Empty);
        }

        internal static string MemberAttributes(string memberKind, string? accessibility)
        {
            return $"data-doc-member=\"true\" data-doc-member-kind=\"{Html(memberKind)}\" data-doc-member-accessibility=\"{Html(NormalizeAccessibility(accessibility))}\"";
        }

        private static string NormalizeAccessibility(string? accessibility)
        {
            return (accessibility ?? string.Empty)
                .Trim()
                .ToLowerInvariant()
                .Replace(' ', '-');
        }

        internal static string SectionAttributes(string memberKind)
        {
            return $"data-doc-section-kind=\"{Html(memberKind)}\"";
        }

        #endregion
    }
}