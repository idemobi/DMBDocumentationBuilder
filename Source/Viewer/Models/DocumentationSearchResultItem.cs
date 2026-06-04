#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Represents a compact documentation search or navigation result.
    /// </summary>
    public sealed class DocumentationSearchResultItem
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the namespace that contains the result object.
        /// </summary>
        public string NamespaceName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the documented object name.
        /// </summary>
        public string ObjectName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the documented object type.
        /// </summary>
        public string ObjectType { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the package identifier that owns the result.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the generated route path for the documentation object.
        /// </summary>
        public string RoutePath { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the documented package version.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        #endregion
    }
}