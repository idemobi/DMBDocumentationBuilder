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
    ///     Provides documentation pages for the DMBDocumentationBuilder package.
    /// </summary>
    public class DocumentationBuilderController : RawBootstrapController
    {
        #region Instance methods

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
