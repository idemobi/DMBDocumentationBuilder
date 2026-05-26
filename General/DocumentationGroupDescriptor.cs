#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationGroupDescriptor.cs create at 2026/04/12 12:04:31
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents the DocumentationGroupDescriptor type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationGroupDescriptor
    {
        #region Instance fields and properties

        /// <summary>
        /// Gets or sets the GroupName value used by generated documentation.
        /// </summary>
        public required string GroupName { get; init; }
        /// <summary>
        /// Gets or sets the Projects value used by generated documentation.
        /// </summary>
        public required List<DocumentationProjectDescriptor> Projects { get; init; }

        #endregion
    }
}