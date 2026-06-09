#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBBootstrapBuilder;
using DMBDocumentationViewer.Controllers;
using DMBPageBuilder;

#endregion

namespace DMBDocumentationBuilderLabs.Navigation;

/// <summary>
///     Provides reusable navigation fragments for DMBDocumentationBuilder labs hosts.
/// </summary>
/// <remarks>
///     The agent only builds Documentation-family menu, sidebar, and API fragments. Host websites remain
///     responsible for assembling these fragments into their navbar providers, sidebar filters, and
///     DocumentationViewer sidebar providers.
/// </remarks>
public static class DMBDocumentationBuilderLabsNavigationAgent
{
    #region Static methods

    /// <summary>
    ///     Adds the standard Documentation-family breadcrumb to a raw Bootstrap controller.
    /// </summary>
    /// <param name="controller">The controller receiving the breadcrumb.</param>
    /// <param name="currentController">The current MVC controller name.</param>
    /// <param name="currentAction">The current MVC action name.</param>
    public static void AddBreadcrumb(RawBootstrapController controller, string? currentController, string? currentAction)
    {
        string controllerName = string.IsNullOrWhiteSpace(currentController) ? "DocumentationHome" : currentController;
        string actionName = string.IsNullOrWhiteSpace(currentAction)
            ? ResolveControllerDefaultAction(controllerName)
            : currentAction;

        controller.AddBreadcrumb(
            ActionItemFactory.Url("Home", "/", IconStruct.Bootstrap("bi-house")),
            ActionItemFactory.AspRoute("DocumentationHome", "Index")
                .SetTitle("Documentation")
                .SetIcon(IconStruct.Bootstrap("bi-book")),
            ActionItemFactory.AspRoute(controllerName, ResolveControllerDefaultAction(controllerName))
                .SetTitle(ResolveControllerTitle(controllerName))
                .SetIcon(ResolveControllerIcon(controllerName)),
            ActionItemFactory.AspRoute(controllerName, actionName)
                .SetTitle(ResolveActionTitle(actionName))
                .SetIcon(ResolveActionIcon(actionName))
        );
    }

    private static AspRouteActionItem CreateApiNamespaceAction(string title, string groupName, string packageId, string namespaceName)
    {
        return ActionItemFactory.AspRoute("Documentation", "ShowNamespace")
            .AddRouteValue("groupName", groupName)
            .AddRouteValue("packageId", packageId)
            .AddRouteValue("namespaceName", namespaceName)
            .SetTitle(title)
            .SetIcon(IconStruct.Bootstrap("bi-journal-code"));
    }

    /// <summary>
    ///     Creates the Documentation-family API sidebar section.
    /// </summary>
    /// <param name="title">The section title shown in the sidebar.</param>
    /// <returns>The configured <see cref="SideBarSectionComponent" /> containing DocumentationViewer API links.</returns>
    public static SideBarSectionComponent CreateApiSidebarSection(string title = "API Reference")
    {
        return new SideBarSectionComponent(title)
            .Add(
                CreateApiNamespaceAction("DMBDocumentationBuilder API", "NuGet", "DMBDocumentationBuilder", "DMBDocumentationBuilder"),
                CreateApiNamespaceAction("DMBDocumentationViewer API", "NuGet", "DMBDocumentationViewer", "DMBDocumentationViewer"),
                CreateApiNamespaceAction("DMBDocumentationTest API", "Documentation coverage", "DMBDocumentationTest", "DMBDocumentationTest.Coverage"),
                CreateApiNamespaceAction("DMBDocumentationImprovementByAI API", "NuGet", "DMBDocumentationImprovementByAI", "DMBDocumentationImprovementByAI"),
                CreateApiNamespaceAction("DMBExampleToRaw API", "NuGet", "DMBExampleToRaw", "DMBExampleToRaw")
            );
    }

