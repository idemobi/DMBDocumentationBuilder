#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Represents one available documentation version for the current rendered page.
    /// </summary>
    public sealed class DocumentationVersionNavigationItem
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets a value indicating whether this version is the currently rendered version.
        /// </summary>
        public bool IsCurrent { get; init; }

        /// <summary>
        ///     Gets the URL that renders this version.
        /// </summary>
        public string Url { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the displayed documentation version.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        #endregion
    }

    /// <summary>
    ///     Provides the model used to render a documentation object page from database content.
    /// </summary>
    public sealed class DocumentationRenderPageViewModel
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the optional AI-generated documentation results associated with the rendered object.
        /// </summary>
        public List<DocumentationAIResultViewModel> AIResults { get; init; } = [];

        /// <summary>
        ///     Gets the builder version or identifier stored with the rendered documentation content.
        /// </summary>
        public string BuilderVersion { get; init; } = string.Empty;

        /// <summary>
        ///     Gets a value indicating whether the page has alternate documentation versions.
        /// </summary>
        public bool HasVersionNavigation => Versions.Count > 1;

        /// <summary>
        ///     Gets the generated HTML content displayed by the documentation view.
        /// </summary>
        public string HtmlContent { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the documented object name represented by the rendered page.
        /// </summary>
        public string ObjectName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the documented object type represented by the rendered page.
        /// </summary>
        public string ObjectType { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the package identifier represented by the rendered page.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the currently rendered documentation version.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the available versions for the current documentation page.
        /// </summary>
        public IReadOnlyList<DocumentationVersionNavigationItem> Versions { get; init; } = [];

        #endregion
    }
}