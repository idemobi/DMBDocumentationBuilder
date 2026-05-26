#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationProjectDescriptor.cs create at 2026/04/12 12:04:31
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Describes one C# project that DocumentationBuilder should compile and document.
    /// </summary>
    public sealed class DocumentationProjectDescriptor
    {
        #region Instance fields and properties

        /// <summary>
        /// Gets or sets the compile item inclusion mode used when reading the project file.
        /// </summary>
        public DocumentationCompileItemMode CompileItemMode { get; set; } = DocumentationCompileItemMode.LocalProjectFilesOnly;

        /// <summary>
        /// Gets the display name shown in generated group, namespace, and sidebar documentation.
        /// </summary>
        public required string DisplayName { get; init; } = string.Empty;
        /// <summary>
        /// Gets the Markdown content folders rendered into DocumentationViewer for this project.
        /// </summary>
        public List<DocumentationMarkdownContentDescriptor> MarkdownContents { get; } = new();
        /// <summary>
        /// Gets the OpenAPI documents rendered into DocumentationViewer for this project.
        /// </summary>
        public List<DocumentationOpenApiDescriptor> OpenApiDocuments { get; } = new();
        /// <summary>
        /// Gets or sets the package identifier stored in generated links and SQLite metadata.
        /// </summary>
        public string PackageId { get; set; } = string.Empty;
        /// <summary>
        /// Gets the path to the `.csproj` file that should be compiled and documented.
        /// </summary>
        public required string ProjectFilePath { get; init; }
        /// <summary>
        /// Gets or sets how project references are included when this project is processed.
        /// </summary>
        public DocumentationProjectReferenceMode ProjectReferenceMode { get; set; } = DocumentationProjectReferenceMode.None;
        /// <summary>
        /// Gets or sets the package version shown in generated pages and metadata.
        /// </summary>
        public string Version { get; set; } = string.Empty;

        #endregion
    }
}
