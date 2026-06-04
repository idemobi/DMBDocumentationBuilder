#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBBootstrapBuilder;
using DMBPageBuilder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

#endregion

namespace DMBDocumentationBuilderLabs.Controllers
{
    /// <summary>
    ///     Provides documentation pages for the DMBDocumentationBuilder package.
    /// </summary>
    public class DocumentationBuilderController : RawBootstrapController
    {
        #region Static methods

        private static IconStruct ResolveBreadcrumbIcon(string actionName)
        {
            return actionName switch
            {
                nameof(GettingStarted) => IconStruct.Bootstrap("bi-play-circle"),
                nameof(Architecture) => IconStruct.Bootstrap("bi-diagram-3"),
                nameof(RenderingPipeline) => IconStruct.Bootstrap("bi-bezier2"),
                _ => IconStruct.Bootstrap("bi-info-circle")
            };
        }

        private static string ResolveBreadcrumbTitle(string actionName)
        {
            return actionName switch
            {
                nameof(GettingStarted) => "Getting Started",
                nameof(Architecture) => "Architecture",
                nameof(RenderingPipeline) => "Rendering Pipeline",
                _ => "Introduction"
            };
        }

        #endregion

        #region Instance methods

        private void AddInformationBreadcrumb(string? currentAction)
        {
            string actionName = string.IsNullOrWhiteSpace(currentAction) ? nameof(Introduction) : currentAction;

            AddBreadcrumb(
                new UrlActionItem().WithUrl("/").SetTitle("Home").SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_house)),
                new AspRouteActionItem("DocumentationHome", "Index").SetTitle("Documentation").SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_book)),
                new AspRouteActionItem("DocumentationBuilder", nameof(Introduction)).SetTitle("DocumentationBuilder").SetIcon(IconStruct.Bootstrap("bi-file-earmark-richtext")),
                new AspRouteActionItem("DocumentationBuilder", actionName).SetTitle(ResolveBreadcrumbTitle(actionName)).SetIcon(ResolveBreadcrumbIcon(actionName))
            );
        }

        /// <summary>
        ///     Renders the DocumentationBuilder architecture page.
        /// </summary>
        /// <returns>The architecture view.</returns>
        public IActionResult Architecture()
        {
            SetTitle("DocumentationBuilder - Architecture");
            SetDescription("DocumentationBuilder architecture");
            SetKeywords("DocumentationBuilder", "DMBDocumentationBuilder", "Architecture", "Roslyn", "SQLite");
            return View();
        }

        /// <summary>
        ///     Renders the DocumentationBuilder getting started page.
        /// </summary>
        /// <returns>The getting started view.</returns>
        public IActionResult GettingStarted()
        {
            SetTitle("DocumentationBuilder - Getting Started");
            SetDescription("DocumentationBuilder getting started guide");
            SetKeywords("DocumentationBuilder", "DMBDocumentationBuilder", "Getting Started", "NuGet");
            return View();
        }

        /// <summary>
        ///     Renders the DocumentationBuilder introduction page.
        /// </summary>
        /// <returns>The introduction view.</returns>
        public IActionResult Introduction()
        {
            SetTitle("DocumentationBuilder - Introduction");
            SetDescription("DocumentationBuilder");
            SetKeywords("DocumentationBuilder", "DMBDocumentationBuilder", "NuGet", "API Documentation");
            return View();
        }

        /// <summary>
        ///     Configures the DocumentationBuilder module sidebar before rendering an action.
        /// </summary>
        /// <param name="context">The current action execution context.</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            string? currentController = context.RouteData.Values["controller"]?.ToString();
            string? currentAction = context.RouteData.Values["action"]?.ToString();
            SetSidebar(DocumentationModuleSidebarAgent.CreateSidebar(currentController, currentAction));
            AddInformationBreadcrumb(currentAction);
        }

        /// <summary>
        ///     Renders the DocumentationBuilder rendering pipeline page.
        /// </summary>
        /// <returns>The rendering pipeline view.</returns>
        public IActionResult RenderingPipeline()
        {
            SetTitle("DocumentationBuilder - Rendering Pipeline");
            SetDescription("DocumentationBuilder rendering pipeline");
            SetKeywords("DocumentationBuilder", "DMBDocumentationBuilder", "Rendering Pipeline", "Roslyn", "SQLite");
            return View();
        }

        #endregion
    }
}
