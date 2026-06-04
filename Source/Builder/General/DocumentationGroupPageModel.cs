#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

using System.Collections.Generic;

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents the DocumentationGroupPageModel type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationGroupPageModel
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the GroupName value used by generated documentation.
        /// </summary>
        public string GroupName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the versioned namespace links used by generated group documentation.
        /// </summary>
        public List<DocumentationNamespaceObjectLinkItem> NamespaceLinks { get; } = [];

        /// <summary>
        ///     Gets or sets the NamespaceNames value used by generated documentation.
        /// </summary>
        public List<string> NamespaceNames { get; } = [];

        /// <summary>
        ///     Gets or sets the PackageId value used by generated documentation.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the Version value used by generated documentation.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        #endregion
    }
}