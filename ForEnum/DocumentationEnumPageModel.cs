#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationEnumPageModel.cs create at 2026/04/12 12:04:31
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents the DocumentationEnumPageModel type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationEnumPageModel
    {
        #region Instance fields and properties

        /// <summary>
        /// Gets or sets the Accessibility value used by generated documentation.
        /// </summary>
        public string Accessibility { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the AssemblyName value used by generated documentation.
        /// </summary>
        public string AssemblyName { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the Declaration value used by generated documentation.
        /// </summary>
        public string Declaration { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the EnumName value used by generated documentation.
        /// </summary>
        public string EnumName { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the ExtensionMethods value used by generated documentation.
        /// </summary>
        public List<DocumentationExtensionMethodItem> ExtensionMethods { get; } = [];
        /// <summary>
        /// Gets or sets the GroupName value used by generated documentation.
        /// </summary>
        public string GroupName { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the IsFlags value used by generated documentation.
        /// </summary>
        public bool IsFlags { get; init; }
        /// <summary>
        /// Gets or sets the IsObsolete value used by generated documentation.
        /// </summary>
        public bool IsObsolete { get; init; }
        /// <summary>
        /// Gets or sets the NamespaceName value used by generated documentation.
        /// </summary>
        public string NamespaceName { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the ObsoleteMessage value used by generated documentation.
        /// </summary>
        public string ObsoleteMessage { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the PackageId value used by generated documentation.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the UnderlyingType value used by generated documentation.
        /// </summary>
        public string UnderlyingType { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the Values value used by generated documentation.
        /// </summary>
        public List<DocumentationEnumValueItem> Values { get; } = [];
        /// <summary>
        /// Gets or sets the Version value used by generated documentation.
        /// </summary>
        public string Version { get; init; } = string.Empty;
        /// <summary>
        /// Gets the collection used by DocumentationBuilder generation.
        /// </summary>
        public DocumentationXmlModel XmlDoc { get; init; } = new();

        #endregion
    }
}