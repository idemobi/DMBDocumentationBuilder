#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationCompileItemMode.cs create at 2026/04/13 14:04:22
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Defines which compile items are included when DocumentationBuilder builds a project compilation.
    /// </summary>
    public enum DocumentationCompileItemMode
    {
        /// <summary>
        /// Uses only compile items physically located under the documented project directory.
        /// </summary>
        LocalProjectFilesOnly = 0,

        /// <summary>
        /// Includes linked compile items referenced by the documented project file.
        /// </summary>
        IncludeLinkedFiles = 1
    }
}
