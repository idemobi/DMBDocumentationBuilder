#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using DMBBootstrapBuilder;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace DMBDocumentationBuilderLabs.Controllers
{
    /// <summary>
    ///     Provides documentation pages for <see cref="DMBDocumentationViewer.DocumentationViewerConfiguration" />.
    /// </summary>
    public class DocumentationViewerController : RawBootstrapController
    {
        #region Instance methods

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