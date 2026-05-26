#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationIndex.cs create at 2026/04/12 12:04:31
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents the DocumentationIndex type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationIndex
    {
        #region Instance fields and properties

        /// <summary>
        /// Gets the collection used by DocumentationBuilder generation.
        /// </summary>
        public List<DocumentationGroupItem> Groups { get; } = new();

        #endregion
    }
}