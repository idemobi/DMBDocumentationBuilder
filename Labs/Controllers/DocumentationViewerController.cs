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
    ///     Provides documentation pages for <see cref="DMBDocumentationViewer.DMBDocumentationViewerConfiguration" />.
    /// </summary>
    public class DocumentationViewerController : RawBootstrapController
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
                new AspRouteActionItem("DocumentationViewer", nameof(Introduction)).SetTitle("DocumentationViewer").SetIcon(IconStruct.Bootstrap("bi-book")),
                new AspRouteActionItem("DocumentationViewer", actionName).SetTitle(ResolveBreadcrumbTitle(actionName)).SetIcon(ResolveBreadcrumbIcon(actionName))
            );
        }

        /// <summary>
        ///     Renders the DocumentationViewer architecture page.
        /// </summary>
        /// <returns>The architecture view.</returns>
        public IActionResult Architecture()
        {
            SetTitle("DocumentationViewer - Architecture");
            SetDescription("DocumentationViewer architecture");
            SetKeywords("DocumentationViewer", "DMBDocumentationViewer", "Architecture", "MCP", "ASP.NET Core");
            return View();
        }

        /// <summary>
        ///     Renders the DocumentationViewer getting started page.
        /// </summary>
        /// <returns>The getting started view.</returns>
        public IActionResult GettingStarted()
        {
            SetTitle("DocumentationViewer - Getting Started");
            SetDescription("DocumentationViewer getting started guide");
            SetKeywords("DocumentationViewer", "DMBDocumentationViewer", "Getting Started", "MCP", "ASP.NET Core");
            return View();
        }

        /// <summary>
        ///     Renders the DocumentationViewer introduction page.
        /// </summary>
        /// <returns>The introduction view.</returns>
        public IActionResult Introduction()
        {
            SetTitle("DocumentationViewer - Introduction");
            SetDescription("DocumentationViewer");
            SetKeywords("DocumentationViewer", "DMBDocumentationViewer", "NuGet", "MCP", "ASP.NET Core");
            return View();
        }

        /// <summary>
        ///     Configures the DocumentationViewer module sidebar before rendering an action.
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
        ///     Renders the DocumentationViewer rendering pipeline page.
        /// </summary>
        /// <returns>The rendering pipeline view.</returns>
        public IActionResult RenderingPipeline()
        {
            SetTitle("DocumentationViewer - Rendering Pipeline");
            SetDescription("DocumentationViewer rendering pipeline");
            SetKeywords("DocumentationViewer", "DMBDocumentationViewer", "Rendering Pipeline", "MCP", "ASP.NET Core");
            return View();
        }

        #endregion
    }
}
