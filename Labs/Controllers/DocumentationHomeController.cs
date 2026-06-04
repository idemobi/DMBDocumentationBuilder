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
    ///     Provides the site-owned documentation overview page.
    /// </summary>
    public class DocumentationHomeController : RawBootstrapController
    {
        #region Instance methods

        /// <summary>
        ///     Renders the documentation overview page.
        /// </summary>
        /// <returns>The documentation overview view.</returns>
        public IActionResult Index()
        {
            SetDescription("Documentation concepts and navigation overview.");
            SetTitle("Documentation");
            SetKeywords("Documentation", "DocumentationBuilder", "DocumentationViewer", "SearchBuilder", "SearchViewer", "API Reference");
            return View();
        }

        /// <summary>
        ///     Configures the documentation overview sidebar and breadcrumb.
        /// </summary>
        /// <param name="context">The current action execution context.</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            SetSidebar(DocumentationModuleSidebarAgent.CreateRootSidebar());
            AddBreadcrumb(
                new UrlActionItem().WithUrl("/").SetTitle("Home").SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_house)),
                new AspRouteActionItem("DocumentationHome", nameof(Index)).SetTitle("Documentation").SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_book))
            );
        }

        #endregion
    }
}
