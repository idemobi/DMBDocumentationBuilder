#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents the DocumentationProjectContextModel type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationProjectContextModel
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the Files value used by generated documentation.
        /// </summary>
        public List<DocumentationProjectContextFileModel> Files { get; } = [];

        /// <summary>
        ///     Gets or sets the PackageId value used by generated documentation.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the ProjectDirectoryPath value used by generated documentation.
        /// </summary>
        public string ProjectDirectoryPath { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the ProjectFilePath value used by generated documentation.
        /// </summary>
        public string ProjectFilePath { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the Version value used by generated documentation.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        #endregion
    }
}