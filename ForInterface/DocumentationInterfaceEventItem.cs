#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationInterfaceEventItem.cs create at 2026/04/12 12:04:31
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents the DocumentationInterfaceEventItem type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationInterfaceEventItem
    {
        #region Instance fields and properties

        /// <summary>
        /// Gets or sets the Accessibility value used by generated documentation.
        /// </summary>
        public string Accessibility { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the EventName value used by generated documentation.
        /// </summary>
        public string EventName { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the IsObsolete value used by generated documentation.
        /// </summary>
        public bool IsObsolete { get; init; }
        /// <summary>
        /// Gets or sets the IsStatic value used by generated documentation.
        /// </summary>
        public bool IsStatic { get; init; }
        /// <summary>
        /// Gets or sets the ObsoleteMessage value used by generated documentation.
        /// </summary>
        public string ObsoleteMessage { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the Signature value used by generated documentation.
        /// </summary>
        public string Signature { get; init; } = string.Empty;
        /// <summary>
        /// Gets the collection used by DocumentationBuilder generation.
        /// </summary>
        public DocumentationXmlModel XmlDoc { get; init; } = new();

        #endregion
    }
}