#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationClassPageModel.cs create at 2026/04/12 12:04:31
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents the DocumentationClassPageModel type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationClassPageModel
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
        /// Gets or sets the BaseType value used by generated documentation.
        /// </summary>
        public DocumentationTypeLinkItem? BaseType { get; init; }
        /// <summary>
        /// Gets or sets the ClassName value used by generated documentation.
        /// </summary>
        public string ClassName { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the Constructors value used by generated documentation.
        /// </summary>
        public List<DocumentationClassConstructorItem> Constructors { get; } = [];
        /// <summary>
        /// Gets or sets the Declaration value used by generated documentation.
        /// </summary>
        public string Declaration { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the ExampleHtml value used by generated documentation.
        /// </summary>
        public string ExampleHtml { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the ExtensionMethods value used by generated documentation.
        /// </summary>
        public List<DocumentationExtensionMethodItem> ExtensionMethods { get; } = [];
        /// <summary>
        /// Gets or sets the Fields value used by generated documentation.
        /// </summary>
        public List<DocumentationClassFieldItem> Fields { get; } = [];
        /// <summary>
        /// Gets or sets the GroupName value used by generated documentation.
        /// </summary>
        public string GroupName { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the ImplementedInterfaces value used by generated documentation.
        /// </summary>
        public List<DocumentationTypeLinkItem> ImplementedInterfaces { get; } = [];
        /// <summary>
        /// Gets or sets the IsAbstract value used by generated documentation.
        /// </summary>
        public bool IsAbstract { get; init; }
        /// <summary>
        /// Gets or sets the IsObsolete value used by generated documentation.
        /// </summary>
        public bool IsObsolete { get; init; }
        /// <summary>
        /// Gets or sets the IsSealed value used by generated documentation.
        /// </summary>
        public bool IsSealed { get; init; }
        /// <summary>
        /// Gets or sets the IsStatic value used by generated documentation.
        /// </summary>
        public bool IsStatic { get; init; }
        /// <summary>
        /// Gets or sets the Methods value used by generated documentation.
        /// </summary>
        public List<DocumentationClassMethodItem> Methods { get; } = [];
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
        /// Gets or sets the Properties value used by generated documentation.
        /// </summary>
        public List<DocumentationClassPropertyItem> Properties { get; } = [];
        /// <summary>
        /// Gets or sets the RemarksHtml value used by generated documentation.
        /// </summary>
        public string RemarksHtml { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the SeeAlsos value used by generated documentation.
        /// </summary>
        public List<DocumentationXmlLinkItem> SeeAlsos { get; } = [];
        /// <summary>
        /// Gets or sets the SummaryHtml value used by generated documentation.
        /// </summary>
        public string SummaryHtml { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the Version value used by generated documentation.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        #endregion
    }
}