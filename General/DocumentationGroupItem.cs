#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationGroupItem.cs create at 2026/04/12 12:04:31
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents the DocumentationGroupItem type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationGroupItem
    {
        #region Instance fields and properties

        /// <summary>
        /// Gets or sets the GroupName value used by generated documentation.
        /// </summary>
        public required string GroupName { get; init; }
        /// <summary>
        /// Gets the collection used by DocumentationBuilder generation.
        /// </summary>
        public List<DocumentationProjectItem> Projects { get; } = new();

        #endregion
    }
}