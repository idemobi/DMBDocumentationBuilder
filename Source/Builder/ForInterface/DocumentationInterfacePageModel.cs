#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents the DocumentationInterfacePageModel type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationInterfacePageModel
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the Accessibility value used by generated documentation.
        /// </summary>
        public string Accessibility { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the AssemblyName value used by generated documentation.
        /// </summary>
        public string AssemblyName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the Declaration value used by generated documentation.
        /// </summary>
        public string Declaration { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the dependency edges discovered from this documented interface to other documented elements.
        /// </summary>
        public List<DocumentationDependencyEdgeItem> DependencyEdges { get; } = [];

        /// <summary>
        ///     Gets or sets the Events value used by generated documentation.
        /// </summary>
        public List<DocumentationInterfaceEventItem> Events { get; } = [];

        /// <summary>
        ///     Gets or sets the ExtensionMethods value used by generated documentation.
        /// </summary>
        public List<DocumentationExtensionMethodItem> ExtensionMethods { get; } = [];

        /// <summary>
        ///     Gets or sets the GroupName value used by generated documentation.
        /// </summary>
        public string GroupName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the InterfaceName value used by generated documentation.
        /// </summary>
        public string InterfaceName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the IsObsolete value used by generated documentation.
        /// </summary>
        public bool IsObsolete { get; init; }

        /// <summary>
        ///     Gets or sets the Methods value used by generated documentation.
        /// </summary>
        public List<DocumentationInterfaceMethodItem> Methods { get; } = [];

        /// <summary>
        ///     Gets or sets the NamespaceName value used by generated documentation.
        /// </summary>
        public string NamespaceName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the ObsoleteMessage value used by generated documentation.
        /// </summary>
        public string ObsoleteMessage { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the PackageId value used by generated documentation.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the Properties value used by generated documentation.
        /// </summary>
        public List<DocumentationInterfacePropertyItem> Properties { get; } = [];

        /// <summary>
        ///     Gets or sets the Version value used by generated documentation.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the collection used by DocumentationBuilder generation.
        /// </summary>
        public DocumentationXmlModel XmlDoc { get; init; } = new();

        #endregion
    }
}
