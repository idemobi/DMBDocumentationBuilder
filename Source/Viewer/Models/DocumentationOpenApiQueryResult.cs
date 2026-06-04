#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Represents one OpenAPI document stored in the generated documentation database.
    /// </summary>
    public sealed class DocumentationOpenApiDocumentQueryResult
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the OpenAPI document description.
        /// </summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the stable document name.
        /// </summary>
        public string DocumentName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the raw OpenAPI JSON content.
        /// </summary>
        public string JsonContent { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the package identifier that owns the document.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the route path used to render the document.
        /// </summary>
        public string RoutePath { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the OpenAPI document title.
        /// </summary>
        public string Title { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the package version that owns the document.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        #endregion
    }

    /// <summary>
    ///     Represents one OpenAPI operation stored in the generated documentation database.
    /// </summary>
    public sealed class DocumentationOpenApiOperationQueryResult
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the operation description.
        /// </summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the document name that owns the operation.
        /// </summary>
        public string DocumentName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the HTTP method.
        /// </summary>
        public string HttpMethod { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the operation identifier.
        /// </summary>
        public string OperationId { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the package identifier that owns the operation.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the OpenAPI parameters array as JSON.
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
        ///     Gets the OpenAPI responses object as JSON.
        /// </summary>
        public string ResponsesJson { get; init; } = "{}";

        /// <summary>
        ///     Gets the route path used to render the operation.
        /// </summary>
        public string RoutePath { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the OpenAPI security requirements array as JSON.
        /// </summary>
        public string SecurityJson { get; init; } = "[]";

        /// <summary>
        ///     Gets the operation summary.
        /// </summary>
        public string Summary { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the OpenAPI tags array as JSON.
        /// </summary>
        public string TagsJson { get; init; } = "[]";

        /// <summary>
        ///     Gets the package version that owns the operation.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        #endregion
    }

    /// <summary>
    ///     Provides data rendered by the DocumentationViewer REST API page.
    /// </summary>
    public sealed class DocumentationOpenApiPageViewModel
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the OpenAPI document displayed by the page.
        /// </summary>
        public DocumentationOpenApiDocumentQueryResult Document { get; init; } = new();

        /// <summary>
        ///     Gets the selected documentation group name.
        /// </summary>
        public string GroupName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets a value indicating whether the page renders one selected operation.
        /// </summary>
        public bool HasSelectedOperation => SelectedOperation is not null;

        /// <summary>
        ///     Gets the indexed operations for the document.
        /// </summary>
        public IReadOnlyList<DocumentationOpenApiOperationQueryResult> Operations { get; init; } = [];

        /// <summary>
        ///     Gets the selected operation, or <see langword="null" /> when the overview is rendered.
        /// </summary>
        public DocumentationOpenApiOperationQueryResult? SelectedOperation { get; init; }

        /// <summary>
        ///     Gets the human-readable contract display model for the selected operation.
        /// </summary>
        public DocumentationOpenApiOperationDisplayModel SelectedOperationDisplay { get; init; } = new();

        /// <summary>
        ///     Gets the namespace sidebar context that should be preserved while browsing REST API pages.
        /// </summary>
        public string SidebarNamespaceName { get; init; } = string.Empty;

        #endregion
    }

    /// <summary>
    ///     Provides human-readable OpenAPI operation contract data for Razor rendering.
    /// </summary>
    public sealed class DocumentationOpenApiOperationDisplayModel
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets a value indicating whether the operation has indexed parameters.
        /// </summary>
        public bool HasParameters => Parameters.Count > 0;

        /// <summary>
        ///     Gets a value indicating whether the operation has an indexed request body.
        /// </summary>
        public bool HasRequestBody => RequestBody.ContentTypes.Count > 0;

        /// <summary>
        ///     Gets a value indicating whether the operation has indexed responses.
        /// </summary>
        public bool HasResponses => Responses.Count > 0;

        /// <summary>
        ///     Gets a value indicating whether the operation has declared security requirements.
        /// </summary>
        public bool HasSecurity => SecurityRequirements.Count > 0;

        /// <summary>
        ///     Gets the explicit header parameters declared by the operation.
        /// </summary>
        public IReadOnlyList<DocumentationOpenApiParameterDisplayItem> HeaderParameters =>
            Parameters
                .Where(parameter => string.Equals(parameter.Location, "header", StringComparison.OrdinalIgnoreCase))
                .ToArray();

        /// <summary>
        ///     Gets the non-header parameters declared by the operation.
        /// </summary>
        public IReadOnlyList<DocumentationOpenApiParameterDisplayItem> NonHeaderParameters =>
            Parameters
                .Where(parameter => !string.Equals(parameter.Location, "header", StringComparison.OrdinalIgnoreCase))
                .ToArray();

        /// <summary>
        ///     Gets the operation parameters.
        /// </summary>
        public IReadOnlyList<DocumentationOpenApiParameterDisplayItem> Parameters { get; init; } = [];

        /// <summary>
        ///     Gets the operation request body.
        /// </summary>
        public DocumentationOpenApiRequestBodyDisplayItem RequestBody { get; init; } = new();

        /// <summary>
        ///     Gets the operation responses.
        /// </summary>
        public IReadOnlyList<DocumentationOpenApiResponseDisplayItem> Responses { get; init; } = [];

        /// <summary>
        ///     Gets the operation security requirements.
        /// </summary>
        public IReadOnlyList<DocumentationOpenApiSecurityRequirementDisplayItem> SecurityRequirements { get; init; } = [];

        #endregion
    }

    /// <summary>
    ///     Represents one human-readable OpenAPI parameter.
    /// </summary>
    public sealed class DocumentationOpenApiParameterDisplayItem
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the default value summary.
        /// </summary>
        public string DefaultValue { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the parameter description.
        /// </summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the parameter format summary.
        /// </summary>
        public string Format { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the parameter location.
        /// </summary>
        public string Location { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the parameter name.
        /// </summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the parameter reference summary.
        /// </summary>
        public string Reference { get; init; } = string.Empty;

        /// <summary>
        ///     Gets a value indicating whether the parameter is required.
        /// </summary>
        public bool Required { get; init; }

        /// <summary>
        ///     Gets the parameter schema summary.
        /// </summary>
        public string Schema { get; init; } = string.Empty;

        #endregion
    }

    /// <summary>
    ///     Represents one human-readable OpenAPI security requirement.
    /// </summary>
    public sealed class DocumentationOpenApiSecurityRequirementDisplayItem
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the authentication header name when the scheme uses one.
        /// </summary>
        public string HeaderName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the OpenAPI security scheme location.
        /// </summary>
        public string Location { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the HTTP authentication scheme.
        /// </summary>
        public string Scheme { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the OpenAPI security scheme name.
        /// </summary>
        public string SchemeName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the required OAuth scopes.
        /// </summary>
        public IReadOnlyList<string> Scopes { get; init; } = [];

        /// <summary>
        ///     Gets the OpenAPI security scheme type.
        /// </summary>
        public string Type { get; init; } = string.Empty;

        #endregion
    }

    /// <summary>
    ///     Represents one human-readable OpenAPI request body.
    /// </summary>
    public sealed class DocumentationOpenApiRequestBodyDisplayItem
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the supported request content types.
        /// </summary>
        public IReadOnlyList<DocumentationOpenApiContentTypeDisplayItem> ContentTypes { get; init; } = [];

        /// <summary>
        ///     Gets the request body description.
        /// </summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>
        ///     Gets a value indicating whether the request body is required.
        /// </summary>
        public bool Required { get; init; }

        #endregion
    }

    /// <summary>
    ///     Represents one human-readable OpenAPI response.
    /// </summary>
    public sealed class DocumentationOpenApiResponseDisplayItem
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the response content types.
        /// </summary>
        public IReadOnlyList<DocumentationOpenApiContentTypeDisplayItem> ContentTypes { get; init; } = [];

        /// <summary>
        ///     Gets the response description.
        /// </summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the response headers.
        /// </summary>
        public IReadOnlyList<DocumentationOpenApiHeaderDisplayItem> Headers { get; init; } = [];

        /// <summary>
        ///     Gets the HTTP status code or OpenAPI default response key.
        /// </summary>
        public string StatusCode { get; init; } = string.Empty;

        #endregion
    }

    /// <summary>
    ///     Represents one OpenAPI media type contract.
    /// </summary>
    public sealed class DocumentationOpenApiContentTypeDisplayItem
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the media type name.
        /// </summary>
        public string MediaType { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the schema summary.
        /// </summary>
        public string Schema { get; init; } = string.Empty;

        #endregion
    }

    /// <summary>
    ///     Represents one OpenAPI response header contract.
    /// </summary>
    public sealed class DocumentationOpenApiHeaderDisplayItem
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the header description.
        /// </summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the header name.
        /// </summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the header schema summary.
        /// </summary>
        public string Schema { get; init; } = string.Empty;

        #endregion
    }
}