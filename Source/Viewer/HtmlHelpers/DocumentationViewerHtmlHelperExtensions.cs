#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using Microsoft.AspNetCore.Mvc.Rendering;

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Provides Razor HTML helpers for rendering documentation viewer components.
    /// </summary>
    public static class DocumentationViewerHtmlHelperExtensions
    {
        #region Static methods

        /// <summary>
        ///     Creates a fluent builder for rendering granular members of one documented object.
        /// </summary>
        /// <param name="htmlHelper">The current Razor HTML helper.</param>
        /// <param name="objectName">Exact documented object name.</param>
        /// <returns>A configured <see cref="DocumentationObjectMembersBuilder" /> instance.</returns>
        public static DocumentationObjectMembersBuilder DocumentationObjectMembers(this IHtmlHelper htmlHelper, string objectName)
        {
            return new DocumentationObjectMembersBuilder(htmlHelper, objectName);
        }

        #endregion
    }
}