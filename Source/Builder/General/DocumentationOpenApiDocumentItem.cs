#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

using System.Collections.Generic;

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents one imported OpenAPI document and its indexed operations.
    /// </summary>
    public sealed class DocumentationOpenApiDocumentItem
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the OpenAPI document description.
        /// </summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the stable document name used in routes.
        /// </summary>
        public string DocumentName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the Bootstrap icon used by the sidebar section.
        /// </summary>
        public string Icon { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the raw OpenAPI JSON content.
        /// </summary>
        public string JsonContent { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the indexed operations contained in the document.
        /// </summary>
        public IReadOnlyList<DocumentationOpenApiOperationItem> Operations { get; init; } = [];

        /// <summary>
        ///     Gets the package identifier that owns the document.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the route path used to render the document overview.
        /// </summary>
        public string RoutePath { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the sidebar section title.
        /// </summary>
        public string SectionTitle { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the OpenAPI document title.
        /// </summary>
        public string Title { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the package version that owns the document.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        #endregion
    }
}