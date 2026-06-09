#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBBootstrapBuilder;
using DMBDocumentationBuilderLabs.Navigation;

#endregion

namespace DMBDocumentationBuilderLabs.Controllers
{
    /// <summary>
    ///     Provides backward-compatible entry points for Documentation module sidebar navigation.
    /// </summary>
    /// <remarks>
    ///     New hosts should use <see cref="DMBDocumentationBuilderLabsNavigationAgent" /> directly.
    /// </remarks>
    public static class DocumentationModuleSidebarAgent
    {
        #region Static methods

        /// <summary>
        ///     Creates the root-level sidebar for the Documentation module.
        /// </summary>
        /// <returns>The root Documentation module sidebar component.</returns>
        public static SideBarComponent CreateRootSidebar()
        {
            return DMBDocumentationBuilderLabsNavigationAgent.CreateSidebar(null, null);
        }

        /// <summary>
        ///     Creates the contextual sidebar for the Documentation module with the active item highlighted.
        /// </summary>
        /// <param name="currentController">The name of the active controller.</param>
        /// <param name="currentAction">The name of the active action.</param>
        /// <returns>The Documentation module sidebar component.</returns>
        public static SideBarComponent CreateSidebar(string? currentController, string? currentAction)
        {
            return DMBDocumentationBuilderLabsNavigationAgent.CreateSidebar(currentController, currentAction);
        }

        #endregion
    }
}