#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Defines how project references are followed during documentation generation.
    /// </summary>
    public enum DocumentationProjectReferenceMode
    {
        /// <summary>
        ///     Generates documentation only for the explicitly configured project.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Generates documentation for the configured project and its direct project references.
        /// </summary>
        DirectOnly = 1,

        /// <summary>
        ///     Generates documentation for the configured project and project references discovered recursively.
        /// </summary>
        Recursive = 2
    }
}