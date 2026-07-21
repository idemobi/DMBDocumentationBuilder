#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Provides the model rendered by the DocumentationViewer context option catalog page.
    /// </summary>
    public sealed class DocumentationContextOptionsViewModel
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets a value indicating whether at least one context option is available.
        /// </summary>
        public bool HasOptions => Options.Count > 0;

        /// <summary>
        ///     Gets a value indicating whether alternate documentation versions are available.
        /// </summary>
        public bool HasVersionNavigation => Versions.Count > 1;

        /// <summary>
        ///     Gets the optional documentation group filter applied to the page.
        /// </summary>
        public string GroupName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the optional package filter applied to the page.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the optional namespace used to keep the documentation sidebar scope.
        /// </summary>
        public string NamespaceName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the context options available to read and copy from the viewer.
        /// </summary>
        public IReadOnlyList<DocumentationContextOption> Options { get; init; } = [];

        /// <summary>
        ///     Gets the optional version filter applied to the page.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the available documentation versions for the selected context option scope.
        /// </summary>
        public IReadOnlyList<DocumentationVersionNavigationItem> Versions { get; init; } = [];

        #endregion
    }
}
