#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationProjectContextFileModel.cs create at 2026/04/20 18:04:18
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents the DocumentationProjectContextFileModel type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationProjectContextFileModel
    {
        #region Instance fields and properties
        /// <summary>
        /// Gets or sets the Content value used by generated documentation.
        /// </summary>
        public string Content { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the ContextType value used by generated documentation.
        /// </summary>
        public string ContextType { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the DirectoryDepth value used by generated documentation.
        /// </summary>
        public int DirectoryDepth { get; init; }
        /// <summary>
        /// Gets or sets the FileName value used by generated documentation.
        /// </summary>
        public string FileName { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the FilePath value used by generated documentation.
        /// </summary>
        public string FilePath { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the SourceFolderType value used by generated documentation.
        /// </summary>
        public string SourceFolderType { get; init; } = string.Empty;
        #endregion
    }
}