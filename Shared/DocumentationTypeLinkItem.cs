#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents the DocumentationTypeLinkItem type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationTypeLinkItem
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the DisplayName value used by generated documentation.
        /// </summary>
        public string DisplayName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the group name of the documented target type.
        /// </summary>
        public string GroupName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the IsDocumented value used by generated documentation.
        /// </summary>
        public bool IsDocumented { get; init; }

        /// <summary>
        ///     Gets or sets the NamespaceName value used by generated documentation.
        /// </summary>
        public string NamespaceName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the ObjectName value used by generated documentation.
        /// </summary>
        public string ObjectName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the package identifier of the documented target type.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the package version of the documented target type.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        #endregion
    }
}