#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents the DocumentationStructPageModel type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationStructPageModel
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
        ///     Gets or sets the ExtensionMethods value used by generated documentation.
        /// </summary>
        public List<DocumentationExtensionMethodItem> ExtensionMethods { get; } = [];

        /// <summary>
        ///     Gets or sets the Fields value used by generated documentation.
        /// </summary>
        public List<DocumentationStructFieldItem> Fields { get; } = [];

        /// <summary>
        ///     Gets or sets the GroupName value used by generated documentation.
        /// </summary>
        public string GroupName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the ImplementedInterfaces value used by generated documentation.
        /// </summary>
        public List<string> ImplementedInterfaces { get; } = [];

        /// <summary>
        ///     Gets or sets the IsObsolete value used by generated documentation.
        /// </summary>
        public bool IsObsolete { get; init; }

        /// <summary>
        ///     Gets or sets the IsReadOnly value used by generated documentation.
        /// </summary>
        public bool IsReadOnly { get; init; }

        /// <summary>
        ///     Gets or sets the IsRefLike value used by generated documentation.
        /// </summary>
        public bool IsRefLike { get; init; }

        /// <summary>
        ///     Gets or sets the Methods value used by generated documentation.
        /// </summary>
        public List<DocumentationStructMethodItem> Methods { get; } = [];

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
        public List<DocumentationStructPropertyItem> Properties { get; } = [];

        /// <summary>
        ///     Gets or sets the StructName value used by generated documentation.
        /// </summary>
        public string StructName { get; init; } = string.Empty;

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