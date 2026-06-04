#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents the DocumentationXmlNamedItem type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationXmlNamedItem
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the Html value used by generated documentation.
        /// </summary>
        public string Html { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the Name value used by generated documentation.
        /// </summary>
        public string Name { get; init; } = string.Empty;

        #endregion
    }
}