#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Describes one Markdown content folder that DocumentationBuilder should render into DocumentationViewer.
    /// </summary>
    public sealed class DocumentationMarkdownContentDescriptor
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the Bootstrap icon used by the sidebar section.
        /// </summary>
        public string Icon { get; set; } = "bi-journal-text";

        /// <summary>
        ///     Gets or sets the documentation object type stored in SQLite.
        /// </summary>
        public string ObjectType { get; set; } = "MarkdownContent";

        /// <summary>
        ///     Gets or sets the root directory that contains Markdown files.
        /// </summary>
        public required string RootDirectoryPath { get; init; }

        /// <summary>
        ///     Gets or sets the sidebar and metadata section title.
        /// </summary>
        public string SectionTitle { get; set; } = "Content";

        #endregion
    }
}