#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationMarkdownContentItem.cs create at 2026/05/18 22:05:00
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents one Markdown file rendered as DocumentationViewer content.
    /// </summary>
    public sealed class DocumentationMarkdownContentItem
    {
        #region Instance fields and properties

        /// <summary>
        /// Gets the display folder for second-level Markdown content.
        /// </summary>
        public string FolderTitle { get; init; } = string.Empty;

        /// <summary>
        /// Gets the sidebar icon attached to the Markdown section.
        /// </summary>
        public string Icon { get; init; } = string.Empty;

        /// <summary>
        /// Gets the documentation object type stored in SQLite.
        /// </summary>
        public string ObjectType { get; init; } = string.Empty;

        /// <summary>
        /// Gets the Markdown page slug.
        /// </summary>
        public string Slug { get; init; } = string.Empty;

        /// <summary>
        /// Gets the Markdown source file path.
        /// </summary>
        public string SourceFilePath { get; init; } = string.Empty;

        /// <summary>
        /// Gets the sidebar section title.
        /// </summary>
        public string SectionTitle { get; init; } = string.Empty;

        /// <summary>
        /// Gets the rendered Markdown page title.
        /// </summary>
        public string Title { get; init; } = string.Empty;

        #endregion
    }
}
