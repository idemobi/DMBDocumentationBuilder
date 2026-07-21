#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Represents the selected context option fingerprints posted by the context pack builder form.
    /// </summary>
    public sealed class DocumentationContextPackDownloadRequest
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the documentation group filter used to rebuild the option list.
        /// </summary>
        public string GroupName { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the namespace used to preserve the documentation sidebar scope.
        /// </summary>
        public string NamespaceName { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the package filter used to rebuild the option list.
        /// </summary>
        public string PackageId { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the selected option fingerprints.
        /// </summary>
        public List<string> SelectedFingerprints { get; set; } = [];

        #endregion
    }
}
