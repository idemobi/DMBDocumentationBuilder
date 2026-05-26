#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationClassFieldItem.cs create at 2026/04/12 12:04:31
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents the DocumentationClassFieldItem type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationClassFieldItem
    {
        #region Instance fields and properties

        /// <summary>
        /// Gets or sets the Accessibility value used by generated documentation.
        /// </summary>
        public string Accessibility { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the ExampleHtml value used by generated documentation.
        /// </summary>
        public string ExampleHtml { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the FieldName value used by generated documentation.
        /// </summary>
        public string FieldName { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the IsConst value used by generated documentation.
        /// </summary>
        public bool IsConst { get; init; }
        /// <summary>
        /// Gets or sets the IsObsolete value used by generated documentation.
        /// </summary>
        public bool IsObsolete { get; init; }
        /// <summary>
        /// Gets or sets the IsReadOnly value used by generated documentation.
        /// </summary>
        public bool IsReadOnly { get; init; }
        /// <summary>
        /// Gets or sets the IsStatic value used by generated documentation.
        /// </summary>
        public bool IsStatic { get; init; }
        /// <summary>
        /// Gets or sets the ObsoleteMessage value used by generated documentation.
        /// </summary>
        public string ObsoleteMessage { get; init; } = string.Empty;
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