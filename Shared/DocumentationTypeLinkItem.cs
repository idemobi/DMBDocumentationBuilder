#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationTypeLinkItem.cs create at 2026/04/12 12:04:31
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents the DocumentationTypeLinkItem type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationTypeLinkItem
    {
        #region Instance fields and properties

        /// <summary>
        /// Gets or sets the DisplayName value used by generated documentation.
        /// </summary>
        public string DisplayName { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the IsDocumented value used by generated documentation.
        /// </summary>
        public bool IsDocumented { get; init; }
        /// <summary>
        /// Gets or sets the group name of the documented target type.
        /// </summary>
        public string GroupName { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the NamespaceName value used by generated documentation.
        /// </summary>
        public string NamespaceName { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the ObjectName value used by generated documentation.
        /// </summary>
        public string ObjectName { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the package identifier of the documented target type.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the package version of the documented target type.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        #endregion
    }
}
