#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationNamespaceObjectLinkItem.cs create at 2026/04/12 12:04:31
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents the DocumentationNamespaceObjectLinkItem type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationNamespaceObjectLinkItem
    {
        #region Instance fields and properties

        /// <summary>
        /// Gets or sets the KindLabel value used by generated documentation.
        /// </summary>
        public string KindLabel { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the Name value used by generated documentation.
        /// </summary>
        public string Name { get; init; } = string.Empty;
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