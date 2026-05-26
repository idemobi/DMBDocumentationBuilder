#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationPartialGenerator.cs create at 2026/04/12 12:04:31
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    internal static class DocumentationPartialGenerator
    {
        #region Static fields and properties

        private static readonly string[] PartialNames =
        [
            "_Before_Page.html",

            "_After_Header.html",

            "_After_Introduction.html",

            "_After_Metadata.html",
            "_After_Declaration.html",
            "_After_ExtensionMethods.html",
            "_After_Fields.html",
            "_After_Properties.html",
            "_After_Methods.html",
            "_After_TypeParameters.html",
            "_After_Constructors.html",
            "_After_Remarks.html",
            "_After_Events.html",
            "_After_Example.html",
            "_After_SeeAlso.html",

            "_After_Page.html",
        ];

        #endregion

        #region Static methods

        /// <summary>
        /// Ensures that shared documentation partial files exist in the host project.
        /// </summary>
        /// <param name="customizeRootDirectory">The customizeRootDirectory value used by the documentation generation operation.</param>
        /// <param name="namespaceName">The namespaceName value used by the documentation generation operation.</param>
        /// <param name="objectName">The objectName value used by the documentation generation operation.</param>
        public static void EnsurePartials(
            string customizeRootDirectory,
            string namespaceName,
            string objectName
        )
        {
            string namespaceFolder = DocumentationPathHelper.NamespaceToFolder(namespaceName);

            string finalDirectory = Path.Combine(
                customizeRootDirectory,
                namespaceFolder,
                objectName);

            Directory.CreateDirectory(finalDirectory);

            foreach (string partialName in PartialNames)
            {
                string filePath = Path.Combine(finalDirectory, partialName);

                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, GenerateDefaultContent(namespaceName, objectName, partialName));
                }
            }
        }

        private static string GenerateDefaultContent(string namespaceName, string objectName, string partialName)
        {
            return
                $@"<!-- Custom documentation slot -->
<!-- Namespace: {namespaceName} -->
<!-- Object: {objectName} -->
<!-- Slot: {partialName} -->

<div class=""badge rounded-pill text-bg-info theme-debug-only my-2""> You can edit ~/Documentation/Customize/{namespaceName}/{objectName}/{partialName} </div>
";
        }

        #endregion
    }
}