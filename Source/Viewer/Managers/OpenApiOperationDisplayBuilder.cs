#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Text.Json;

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Builds human-readable display contracts from stored OpenAPI operation JSON fragments.
    /// </summary>
    internal static class OpenApiOperationDisplayBuilder
    {
        #region Static methods

        /// <summary>
        ///     Builds a display contract for one OpenAPI operation.
        /// </summary>
        /// <param name="documentJson">The raw OpenAPI document JSON that contains component-level security schemes.</param>
        /// <param name="operation">The operation record to transform.</param>
        /// <returns>A human-readable operation display contract.</returns>
        public static DocumentationOpenApiOperationDisplayModel Build(
            string documentJson,
            DocumentationOpenApiOperationQueryResult? operation
        )
        {
            if (operation is null)
            {
                return new DocumentationOpenApiOperationDisplayModel();
            }

            return new DocumentationOpenApiOperationDisplayModel
            {
                Parameters = ReadParameters(operation.ParametersJson),
                RequestBody = ReadRequestBody(operation.RequestBodyJson),
                Responses = ReadResponses(operation.ResponsesJson),
                SecurityRequirements = ReadSecurityRequirements(documentJson, operation.SecurityJson)
            };
        }

        private static string DescribeComposition(string compositionName, JsonElement values)
        {
            if (values.ValueKind != JsonValueKind.Array)
            {
                return compositionName;
            }

            string[] schemas = values
                .EnumerateArray()
                .Select(DescribeSchema)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToArray();

            return schemas.Length == 0
                ? compositionName
                : $"{compositionName}: {string.Join(", ", schemas)}";
        }

        private static string DescribeSchema(JsonElement schema)
        {
            if (schema.ValueKind != JsonValueKind.Object)
            {
                return string.Empty;
            }

            string reference = ReadReference(schema);

            if (!string.IsNullOrWhiteSpace(reference))
            {
                return reference;
            }

            if (schema.TryGetProperty("oneOf", out JsonElement oneOf))
            {
                return DescribeComposition("oneOf", oneOf);
            }

            if (schema.TryGetProperty("anyOf", out JsonElement anyOf))
            {
                return DescribeComposition("anyOf", anyOf);
            }

            if (schema.TryGetProperty("allOf", out JsonElement allOf))
            {
                return DescribeComposition("allOf", allOf);
            }

            string type = ReadString(schema, "type");

            if (string.Equals(type, "array", StringComparison.OrdinalIgnoreCase) &&
                schema.TryGetProperty("items", out JsonElement items))
            {
                string itemSchema = DescribeSchema(items);
                return string.IsNullOrWhiteSpace(itemSchema) ? "array" : $"array of {itemSchema}";
            }

            List<string> parts = [];

            if (!string.IsNullOrWhiteSpace(type))
            {
                parts.Add(type);
            }

            string format = ReadString(schema, "format");

            if (!string.IsNullOrWhiteSpace(format))
            {
                parts.Add(format);
            }

            if (schema.TryGetProperty("enum", out JsonElement enumValues) &&
                enumValues.ValueKind == JsonValueKind.Array)
            {
                string[] values = enumValues
                    .EnumerateArray()
                    .Select(ReadScalarValue)
                    .Where(value => !string.IsNullOrWhiteSpace(value))
                    .ToArray();

                if (values.Length > 0)
                {
                    parts.Add("enum: " + string.Join(", ", values));
                }
            }

            return parts.Count == 0 ? "object" : string.Join(" / ", parts);
        }

        private static bool ReadBoolean(JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out JsonElement property) &&
                   property.ValueKind == JsonValueKind.True;
        }

        private static IReadOnlyList<DocumentationOpenApiContentTypeDisplayItem> ReadContentTypes(JsonElement value)
        {
            if (!value.TryGetProperty("content", out JsonElement content) ||
                content.ValueKind != JsonValueKind.Object)
            {
                return [];
            }

            return content
                .EnumerateObject()
                .Select(mediaType => new DocumentationOpenApiContentTypeDisplayItem
                {
                    MediaType = mediaType.Name,
                    Schema = mediaType.Value.TryGetProperty("schema", out JsonElement schema)
                        ? DescribeSchema(schema)
                        : string.Empty
                })
                .ToArray();
        }

        private static string ReadDefaultValue(JsonElement parameter)
        {
            if (!parameter.TryGetProperty("schema", out JsonElement schema) ||
                schema.ValueKind != JsonValueKind.Object ||
                !schema.TryGetProperty("default", out JsonElement defaultValue))
            {
                return string.Empty;
            }

            return ReadScalarValue(defaultValue);
        }

        private static string ReadFormat(JsonElement parameter)
        {
            if (!parameter.TryGetProperty("schema", out JsonElement schema) ||
                schema.ValueKind != JsonValueKind.Object)
            {
                return string.Empty;
            }

            return ReadString(schema, "format");
        }

        private static IReadOnlyList<DocumentationOpenApiHeaderDisplayItem> ReadHeaders(JsonElement response)
        {
            if (!response.TryGetProperty("headers", out JsonElement headers) ||
                headers.ValueKind != JsonValueKind.Object)
            {
                return [];
            }

            return headers
                .EnumerateObject()
                .Select(header => new DocumentationOpenApiHeaderDisplayItem
                {
                    Name = header.Name,
                    Description = ReadString(header.Value, "description"),
                    Schema = header.Value.TryGetProperty("schema", out JsonElement schema)
                        ? DescribeSchema(schema)
                        : string.Empty
                })
                .ToArray();
        }

        private static IReadOnlyList<DocumentationOpenApiParameterDisplayItem> ReadParameters(string json)
        {
            try
            {
                using JsonDocument document = JsonDocument.Parse(json);

                if (document.RootElement.ValueKind != JsonValueKind.Array)
                {
                    return [];
                }

                return document.RootElement
                    .EnumerateArray()
                    .Where(parameter => parameter.ValueKind == JsonValueKind.Object)
                    .Select(parameter => new DocumentationOpenApiParameterDisplayItem
                    {
                        Name = ReadString(parameter, "name"),
                        Location = ReadString(parameter, "in"),
                        Required = ReadBoolean(parameter, "required"),
                        Description = ReadString(parameter, "description"),
                        Format = ReadFormat(parameter),
                        DefaultValue = ReadDefaultValue(parameter),
                        Reference = ReadReference(parameter),
                        Schema = parameter.TryGetProperty("schema", out JsonElement schema)
                            ? DescribeSchema(schema)
                            : ReadReference(parameter)
                    })
                    .ToArray();
            }
            catch (JsonException)
            {
                return [];
            }
        }

        private static string ReadReference(JsonElement element)
        {
            string reference = ReadString(element, "$ref");

            if (string.IsNullOrWhiteSpace(reference))
            {
                return string.Empty;
            }

            int index = reference.LastIndexOf("/", StringComparison.Ordinal);
            return index >= 0 && index < reference.Length - 1
                ? reference[(index + 1)..]
                : reference;
        }

        private static DocumentationOpenApiRequestBodyDisplayItem ReadRequestBody(string json)
        {
            try
            {
                using JsonDocument document = JsonDocument.Parse(json);

                if (document.RootElement.ValueKind != JsonValueKind.Object)
                {
                    return new DocumentationOpenApiRequestBodyDisplayItem();
                }

                return new DocumentationOpenApiRequestBodyDisplayItem
                {
                    Description = ReadString(document.RootElement, "description"),
                    Required = ReadBoolean(document.RootElement, "required"),
                    ContentTypes = ReadContentTypes(document.RootElement)
                };
            }
            catch (JsonException)
            {
                return new DocumentationOpenApiRequestBodyDisplayItem();
            }
        }

        private static IReadOnlyList<DocumentationOpenApiResponseDisplayItem> ReadResponses(string json)
        {
            try
            {
                using JsonDocument document = JsonDocument.Parse(json);

                if (document.RootElement.ValueKind != JsonValueKind.Object)
                {
                    return [];
                }

                return document.RootElement
                    .EnumerateObject()
                    .Select(response => new DocumentationOpenApiResponseDisplayItem
                    {
                        StatusCode = response.Name,
                        Description = ReadString(response.Value, "description"),
                        Headers = ReadHeaders(response.Value),
                        ContentTypes = ReadContentTypes(response.Value)
                    })
                    .ToArray();
            }
            catch (JsonException)
            {
                return [];
            }
        }

        private static string ReadScalarValue(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString() ?? string.Empty,
                JsonValueKind.Number => element.GetRawText(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                JsonValueKind.Null => "null",
                _ => string.Empty
            };
        }

        private static IReadOnlyList<DocumentationOpenApiSecurityRequirementDisplayItem> ReadSecurityRequirements(
            string documentJson,
            string securityJson
        )
        {
            try
            {
                using JsonDocument document = JsonDocument.Parse(documentJson);
                using JsonDocument securityDocument = JsonDocument.Parse(securityJson);

                if (securityDocument.RootElement.ValueKind != JsonValueKind.Array)
                {
                    return [];
                }

                Dictionary<string, JsonElement> schemes = ReadSecuritySchemes(document.RootElement);
                List<DocumentationOpenApiSecurityRequirementDisplayItem> requirements = [];

                foreach (JsonElement requirement in securityDocument.RootElement.EnumerateArray())
                {
                    if (requirement.ValueKind != JsonValueKind.Object)
                    {
                        continue;
                    }

                    foreach (JsonProperty schemeRequirement in requirement.EnumerateObject())
                    {
                        schemes.TryGetValue(schemeRequirement.Name, out JsonElement scheme);

                        requirements.Add(new DocumentationOpenApiSecurityRequirementDisplayItem
                        {
                            SchemeName = schemeRequirement.Name,
                            Type = ReadString(scheme, "type"),
                            Location = ReadString(scheme, "in"),
                            HeaderName = ResolveSecurityHeaderName(scheme),
                            Scheme = ReadString(scheme, "scheme"),
                            Scopes = ReadSecurityScopes(schemeRequirement.Value)
                        });
                    }
                }

                return requirements;
            }
            catch (JsonException)
            {
                return [];
            }
        }

        private static Dictionary<string, JsonElement> ReadSecuritySchemes(JsonElement document)
        {
            Dictionary<string, JsonElement> result = new(StringComparer.Ordinal);

            if (!document.TryGetProperty("components", out JsonElement components) ||
                components.ValueKind != JsonValueKind.Object ||
                !components.TryGetProperty("securitySchemes", out JsonElement schemes) ||
                schemes.ValueKind != JsonValueKind.Object)
            {
                return result;
            }

            foreach (JsonProperty scheme in schemes.EnumerateObject())
            {
                result[scheme.Name] = scheme.Value.Clone();
            }

            return result;
        }

        private static IReadOnlyList<string> ReadSecurityScopes(JsonElement value)
        {
            if (value.ValueKind != JsonValueKind.Array)
            {
                return [];
            }

            return value
                .EnumerateArray()
                .Select(ReadScalarValue)
                .Where(scope => !string.IsNullOrWhiteSpace(scope))
                .ToArray();
        }

        private static string ReadString(JsonElement element, string propertyName)
        {
            return element.ValueKind == JsonValueKind.Object &&
                   element.TryGetProperty(propertyName, out JsonElement property) &&
                   property.ValueKind == JsonValueKind.String
                ? property.GetString() ?? string.Empty
                : string.Empty;
        }

        private static string ResolveSecurityHeaderName(JsonElement scheme)
        {
            string type = ReadString(scheme, "type");
            string location = ReadString(scheme, "in");

            if (string.Equals(type, "apiKey", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(location, "header", StringComparison.OrdinalIgnoreCase))
            {
                return ReadString(scheme, "name");
            }

            if (string.Equals(type, "http", StringComparison.OrdinalIgnoreCase))
            {
                return "Authorization";
            }

            return string.Empty;
        }

        #endregion
    }
}