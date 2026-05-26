#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationObjectItem.cs create at 2026/04/12 12:04:31
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents the DocumentationObjectItem type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationObjectItem
    {
        #region Instance fields and properties

        /// <summary>
        /// Gets or sets the KindName value used by generated documentation.
        /// </summary>
        public required string KindName { get; init; }
        /// <summary>
        /// Gets or sets the NamespaceName value used by generated documentation.
        /// </summary>
        public required string NamespaceName { get; init; }
        /// <summary>
        /// Gets or sets the ObjectName value used by generated documentation.
        /// </summary>
        public required string ObjectName { get; init; }

        #endregion
    }
}