    private static AspRouteActionItem CreateLabsAction(
        string controller,
        string action,
        string title,
        string icon,
        string? currentController = null,
        string? currentAction = null
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
    ///     Creates the Documentation-family navbar menu group.
    /// </summary>
    /// <returns>The configured <see cref="GroupActionItem" /> containing Documentation labs page links.</returns>
    public static GroupActionItem CreateMenuGroup()
    {
        return ActionItemFactory.Group("DMB Documentation", IconStruct.Bootstrap("bi-book"))
            .AddItems(
                CreateLabsAction("DocumentationHome", "Index", "Overview", "bi-compass"),
                ActionItemFactory.Group("DMBDocumentationBuilder", IconStruct.Bootstrap("bi-file-earmark-richtext"))
                    .AddItems(
                        CreateLabsAction("DocumentationBuilder", "Introduction", "Introduction", "bi-info-circle"),
                        CreateLabsAction("DocumentationBuilder", "GettingStarted", "Getting Started", "bi-play-circle"),
                        CreateLabsAction("DocumentationBuilder", "Architecture", "Architecture", "bi-diagram-3"),
                        CreateLabsAction("DocumentationBuilder", "RenderingPipeline", "Rendering Pipeline", "bi-bezier2")
                    ),
                ActionItemFactory.Group("DMBDocumentationViewer", IconStruct.Bootstrap("bi-eye"))
                    .AddItems(
                        CreateLabsAction("DocumentationViewer", "Introduction", "Introduction", "bi-info-circle"),
                        CreateLabsAction("DocumentationViewer", "GettingStarted", "Getting Started", "bi-play-circle"),
                        CreateLabsAction("DocumentationViewer", "Architecture", "Architecture", "bi-diagram-3"),
                        CreateLabsAction("DocumentationViewer", "RenderingPipeline", "Rendering Pipeline", "bi-bezier2")
                    )
            );
    }

    /// <summary>
    ///     Creates the full Documentation-family sidebar component.
    /// </summary>
    /// <param name="currentController">The current MVC controller name used to mark active links.</param>
    /// <param name="currentAction">The current MVC action name used to mark active links.</param>
    /// <param name="includeApiSection">Whether to include the DocumentationViewer API sidebar section.</param>
    /// <param name="sidebarId">The HTML identifier applied to the sidebar component.</param>
    /// <param name="localStorageKey">The browser local-storage key used for sidebar state.</param>
    /// <returns>The configured <see cref="SideBarComponent" />.</returns>
    public static SideBarComponent CreateSidebar(
        string? currentController,
        string? currentAction,
        bool includeApiSection = true,
        string sidebarId = "documentation_sidebar",
        string localStorageKey = "dmbdocumentationbuilder.labs.sidebar"
    )
    {
        SideBarComponent sidebar = new SideBarComponent()
            .WithId(sidebarId)
            .WithLocalStorageKey(localStorageKey)
            .WithAutoExpandActivePath()
            .WithRememberExpandedState();

        sidebar.AddSection(CreateSidebarSection(currentController, currentAction));

        if (includeApiSection)
        {
            sidebar.AddSection(CreateApiSidebarSection());
        }

        return sidebar;
    }

    /// <summary>
    ///     Creates the Documentation-family labs pages sidebar section.
    /// </summary>
    /// <param name="currentController">The current MVC controller name used to mark active links.</param>
    /// <param name="currentAction">The current MVC action name used to mark active links.</param>
    /// <returns>The configured <see cref="SideBarSectionComponent" />.</returns>
    public static SideBarSectionComponent CreateSidebarSection(string? currentController, string? currentAction)
    {
        return new SideBarSectionComponent("DMB Documentation")
            .Add(
                CreateLabsAction("DocumentationHome", "Index", "Overview", "bi-compass", currentController, currentAction),
                ActionItemFactory.Group("DMBDocumentationBuilder", IconStruct.Bootstrap("bi-file-earmark-richtext"))
                    .AddItems(
                        CreateLabsAction("DocumentationBuilder", "Introduction", "Introduction", "bi-info-circle", currentController, currentAction),
                        CreateLabsAction("DocumentationBuilder", "GettingStarted", "Getting Started", "bi-play-circle", currentController, currentAction),
                        CreateLabsAction("DocumentationBuilder", "Architecture", "Architecture", "bi-diagram-3", currentController, currentAction),
                        CreateLabsAction("DocumentationBuilder", "RenderingPipeline", "Rendering Pipeline", "bi-bezier2", currentController, currentAction)
                    ),
                ActionItemFactory.Group("DMBDocumentationViewer", IconStruct.Bootstrap("bi-eye"))
                    .AddItems(
                        CreateLabsAction("DocumentationViewer", "Introduction", "Introduction", "bi-info-circle", currentController, currentAction),
                        CreateLabsAction("DocumentationViewer", "GettingStarted", "Getting Started", "bi-play-circle", currentController, currentAction),
                        CreateLabsAction("DocumentationViewer", "Architecture", "Architecture", "bi-diagram-3", currentController, currentAction),
                        CreateLabsAction("DocumentationViewer", "RenderingPipeline", "Rendering Pipeline", "bi-bezier2", currentController, currentAction)
                    )
            );
    }

    /// <summary>
    ///     Determines whether a controller belongs to the DMBDocumentationBuilder labs module.
    /// </summary>
    /// <param name="controllerName">The MVC controller name to evaluate.</param>
    /// <returns>
    ///     <see langword="true" /> when the controller is part of the Documentation labs module; otherwise,
    ///     <see langword="false" />.
    /// </returns>
    public static bool IsModuleController(string? controllerName)
    {
        return string.Equals(controllerName, "DocumentationHome", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(controllerName, "DocumentationBuilder", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(controllerName, "DocumentationViewer", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Resolves the Bootstrap icon for a Documentation-family labs action.
    /// </summary>
    /// <param name="actionName">The MVC action name to resolve.</param>
    /// <returns>The icon value represented as an <see cref="IconStruct" />.</returns>
    public static IconStruct ResolveActionIcon(string? actionName)
    {
        return actionName switch
        {
            "GettingStarted" => IconStruct.Bootstrap("bi-play-circle"),
            "Architecture" => IconStruct.Bootstrap("bi-diagram-3"),
            "RenderingPipeline" => IconStruct.Bootstrap("bi-bezier2"),
            "Index" => IconStruct.Bootstrap("bi-compass"),
            _ => IconStruct.Bootstrap("bi-info-circle")
        };
    }

    /// <summary>
    ///     Resolves the display title for a Documentation-family labs action.
    /// </summary>
    /// <param name="actionName">The MVC action name to resolve.</param>
    /// <returns>The display title for the action.</returns>
    public static string ResolveActionTitle(string? actionName)
    {
        return actionName switch
        {
            "GettingStarted" => "Getting Started",
            "Architecture" => "Architecture",
            "RenderingPipeline" => "Rendering Pipeline",
            "Index" => "Overview",
            _ => "Introduction"
        };
    }

    /// <summary>
    ///     Resolves the default action for a Documentation-family labs controller.
    /// </summary>
    /// <param name="controllerName">The MVC controller name to resolve.</param>
    /// <returns>The default action name for the controller.</returns>
    public static string ResolveControllerDefaultAction(string? controllerName)
    {
        return string.Equals(controllerName, "DocumentationHome", StringComparison.OrdinalIgnoreCase)
            ? "Index"
            : "Introduction";
    }

    /// <summary>
    ///     Resolves the Bootstrap icon for a Documentation-family labs controller.
    /// </summary>
    /// <param name="controllerName">The MVC controller name to resolve.</param>
    /// <returns>The icon value represented as an <see cref="IconStruct" />.</returns>
    public static IconStruct ResolveControllerIcon(string? controllerName)
    {
        return controllerName switch
        {
            "DocumentationBuilder" => IconStruct.Bootstrap("bi-file-earmark-richtext"),
            "DocumentationViewer" => IconStruct.Bootstrap("bi-eye"),
            _ => IconStruct.Bootstrap("bi-book")
        };
    }

    /// <summary>
    ///     Resolves the display title for a Documentation-family labs controller.
    /// </summary>
    /// <param name="controllerName">The MVC controller name to resolve.</param>
    /// <returns>The display title for the controller.</returns>
    public static string ResolveControllerTitle(string? controllerName)
    {
        return controllerName switch
        {
            "DocumentationBuilder" => "DMBDocumentationBuilder",
            "DocumentationViewer" => "DMBDocumentationViewer",
            _ => "Documentation"
        };
    }

    #endregion
}

/// <summary>
///     Adapts DMBDocumentationBuilder labs navigation fragments to the DocumentationViewer sidebar provider contract.
/// </summary>
public sealed class DMBDocumentationBuilderLabsDocumentationSidebarProvider : IDocumentationSidebarProvider
{
    #region Instance methods

    #region From interface IDocumentationSidebarProvider

    /// <inheritdoc />
    public SideBarSectionComponent CreateGroupSidebar(string groupName)
    {
        return DMBDocumentationBuilderLabsNavigationAgent.CreateApiSidebarSection(groupName);
    }

    /// <inheritdoc />
    public SideBarSectionComponent CreateNamespaceSidebar(string groupName, string packageId, string version, string namespaceName)
    {
        return DMBDocumentationBuilderLabsNavigationAgent.CreateApiSidebarSection(groupName);
    }

    /// <inheritdoc />
    public SideBarSectionComponent CreateRootSidebar()
    {
        return DMBDocumentationBuilderLabsNavigationAgent.CreateApiSidebarSection();
    }

    #endregion

    #endregion
}
