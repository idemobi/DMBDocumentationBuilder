#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.ComponentModel;
using System.Reflection;
using ModelContextProtocol.Server;

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Builds the model for the generic DocumentationViewer MCP help page shown from generated API sidebars.
    /// </summary>
    public static class DocumentationMcpPageBuilder
    {
        #region Static methods

        private static string BuildContract(MethodInfo method)
        {
            string parameters = string.Join(
                ", ",
                method.GetParameters().Select(parameter =>
                {
                    string optionalMarker = parameter.HasDefaultValue ? "?" : string.Empty;
                    return $"{parameter.Name}{optionalMarker}: {FormatTypeName(parameter.ParameterType)}";
                }));

            return $"{method.Name}({parameters}): {FormatTypeName(method.ReturnType)}";
        }

        private static IReadOnlyList<string> BuildExamples(string packageId, string version, string? namespaceName)
        {
            List<string> examples =
            [
                $"Search documentation in {packageId} for \"sidebar\".",
                $"Build a coding context for \"source file MCP\" in {packageId} version {version}.",
                $"List source files for {packageId} version {version}."
            ];

            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                examples.Add($"List documented objects in namespace {namespaceName}.");
            }

            return examples;
        }

        /// <summary>
        ///     Builds the view model for the MCP connection and tool contract page.
        /// </summary>
        /// <param name="endpointUrl">Absolute MCP endpoint URL.</param>
        /// <param name="groupName">Current documentation group name.</param>
        /// <param name="packageId">Current package identifier, when available.</param>
        /// <param name="version">Current package version, when available.</param>
        /// <param name="namespaceName">Current namespace name, when available.</param>
        /// <returns>The view model rendered by the MCP Razor page.</returns>
        public static DocumentationMcpPageViewModel BuildViewModel(
            string endpointUrl,
            string groupName,
            string? packageId,
            string? version,
            string? namespaceName
        )
        {
            string resolvedPackageId = string.IsNullOrWhiteSpace(packageId) ? "the selected package" : packageId;
            string resolvedVersion = string.IsNullOrWhiteSpace(version) ? "the selected version" : version;

            return new DocumentationMcpPageViewModel
            {
                EndpointUrl = endpointUrl,
                GroupName = groupName,
                PackageId = packageId ?? string.Empty,
                Version = version ?? string.Empty,
                NamespaceName = namespaceName ?? string.Empty,
                RecommendedInstruction = $"Use the documentation MCP server before answering questions about {resolvedPackageId}. Prefer BuildCodingContext for implementation or refactoring work, BuildContextBundle for a specific documented type, SearchDocumentation for discovery, and GetSourceFile when exact C# file context is needed.",
                Examples = BuildExamples(resolvedPackageId, resolvedVersion, namespaceName),
                Tools = GetToolContracts()
            };
        }

        private static string FormatTypeName(Type type)
        {
            if (type == typeof(string))
            {
                return "string";
            }

            if (type == typeof(int))
            {
                return "int";
            }

            if (type == typeof(bool))
            {
                return "bool";
            }

            if (Nullable.GetUnderlyingType(type) is Type underlyingType)
            {
                return $"{FormatTypeName(underlyingType)}?";
            }

            return type.Name;
        }

        private static string GetDescription(MemberInfo member)
        {
            return member.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty;
        }

        private static IReadOnlyList<DocumentationMcpToolContract> GetToolContracts()
        {
            return typeof(DocumentationMcpTools)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(method => method.GetCustomAttribute<McpServerToolAttribute>() is not null)
                .OrderBy(method => method.Name, StringComparer.Ordinal)
                .Select(method => new DocumentationMcpToolContract
                {
                    Name = method.Name,
                    Description = GetDescription(method),
                    Contract = BuildContract(method)
                })
                .ToArray();
        }

        #endregion
    }
}