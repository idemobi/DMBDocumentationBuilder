#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationGroupPageModel.cs create at 2026/04/12 12:04:31
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents the DocumentationGroupPageModel type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationGroupPageModel
    {
        #region Instance fields and properties

        /// <summary>
        /// Gets or sets the GroupName value used by generated documentation.
        /// </summary>
        public string GroupName { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the NamespaceNames value used by generated documentation.
        /// </summary>
        public List<string> NamespaceNames { get; } = [];
        /// <summary>
        /// Gets the versioned namespace links used by generated group documentation.
        /// </summary>
        public List<DocumentationNamespaceObjectLinkItem> NamespaceLinks { get; } = [];
        /// <summary>
        /// Gets or sets the PackageId value used by generated documentation.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the Version value used by generated documentation.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        #endregion
    }
}
