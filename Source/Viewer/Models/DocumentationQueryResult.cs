#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Represents a documentation object loaded from the generated documentation database.
    /// </summary>
    public sealed class DocumentationQueryResult
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the builder version or builder identifier stored with the generated documentation object.
        /// </summary>
        public string Builder { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the generated HTML content rendered by the documentation page.
        /// </summary>
        public string HtmlContent { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the database identifier of the documentation object.
        /// </summary>
        public long Id { get; init; }

        /// <summary>
        ///     Gets the human-oriented keywords stored for search and discovery.
        /// </summary>
        public string Keywords { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the namespace that contains the documented object.
        /// </summary>
        public string NamespaceName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the documented object name.
        /// </summary>
        public string ObjectName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the documented object type, such as class, interface, enum, namespace, or group.
        /// </summary>
        public string ObjectType { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the package identifier that owns the documentation object.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the generated route path for the documentation object.
        /// </summary>
        public string RoutePath { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the technical keywords used for API-oriented search.
        /// </summary>
        public string TechnicalKeywords { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the documented package version.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        #endregion
    }
}