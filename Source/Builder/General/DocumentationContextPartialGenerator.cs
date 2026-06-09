#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Text;

#endregion

namespace DMBDocumentationBuilder
{
    internal static class DocumentationContextPartialGenerator
    {
        #region Static fields and properties

        private static readonly string[] GroupPartialNames =
        [
            "_At_Begin_Of_Group.html",
            "_After_Introduction.html",
            "_At_End_Of_Group.html"
        ];

        private static readonly string[] NamespacePartialNames =
        [
            "_At_Begin_Of_Namespace.html",
            "_After_Introduction.html",
            "_After_Classes.html",
            "_After_Structs.html",
            "_After_Enums.html",
            "_After_Interfaces.html",
            "_After_Records.html",
            "_At_End_Of_Namespace.html"
        ];

        #endregion

        #region Static methods

        private static string BuildPlaceholderPartialContent(string filePath, string partialName)
        {
            string label = partialName.Replace(".html", string.Empty);

            return
                $@"<!-- Custom documentation slot -->
<!-- Slot: {partialName} -->
<!-- {partialName} -->
<div class=""badge rounded-pill text-bg-info theme-debug-only my-2""> You can edit {filePath} </div>
";
        }

        /// <summary>
        ///     Ensures that group customization partial files exist for generated documentation pages.
        /// </summary>
        /// <param name="customizeRootDirectory">The customizeRootDirectory value used by the documentation generation operation.</param>
        /// <param name="groupName">The groupName value used by the documentation generation operation.</param>
        public static void EnsureGroupPartials(
            string customizeRootDirectory,
            string groupName
        )
        {
            if (string.IsNullOrWhiteSpace(customizeRootDirectory)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(customizeRootDirectory));

            if (string.IsNullOrWhiteSpace(groupName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(groupName));

            string groupDirectory = Path.Combine(
                customizeRootDirectory,
                "Groups",
                DocumentationPathHelper.ToSafeName(groupName));

            Directory.CreateDirectory(groupDirectory);

            foreach (string partialName in GroupPartialNames)
            {
                string filePath = Path.Combine(groupDirectory, partialName);

                if (!File.Exists(filePath)) File.WriteAllText(filePath, BuildPlaceholderPartialContent("~/Documentation/Customize/Groups/" + groupName + "/" + partialName, partialName), Encoding.UTF8);
            }
        }

        /// <summary>
        ///     Ensures that namespace customization partial files exist for generated documentation pages.
        /// </summary>
        /// <param name="customizeRootDirectory">The customizeRootDirectory value used by the documentation generation operation.</param>
        /// <param name="namespaceName">The namespaceName value used by the documentation generation operation.</param>
        public static void EnsureNamespacePartials(
            string customizeRootDirectory,
            string namespaceName
        )
        {
            if (string.IsNullOrWhiteSpace(customizeRootDirectory)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(customizeRootDirectory));

            if (string.IsNullOrWhiteSpace(namespaceName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(namespaceName));

            string namespaceDirectory = Path.Combine(
                customizeRootDirectory,
                "Namespaces",
                DocumentationPathHelper.NamespaceToPath(namespaceName));

            Directory.CreateDirectory(namespaceDirectory);

            foreach (string partialName in NamespacePartialNames)
            {
                string filePath = Path.Combine(namespaceDirectory, partialName);

                if (!File.Exists(filePath)) File.WriteAllText(filePath, BuildPlaceholderPartialContent("~/Documentation/Customize/Namespaces/" + namespaceName + "/" + partialName, partialName), Encoding.UTF8);
            }
        }

        #endregion
    }
}
