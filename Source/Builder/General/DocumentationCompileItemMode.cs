#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Defines which compile items are included when DocumentationBuilder builds a project compilation.
    /// </summary>
    public enum DocumentationCompileItemMode
    {
        /// <summary>
        ///     Uses only compile items physically located under the documented project directory.
        /// </summary>
        LocalProjectFilesOnly = 0,

        /// <summary>
        ///     Includes linked compile items referenced by the documented project file.
        /// </summary>
        IncludeLinkedFiles = 1
    }
}