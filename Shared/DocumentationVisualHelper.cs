#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationVisualHelper.cs create at 2026/04/12 12:04:31
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using System.Reflection;
using Microsoft.CodeAnalysis;

#endregion

namespace DMBDocumentationBuilder
{
    internal static class DocumentationVisualHelper
    {
        #region Constants

        /// <summary>
        /// Gets the pre-rendered icon badge HTML used by generated documentation pages.
        /// </summary>
        public static readonly string AbstractIconHtml = RenderIconBadge("secondary", "bi-circle-square", "Abstract");
        /// <summary>
        /// Gets the pre-rendered icon badge HTML used by generated documentation pages.
        /// </summary>
        public static readonly string ConstIconHtml = RenderIconBadge("info", "bi-pin-angle", "Constant");
        /// <summary>
        /// Gets the pre-rendered icon badge HTML used by generated documentation pages.
        /// </summary>
        public static readonly string ExtensionIconHtml = RenderIconBadge("info", "bi-link-45deg", "Extension");
        /// <summary>
        /// Gets the pre-rendered icon badge HTML used by generated documentation pages.
        /// </summary>
        public static readonly string OverrideIconHtml = RenderIconBadge("success", "bi-arrow-down-right-square", "Override");
        /// <summary>
        /// Gets the pre-rendered icon badge HTML used by generated documentation pages.
        /// </summary>
        public static readonly string ReadOnlyIconHtml = RenderIconBadge("warning", "bi-dash-circle", "Read-only");
        /// <summary>
        /// Gets the pre-rendered icon badge HTML used by generated documentation pages.
        /// </summary>
        public static readonly string RefLikeIconHtml = RenderIconBadge("warning", "bi-box-arrow-in-right", "Ref-like");
        /// <summary>
        /// Gets the pre-rendered icon badge HTML used by generated documentation pages.
        /// </summary>
        public static readonly string SealedIconHtml = RenderIconBadge("dark", "bi-lock-fill", "Sealed");
        /// <summary>
        /// Gets the pre-rendered icon badge HTML used by generated documentation pages.
        /// </summary>
        public static readonly string StaticIconHtml = RenderIconBadge("info", "bi-lightning-charge", "Static");
        /// <summary>
        /// Gets the pre-rendered icon badge HTML used by generated documentation pages.
        /// </summary>
        public static readonly string VirtualIconHtml = RenderIconBadge("primary", "bi-arrow-repeat", "Virtual");

        #endregion

        #region Static fields and properties

        /// <summary>
        /// Gets or sets the DocumentationBuilderVersion value used by generated documentation.
        /// </summary>
        public static string DocumentationBuilderVersion
        {
            get
            {
                string? version = typeof(DocumentationVisualHelper)
                    .Assembly
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    ?.InformationalVersion;

                if (string.IsNullOrWhiteSpace(version))
                {
                    return "unknown";
                }

                string clean = version.Split('+')[0];
                string[] parts = clean.Split('.');
                if (parts.Length >= 2)
                {
                    return $"{parts[0]}.{parts[1]}";
                }

                return clean;
            }
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Initializes shared DocumentationBuilder visual rendering metadata.
        /// </summary>
        /// <param name="symbol">The symbol value used by the documentation generation operation.</param>
        /// <returns>The combined source text and the number of syntax files that contributed to it.</returns>
        public static (string Source, int FileCount) ExtractFullSource(INamedTypeSymbol symbol)
        {
            List<string> parts = [];
            foreach (var decl in symbol.DeclaringSyntaxReferences)
            {
                var syntax = decl.GetSyntax();
                var tree = syntax.SyntaxTree;
                //parts.Add($"// ===== FILE: {tree.FilePath} ===== \n\n");
                parts.Add(syntax.ToFullString());
            }

            return (string.Join("\n\n", parts), parts.Count);
        }

        /// <summary>
        /// Renders the member signature line used by generated documentation pages.
        /// </summary>
        /// <param name="iconHtml">The pre-rendered icon HTML to display beside the signature.</param>
        /// <param name="signature">The source signature to render.</param>
        /// <param name="isObsolete">A value indicating whether the signature should be rendered as obsolete.</param>
        /// <returns>The RenderMemberSignature result produced by DocumentationBuilder generation.</returns>
        public static string RenderMemberSignature(string iconHtml, string signature, bool isObsolete)
        {
            string signatureHtml = System.Net.WebUtility.HtmlEncode(signature) ?? string.Empty;
            if (isObsolete)
            {
                signatureHtml = $"<span class=\"text-decoration-line-through text-danger\">{signatureHtml}</span>";
            }

            if (string.IsNullOrWhiteSpace(iconHtml))
            {
                return $"<span class=\"documentation-member-signature position-relative d-block w-100\"><code class=\"d-block w-100 text-break\">{signatureHtml}</code></span>";
            }

            return $"<span class=\"documentation-member-signature position-relative d-block w-100\"><code class=\"d-block w-100 text-break pe-5\">{signatureHtml}</code><span class=\"documentation-member-badges position-absolute end-0 translate-middle-y d-inline-flex align-items-center gap-1\" style=\"top: 0em;\">{iconHtml}</span></span>";
        }

        /// <summary>
        /// Renders the accessibility icon used by generated documentation pages.
        /// </summary>
        /// <param name="accessibility">The accessibility value used by the documentation generation operation.</param>
        /// <returns>The RenderAccessibilityIcon result produced by DocumentationBuilder generation.</returns>
        public static string RenderAccessibilityIcon(string? accessibility)
        {
            return accessibility switch
            {
                "public" => RenderIconBadge("success", "bi-unlock", "Public"),
                "private" => RenderIconBadge("danger", "bi-lock", "Private"),
                "protected" => RenderIconBadge("warning", "bi-shield-lock", "Protected"),
                "internal" => RenderIconBadge("warning", "bi-box", "Internal"),
                "protected internal" => RenderIconBadge("warning", "bi-shield", "Protected internal"),
                "private protected" => RenderIconBadge("warning", "bi-shield", "Private protected"),
                _ => string.Empty
            };
        }

        private static string RenderIconBadge(string variant, string iconClass, string label)
        {
            return $"<span class=\"badge text-bg-{variant} rounded-pill me-1\" title=\"{label}\" aria-label=\"{label}\" data-bs-toggle=\"tooltip\" data-bs-title=\"{label}\"><i class=\"bi {iconClass}\" aria-hidden=\"true\"></i></span>";
        }

        #endregion
    }
}
