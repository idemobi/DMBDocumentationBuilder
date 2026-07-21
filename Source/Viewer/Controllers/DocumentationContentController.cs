#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using Microsoft.AspNetCore.Hosting;

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Renders generated Markdown documentation content from the documentation database.
    /// </summary>
    public sealed class DocumentationContentController : DocumentationController
    {
        #region Instance constructors and destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DocumentationContentController" /> class.
        /// </summary>
        /// <param name="environment">Host environment used to resolve the generated documentation database path.</param>
        public DocumentationContentController(IWebHostEnvironment environment)
            : base(environment)
        {
        }

        #endregion
    }
}
