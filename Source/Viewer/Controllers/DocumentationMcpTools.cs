#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.ComponentModel;
using Microsoft.AspNetCore.Hosting;
using ModelContextProtocol.Server;

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Exposes documentation lookup, search, source-code, and project-context tools through MCP.
    /// </summary>
    /// <remarks>
    ///     The tools read from the generated documentation database located under the host application's
    ///     `Documentation/data.db` path.
    /// </remarks>
    [McpServerToolType]
    public sealed class DocumentationMcpTools
    {
        #region Instance fields and properties

        private readonly DocumentationQueryService _documentationQueryService;
        private readonly OpenApiQueryService _openApiQueryService;
        private readonly ProjectContextQueryService _projectContextQueryService;
        private readonly SourceCodeQueryService _sourceCodeQueryService;
        private readonly SourceFileQueryService _sourceFileQueryService;

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DocumentationMcpTools" /> class.
        /// </summary>
        /// <param name="environment">Host environment used to resolve the generated documentation database path.</param>
        public DocumentationMcpTools(IWebHostEnvironment environment)
        {
            string dbPath = Path.Combine(environment.ContentRootPath, "Documentation", "data.db");

            _documentationQueryService = new DocumentationQueryService(dbPath);
            _openApiQueryService = new OpenApiQueryService(dbPath);
            _projectContextQueryService = new ProjectContextQueryService(dbPath);
            _sourceCodeQueryService = new SourceCodeQueryService(dbPath);
            _sourceFileQueryService = new SourceFileQueryService(dbPath);
        }

        #endregion

        #region Instance methods

        /// <summary>
        ///     Builds a coding-oriented source context bundle for refactoring, deduplication, and implementation assistance.
        /// </summary>
        /// <param name="query">Search text used to find related source files.</param>
        /// <param name="packageId">Package identifier whose generated source snapshots should be searched.</param>
        /// <param name="version">Package version whose generated source snapshots should be searched.</param>
        /// <returns>An MCP-friendly bundle containing relevant captured C# source files and project context.</returns>
        [McpServerTool, Description("Build a coding context bundle from captured C# source files and project AI context for refactoring or implementation assistance.")]
        public string BuildCodingContext(string query, string packageId, string version)
        {
            return string.Join(
                "\n\n",
                [
                    "=== SOURCE FILES ===",
                    _sourceFileQueryService.BuildCodingContext(query, packageId, version),
                    "=== PROJECT CONTEXT ===",
                    GetProjectContext(packageId, version)
                ]);
        }

        /// <summary>
        ///     Builds a combined documentation, source-code, and project-context bundle for one documented object.
        /// </summary>
        /// <param name="objectName">Exact documented object name.</param>
        /// <param name="packageId">Package identifier that owns the documented object.</param>
        /// <param name="version">Package version that owns the documented object.</param>
        /// <param name="namespaceName">Namespace that contains the documented object.</param>
        /// <param name="objectType">Documented object type.</param>
        /// <returns>An MCP-friendly text bundle containing documentation, source code, and project context.</returns>
        [McpServerTool, Description("Build a combined context bundle for one object using documentation, source code, and project AI context.")]
        public string BuildContextBundle(string objectName, string packageId, string version, string namespaceName, string objectType)
        {
            return string.Join(
                "\n\n",
                [
                    "=== DOCUMENTATION ===",
                    GetDocumentation(objectName, packageId, version, namespaceName, objectType),
                    "=== SOURCE CODE ===",
                    GetSourceCode(objectName, packageId, version, namespaceName, objectType),
                    "=== PROJECT CONTEXT ===",
                    GetProjectContext(packageId, version)
                ]);
        }

        /// <summary>
        ///     Finds documentation objects related to a documented object name.
        /// </summary>
        /// <param name="objectName">Object name used as the related-object search token.</param>
        /// <param name="packageId">Optional package identifier filter.</param>
        /// <param name="version">Optional package version filter.</param>
        /// <returns>An MCP-friendly related-object listing.</returns>
        [McpServerTool, Description("Find objects related to one documented object.")]
        public string FindRelatedObjects(
            [Description("Object name, for example DocumentationController")]
            string objectName,
            [Description("Optional package id filter")]
            string? packageId = null,
            [Description("Optional version filter")]
            string? version = null
        )
        {
            IReadOnlyList<DocumentationSearchResultItem> results =
                _documentationQueryService.FindRelatedObjects(objectName, packageId, version);

            return DocumentationMcpTextFormatter.FormatRelatedObjects(objectName, results);
        }

        /// <summary>
        ///     Gets one indexed REST API operation from an imported OpenAPI document.
        /// </summary>
        /// <param name="packageId">Package identifier that owns the OpenAPI document.</param>
        /// <param name="version">Package version that owns the OpenAPI document.</param>
        /// <param name="documentName">Stable OpenAPI document name.</param>
        /// <param name="operationId">Stable OpenAPI operation identifier.</param>
        /// <returns>An MCP-friendly REST operation contract block.</returns>
        [McpServerTool, Description("Get one REST API operation contract from an imported OpenAPI document.")]
        public string GetApiOperation(string packageId, string version, string documentName, string operationId)
        {
            return _openApiQueryService.GetApiOperation(packageId, version, documentName, operationId);
        }

        /// <summary>
        ///     Gets one generated documentation page as metadata and HTML content.
        /// </summary>
        /// <param name="objectName">Exact documented object name.</param>
        /// <param name="packageId">Optional package identifier filter.</param>
        /// <param name="version">Optional package version filter.</param>
        /// <param name="namespaceName">Optional namespace filter.</param>
        /// <param name="objectType">Optional documented object type filter.</param>
        /// <returns>An MCP-friendly documentation block, or a not-found message.</returns>
        [McpServerTool, Description("Get one documentation page rendered as HTML plus metadata.")]
        public string GetDocumentation(string objectName, string? packageId = null, string? version = null, string? namespaceName = null, string? objectType = null)
        {
            var result = _documentationQueryService.GetDocumentation(objectName, packageId, version, namespaceName, objectType);
            return DocumentationMcpTextFormatter.FormatDocumentation(result);
        }

        /// <summary>
        ///     Gets the raw imported OpenAPI document JSON.
        /// </summary>
        /// <param name="packageId">Package identifier that owns the OpenAPI document.</param>
        /// <param name="version">Package version that owns the OpenAPI document.</param>
        /// <param name="documentName">Stable OpenAPI document name.</param>
        /// <returns>The raw OpenAPI JSON, or a not-found message.</returns>
        [McpServerTool, Description("Get the raw imported OAS3/OpenAPI JSON document for a documented package.")]
        public string GetOpenApiDocument(string packageId, string version, string documentName)
        {
            return _openApiQueryService.GetOpenApiDocument(packageId, version, documentName);
        }

        /// <summary>
        ///     Gets all project-context files for a package version as one merged text block.
        /// </summary>
        /// <param name="packageId">Package identifier whose project context should be loaded.</param>
        /// <param name="version">Package version whose project context should be loaded.</param>
        /// <returns>An MCP-friendly project-context bundle.</returns>
        [McpServerTool, Description("Get merged AI/project context files for one package and version.")]
        public string GetProjectContext(string packageId, string version)
        {
            return _projectContextQueryService.GetProjectContext(packageId, version);
        }

        /// <summary>
        ///     Gets the stored source-code snapshot for one documented object.
        /// </summary>
        /// <param name="objectName">Exact documented object name.</param>
        /// <param name="packageId">Package identifier that owns the documented object.</param>
        /// <param name="version">Package version that owns the documented object.</param>
        /// <param name="namespaceName">Namespace that contains the documented object.</param>
        /// <param name="objectType">Documented object type.</param>
        /// <returns>An MCP-friendly source-code block, or a not-found message.</returns>
        [McpServerTool, Description("Get the full stored source code snapshot for one documented object.")]
        public string GetSourceCode(string objectName, string packageId, string version, string namespaceName, string objectType)
        {
            return _sourceCodeQueryService.GetSourceCode(objectName, packageId, version, namespaceName, objectType);
        }

        /// <summary>
        ///     Gets one captured C# source file by relative path.
        /// </summary>
        /// <param name="packageId">Package identifier that owns the captured source file.</param>
        /// <param name="version">Package version that owns the captured source file.</param>
        /// <param name="relativeFilePath">Source file path relative to the documented project directory.</param>
        /// <returns>An MCP-friendly source file block, or a not-found message.</returns>
        [McpServerTool, Description("Get one captured C# source file by project-relative path.")]
        public string GetSourceFile(string packageId, string version, string relativeFilePath)
        {
            return _sourceFileQueryService.GetSourceFile(packageId, version, relativeFilePath);
        }

        /// <summary>
        ///     Lists indexed REST API operations from one imported OpenAPI document.
        /// </summary>
        /// <param name="packageId">Package identifier that owns the OpenAPI document.</param>
        /// <param name="version">Package version that owns the OpenAPI document.</param>
        /// <param name="documentName">Stable OpenAPI document name.</param>
        /// <param name="tag">Optional OpenAPI tag filter.</param>
        /// <returns>An MCP-friendly REST operation listing.</returns>
        [McpServerTool, Description("List REST API operations from an imported OpenAPI document, optionally filtered by tag.")]
        public string ListApiOperations(string packageId, string version, string documentName, string? tag = null)
        {
            return _openApiQueryService.ListApiOperations(packageId, version, documentName, tag);
        }

        /// <summary>
        ///     Lists generated documentation objects inside one namespace.
        /// </summary>
        /// <param name="namespaceName">Exact C# namespace name.</param>
        /// <param name="packageId">Optional package identifier filter.</param>
        /// <param name="version">Optional package version filter.</param>
        /// <returns>An MCP-friendly namespace object listing.</returns>
        [McpServerTool, Description("Use this tool when the user asks to list classes, enums, interfaces, records, structs, or documented objects inside a C# namespace.")]
        public string ListNamespaceObjects(
            [Description("Exact C# namespace name, for example DMBPageBuilder")]
            string namespaceName,
            [Description("Optional package id filter if several packages contain the same namespace")]
            string? packageId = null,
            [Description("Optional version filter")]
            string? version = null
        )
        {
            IReadOnlyList<DocumentationSearchResultItem> results =
                _documentationQueryService.ListNamespaceObjects(namespaceName, packageId, version);
            return DocumentationMcpTextFormatter.FormatNamespaceObjects(namespaceName, results);
        }

        /// <summary>
        ///     Lists project-context files available for one package version.
        /// </summary>
        /// <param name="packageId">Package identifier whose project context files should be listed.</param>
        /// <param name="version">Package version whose project context files should be listed.</param>
        /// <returns>An MCP-friendly list of project-context files.</returns>
        [McpServerTool, Description("Use this tool only when the user explicitly asks for AI context files, project rules files, architecture files, or .ai/.aiassistant files for a package and version.")]
        public string ListProjectContextFiles(string packageId, string version)
        {
            return _projectContextQueryService.ListProjectContextFiles(packageId, version);
        }

        /// <summary>
        ///     Lists captured C# source files for one package version.
        /// </summary>
        /// <param name="packageId">Package identifier whose source files should be listed.</param>
        /// <param name="version">Package version whose source files should be listed.</param>
        /// <param name="namespaceName">Optional namespace filter.</param>
        /// <returns>An MCP-friendly list of captured source files.</returns>
        [McpServerTool, Description("List captured C# source files for one package version, optionally filtered by namespace.")]
        public string ListSourceFiles(string packageId, string version, string? namespaceName = null)
        {
            return _sourceFileQueryService.ListSourceFiles(packageId, version, namespaceName);
        }

        /// <summary>
        ///     Searches indexed REST API operations imported from OpenAPI documents.
        /// </summary>
        /// <param name="query">Search text.</param>
        /// <param name="packageId">Optional package identifier filter.</param>
        /// <param name="version">Optional package version filter.</param>
        /// <returns>An MCP-friendly REST operation search result listing.</returns>
        [McpServerTool, Description("Search REST API operations by path, method, tag, summary, description, or operation id.")]
        public string SearchApiOperations(string query, string? packageId = null, string? version = null)
        {
            return _openApiQueryService.SearchApiOperations(query, packageId, version);
        }

        /// <summary>
        ///     Searches generated documentation by technical keywords, text keywords, or object name.
        /// </summary>
        /// <param name="query">Search text.</param>
        /// <param name="packageId">Optional package identifier filter.</param>
        /// <param name="version">Optional package version filter.</param>
        /// <returns>An MCP-friendly search result listing.</returns>
        [McpServerTool, Description("Search documentation by technical or text keywords.")]
        public string SearchDocumentation(string query, string? packageId = null, string? version = null)
        {
            var results = _documentationQueryService.SearchDocumentation(query, packageId, version);
            return DocumentationMcpTextFormatter.FormatDocumentationSearch(query, results);
        }

        /// <summary>
        ///     Searches project-context files by text content for one package version.
        /// </summary>
        /// <param name="query">Search text.</param>
        /// <param name="packageId">Package identifier whose context files should be searched.</param>
        /// <param name="version">Package version whose context files should be searched.</param>
        /// <returns>An MCP-friendly list of context file excerpts.</returns>
        [McpServerTool, Description("Search project AI/context files by text content.")]
        public string SearchProjectContext(string query, string packageId, string version)
        {
            return _projectContextQueryService.SearchProjectContext(query, packageId, version);
        }

        /// <summary>
        ///     Searches captured C# source files by path, type metadata, namespace metadata, or content.
        /// </summary>
        /// <param name="query">Search text.</param>
        /// <param name="packageId">Package identifier whose source files should be searched.</param>
        /// <param name="version">Optional package version filter.</param>
        /// <returns>An MCP-friendly source-code search result listing.</returns>
        [McpServerTool, Description("Search captured C# source files by path, type metadata, namespace metadata, or content.")]
        public string SearchSourceCode(string query, string packageId, string? version = null)
        {
            return _sourceFileQueryService.SearchSourceCode(query, packageId, version);
        }

        #endregion
    }
}