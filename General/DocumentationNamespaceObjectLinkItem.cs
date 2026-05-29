#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents the DocumentationNamespaceObjectLinkItem type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationNamespaceObjectLinkItem
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the KindLabel value used by generated documentation.
        /// </summary>
        public string KindLabel { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the Name value used by generated documentation.
        /// </summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the PackageId value used by generated documentation.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the Version value used by generated documentation.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        #endregion
    }
}