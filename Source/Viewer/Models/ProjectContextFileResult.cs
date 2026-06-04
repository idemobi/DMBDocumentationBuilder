#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Represents a project-context file stored in the generated documentation database.
    /// </summary>
    public sealed class ProjectContextFileResult
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the stored text content of the project-context file.
        /// </summary>
        public string Content { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the semantic context type, such as rules, architecture, project, glossary, or readme.
        /// </summary>
        public string ContextType { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the directory depth used to order context files from broad to specific.
        /// </summary>
        public int DirectoryDepth { get; init; }

        /// <summary>
        ///     Gets the file name of the stored context file.
        /// </summary>
        public string FileName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the original project-relative file path.
        /// </summary>
        public string FilePath { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the source folder family that contributed the context file.
        /// </summary>
        public string SourceFolderType { get; init; } = string.Empty;

        #endregion
    }
}