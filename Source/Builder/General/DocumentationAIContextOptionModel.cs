#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents the AI context options extracted for one documented package version.
    /// </summary>
    public sealed class DocumentationAIContextOptionModel
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the extracted AI context option files.
        /// </summary>
        public List<DocumentationAIContextOptionFileModel> Files { get; } = [];

        /// <summary>
        ///     Gets the documentation group name associated with the extracted options.
        /// </summary>
        public string GroupName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the package identifier associated with the extracted options.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the documented project directory path.
        /// </summary>
        public string ProjectDirectoryPath { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the documented project file path.
        /// </summary>
        public string ProjectFilePath { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the package version associated with the extracted options.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        #endregion
    }
}
