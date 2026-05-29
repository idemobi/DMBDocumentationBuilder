#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents one OpenAPI operation indexed for DocumentationViewer rendering and MCP lookup.
    /// </summary>
    public sealed class DocumentationOpenApiOperationItem
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the OpenAPI operation description.
        /// </summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the OpenAPI document name that owns the operation.
        /// </summary>
        public string DocumentName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the HTTP method.
        /// </summary>
        public string HttpMethod { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the stable operation identifier.
        /// </summary>
        public string OperationId { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the package identifier that owns the operation.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the OpenAPI parameter array as JSON.
        /// </summary>
        public string ParametersJson { get; init; } = "[]";

        /// <summary>
        ///     Gets the API path template.
        /// </summary>
        public string Path { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the OpenAPI request body object as JSON.
        /// </summary>
        public string RequestBodyJson { get; init; } = "{}";

        /// <summary>
        ///     Gets the OpenAPI response object as JSON.
        /// </summary>
        public string ResponsesJson { get; init; } = "{}";

        /// <summary>
        ///     Gets the route path used to render the operation.
        /// </summary>
        public string RoutePath { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the OpenAPI security requirements as JSON.
        /// </summary>
        public string SecurityJson { get; init; } = "[]";

        /// <summary>
        ///     Gets the operation summary.
        /// </summary>
        public string Summary { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the operation tags as JSON.
        /// </summary>
        public string TagsJson { get; init; } = "[]";

        /// <summary>
        ///     Gets the package version that owns the operation.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        #endregion
    }
}