#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Defines common display presets for rendered documentation members.
    /// </summary>
    public enum DocumentationMemberDisplayMode
    {
        /// <summary>
        ///     Uses the explicit <see cref="DocumentationMemberDisplayFlags" /> configuration.
        /// </summary>
        Default,

        /// <summary>
        ///     Renders only member signatures.
        /// </summary>
        SignatureOnly,

        /// <summary>
        ///     Renders member signatures with their summary description.
        /// </summary>
        SignatureAndDescription
    }
}