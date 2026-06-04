#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents the DocumentationProjectContextFileModel type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationProjectContextFileModel
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the Content value used by generated documentation.
        /// </summary>
        public string Content { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the ContextType value used by generated documentation.
        /// </summary>
        public string ContextType { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the DirectoryDepth value used by generated documentation.
        /// </summary>
        public int DirectoryDepth { get; init; }

        /// <summary>
        ///     Gets or sets the FileName value used by generated documentation.
        /// </summary>
        public string FileName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the FilePath value used by generated documentation.
        /// </summary>
        public string FilePath { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the SourceFolderType value used by generated documentation.
        /// </summary>
        public string SourceFolderType { get; init; } = string.Empty;

        #endregion
    }
}