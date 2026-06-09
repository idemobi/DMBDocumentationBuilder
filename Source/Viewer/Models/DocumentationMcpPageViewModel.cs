#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Represents one MCP tool contract displayed on the DocumentationViewer MCP help page.
    /// </summary>
    public sealed class DocumentationMcpToolContract
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the compact callable signature for the MCP tool.
        /// </summary>
        public string Contract { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the human-readable tool description.
        /// </summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the MCP tool name.
        /// </summary>
        public string Name { get; init; } = string.Empty;

        #endregion
    }

    /// <summary>
    ///     Provides data rendered by the DocumentationViewer MCP connection page.
    /// </summary>
    public sealed class DocumentationMcpPageViewModel
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the absolute MCP endpoint URL displayed to human users.
        /// </summary>
        public string EndpointUrl { get; init; } = string.Empty;

        /// <summary>
        ///     Gets example prompts contextualized for the selected documentation scope.
        /// </summary>
        public IReadOnlyList<string> Examples { get; init; } = [];

        /// <summary>
        ///     Gets the selected documentation group name.
        /// </summary>
        public string GroupName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the selected namespace name, when available.
        /// </summary>
        public string NamespaceName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the selected package identifier, when available.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the recommended instruction to paste into an AI assistant.
        /// </summary>
        public string RecommendedInstruction { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the MCP tool contracts exposed by <see cref="DocumentationMcpTools" />.
        /// </summary>
        public IReadOnlyList<DocumentationMcpToolContract> Tools { get; init; } = [];

        /// <summary>
        ///     Gets the selected package version, when available.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        #endregion
    }
}