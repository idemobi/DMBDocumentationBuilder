#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents the DocumentationProjectItem type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationProjectItem
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the collection used by DocumentationBuilder generation.
        /// </summary>
        public List<DocumentationNamespaceItem> Namespaces { set; get; } = new();

        /// <summary>
        ///     Gets or sets the PackageId value used by generated documentation.
        /// </summary>
        public required string PackageId { get; init; }

        /// <summary>
        ///     Gets or sets the ProjectFilePath value used by generated documentation.
        /// </summary>
        public required string ProjectFilePath { get; init; }

        /// <summary>
        ///     Gets or sets the ProjectName value used by generated documentation.
        /// </summary>
        public required string ProjectName { get; init; }

        /// <summary>
        ///     Gets or sets the Version value used by generated documentation.
        /// </summary>
        public required string Version { get; init; }

        #endregion
    }
}