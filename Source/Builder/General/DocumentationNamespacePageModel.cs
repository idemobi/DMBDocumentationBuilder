#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents the DocumentationNamespacePageModel type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationNamespacePageModel
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the Classes value used by generated documentation.
        /// </summary>
        public List<DocumentationNamespaceObjectLinkItem> Classes { get; } = [];

        /// <summary>
        ///     Gets or sets the Enums value used by generated documentation.
        /// </summary>
        public List<DocumentationNamespaceObjectLinkItem> Enums { get; } = [];

        /// <summary>
        ///     Gets or sets the GroupName value used by generated documentation.
        /// </summary>
        public string GroupName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the Interfaces value used by generated documentation.
        /// </summary>
        public List<DocumentationNamespaceObjectLinkItem> Interfaces { get; } = [];

        /// <summary>
        ///     Gets or sets the NamespaceName value used by generated documentation.
        /// </summary>
        public string NamespaceName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the PackageId value used by generated documentation.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the Records value used by generated documentation.
        /// </summary>
        public List<DocumentationNamespaceObjectLinkItem> Records { get; } = [];

        /// <summary>
        ///     Gets or sets the Structs value used by generated documentation.
        /// </summary>
        public List<DocumentationNamespaceObjectLinkItem> Structs { get; } = [];

        /// <summary>
        ///     Gets or sets the Version value used by generated documentation.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        #endregion
    }
}