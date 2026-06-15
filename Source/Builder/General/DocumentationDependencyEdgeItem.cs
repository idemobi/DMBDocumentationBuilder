#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Describes one dependency relation between two documented elements.
    /// </summary>
    public sealed class DocumentationDependencyEdgeItem
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the relationship label rendered between the source and target elements.
        /// </summary>
        public string RelationshipKind { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the group name that owns the source element.
        /// </summary>
        public string SourceGroupName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the kind label shown for the source element.
        /// </summary>
        public string SourceKindLabel { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the source element name.
        /// </summary>
        public string SourceName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the namespace that owns the source element.
        /// </summary>
        public string SourceNamespaceName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the package identifier that owns the source element.
        /// </summary>
        public string SourcePackageId { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the package version that owns the source element.
        /// </summary>
        public string SourceVersion { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the group name that owns the target element.
        /// </summary>
        public string TargetGroupName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the kind label shown for the target element.
        /// </summary>
        public string TargetKindLabel { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the target element name.
        /// </summary>
        public string TargetName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the namespace that owns the target element.
        /// </summary>
        public string TargetNamespaceName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the package identifier that owns the target element.
        /// </summary>
        public string TargetPackageId { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the package version that owns the target element.
        /// </summary>
        public string TargetVersion { get; init; } = string.Empty;

        #endregion
    }
}