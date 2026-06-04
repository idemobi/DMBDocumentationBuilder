#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

using System.Collections.Generic;

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents the DocumentationGroupItem type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationGroupItem
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the GroupName value used by generated documentation.
        /// </summary>
        public required string GroupName { get; init; }

        /// <summary>
        ///     Gets the collection used by DocumentationBuilder generation.
        /// </summary>
        public List<DocumentationProjectItem> Projects { get; } = new();

        #endregion
    }
}