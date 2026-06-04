#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System;
using DMBBootstrapBuilder;
using DMBPageBuilder;

#endregion

namespace DMBDocumentationBuilderLabs.Controllers
{
    /// <summary>
    ///     Builds the Documentation module sidebar navigation structure.
    /// </summary>
    public static class DocumentationModuleSidebarAgent
    {
        #region Static methods

        private static AspRouteActionItem CreateApiNamespaceAction(string title, string packageId, string namespaceName)
        {
            return ActionItemFactory.AspRoute("Documentation", "ShowNamespace")
                .AddRouteValue("groupName", "NuGet")
                .AddRouteValue("packageId", packageId)
                .AddRouteValue("namespaceName", namespaceName)
                .SetTitle(title)
                .SetIcon(IconStruct.Bootstrap("bi-journal-code"));
        }

        internal static SideBarSectionComponent CreateApiReferenceSection()
        {
            return new SideBarSectionComponent("API Reference")
                .Add(
                    CreateApiNamespaceAction("DMBDocumentationBuilder API", "DMBDocumentationBuilder", "DMBDocumentationBuilder"),
                    CreateApiNamespaceAction("DMBDocumentationViewer API", "DMBDocumentationViewer", "DMBDocumentationViewer"),
                    CreateApiNamespaceAction("DMBDocumentationImprovementByAI API", "DMBDocumentationImprovementByAI", "DMBDocumentationImprovementByAI"),
                    CreateApiNamespaceAction("DMBSearchBuilder API", "DMBSearchBuilder", "DMBSearchBuilder"),
                    CreateApiNamespaceAction("DMBSearchViewer API", "DMBSearchViewer", "DMBSearchViewer")
                );
        }

        internal static SideBarSectionComponent CreateBuilderSection(string? currentController, string? currentAction)
        {
            return new SideBarSectionComponent("DocumentationBuilder")
                .Add(
                    ActionItemFactory.Group("General", IconStruct.Bootstrap("bi-info-circle"))
                        .AddItems(
                            CreateModuleAction("DocumentationBuilder", "Introduction", "Introduction", "bi-info-circle", currentController, currentAction),
                            CreateModuleAction("DocumentationBuilder", "Architecture", "Architecture", "bi-diagram-3", currentController, currentAction)
                        ),
                    ActionItemFactory.Group("Usage", IconStruct.Bootstrap("bi-terminal"))
                        .AddItems(
                            CreateModuleAction("DocumentationBuilder", "GettingStarted", "Getting Started", "bi-play-circle", currentController, currentAction),
                            CreateModuleAction("DocumentationBuilder", "RenderingPipeline", "Rendering Pipeline", "bi-bezier2", currentController, currentAction)
                        )
                );
        }

        internal static SideBarSectionComponent CreateCoverageExampleSection()
        {
            return new SideBarSectionComponent("Coverage Example")
                .Add(
                    CreateCoverageNamespaceAction("Coverage cases", "DMBDocumentationTest.Coverage"),
                    CreateCoverageNamespaceAction("Secondary namespace", "DMBDocumentationTest.Secondary")
                );
        }

        private static AspRouteActionItem CreateCoverageNamespaceAction(string title, string namespaceName)
        {
            return ActionItemFactory.AspRoute("Documentation", "ShowNamespace")
                .AddRouteValue("groupName", "Documentation coverage")
                .AddRouteValue("packageId", "DMBDocumentationTest")
                .AddRouteValue("namespaceName", namespaceName)
                .SetTitle(title)
                .SetIcon(IconStruct.Bootstrap("bi-ui-checks-grid"));
        }

