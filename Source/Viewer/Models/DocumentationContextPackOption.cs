#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Represents one latest-version context option that can be selected for a context pack export.
    /// </summary>
    public sealed class DocumentationContextPackOption
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the primary category used by the context pack builder.
        /// </summary>
        public string Category { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the context text exported when this option is selected.
        /// </summary>
        public string ContextText { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the short display description.
        /// </summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the stable fingerprint for this exact context option content.
        /// </summary>
        public string Fingerprint { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the documentation group that owns this option.
        /// </summary>
        public string GroupName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the package identifier that owns this option.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the project styles for which this option is relevant.
        /// </summary>
        public IReadOnlyList<string> ProjectStyles { get; init; } = [];

        /// <summary>
        ///     Gets the source rule name.
        /// </summary>
        public string RuleName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the display sort order.
        /// </summary>
        public int SortOrder { get; init; }

        /// <summary>
        ///     Gets the secondary category used by the context pack builder.
        /// </summary>
        public string SubCategory { get; init; } = string.Empty;

        /// <summary>
        ///     Gets additional display tags.
        /// </summary>
        public IReadOnlyList<string> Tags { get; init; } = [];

        /// <summary>
        ///     Gets the display title.
        /// </summary>
        public string Title { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the latest documented package version represented by this option.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        #endregion
    }
}
