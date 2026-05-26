#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationNamespaceItem.cs create at 2026/04/12 12:04:31
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents the DocumentationNamespaceItem type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationNamespaceItem
    {
        #region Instance fields and properties

        /// <summary>
        /// Gets the collection used by DocumentationBuilder generation.
        /// </summary>
        public List<string> Classes { get; } = new();
        /// <summary>
        /// Gets the collection used by DocumentationBuilder generation.
        /// </summary>
        public List<string> Enums { get; } = new();
        /// <summary>
        /// Gets the collection used by DocumentationBuilder generation.
        /// </summary>
        public List<string> Interfaces { get; } = new();
        /// <summary>
        /// Gets or sets the NamespaceName value used by generated documentation.
        /// </summary>
        public required string NamespaceName { get; init; }
        /// <summary>
        /// Gets the collection used by DocumentationBuilder generation.
        /// </summary>
        public List<string> Records { get; } = new();
        /// <summary>
        /// Gets the collection used by DocumentationBuilder generation.
        /// </summary>
        public List<string> Structs { get; } = new();

        #endregion
    }
}