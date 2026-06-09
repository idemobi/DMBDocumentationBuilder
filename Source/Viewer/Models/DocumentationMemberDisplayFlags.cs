#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Defines which granular member fields are rendered by the documentation member list.
    /// </summary>
    [Flags]
    public enum DocumentationMemberDisplayFlags
    {
        /// <summary>
        ///     Renders no optional member field.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Renders the member signature.
        /// </summary>
        Signature = 1,

        /// <summary>
        ///     Renders the member summary.
        /// </summary>
        Summary = 2,

        /// <summary>
        ///     Renders the obsolete badge when the member is marked obsolete.
        /// </summary>
        Obsolete = 4,

        /// <summary>
        ///     Renders the extension declaring type badge for extension methods.
        /// </summary>
        ExtensionType = 8,

        /// <summary>
        ///     Renders the link to the full reference documentation page.
        /// </summary>
        ReferenceLink = 16,

        /// <summary>
        ///     Renders the default compact documentation member view.
        /// </summary>
        Default = Signature | Summary | Obsolete | ExtensionType | ReferenceLink
    }
}