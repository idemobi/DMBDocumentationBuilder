#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationSidebarItem.cs create at 2026/05/18 22:05:00
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents one persisted documentation sidebar item generated for a specific route scope.
    /// </summary>
    public sealed class DocumentationSidebarItem
    {
        #region Instance fields and properties

        /// <summary>
        /// Gets the MVC action name used by route sidebar items.
        /// </summary>
        public string ActionName { get; init; } = string.Empty;

        /// <summary>
        /// Gets the MVC controller name used by route sidebar items.
        /// </summary>
        public string ControllerName { get; init; } = string.Empty;

        /// <summary>
        /// Gets the Bootstrap icon class used by the sidebar item.
        /// </summary>
        public string Icon { get; init; } = string.Empty;

        /// <summary>
        /// Gets the stable item key inside the sidebar scope.
        /// </summary>
        public string ItemKey { get; init; } = string.Empty;

        /// <summary>
        /// Gets the item kind, such as group or route.
        /// </summary>
        public string ItemKind { get; init; } = string.Empty;

        /// <summary>
        /// Gets the parent item key inside the sidebar scope.
        /// </summary>
        public string ParentItemKey { get; init; } = string.Empty;

        /// <summary>
        /// Gets the package identifier associated with the sidebar scope.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;

        /// <summary>
        /// Gets the route group name applied to route sidebar items.
        /// </summary>
        public string RouteGroupName { get; init; } = string.Empty;

        /// <summary>
        /// Gets the route namespace name applied to route sidebar items.
        /// </summary>
        public string RouteNamespaceName { get; init; } = string.Empty;

        /// <summary>
        /// Gets the route object name applied to route sidebar items.
        /// </summary>
        public string RouteObjectName { get; init; } = string.Empty;

        /// <summary>
        /// Gets the route package identifier applied to route sidebar items.
        /// </summary>
        public string RoutePackageId { get; init; } = string.Empty;

        /// <summary>
        /// Gets the route package version applied to route sidebar items.
        /// </summary>
        public string RouteVersion { get; init; } = string.Empty;

        /// <summary>
        /// Gets the sidebar group name associated with the sidebar scope.
        /// </summary>
        public string GroupName { get; init; } = string.Empty;

        /// <summary>
        /// Gets the sidebar kind, such as root, group, or namespace.
        /// </summary>
        public string SidebarKind { get; init; } = string.Empty;

        /// <summary>
        /// Gets the namespace associated with the sidebar scope.
        /// </summary>
        public string NamespaceName { get; init; } = string.Empty;

        /// <summary>
        /// Gets the display order within the parent item.
        /// </summary>
        public int SortOrder { get; init; }

        /// <summary>
        /// Gets the sidebar item title.
        /// </summary>
        public string Title { get; init; } = string.Empty;

        /// <summary>
        /// Gets the package version associated with the sidebar scope.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        #endregion
    }
}
