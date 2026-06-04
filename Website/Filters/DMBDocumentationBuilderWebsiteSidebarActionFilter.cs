#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBBootstrapBuilder;
using DMBDocumentationBuilderLabs.Navigation;
using DMBPageBuilder;
using Microsoft.AspNetCore.Mvc.Filters;

#endregion

namespace DMBDocumentationBuilderWebsite;

/// <summary>
///     Applies local DMBDocumentationBuilder labs sidebar and breadcrumb fragments to website pages.
/// </summary>
internal sealed class DMBDocumentationBuilderWebsiteSidebarActionFilter : IActionFilter
{
    #region Instance methods

    /// <inheritdoc />
    public void OnActionExecuted(ActionExecutedContext context)
    {
    }

    /// <inheritdoc />
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.Controller is not RawBootstrapController controller)
        {
            return;
        }

        string? currentController = context.RouteData.Values["controller"]?.ToString();
        string? currentAction = context.RouteData.Values["action"]?.ToString();

        if (!DMBDocumentationBuilderLabsNavigationAgent.IsModuleController(currentController))
        {
            return;
        }

        string actionName = string.IsNullOrWhiteSpace(currentAction)
            ? DMBDocumentationBuilderLabsNavigationAgent.ResolveControllerDefaultAction(currentController)
            : currentAction;

        controller.SetSidebar(DMBDocumentationBuilderLabsNavigationAgent.CreateSidebar(currentController, actionName));
        DMBDocumentationBuilderLabsNavigationAgent.AddBreadcrumb(controller, currentController, actionName);
    }

    #endregion
}
