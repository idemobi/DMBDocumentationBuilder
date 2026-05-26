#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationEnumValueItem.cs create at 2026/04/10 11:04:41
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents the DocumentationEnumValueItem type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationEnumValueItem
    {
        #region Instance fields and properties

        /// <summary>
        /// Gets or sets the BinaryValue value used by generated documentation.
        /// </summary>
        public string BinaryValue { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the BitShiftValue value used by generated documentation.
        /// </summary>
        public string BitShiftValue { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the HexValue value used by generated documentation.
        /// </summary>
        public string HexValue { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the IsObsolete value used by generated documentation.
        /// </summary>
        public bool IsObsolete { get; init; }
        /// <summary>
        /// Gets or sets the Name value used by generated documentation.
        /// </summary>
        public string Name { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the NumericValue value used by generated documentation.
        /// </summary>
        public long NumericValue { get; init; }
        /// <summary>
        /// Gets or sets the ObsoleteMessage value used by generated documentation.
        /// </summary>
        public string ObsoleteMessage { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the Value value used by generated documentation.
        /// </summary>
        public string Value { get; init; } = string.Empty;
        /// <summary>
        /// Gets the collection used by DocumentationBuilder generation.
        /// </summary>
        public DocumentationXmlModel XmlDoc { get; init; } = new();

        #endregion
    }
}