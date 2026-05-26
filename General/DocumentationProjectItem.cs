#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationProjectItem.cs create at 2026/04/12 12:04:31
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents the DocumentationProjectItem type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationProjectItem
    {
        #region Instance fields and properties

        /// <summary>
        /// Gets the collection used by DocumentationBuilder generation.
        /// </summary>
        public List<DocumentationNamespaceItem> Namespaces { set; get; } = new();
        /// <summary>
        /// Gets or sets the PackageId value used by generated documentation.
        /// </summary>
        public required string PackageId { get; init; }
        /// <summary>
        /// Gets or sets the ProjectFilePath value used by generated documentation.
        /// </summary>
        public required string ProjectFilePath { get; init; }
        /// <summary>
        /// Gets or sets the ProjectName value used by generated documentation.
        /// </summary>
        public required string ProjectName { get; init; }
        /// <summary>
        /// Gets or sets the Version value used by generated documentation.
        /// </summary>
        public required string Version { get; init; }

        #endregion
    }
}