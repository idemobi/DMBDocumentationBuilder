#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationProjectContextModel.cs create at 2026/04/20 18:04:12
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents the DocumentationProjectContextModel type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationProjectContextModel
    {
        #region Instance fields and properties
        /// <summary>
        /// Gets or sets the Files value used by generated documentation.
        /// </summary>
        public List<DocumentationProjectContextFileModel> Files { get; } = [];
        /// <summary>
        /// Gets or sets the PackageId value used by generated documentation.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the ProjectDirectoryPath value used by generated documentation.
        /// </summary>
        public string ProjectDirectoryPath { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the ProjectFilePath value used by generated documentation.
        /// </summary>
        public string ProjectFilePath { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the Version value used by generated documentation.
        /// </summary>
        public string Version { get; init; } = string.Empty;
        #endregion
    }
}