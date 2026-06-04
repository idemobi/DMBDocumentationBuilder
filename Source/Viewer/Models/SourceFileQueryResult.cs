#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Represents one captured C# source file available to DocumentationViewer MCP tools.
    /// </summary>
    public sealed class SourceFileQueryResult
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the captured source file content.
        /// </summary>
        public string Content { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the SHA-256 hash of the captured source file content.
        /// </summary>
        public string ContentHash { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the captured source file name.
        /// </summary>
        public string FileName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the source file namespace metadata serialized as JSON.
        /// </summary>
        public string NamespaceNamesJson { get; init; } = "[]";

        /// <summary>
        ///     Gets the package identifier that owns the source file snapshot.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the primary namespace declared in the source file.
        /// </summary>
        public string PrimaryNamespaceName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the source path relative to the documented project directory.
        /// </summary>
        public string RelativeFilePath { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the source file type metadata serialized as JSON.
        /// </summary>
        public string TypeNamesJson { get; init; } = "[]";

        /// <summary>
        ///     Gets the package version that owns the source file snapshot.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        #endregion
    }
}