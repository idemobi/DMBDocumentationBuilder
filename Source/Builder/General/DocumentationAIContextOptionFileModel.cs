#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents one AI context option file extracted from an <c>AIContextOptions</c> directory.
    /// </summary>
    public sealed class DocumentationAIContextOptionFileModel
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the copyable context text exposed by the option.
        /// </summary>
        public string ContextText { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the option description displayed by DocumentationViewer.
        /// </summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the JSON file path that defined this option.
        /// </summary>
        public string FilePath { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the project styles for which the option is relevant.
        /// </summary>
        public IReadOnlyList<string> ProjectStyles { get; init; } = [];

        /// <summary>
        ///     Gets the stable rule name. By default this is the JSON file name without extension.
        /// </summary>
        public string RuleName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the scenario name associated with the option.
        /// </summary>
        public string ScenarioName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the display order within the module and version.
        /// </summary>
        public int SortOrder { get; init; }

        /// <summary>
        ///     Gets the option tags used for filtering or display.
        /// </summary>
        public IReadOnlyList<string> Tags { get; init; } = [];

        /// <summary>
        ///     Gets the option title displayed by DocumentationViewer.
        /// </summary>
        public string Title { get; init; } = string.Empty;

        #endregion
    }
}
