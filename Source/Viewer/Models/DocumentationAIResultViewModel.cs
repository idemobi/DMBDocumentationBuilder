#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Represents AI-generated documentation content displayed next to a rendered documentation page.
    /// </summary>
    public sealed class DocumentationAIResultViewModel
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the display name of the AI model that produced the content.
        /// </summary>
        public string AIModel { get; init; } = string.Empty;

        /// <summary>
        ///     Gets a value indicating whether the result contains any summary, short summary, or keywords.
        /// </summary>
        public bool HasContent =>
            !string.IsNullOrWhiteSpace(Summary) ||
            !string.IsNullOrWhiteSpace(ShortSummary) ||
            !string.IsNullOrWhiteSpace(Keywords);

        /// <summary>
        ///     Gets the AI-generated keywords associated with the documentation object.
        /// </summary>
        public string Keywords { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the raw model identifier associated with the AI result.
        /// </summary>
        public string Model { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the provider identifier associated with the AI result.
        /// </summary>
        public string Provider { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the short AI-generated summary for compact display.
        /// </summary>
        public string ShortSummary { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the full AI-generated summary for the documentation object.
        /// </summary>
        public string Summary { get; init; } = string.Empty;

        #endregion
    }
}