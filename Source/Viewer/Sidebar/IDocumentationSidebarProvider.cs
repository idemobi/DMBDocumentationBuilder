#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBBootstrapBuilder;

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Defines host-provided sidebar creation for DocumentationViewer pages.
    /// </summary>
    public interface IDocumentationSidebarProvider
    {
        #region Instance methods

        /// <summary>
        ///     Creates the sidebar used by a documentation group page.
        /// </summary>
        /// <param name="groupName">Documentation group name from the current route.</param>
        /// <returns>The sidebar section to render for the group page.</returns>
        SideBarSectionComponent CreateGroupSidebar(string groupName);

        /// <summary>
        ///     Creates the sidebar used by a documentation namespace or object page.
        /// </summary>
        /// <param name="groupName">Documentation group name from the current route.</param>
        /// <param name="packageId">Package identifier from the current route.</param>
        /// <param name="version">Package version from the current route.</param>
        /// <param name="namespaceName">Namespace name from the current route.</param>
        /// <returns>The sidebar section to render for the namespace or object page.</returns>
        SideBarSectionComponent CreateNamespaceSidebar(string groupName, string packageId, string version, string namespaceName);

        /// <summary>
        ///     Creates the sidebar used by the documentation root page.
        /// </summary>
        /// <returns>The root documentation sidebar section.</returns>
        SideBarSectionComponent CreateRootSidebar();

        #endregion
    }
}