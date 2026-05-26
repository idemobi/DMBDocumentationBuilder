#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationOpenApiDescriptor.cs create at 2026/05/22 00:00:00
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Describes one OpenAPI document that DocumentationBuilder should import into DocumentationViewer.
    /// </summary>
    public sealed class DocumentationOpenApiDescriptor
    {
        #region Instance fields and properties

        /// <summary>
        /// Gets or sets the stable document name used in routes and SQLite metadata.
        /// </summary>
        public string DocumentName { get; set; } = "default";

        /// <summary>
        /// Gets or sets the Bootstrap icon used by the generated REST API sidebar section.
        /// </summary>
        public string Icon { get; set; } = "bi-hdd-network";

        /// <summary>
        /// Gets the path to the OpenAPI JSON document.
        /// </summary>
        public required string JsonFilePath { get; init; }

        /// <summary>
        /// Gets or sets the sidebar section title.
        /// </summary>
        public string SectionTitle { get; set; } = "REST API";

        #endregion
    }
}
