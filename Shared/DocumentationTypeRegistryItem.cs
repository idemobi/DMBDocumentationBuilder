#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationTypeRegistryItem.cs create at 2026/05/18 00:00:00
// ©2024-2026 idéMobi SARL FRANCE

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
