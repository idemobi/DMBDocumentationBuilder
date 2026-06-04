#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBBootstrapBuilder;
using DMBPageBuilder;
using Microsoft.AspNetCore.Mvc;

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

        #endregion
    }
}
