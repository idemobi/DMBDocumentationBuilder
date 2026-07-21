#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Represents one reusable context option exposed by a documentation module for human reading and copy workflows.
    /// </summary>
    public sealed class DocumentationContextOption
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the context text that can be copied into an AI context pack, prompt, or documentation file.
        /// </summary>
        public string ContextText { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the short human-readable description displayed before the copyable context text.
        /// </summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the JSON file path that defined this option.
        /// </summary>
        public string FilePath { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the documentation group that owns this option.
        /// </summary>
        public string GroupName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the documented package identifier that owns this option.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the project styles for which this option is relevant.
        /// </summary>
        public IReadOnlyList<string> ProjectStyles { get; init; } = [];

        /// <summary>
        ///     Gets the stable rule name stored by DocumentationBuilder.
        /// </summary>
        public string RuleName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the scenario name for which this option is intended.
        /// </summary>
        public string ScenarioName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the display order inside the context option page.
        /// </summary>
        public int SortOrder { get; init; }

        /// <summary>
        ///     Gets additional search or grouping tags for the option.
        /// </summary>
        public IReadOnlyList<string> Tags { get; init; } = [];

        /// <summary>
        ///     Gets the human-readable option title.
        /// </summary>
        public string Title { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the documented package version that owns this option.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        #endregion
    }
}
