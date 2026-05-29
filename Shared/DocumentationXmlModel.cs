#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents the DocumentationXmlModel type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationXmlModel
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the ExampleHtml value used by generated documentation.
        /// </summary>
        public string ExampleHtml { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the Exceptions value used by generated documentation.
        /// </summary>
        public List<DocumentationXmlNamedItem> Exceptions { get; } = [];

        /// <summary>
        ///     Gets a value indicating whether the XML documentation fragment is available.
        /// </summary>
        public bool HasExample => !string.IsNullOrWhiteSpace(ExampleHtml);

        /// <summary>
        ///     Gets or sets the HasExceptions value used by generated documentation.
        /// </summary>
        public bool HasExceptions => Exceptions.Count > 0;

        /// <summary>
        ///     Gets or sets the HasParameters value used by generated documentation.
        /// </summary>
        public bool HasParameters => Parameters.Count > 0;

        /// <summary>
        ///     Gets a value indicating whether the XML documentation fragment is available.
        /// </summary>
        public bool HasRemarks => !string.IsNullOrWhiteSpace(RemarksHtml);

        /// <summary>
        ///     Gets a value indicating whether the XML documentation fragment is available.
        /// </summary>
        public bool HasReturns => !string.IsNullOrWhiteSpace(ReturnsHtml);

        /// <summary>
        ///     Gets or sets the HasSeeAlsos value used by generated documentation.
        /// </summary>
        public bool HasSeeAlsos => SeeAlsos.Count > 0;

        /// <summary>
        ///     Gets a value indicating whether the XML documentation fragment is available.
        /// </summary>
        public bool HasSummary => !string.IsNullOrWhiteSpace(SummaryHtml);

        /// <summary>
        ///     Gets or sets the HasTypeParameters value used by generated documentation.
        /// </summary>
        public bool HasTypeParameters => TypeParameters.Count > 0;

        /// <summary>
        ///     Gets a value indicating whether the XML documentation fragment is available.
        /// </summary>
        public bool HasValue => !string.IsNullOrWhiteSpace(ValueHtml);

        /// <summary>
        ///     Gets or sets the Parameters value used by generated documentation.
        /// </summary>
        public List<DocumentationXmlNamedItem> Parameters { get; } = [];

        /// <summary>
        ///     Gets or sets the RemarksHtml value used by generated documentation.
        /// </summary>
        public string RemarksHtml { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the ReturnsHtml value used by generated documentation.
        /// </summary>
        public string ReturnsHtml { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the SeeAlsos value used by generated documentation.
        /// </summary>
        public List<DocumentationXmlLinkItem> SeeAlsos { get; } = [];

        /// <summary>
        ///     Gets or sets the SummaryHtml value used by generated documentation.
        /// </summary>
        public string SummaryHtml { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the TypeParameters value used by generated documentation.
        /// </summary>
        public List<DocumentationXmlNamedItem> TypeParameters { get; } = [];

        /// <summary>
        ///     Gets or sets the ValueHtml value used by generated documentation.
        /// </summary>
        public string ValueHtml { get; init; } = string.Empty;

        #endregion
    }
}