#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBBootstrapBuilder;

#endregion

namespace DMBDocumentationViewer.Controllers
{
    /// <summary>
    ///     Creates DocumentationViewer sidebar sections through an optional host-provided provider.
    /// </summary>
    /// <remarks>
    ///     Host applications can assign <see cref="Provider" /> to customize root, group, and namespace
    ///     sidebars. When no provider is configured, the factory returns stable fallback sidebars.
    /// </remarks>
    public static class DocumentationSidebarFactory
    {
        #region Static fields and properties

        /// <summary>
        ///     Gets or sets the host-provided sidebar provider used by DocumentationViewer pages.
        /// </summary>
        /// <remarks>
        ///     When this value is <see langword="null" />, root, group, and namespace sidebar calls use
        ///     deterministic fallback sections.
        /// </remarks>
        public static IDocumentationSidebarProvider? Provider { get; set; }

        #endregion

        #region Static methods

        /// <summary>
        ///     Creates a sidebar section component for a documentation group page.
        /// </summary>
        /// <param name="groupName">The name of the group for which the sidebar is being created.</param>
        /// <returns>A <see cref="SideBarSectionComponent" /> representing the sidebar for the specified group.</returns>
        /// <remarks>
        ///     The provider result is used when available. Otherwise the method falls back to
        ///     <see cref="CreateRootSidebar" /> so the page always has a sidebar section.
        /// </remarks>
        /// <seealso cref="DocumentationSidebarFactory" />
        /// <seealso cref="SideBarSectionComponent" />
        /// <seealso cref="DocumentationController.ShowGroup" />
        public static SideBarSectionComponent CreateGroupSidebar(string groupName)
        {
            return Provider?.CreateGroupSidebar(groupName) ?? CreateRootSidebar();
        }

        /// <summary>
        ///     Creates a sidebar section component for a documentation namespace or object page.
        /// </summary>
        /// <param name="groupName">The name of the group to which the namespace belongs.</param>
        /// <param name="packageId">The identifier of the package containing the namespace.</param>
        /// <param name="version">The version of the package.</param>
        /// <param name="namespaceName">The name of the namespace for which the sidebar is being created.</param>
        /// <returns>A <see cref="SideBarSectionComponent" /> representing the sidebar for the specified namespace.</returns>
        /// <remarks>
        ///     The provider result is used when available. Otherwise the method falls back to
        ///     <see cref="CreateGroupSidebar" /> for the same group.
        /// </remarks>
        /// <seealso cref="DocumentationSidebarFactory" />
        /// <seealso cref="SideBarSectionComponent" />
        /// <seealso cref="DocumentationController.Show" />
        public static SideBarSectionComponent CreateNamespaceSidebar(string groupName, string packageId, string version, string namespaceName)
        {
            return Provider?.CreateNamespaceSidebar(groupName, packageId, version, namespaceName)
                   ?? CreateGroupSidebar(groupName);
        }

        /// <summary>
        ///     Creates the root sidebar section component for DocumentationViewer pages.
        /// </summary>
        /// <returns>A <see cref="SideBarSectionComponent" /> representing the root sidebar.</returns>
        /// <remarks>
        ///     The provider result is used when available. Otherwise a fallback section titled
        ///     `Documentation` is returned.
        /// </remarks>
        /// <seealso cref="DocumentationSidebarFactory" />
        /// <seealso cref="SideBarSectionComponent" />
        /// <seealso cref="DocumentationController.ShowGroup" />
        public static SideBarSectionComponent CreateRootSidebar()
        {
            return Provider?.CreateRootSidebar() ?? new SideBarSectionComponent("Documentation");
        }

        #endregion
    }
}