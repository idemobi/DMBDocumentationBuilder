#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationPathHelper.cs create at 2026/04/12 12:04:31
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents the DocumentationPathHelper type used by DocumentationBuilder generation.
    /// </summary>
    public static class DocumentationPathHelper
    {
        #region Static methods

        /// <summary>
        /// Converts a namespace name to the generated documentation folder name.
        /// </summary>
        /// <param name="namespaceName">The namespaceName value used by the documentation generation operation.</param>
        /// <returns>The NamespaceToFolder result produced by DocumentationBuilder generation.</returns>
        public static string NamespaceToFolder(string namespaceName)
        {
            return namespaceName
                .Replace('.', Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar)
                .Replace('/', Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Converts a namespace name to a generated documentation path segment.
        /// </summary>
        /// <param name="namespaceName">The namespaceName value used by the documentation generation operation.</param>
        /// <returns>The NamespaceToPath result produced by DocumentationBuilder generation.</returns>
        public static string NamespaceToPath(string namespaceName)
        {
            return namespaceName.Replace('.', '/');
        }

        /// <summary>
        /// Converts a documentation label to a safe generated file or route name.
        /// </summary>
        /// <param name="value">The value value used by the documentation generation operation.</param>
        /// <returns>The ToSafeName result produced by DocumentationBuilder generation.</returns>
        public static string ToSafeName(string value)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            return new string(value.Select(c => invalidChars.Contains(c) ? '_' : c).ToArray());
        }

        #endregion
    }
}