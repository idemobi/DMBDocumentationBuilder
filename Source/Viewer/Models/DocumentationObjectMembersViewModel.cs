#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Represents the compact member list rendered by the documentation object members component.
    /// </summary>
    public sealed class DocumentationObjectMembersViewModel
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the display flags used by the component.
        /// </summary>
        public DocumentationMemberDisplayFlags DisplayFlags { get; init; } = DocumentationMemberDisplayFlags.Default;

        /// <summary>
        ///     Gets the members rendered by the component.
        /// </summary>
        public IReadOnlyList<DocumentationMemberQueryResult> Members { get; init; } = [];

        /// <summary>
        ///     Gets the documented object name.
        /// </summary>
        public string ObjectName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the URL of the full generated documentation page for the object.
        /// </summary>
        public string ReferenceRoutePath { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the text displayed for the reference documentation link.
        /// </summary>
        public string ReferenceTitle { get; init; } = string.Empty;

        /// <summary>
        ///     Gets a value indicating whether the reference documentation link should be rendered.
        /// </summary>
        public bool ShowReferenceLink { get; init; }

        /// <summary>
        ///     Gets the optional component title.
        /// </summary>
        public string Title { get; init; } = string.Empty;

        #endregion
    }
}