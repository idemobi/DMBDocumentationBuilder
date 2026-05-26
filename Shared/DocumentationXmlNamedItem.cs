#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationXmlNamedItem.cs create at 2026/04/12 12:04:31
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents the DocumentationXmlNamedItem type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationXmlNamedItem
    {
        #region Instance fields and properties

        /// <summary>
        /// Gets or sets the Html value used by generated documentation.
        /// </summary>
        public string Html { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the Name value used by generated documentation.
        /// </summary>
        public string Name { get; init; } = string.Empty;

        #endregion
    }
}