        private static AspRouteActionItem CreateModuleAction(
            string controller,
            string action,
            string title,
            string icon,
            string? currentController,
            string? currentAction
        )
        {
            bool active =
                string.Equals(currentController, controller, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(currentAction, action, StringComparison.OrdinalIgnoreCase);

            return ActionItemFactory.AspRoute(controller, action)
                .SetTitle(title)
                .SetIcon(IconStruct.Bootstrap(icon))
                .SetActive(active);
        }

        /// <summary>
        ///     Creates the root-level sidebar for the Documentation module.
        /// </summary>
        /// <returns>The root Documentation module sidebar component.</returns>
        internal static SideBarComponent CreateRootSidebar()
        {
            return CreateSidebar(
                "documentation_sidebar",
                "labs.documentation.sidebar",
                CreateBuilderSection(null, null),
                CreateViewerSection(null, null),
                CreateSearchBuilderSection(null, null),
                CreateSearchViewerSection(null, null),
                CreateCoverageExampleSection(),
                CreateApiReferenceSection()
            );
        }

        internal static SideBarSectionComponent CreateSearchBuilderSection(string? currentController, string? currentAction)
        {
            return new SideBarSectionComponent("SearchBuilder")
                .Add(
                    ActionItemFactory.Group("General", IconStruct.Bootstrap("bi-info-circle"))
                        .AddItems(
                            CreateModuleAction("SearchBuilder", "Introduction", "Introduction", "bi-info-circle", currentController, currentAction),
                            CreateModuleAction("SearchBuilder", "Architecture", "Architecture", "bi-diagram-3", currentController, currentAction)
                        ),
                    ActionItemFactory.Group("Usage", IconStruct.Bootstrap("bi-terminal"))
                        .AddItems(
                            CreateModuleAction("SearchBuilder", "GettingStarted", "Getting Started", "bi-play-circle", currentController, currentAction),
                            CreateModuleAction("SearchBuilder", "RenderingPipeline", "Rendering Pipeline", "bi-bezier2", currentController, currentAction)
                        )
                );
        }

        internal static SideBarSectionComponent CreateSearchViewerSection(string? currentController, string? currentAction)
        {
            return new SideBarSectionComponent("SearchViewer")
                .Add(
                    ActionItemFactory.Group("General", IconStruct.Bootstrap("bi-info-circle"))
                        .AddItems(
                            CreateModuleAction("SearchViewer", "Introduction", "Introduction", "bi-info-circle", currentController, currentAction),
                            CreateModuleAction("SearchViewer", "Architecture", "Architecture", "bi-diagram-3", currentController, currentAction)
                        ),
                    ActionItemFactory.Group("Usage", IconStruct.Bootstrap("bi-terminal"))
                        .AddItems(
                            CreateModuleAction("SearchViewer", "GettingStarted", "Getting Started", "bi-play-circle", currentController, currentAction),
                            CreateModuleAction("SearchViewer", "RenderingPipeline", "Rendering Pipeline", "bi-bezier2", currentController, currentAction)
                        )
                );
        }

        /// <summary>
        ///     Creates the contextual sidebar for the Documentation module with the active item highlighted.
        /// </summary>
        /// <param name="currentController">The name of the active controller.</param>
        /// <param name="currentAction">The name of the active action.</param>
        /// <returns>The Documentation module sidebar component.</returns>
        public static SideBarComponent CreateSidebar(string? currentController, string? currentAction)
        {
            return CreateSidebar(
                "documentation_sidebar",
                "labs.documentation.sidebar",
                CreateBuilderSection(currentController, currentAction),
                CreateViewerSection(currentController, currentAction),
                CreateSearchBuilderSection(currentController, currentAction),
                CreateSearchViewerSection(currentController, currentAction),
                CreateCoverageExampleSection(),
                CreateApiReferenceSection()
            );
        }

        private static SideBarComponent CreateSidebar(string id, string localStorageKey, params SideBarSectionComponent[] sections)
        {
            SideBarComponent sidebar = new SideBarComponent()
                .WithId(id)
                .WithLocalStorageKey(localStorageKey)
                .WithAutoExpandActivePath()
                .WithRememberExpandedState();

            foreach (SideBarSectionComponent section in sections)
            {
                sidebar.AddSection(section);
            }

            return sidebar;
        }

        internal static SideBarSectionComponent CreateViewerSection(string? currentController, string? currentAction)
        {
            return new SideBarSectionComponent("DocumentationViewer")
                .Add(
                    ActionItemFactory.Group("General", IconStruct.Bootstrap("bi-info-circle"))
                        .AddItems(
                            CreateModuleAction("DocumentationViewer", "Introduction", "Introduction", "bi-info-circle", currentController, currentAction),
                            CreateModuleAction("DocumentationViewer", "Architecture", "Architecture", "bi-diagram-3", currentController, currentAction)
                        ),
                    ActionItemFactory.Group("Usage", IconStruct.Bootstrap("bi-terminal"))
                        .AddItems(
                            CreateModuleAction("DocumentationViewer", "GettingStarted", "Getting Started", "bi-play-circle", currentController, currentAction),
                            CreateModuleAction("DocumentationViewer", "RenderingPipeline", "Rendering Pipeline", "bi-bezier2", currentController, currentAction)
                        )
                );
        }

        #endregion
    }
}
