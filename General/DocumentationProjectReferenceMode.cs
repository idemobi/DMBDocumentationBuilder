#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationProjectReferenceMode.cs create at 2026/04/13 14:04:43
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Defines how project references are followed during documentation generation.
    /// </summary>
    public enum DocumentationProjectReferenceMode
    {
        /// <summary>
        /// Generates documentation only for the explicitly configured project.
        /// </summary>
        None = 0,

        /// <summary>
        /// Generates documentation for the configured project and its direct project references.
        /// </summary>
        DirectOnly = 1,

        /// <summary>
        /// Generates documentation for the configured project and project references discovered recursively.
        /// </summary>
        Recursive = 2
    }
}
