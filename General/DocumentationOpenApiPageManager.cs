#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationOpenApiPageManager.cs create at 2026/05/22 00:00:00
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Persists configured OpenAPI documents and operation indexes into the documentation database.
    /// </summary>
    public static class DocumentationOpenApiPageManager
    {
        #region Static methods

        /// <summary>
        /// Generates OpenAPI document and operation records for the supplied documentation groups.
        /// </summary>
        /// <param name="groups">The documentation groups whose projects may contain OpenAPI descriptors.</param>
        /// <param name="sqliteDatabasePath">The SQLite database path that receives OpenAPI records.</param>
        public static void Generate(
            IEnumerable<DocumentationGroupDescriptor> groups,
            string sqliteDatabasePath
        )
        {
            foreach (DocumentationGroupDescriptor group in groups)
            {
                foreach (DocumentationProjectDescriptor project in group.Projects)
                {
                    IReadOnlyList<DocumentationOpenApiDocumentItem> documents = DocumentationOpenApiExtractor.Extract(group.GroupName, project);

                    DocumentationDatabaseManager.DeleteOpenApiDocuments(
                        sqliteDatabasePath,
                        project.PackageId,
                        project.Version);

                    foreach (DocumentationOpenApiDocumentItem document in documents)
                    {
                        DocumentationDatabaseManager.ReplaceOpenApiDocument(
                            sqliteDatabasePath,
                            document);
                    }
                }
            }
        }

        #endregion
    }
}
