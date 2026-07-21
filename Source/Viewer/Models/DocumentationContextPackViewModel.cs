#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Provides the model rendered by the context pack builder form.
    /// </summary>
    public sealed class DocumentationContextPackViewModel
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the documentation group filter applied to the form.
        /// </summary>
        public string GroupName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets a value indicating whether selectable options are available.
        /// </summary>
        public bool HasOptions => Options.Count > 0;

        /// <summary>
        ///     Gets the namespace used to preserve the documentation sidebar scope.
        /// </summary>
        public string NamespaceName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the selectable latest-version context options.
        /// </summary>
        public IReadOnlyList<DocumentationContextPackOption> Options { get; init; } = [];

        /// <summary>
        ///     Gets the package filter applied to the form.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;

        #endregion
    }
}
