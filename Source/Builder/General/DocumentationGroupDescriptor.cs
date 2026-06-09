#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents the DocumentationGroupDescriptor type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationGroupDescriptor
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the GroupName value used by generated documentation.
        /// </summary>
        public required string GroupName { get; init; }

        /// <summary>
        ///     Gets or sets the Projects value used by generated documentation.
        /// </summary>
        public required List<DocumentationProjectDescriptor> Projects { get; init; }

        #endregion
    }
}