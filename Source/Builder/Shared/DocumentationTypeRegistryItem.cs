#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    internal sealed class DocumentationTypeRegistryItem
    {
        #region Instance fields and properties

        public required string GroupName { get; init; }
        public required string NamespaceName { get; init; }
        public required string ObjectName { get; init; }
        public required string PackageId { get; init; }
        public required string Version { get; init; }

        #endregion
    }
}