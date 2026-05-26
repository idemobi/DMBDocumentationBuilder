#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationDisplayFilterHtml.cs create at 2026/05/18 13:56:18
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    internal static class DocumentationDisplayFilterHtml
    {
        #region Static methods

        internal static string MemberAttributes(string memberKind, string? accessibility)
        {
            return $"data-doc-member=\"true\" data-doc-member-kind=\"{Html(memberKind)}\" data-doc-member-accessibility=\"{Html(NormalizeAccessibility(accessibility))}\"";
        }

        internal static string SectionAttributes(string memberKind)
        {
            return $"data-doc-section-kind=\"{Html(memberKind)}\"";
        }

        private static string Html(string? value)
        {
            return System.Net.WebUtility.HtmlEncode(value ?? string.Empty);
        }

        private static string NormalizeAccessibility(string? accessibility)
        {
            return (accessibility ?? string.Empty)
                .Trim()
                .ToLowerInvariant()
                .Replace(' ', '-');
        }

        #endregion
    }
}
