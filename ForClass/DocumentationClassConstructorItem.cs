#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationClassConstructorItem.cs create at 2026/04/12 12:04:31
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    #region

    #endregion

    /// <summary>
    /// Represents the DocumentationClassConstructorItem type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationClassConstructorItem
    {
        #region Instance fields and properties

        /// <summary>
        /// Gets or sets the Accessibility value used by generated documentation.
        /// </summary>
        public string Accessibility { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the ConstructorName value used by generated documentation.
        /// </summary>
        public string ConstructorName { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the ExampleHtml value used by generated documentation.
        /// </summary>
        public string ExampleHtml { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the Exceptions value used by generated documentation.
        /// </summary>
        public List<DocumentationXmlNamedItem> Exceptions { get; } = [];
        /// <summary>
        /// Gets or sets the IsObsolete value used by generated documentation.
        /// </summary>
        public bool IsObsolete { get; init; }
        /// <summary>
        /// Gets or sets the ObsoleteMessage value used by generated documentation.
        /// </summary>
        public string ObsoleteMessage { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the Parameters value used by generated documentation.
        /// </summary>
        public List<DocumentationXmlNamedItem> Parameters { get; } = [];
        /// <summary>
        /// Gets or sets the RemarksHtml value used by generated documentation.
        /// </summary>
        public string RemarksHtml { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the Signature value used by generated documentation.
        /// </summary>
        public string Signature { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the SummaryHtml value used by generated documentation.
        /// </summary>
        public string SummaryHtml { get; init; } = string.Empty;

        #endregion
    }
}