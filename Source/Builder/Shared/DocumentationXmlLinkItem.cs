#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents the DocumentationXmlLinkItem type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationXmlLinkItem
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the Anchor value used by generated documentation.
        /// </summary>
        public string Anchor { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the Href value used by generated documentation.
        /// </summary>
        public string Href { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the IsIntraPage value used by generated documentation.
        /// </summary>
        public bool IsIntraPage { get; init; }

        /// <summary>
        ///     Gets or sets the IsKeyword value used by generated documentation.
        /// </summary>
        public bool IsKeyword { get; init; }

        /// <summary>
        ///     Gets or sets the Label value used by generated documentation.
        /// </summary>
        public string Label { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the NamespaceName value used by generated documentation.
        /// </summary>
        public string NamespaceName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the ObjectName value used by generated documentation.
        /// </summary>
        public string ObjectName { get; init; } = string.Empty;

        #endregion
    }
}