#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Net;
using System.Text.Json;

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Imports OpenAPI 3 JSON documents into DocumentationBuilder data models.
    /// </summary>
    public static class DocumentationOpenApiExtractor
    {
        #region Static fields and properties

        private static readonly HashSet<string> HttpMethods = new(StringComparer.OrdinalIgnoreCase)
        {
            "get",
            "put",
            "post",
            "delete",
            "options",
            "head",
            "patch",
            "trace"
        };

        #endregion

        #region Static methods

        private static string BuildOperationId(string method, string path)
        {
            string normalizedPath = path
                .Replace("{", string.Empty, StringComparison.Ordinal)
                .Replace("}", string.Empty, StringComparison.Ordinal);

            string[] tokens = normalizedPath
                .Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(ToPascalToken)
                .Where(token => !string.IsNullOrWhiteSpace(token))
                .ToArray();

            return method.ToLowerInvariant() + string.Join(string.Empty, tokens);
        }

        private static string BuildRoutePath(
            string groupName,
            string packageId,
            string version,
            string documentName,
            string operationId
        )
        {
            string routePath = "/Documentation/ShowOpenApi?groupName=" + WebUtility.UrlEncode(groupName)
                                                                       + "&packageId=" + WebUtility.UrlEncode(packageId)
                                                                       + "&version=" + WebUtility.UrlEncode(version)
                                                                       + "&namespaceName=" + WebUtility.UrlEncode(documentName);

            if (!string.IsNullOrWhiteSpace(operationId))
            {
                routePath += "&objectName=" + WebUtility.UrlEncode(operationId);
            }

            return routePath;
        }

        /// <summary>
        ///     Extracts configured OpenAPI documents for one documented project.
        /// </summary>
        /// <param name="groupName">The documentation group that owns the project.</param>
        /// <param name="project">The documented project whose OpenAPI documents should be imported.</param>
        /// <returns>The imported OpenAPI documents.</returns>
        public static IReadOnlyList<DocumentationOpenApiDocumentItem> Extract(
            string groupName,
            DocumentationProjectDescriptor project
        )
        {
            if (project is null) throw new ArgumentNullException(nameof(project));

            List<DocumentationOpenApiDocumentItem> result = [];

            foreach (DocumentationOpenApiDescriptor descriptor in project.OpenApiDocuments)
            {
                if (string.IsNullOrWhiteSpace(descriptor.JsonFilePath) || !File.Exists(descriptor.JsonFilePath))
                {
                    continue;
                }

                string json = File.ReadAllText(descriptor.JsonFilePath);
                using JsonDocument document = JsonDocument.Parse(json);
                JsonElement root = document.RootElement;

                string documentName = string.IsNullOrWhiteSpace(descriptor.DocumentName)
                    ? "default"
                    : descriptor.DocumentName;

                string title = ReadString(root, "info", "title");
                string description = ReadString(root, "info", "description");
                string routePath = BuildRoutePath(groupName, project.PackageId, project.Version, documentName, string.Empty);
                IReadOnlyList<DocumentationOpenApiOperationItem> operations = ExtractOperations(
                    root,
                    groupName,
                    project,
                    documentName);

                result.Add(new DocumentationOpenApiDocumentItem
                {
                    PackageId = project.PackageId,
                    Version = project.Version,
                    DocumentName = documentName,
                    SectionTitle = descriptor.SectionTitle,
                    Icon = descriptor.Icon,
                    Title = string.IsNullOrWhiteSpace(title) ? documentName : title,
                    Description = description,
                    JsonContent = json,
                    RoutePath = routePath,
                    Operations = operations
                });
            }

            return result;
        }

        private static IReadOnlyList<DocumentationOpenApiOperationItem> ExtractOperations(
            JsonElement root,
            string groupName,
            DocumentationProjectDescriptor project,
            string documentName
        )
        {
            List<DocumentationOpenApiOperationItem> operations = [];

            if (!root.TryGetProperty("paths", out JsonElement paths) ||
                paths.ValueKind != JsonValueKind.Object)
            {
                return operations;
            }

            foreach (JsonProperty pathProperty in paths.EnumerateObject())
            {
                if (pathProperty.Value.ValueKind != JsonValueKind.Object)
                {
                    continue;
                }

                foreach (JsonProperty methodProperty in pathProperty.Value.EnumerateObject())
                {
                    if (!HttpMethods.Contains(methodProperty.Name) ||
                        methodProperty.Value.ValueKind != JsonValueKind.Object)
                    {
                        continue;
                    }

                    JsonElement operation = methodProperty.Value;
                    string operationId = ReadString(operation, "operationId");

                    if (string.IsNullOrWhiteSpace(operationId))
                    {
                        operationId = BuildOperationId(methodProperty.Name, pathProperty.Name);
                    }

                    operations.Add(new DocumentationOpenApiOperationItem
                    {
                        PackageId = project.PackageId,
                        Version = project.Version,
                        DocumentName = documentName,
                        OperationId = operationId,
                        HttpMethod = methodProperty.Name.ToUpperInvariant(),
                        Path = pathProperty.Name,
                        Summary = ReadString(operation, "summary"),
                        Description = ReadString(operation, "description"),
                        TagsJson = ReadRawJson(operation, "tags", "[]"),
                        ParametersJson = ReadRawJson(operation, "parameters", "[]"),
                        RequestBodyJson = ReadRawJson(operation, "requestBody", "{}"),
                        ResponsesJson = ReadRawJson(operation, "responses", "{}"),
                        SecurityJson = operation.TryGetProperty("security", out JsonElement security)
                            ? security.GetRawText()
                            : ReadRawJson(root, "security", "[]"),
                        RoutePath = BuildRoutePath(groupName, project.PackageId, project.Version, documentName, operationId)
                    });
                }
            }

            return operations
                .OrderBy(operation => operation.Path, StringComparer.Ordinal)
                .ThenBy(operation => operation.HttpMethod, StringComparer.Ordinal)
                .ToArray();
        }

        private static string ReadRawJson(JsonElement element, string propertyName, string fallback)
        {
            return element.TryGetProperty(propertyName, out JsonElement property)
                ? property.GetRawText()
                : fallback;
        }

        private static string ReadString(JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out JsonElement property) &&
                   property.ValueKind == JsonValueKind.String
                ? property.GetString() ?? string.Empty
                : string.Empty;
        }

        private static string ReadString(JsonElement element, string objectName, string propertyName)
        {
            if (!element.TryGetProperty(objectName, out JsonElement nested) ||
                nested.ValueKind != JsonValueKind.Object)
            {
                return string.Empty;
            }

            return ReadString(nested, propertyName);
        }

        private static string ToPascalToken(string value)
        {
            string[] parts = value.Split(
                ['-', '_', '.', ':'],
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return string.Join(
                string.Empty,
                parts.Select(part => char.ToUpperInvariant(part[0]) + part[1..]));
        }

        #endregion
    }
}