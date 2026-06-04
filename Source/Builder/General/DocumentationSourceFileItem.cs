#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

using System.Collections.Generic;

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents one C# source file snapshot stored for DocumentationViewer MCP access.
    /// </summary>
    public sealed class DocumentationSourceFileItem
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the full source file content captured during documentation generation.
        /// </summary>
        public string Content { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the SHA-256 hash of the captured source content.
        /// </summary>
        public string ContentHash { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the source file name.
        /// </summary>
        public string FileName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the absolute source file path used during generation.
        /// </summary>
        public string FilePath { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the namespace names declared in the source file.
        /// </summary>
        public IReadOnlyList<string> NamespaceNames { get; init; } = [];

        /// <summary>
        ///     Gets the package identifier that owns the source file.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the primary namespace declared in the source file.
        /// </summary>
        public string PrimaryNamespaceName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the absolute project directory path used during generation.
        /// </summary>
        public string ProjectDirectoryPath { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the absolute project file path used during generation.
        /// </summary>
        public string ProjectFilePath { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the source path relative to the project directory when possible.
        /// </summary>
        public string RelativeFilePath { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the type names declared in the source file.
        /// </summary>
        public IReadOnlyList<string> TypeNames { get; init; } = [];

        /// <summary>
        ///     Gets the package version that owns the source file.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        #endregion
    }
}