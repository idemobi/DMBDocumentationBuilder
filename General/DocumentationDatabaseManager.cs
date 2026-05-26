#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationDatabaseManager.cs create at 2026/04/12 12:04:31
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using System.Text.Json;
using Microsoft.Data.Sqlite;

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents the DocumentationDatabaseManager type used by DocumentationBuilder generation.
    /// </summary>
    public static class DocumentationDatabaseManager
    {
        #region Static fields and properties

        private static readonly object TableCreationLock = new();

        private static readonly HashSet<string> TableCreatedDatabasePaths = new(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Static methods

        /// <summary>
        /// Ensures that the SQLite documentation metadata schema exists.
        /// </summary>
        /// <param name="sqliteDatabasePath">The sqliteDatabasePath value used by the documentation generation operation.</param>
        public static void EnsureTableCreated(string sqliteDatabasePath)
        {
            string resolvedDatabasePath = Path.GetFullPath(sqliteDatabasePath);

            lock (TableCreationLock)
            {
                if (TableCreatedDatabasePaths.Contains(resolvedDatabasePath)) return;

                using (var connection = new SqliteConnection($"Data Source={sqliteDatabasePath}"))
                {
                    connection.Open();

                    string createTableSql = @"
                CREATE TABLE IF NOT EXISTS DocumentationObjects
                (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PackageId TEXT NOT NULL,
                    Version TEXT NOT NULL,
                    NamespaceName TEXT NOT NULL,
                    ObjectName TEXT NOT NULL,
                    ObjectType TEXT NOT NULL,
                    RoutePath TEXT NOT NULL,
                    ModelInJson TEXT NOT NULL,
                    HtmlContent TEXT,
                    Builder TEXT NOT NULL DEFAULT '',
                    TechnicalKeywords TEXT NOT NULL DEFAULT '',
                    Keywords TEXT NOT NULL DEFAULT '',
                    CreatedUtc TEXT NOT NULL,
                    UpdatedUtc TEXT NOT NULL
                );

                CREATE UNIQUE INDEX IF NOT EXISTS IX_DocumentationObjects_Unique
                ON DocumentationObjects (PackageId, Version, NamespaceName, ObjectName, ObjectType);

                CREATE UNIQUE INDEX IF NOT EXISTS IX_DocumentationObjects_RoutePath
                ON DocumentationObjects (RoutePath);

                CREATE INDEX IF NOT EXISTS IX_DocumentationObjects_Package_Version_Namespace
                ON DocumentationObjects (PackageId, Version, NamespaceName);

                CREATE INDEX IF NOT EXISTS IX_DocumentationObjects_Package_Version_Type
                ON DocumentationObjects (PackageId, Version, ObjectType);

                CREATE INDEX IF NOT EXISTS IX_DocumentationObjects_Package_Version
                ON DocumentationObjects (PackageId, Version);

                CREATE INDEX IF NOT EXISTS IX_DocumentationObjects_Keywords
                ON DocumentationObjects (Keywords);

                CREATE INDEX IF NOT EXISTS IX_DocumentationObjects_TechnicalKeywords
                ON DocumentationObjects (TechnicalKeywords);

                CREATE TABLE IF NOT EXISTS DocumentationObjectSources
                (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PackageId TEXT NOT NULL,
                    Version TEXT NOT NULL,
                    NamespaceName TEXT NOT NULL,
                    ObjectName TEXT NOT NULL,
                    ObjectType TEXT NOT NULL,
                    SourceCode TEXT NOT NULL,
                    SourceFileCount INTEGER NOT NULL DEFAULT 0,
                    CreatedUtc TEXT NOT NULL,
                    UpdatedUtc TEXT NOT NULL,
                    UNIQUE (PackageId, Version, NamespaceName, ObjectName, ObjectType)
                );

                CREATE TABLE IF NOT EXISTS DocumentationSourceFiles
                (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PackageId TEXT NOT NULL,
                    Version TEXT NOT NULL,
                    ProjectFilePath TEXT NOT NULL,
                    ProjectDirectoryPath TEXT NOT NULL,
                    FilePath TEXT NOT NULL,
                    RelativeFilePath TEXT NOT NULL,
                    FileName TEXT NOT NULL,
                    PrimaryNamespaceName TEXT NOT NULL DEFAULT '',
                    NamespaceNamesJson TEXT NOT NULL DEFAULT '[]',
                    TypeNamesJson TEXT NOT NULL DEFAULT '[]',
                    Content TEXT NOT NULL,
                    ContentHash TEXT NOT NULL DEFAULT '',
                    CreatedUtc TEXT NOT NULL,
                    UpdatedUtc TEXT NOT NULL,
                    UNIQUE (PackageId, Version, RelativeFilePath)
                );

                CREATE INDEX IF NOT EXISTS IX_DocumentationSourceFiles_Package_Version
                ON DocumentationSourceFiles (PackageId, Version);

                CREATE INDEX IF NOT EXISTS IX_DocumentationSourceFiles_PrimaryNamespace
                ON DocumentationSourceFiles (PackageId, Version, PrimaryNamespaceName);

                CREATE INDEX IF NOT EXISTS IX_DocumentationSourceFiles_FileName
                ON DocumentationSourceFiles (FileName);

                CREATE TABLE IF NOT EXISTS DocumentationOpenApiDocuments
                (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PackageId TEXT NOT NULL,
                    Version TEXT NOT NULL,
                    DocumentName TEXT NOT NULL,
                    Title TEXT NOT NULL DEFAULT '',
                    Description TEXT NOT NULL DEFAULT '',
                    JsonContent TEXT NOT NULL,
                    RoutePath TEXT NOT NULL DEFAULT '',
                    CreatedUtc TEXT NOT NULL,
                    UpdatedUtc TEXT NOT NULL,
                    UNIQUE (PackageId, Version, DocumentName)
                );

                CREATE INDEX IF NOT EXISTS IX_DocumentationOpenApiDocuments_Package_Version
                ON DocumentationOpenApiDocuments (PackageId, Version);

                CREATE TABLE IF NOT EXISTS DocumentationOpenApiOperations
                (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PackageId TEXT NOT NULL,
                    Version TEXT NOT NULL,
                    DocumentName TEXT NOT NULL,
                    OperationId TEXT NOT NULL,
                    HttpMethod TEXT NOT NULL,
                    Path TEXT NOT NULL,
                    Summary TEXT NOT NULL DEFAULT '',
                    Description TEXT NOT NULL DEFAULT '',
                    TagsJson TEXT NOT NULL DEFAULT '[]',
                    ParametersJson TEXT NOT NULL DEFAULT '[]',
                    RequestBodyJson TEXT NOT NULL DEFAULT '{}',
                    ResponsesJson TEXT NOT NULL DEFAULT '{}',
                    SecurityJson TEXT NOT NULL DEFAULT '[]',
                    RoutePath TEXT NOT NULL DEFAULT '',
                    CreatedUtc TEXT NOT NULL,
                    UpdatedUtc TEXT NOT NULL,
                    UNIQUE (PackageId, Version, DocumentName, OperationId)
                );

                CREATE INDEX IF NOT EXISTS IX_DocumentationOpenApiOperations_Document
                ON DocumentationOpenApiOperations (PackageId, Version, DocumentName);

                CREATE INDEX IF NOT EXISTS IX_DocumentationOpenApiOperations_Path_Method
                ON DocumentationOpenApiOperations (Path, HttpMethod);

                CREATE TABLE IF NOT EXISTS DocumentationMembers
                (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PackageId TEXT NOT NULL,
                    Version TEXT NOT NULL,
                    GroupName TEXT NOT NULL DEFAULT '',
                    NamespaceName TEXT NOT NULL,
                    ObjectName TEXT NOT NULL,
                    ObjectType TEXT NOT NULL,
                    MemberKind TEXT NOT NULL,
                    MemberKey TEXT NOT NULL,
                    MemberName TEXT NOT NULL,
                    Signature TEXT NOT NULL,
                    SummaryHtml TEXT NOT NULL DEFAULT '',
                    RemarksHtml TEXT NOT NULL DEFAULT '',
                    ReturnsHtml TEXT NOT NULL DEFAULT '',
                    ValueHtml TEXT NOT NULL DEFAULT '',
                    ExampleHtml TEXT NOT NULL DEFAULT '',
                    Accessibility TEXT NOT NULL DEFAULT '',
                    IsStatic INTEGER NOT NULL DEFAULT 0,
                    IsAbstract INTEGER NOT NULL DEFAULT 0,
                    IsVirtual INTEGER NOT NULL DEFAULT 0,
                    IsOverride INTEGER NOT NULL DEFAULT 0,
                    IsSealed INTEGER NOT NULL DEFAULT 0,
                    IsReadOnly INTEGER NOT NULL DEFAULT 0,
                    IsConst INTEGER NOT NULL DEFAULT 0,
                    IsObsolete INTEGER NOT NULL DEFAULT 0,
                    ObsoleteMessage TEXT NOT NULL DEFAULT '',
                    ExtensionTypeName TEXT NOT NULL DEFAULT '',
                    ExtensionNamespaceName TEXT NOT NULL DEFAULT '',
                    ParametersJson TEXT NOT NULL DEFAULT '[]',
                    ExceptionsJson TEXT NOT NULL DEFAULT '[]',
                    SortOrder INTEGER NOT NULL DEFAULT 0,
                    CreatedUtc TEXT NOT NULL,
                    UpdatedUtc TEXT NOT NULL,
                    UNIQUE (PackageId, Version, NamespaceName, ObjectName, ObjectType, MemberKey)
                );

                CREATE INDEX IF NOT EXISTS IX_DocumentationMembers_Object
                ON DocumentationMembers (PackageId, Version, NamespaceName, ObjectName, ObjectType);

                CREATE INDEX IF NOT EXISTS IX_DocumentationMembers_Kind
                ON DocumentationMembers (PackageId, Version, MemberKind);

                CREATE INDEX IF NOT EXISTS IX_DocumentationMembers_Name
                ON DocumentationMembers (MemberName);

                CREATE TABLE IF NOT EXISTS DocumentationProjectContextFiles
                (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,

                    PackageId TEXT NOT NULL,
                    Version TEXT NOT NULL,

                    ProjectFilePath TEXT NOT NULL,
                    ProjectDirectoryPath TEXT NOT NULL,

                    FilePath TEXT NOT NULL,
                    FileName TEXT NOT NULL,

                    ContextType TEXT NOT NULL,
                    SourceFolderType TEXT NOT NULL,
                    DirectoryDepth INTEGER NOT NULL,

                    Content TEXT NOT NULL,

                    CreatedUtc TEXT NOT NULL,
                    UpdatedUtc TEXT NOT NULL,

                    UNIQUE (PackageId, Version, FilePath)
                );

                CREATE INDEX IF NOT EXISTS IX_ProjectContext_Package_Version
                ON DocumentationProjectContextFiles (PackageId, Version);

                CREATE INDEX IF NOT EXISTS IX_ProjectContext_Type
                ON DocumentationProjectContextFiles (ContextType);

                CREATE INDEX IF NOT EXISTS IX_ProjectContext_Source
                ON DocumentationProjectContextFiles (SourceFolderType);

                CREATE TABLE IF NOT EXISTS DocumentationSidebarItems
                (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    SidebarKind TEXT NOT NULL,
                    GroupName TEXT NOT NULL DEFAULT '',
                    PackageId TEXT NOT NULL DEFAULT '',
                    Version TEXT NOT NULL DEFAULT '',
                    NamespaceName TEXT NOT NULL DEFAULT '',
                    ItemKey TEXT NOT NULL,
                    ParentItemKey TEXT NOT NULL DEFAULT '',
                    ItemKind TEXT NOT NULL,
                    Title TEXT NOT NULL,
                    Icon TEXT NOT NULL DEFAULT '',
                    ControllerName TEXT NOT NULL DEFAULT '',
                    ActionName TEXT NOT NULL DEFAULT '',
                    RouteGroupName TEXT NOT NULL DEFAULT '',
                    RoutePackageId TEXT NOT NULL DEFAULT '',
                    RouteVersion TEXT NOT NULL DEFAULT '',
                    RouteNamespaceName TEXT NOT NULL DEFAULT '',
                    RouteObjectName TEXT NOT NULL DEFAULT '',
                    SortOrder INTEGER NOT NULL DEFAULT 0,
                    CreatedUtc TEXT NOT NULL,
                    UpdatedUtc TEXT NOT NULL
                );

                CREATE UNIQUE INDEX IF NOT EXISTS IX_DocumentationSidebarItems_Unique
                ON DocumentationSidebarItems (SidebarKind, GroupName, PackageId, Version, NamespaceName, ItemKey);

                CREATE INDEX IF NOT EXISTS IX_DocumentationSidebarItems_Scope
                ON DocumentationSidebarItems (SidebarKind, GroupName, PackageId, Version, NamespaceName);


";

                    using (var command = new SqliteCommand(createTableSql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    EnsureColumnExists(
                        connection,
                        "DocumentationOpenApiOperations",
                        "SecurityJson",
                        "TEXT NOT NULL DEFAULT '[]'");
                }

                TableCreatedDatabasePaths.Add(resolvedDatabasePath);
            }
        }

        private static void EnsureColumnExists(
            SqliteConnection connection,
            string tableName,
            string columnName,
            string columnDefinition
        )
        {
            using (SqliteCommand checkCommand = connection.CreateCommand())
            {
                checkCommand.CommandText = $"PRAGMA table_info({tableName})";

                using SqliteDataReader reader = checkCommand.ExecuteReader();

                while (reader.Read())
                {
                    if (string.Equals(reader["name"]?.ToString(), columnName, StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }
                }
            }

            using SqliteCommand alterCommand = connection.CreateCommand();
            alterCommand.CommandText = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnDefinition}";
            alterCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Saves generated documentation object metadata and rendered content to SQLite.
        /// </summary>
        /// <param name="sqliteDatabasePath">The sqliteDatabasePath value used by the documentation generation operation.</param>
        /// <param name="packageId">The packageId value used by the documentation generation operation.</param>
        /// <param name="version">The version value used by the documentation generation operation.</param>
        /// <param name="namespaceName">The namespaceName value used by the documentation generation operation.</param>
        /// <param name="objectName">The objectName value used by the documentation generation operation.</param>
        /// <param name="objectType">The objectType value used by the documentation generation operation.</param>
        /// <param name="model">The model value used by the documentation generation operation.</param>
        /// <param name="htmlContent">The htmlContent value used by the documentation generation operation.</param>
        /// <param name="technicalKeywords">The technicalKeywords value used by the documentation generation operation.</param>
        /// <param name="keywords">The keywords value used by the documentation generation operation.</param>
        /// <param name="routePath">The routePath value used by the documentation generation operation.</param>
        public static void SaveObject(
            string sqliteDatabasePath,
            string packageId,
            string version,
            string namespaceName,
            string objectName,
            string objectType,
            object model,
            string htmlContent,
            string technicalKeywords,
            string keywords,
            string routePath
        )
        {
            EnsureTableCreated(sqliteDatabasePath);

            string modelJson = JsonSerializer.Serialize(model);
            string nowUtc = DateTime.UtcNow.ToString("O");

            using (var connection = new SqliteConnection($"Data Source={sqliteDatabasePath}"))
            {
                connection.Open();

                if (string.Equals(objectType, "Group", StringComparison.Ordinal) &&
                    string.IsNullOrWhiteSpace(packageId) &&
                    !string.IsNullOrWhiteSpace(version))
                {
                    using var deleteLegacyGroupCommand = new SqliteCommand(
                        """
                        DELETE FROM DocumentationObjects
                        WHERE PackageId = ''
                          AND Version = ''
                          AND NamespaceName = ''
                          AND ObjectName = @ObjectName
                          AND ObjectType = 'Group';
                        """,
                        connection);
                    deleteLegacyGroupCommand.Parameters.AddWithValue("@ObjectName", objectName);
                    deleteLegacyGroupCommand.ExecuteNonQuery();
                }

                string insertSql = @"
                INSERT INTO DocumentationObjects 
                (PackageId, Version, NamespaceName, ObjectName, ObjectType, RoutePath, ModelInJson, HtmlContent, TechnicalKeywords, Keywords, CreatedUtc, UpdatedUtc, Builder)
                VALUES (@PackageId, @Version, @NamespaceName, @ObjectName, @ObjectType, @RoutePath, @ModelInJson, @HtmlContent, @TechnicalKeywords, @Keywords, @CreatedUtc, @UpdatedUtc, @Builder)
                ON CONFLICT(PackageId, Version, NamespaceName, ObjectName, ObjectType) DO UPDATE SET
                    RoutePath = excluded.RoutePath,
                    ModelInJson = excluded.ModelInJson,
                    HtmlContent = excluded.HtmlContent,
                    TechnicalKeywords = excluded.TechnicalKeywords,
                    Keywords = excluded.Keywords,
                    UpdatedUtc = excluded.UpdatedUtc,
                    Builder = excluded.Builder;";

                using (var command = new SqliteCommand(insertSql, connection))
                {
                    command.Parameters.AddWithValue("@PackageId", packageId);
                    command.Parameters.AddWithValue("@Version", version);
                    command.Parameters.AddWithValue("@NamespaceName", namespaceName);
                    command.Parameters.AddWithValue("@ObjectName", objectName);
                    command.Parameters.AddWithValue("@ObjectType", objectType);
                    command.Parameters.AddWithValue("@ModelInJson", modelJson);
                    command.Parameters.AddWithValue("@HtmlContent", htmlContent);
                    command.Parameters.AddWithValue("@TechnicalKeywords", technicalKeywords);
                    command.Parameters.AddWithValue("@Keywords", keywords);
                    command.Parameters.AddWithValue("@RoutePath", routePath);
                    command.Parameters.AddWithValue("@CreatedUtc", nowUtc);
                    command.Parameters.AddWithValue("@UpdatedUtc", nowUtc);
                    command.Parameters.AddWithValue("@Builder", DocumentationVisualHelper.DocumentationBuilderVersion);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqliteException e)
                    {
                        Console.WriteLine($"[DEBUG_LOG] Error saving object: {packageId} v{version} - {namespaceName}.{objectName} ({objectType})");
                        Console.WriteLine($"[DEBUG_LOG] RoutePath: {routePath}");
                        Console.WriteLine($"[DEBUG_LOG] SQLite Error: {e.Message}");
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Replaces granular member metadata for one generated documentation object.
        /// </summary>
        /// <param name="sqliteDatabasePath">The SQLite database path that receives the generated member rows.</param>
        /// <param name="packageId">The package identifier that owns the documented object.</param>
        /// <param name="version">The package version that owns the documented object.</param>
        /// <param name="groupName">The documentation group name that owns the documented object.</param>
        /// <param name="namespaceName">The namespace that contains the documented object.</param>
        /// <param name="objectName">The documented object name.</param>
        /// <param name="objectType">The documented object type.</param>
        /// <param name="members">The granular members extracted for the documented object.</param>
        public static void ReplaceObjectMembers(
            string sqliteDatabasePath,
            string packageId,
            string version,
            string groupName,
            string namespaceName,
            string objectName,
            string objectType,
            IEnumerable<DocumentationMemberDatabaseItem> members
        )
        {
            EnsureTableCreated(sqliteDatabasePath);

            DocumentationMemberDatabaseItem[] memberItems = members.ToArray();
            string nowUtc = DateTime.UtcNow.ToString("O");

            using var connection = new SqliteConnection($"Data Source={sqliteDatabasePath}");
            connection.Open();

            using SqliteTransaction transaction = connection.BeginTransaction();

            using var deleteCommand = new SqliteCommand(
                """
                DELETE FROM DocumentationMembers
                WHERE PackageId = @PackageId
                  AND Version = @Version
                  AND NamespaceName = @NamespaceName
                  AND ObjectName = @ObjectName
                  AND ObjectType = @ObjectType;
                """,
                connection,
                transaction);
            deleteCommand.Parameters.AddWithValue("@PackageId", packageId);
            deleteCommand.Parameters.AddWithValue("@Version", version);
            deleteCommand.Parameters.AddWithValue("@NamespaceName", namespaceName);
            deleteCommand.Parameters.AddWithValue("@ObjectName", objectName);
            deleteCommand.Parameters.AddWithValue("@ObjectType", objectType);
            deleteCommand.ExecuteNonQuery();

            const string insertSql = """
                                     INSERT INTO DocumentationMembers
                                     (PackageId, Version, GroupName, NamespaceName, ObjectName, ObjectType,
                                      MemberKind, MemberKey, MemberName, Signature, SummaryHtml, RemarksHtml,
                                      ReturnsHtml, ValueHtml, ExampleHtml, Accessibility, IsStatic, IsAbstract,
                                      IsVirtual, IsOverride, IsSealed, IsReadOnly, IsConst, IsObsolete,
                                      ObsoleteMessage, ExtensionTypeName, ExtensionNamespaceName, ParametersJson,
                                      ExceptionsJson, SortOrder, CreatedUtc, UpdatedUtc)
                                     VALUES
                                     (@PackageId, @Version, @GroupName, @NamespaceName, @ObjectName, @ObjectType,
                                      @MemberKind, @MemberKey, @MemberName, @Signature, @SummaryHtml, @RemarksHtml,
                                      @ReturnsHtml, @ValueHtml, @ExampleHtml, @Accessibility, @IsStatic, @IsAbstract,
                                      @IsVirtual, @IsOverride, @IsSealed, @IsReadOnly, @IsConst, @IsObsolete,
                                      @ObsoleteMessage, @ExtensionTypeName, @ExtensionNamespaceName, @ParametersJson,
                                      @ExceptionsJson, @SortOrder, @CreatedUtc, @UpdatedUtc);
                                     """;

            foreach (DocumentationMemberDatabaseItem item in memberItems)
            {
                using var insertCommand = new SqliteCommand(insertSql, connection, transaction);
                insertCommand.Parameters.AddWithValue("@PackageId", packageId);
                insertCommand.Parameters.AddWithValue("@Version", version);
                insertCommand.Parameters.AddWithValue("@GroupName", groupName);
                insertCommand.Parameters.AddWithValue("@NamespaceName", namespaceName);
                insertCommand.Parameters.AddWithValue("@ObjectName", objectName);
                insertCommand.Parameters.AddWithValue("@ObjectType", objectType);
                insertCommand.Parameters.AddWithValue("@MemberKind", item.MemberKind);
                insertCommand.Parameters.AddWithValue("@MemberKey", item.MemberKey);
                insertCommand.Parameters.AddWithValue("@MemberName", item.MemberName);
                insertCommand.Parameters.AddWithValue("@Signature", item.Signature);
                insertCommand.Parameters.AddWithValue("@SummaryHtml", item.SummaryHtml);
                insertCommand.Parameters.AddWithValue("@RemarksHtml", item.RemarksHtml);
                insertCommand.Parameters.AddWithValue("@ReturnsHtml", item.ReturnsHtml);
                insertCommand.Parameters.AddWithValue("@ValueHtml", item.ValueHtml);
                insertCommand.Parameters.AddWithValue("@ExampleHtml", item.ExampleHtml);
                insertCommand.Parameters.AddWithValue("@Accessibility", item.Accessibility);
                insertCommand.Parameters.AddWithValue("@IsStatic", item.IsStatic ? 1 : 0);
                insertCommand.Parameters.AddWithValue("@IsAbstract", item.IsAbstract ? 1 : 0);
                insertCommand.Parameters.AddWithValue("@IsVirtual", item.IsVirtual ? 1 : 0);
                insertCommand.Parameters.AddWithValue("@IsOverride", item.IsOverride ? 1 : 0);
                insertCommand.Parameters.AddWithValue("@IsSealed", item.IsSealed ? 1 : 0);
                insertCommand.Parameters.AddWithValue("@IsReadOnly", item.IsReadOnly ? 1 : 0);
                insertCommand.Parameters.AddWithValue("@IsConst", item.IsConst ? 1 : 0);
                insertCommand.Parameters.AddWithValue("@IsObsolete", item.IsObsolete ? 1 : 0);
                insertCommand.Parameters.AddWithValue("@ObsoleteMessage", item.ObsoleteMessage);
                insertCommand.Parameters.AddWithValue("@ExtensionTypeName", item.ExtensionTypeName);
                insertCommand.Parameters.AddWithValue("@ExtensionNamespaceName", item.ExtensionNamespaceName);
                insertCommand.Parameters.AddWithValue("@ParametersJson", item.ParametersJson);
                insertCommand.Parameters.AddWithValue("@ExceptionsJson", item.ExceptionsJson);
                insertCommand.Parameters.AddWithValue("@SortOrder", item.SortOrder);
                insertCommand.Parameters.AddWithValue("@CreatedUtc", nowUtc);
                insertCommand.Parameters.AddWithValue("@UpdatedUtc", nowUtc);
                insertCommand.ExecuteNonQuery();
            }

            transaction.Commit();
        }

        /// <summary>
        /// Replaces only the sidebar scopes affected by the generated package versions.
        /// </summary>
        /// <param name="sqliteDatabasePath">The SQLite database path that receives the generated sidebar items.</param>
        /// <param name="items">The sidebar items to persist for the generated package versions.</param>
        public static void ReplaceGeneratedSidebarItems(
            string sqliteDatabasePath,
            IEnumerable<DocumentationSidebarItem> items
        )
        {
            EnsureTableCreated(sqliteDatabasePath);

            DocumentationSidebarItem[] sidebarItems = items.ToArray();
            string nowUtc = DateTime.UtcNow.ToString("O");

            using var connection = new SqliteConnection($"Data Source={sqliteDatabasePath}");
            connection.Open();

            using SqliteTransaction transaction = connection.BeginTransaction();

            foreach ((string PackageId, string Version) generatedVersion in sidebarItems
                         .Where(item => !string.IsNullOrWhiteSpace(item.PackageId) && !string.IsNullOrWhiteSpace(item.Version))
                         .Select(item => (item.PackageId, item.Version))
                         .Distinct())
            {
                using var deleteNamespaceCommand = new SqliteCommand(
                    """
                    DELETE FROM DocumentationSidebarItems
                    WHERE SidebarKind IN ('Namespace', 'Project')
                      AND PackageId = @PackageId
                      AND Version = @Version;
                    """,
                    connection,
                    transaction);
                deleteNamespaceCommand.Parameters.AddWithValue("@PackageId", generatedVersion.PackageId);
                deleteNamespaceCommand.Parameters.AddWithValue("@Version", generatedVersion.Version);
                deleteNamespaceCommand.ExecuteNonQuery();

                using var deleteGroupCommand = new SqliteCommand(
                    """
                    DELETE FROM DocumentationSidebarItems
                    WHERE SidebarKind = 'Group'
                      AND (
                          (RoutePackageId = @PackageId AND RouteVersion = @Version)
                          OR (PackageId = '' AND Version = @Version)
                          OR ItemKey LIKE @ProjectKeyPrefix
                          OR ParentItemKey LIKE @ProjectKeyPrefix
                      );
                    """,
                    connection,
                    transaction);
                deleteGroupCommand.Parameters.AddWithValue("@PackageId", generatedVersion.PackageId);
                deleteGroupCommand.Parameters.AddWithValue("@Version", generatedVersion.Version);
                deleteGroupCommand.Parameters.AddWithValue("@ProjectKeyPrefix", $"project:{generatedVersion.PackageId}:{generatedVersion.Version}:%");
                deleteGroupCommand.ExecuteNonQuery();
            }

            const string insertSql = """
                                     INSERT INTO DocumentationSidebarItems
                                     (SidebarKind, GroupName, PackageId, Version, NamespaceName, ItemKey, ParentItemKey, ItemKind,
                                      Title, Icon, ControllerName, ActionName, RouteGroupName, RoutePackageId, RouteVersion,
                                      RouteNamespaceName, RouteObjectName, SortOrder, CreatedUtc, UpdatedUtc)
                                     VALUES
                                     (@SidebarKind, @GroupName, @PackageId, @Version, @NamespaceName, @ItemKey, @ParentItemKey, @ItemKind,
                                      @Title, @Icon, @ControllerName, @ActionName, @RouteGroupName, @RoutePackageId, @RouteVersion,
                                      @RouteNamespaceName, @RouteObjectName, @SortOrder, @CreatedUtc, @UpdatedUtc)
                                     ON CONFLICT(SidebarKind, GroupName, PackageId, Version, NamespaceName, ItemKey) DO UPDATE SET
                                         ParentItemKey = excluded.ParentItemKey,
                                         ItemKind = excluded.ItemKind,
                                         Title = excluded.Title,
                                         Icon = excluded.Icon,
                                         ControllerName = excluded.ControllerName,
                                         ActionName = excluded.ActionName,
                                         RouteGroupName = excluded.RouteGroupName,
                                         RoutePackageId = excluded.RoutePackageId,
                                         RouteVersion = excluded.RouteVersion,
                                         RouteNamespaceName = excluded.RouteNamespaceName,
                                         RouteObjectName = excluded.RouteObjectName,
                                         SortOrder = excluded.SortOrder,
                                         UpdatedUtc = excluded.UpdatedUtc;
                                     """;

            foreach (DocumentationSidebarItem item in sidebarItems)
            {
                using var insertCommand = new SqliteCommand(insertSql, connection, transaction);
                insertCommand.Parameters.AddWithValue("@SidebarKind", item.SidebarKind);
                insertCommand.Parameters.AddWithValue("@GroupName", item.GroupName);
                insertCommand.Parameters.AddWithValue("@PackageId", item.PackageId);
                insertCommand.Parameters.AddWithValue("@Version", item.Version);
                insertCommand.Parameters.AddWithValue("@NamespaceName", item.NamespaceName);
                insertCommand.Parameters.AddWithValue("@ItemKey", item.ItemKey);
                insertCommand.Parameters.AddWithValue("@ParentItemKey", item.ParentItemKey);
                insertCommand.Parameters.AddWithValue("@ItemKind", item.ItemKind);
                insertCommand.Parameters.AddWithValue("@Title", item.Title);
                insertCommand.Parameters.AddWithValue("@Icon", item.Icon);
                insertCommand.Parameters.AddWithValue("@ControllerName", item.ControllerName);
                insertCommand.Parameters.AddWithValue("@ActionName", item.ActionName);
                insertCommand.Parameters.AddWithValue("@RouteGroupName", item.RouteGroupName);
                insertCommand.Parameters.AddWithValue("@RoutePackageId", item.RoutePackageId);
                insertCommand.Parameters.AddWithValue("@RouteVersion", item.RouteVersion);
                insertCommand.Parameters.AddWithValue("@RouteNamespaceName", item.RouteNamespaceName);
                insertCommand.Parameters.AddWithValue("@RouteObjectName", item.RouteObjectName);
                insertCommand.Parameters.AddWithValue("@SortOrder", item.SortOrder);
                insertCommand.Parameters.AddWithValue("@CreatedUtc", nowUtc);
                insertCommand.Parameters.AddWithValue("@UpdatedUtc", nowUtc);
                insertCommand.ExecuteNonQuery();
            }

            transaction.Commit();
        }

        /// <summary>
        /// Deletes imported OpenAPI documents and operation indexes for one package version.
        /// </summary>
        /// <param name="sqliteDatabasePath">The SQLite database path that contains OpenAPI records.</param>
        /// <param name="packageId">The package identifier whose OpenAPI records should be deleted.</param>
        /// <param name="version">The package version whose OpenAPI records should be deleted.</param>
        public static void DeleteOpenApiDocuments(
            string sqliteDatabasePath,
            string packageId,
            string version
        )
        {
            EnsureTableCreated(sqliteDatabasePath);

            using var connection = new SqliteConnection($"Data Source={sqliteDatabasePath}");
            connection.Open();

            using SqliteTransaction transaction = connection.BeginTransaction();

            using (SqliteCommand deleteOperationsCommand = connection.CreateCommand())
            {
                deleteOperationsCommand.Transaction = transaction;
                deleteOperationsCommand.CommandText = """
                                                      DELETE FROM DocumentationOpenApiOperations
                                                      WHERE PackageId = @PackageId
                                                        AND Version = @Version
                                                      """;
                deleteOperationsCommand.Parameters.AddWithValue("@PackageId", packageId);
                deleteOperationsCommand.Parameters.AddWithValue("@Version", version);
                deleteOperationsCommand.ExecuteNonQuery();
            }

            using (SqliteCommand deleteDocumentsCommand = connection.CreateCommand())
            {
                deleteDocumentsCommand.Transaction = transaction;
                deleteDocumentsCommand.CommandText = """
                                                     DELETE FROM DocumentationOpenApiDocuments
                                                     WHERE PackageId = @PackageId
                                                       AND Version = @Version
                                                     """;
                deleteDocumentsCommand.Parameters.AddWithValue("@PackageId", packageId);
                deleteDocumentsCommand.Parameters.AddWithValue("@Version", version);
                deleteDocumentsCommand.ExecuteNonQuery();
            }

            transaction.Commit();
        }

        /// <summary>
        /// Replaces one imported OpenAPI document and its operation index.
        /// </summary>
        /// <param name="sqliteDatabasePath">The SQLite database path that receives OpenAPI records.</param>
        /// <param name="document">The imported OpenAPI document to persist.</param>
        public static void ReplaceOpenApiDocument(
            string sqliteDatabasePath,
            DocumentationOpenApiDocumentItem document
        )
        {
            if (document is null) throw new ArgumentNullException(nameof(document));

            EnsureTableCreated(sqliteDatabasePath);

            string nowUtc = DateTime.UtcNow.ToString("O");

            using var connection = new SqliteConnection($"Data Source={sqliteDatabasePath}");
            connection.Open();

            using SqliteTransaction transaction = connection.BeginTransaction();

            using (SqliteCommand deleteOperationsCommand = connection.CreateCommand())
            {
                deleteOperationsCommand.Transaction = transaction;
                deleteOperationsCommand.CommandText = """
                                                      DELETE FROM DocumentationOpenApiOperations
                                                      WHERE PackageId = @PackageId
                                                        AND Version = @Version
                                                        AND DocumentName = @DocumentName
                                                      """;
                deleteOperationsCommand.Parameters.AddWithValue("@PackageId", document.PackageId);
                deleteOperationsCommand.Parameters.AddWithValue("@Version", document.Version);
                deleteOperationsCommand.Parameters.AddWithValue("@DocumentName", document.DocumentName);
                deleteOperationsCommand.ExecuteNonQuery();
            }

            using (SqliteCommand documentCommand = connection.CreateCommand())
            {
                documentCommand.Transaction = transaction;
                documentCommand.CommandText = """
                                              INSERT INTO DocumentationOpenApiDocuments
                                              (PackageId, Version, DocumentName, Title, Description, JsonContent, RoutePath, CreatedUtc, UpdatedUtc)
                                              VALUES
                                              (@PackageId, @Version, @DocumentName, @Title, @Description, @JsonContent, @RoutePath, @CreatedUtc, @UpdatedUtc)
                                              ON CONFLICT(PackageId, Version, DocumentName) DO UPDATE SET
                                                  Title = excluded.Title,
                                                  Description = excluded.Description,
                                                  JsonContent = excluded.JsonContent,
                                                  RoutePath = excluded.RoutePath,
                                                  UpdatedUtc = excluded.UpdatedUtc;
                                              """;
                documentCommand.Parameters.AddWithValue("@PackageId", document.PackageId);
                documentCommand.Parameters.AddWithValue("@Version", document.Version);
                documentCommand.Parameters.AddWithValue("@DocumentName", document.DocumentName);
                documentCommand.Parameters.AddWithValue("@Title", document.Title);
                documentCommand.Parameters.AddWithValue("@Description", document.Description);
                documentCommand.Parameters.AddWithValue("@JsonContent", document.JsonContent);
                documentCommand.Parameters.AddWithValue("@RoutePath", document.RoutePath);
                documentCommand.Parameters.AddWithValue("@CreatedUtc", nowUtc);
                documentCommand.Parameters.AddWithValue("@UpdatedUtc", nowUtc);
                documentCommand.ExecuteNonQuery();
            }

            const string operationSql = """
                                        INSERT INTO DocumentationOpenApiOperations
                                        (PackageId, Version, DocumentName, OperationId, HttpMethod, Path, Summary, Description,
                                         TagsJson, ParametersJson, RequestBodyJson, ResponsesJson, SecurityJson, RoutePath, CreatedUtc, UpdatedUtc)
                                        VALUES
                                        (@PackageId, @Version, @DocumentName, @OperationId, @HttpMethod, @Path, @Summary, @Description,
                                         @TagsJson, @ParametersJson, @RequestBodyJson, @ResponsesJson, @SecurityJson, @RoutePath, @CreatedUtc, @UpdatedUtc)
                                        """;

            using SqliteCommand operationCommand = connection.CreateCommand();
            operationCommand.Transaction = transaction;
            operationCommand.CommandText = operationSql;

            foreach (DocumentationOpenApiOperationItem operation in document.Operations)
            {
                operationCommand.Parameters.Clear();
                operationCommand.Parameters.AddWithValue("@PackageId", operation.PackageId);
                operationCommand.Parameters.AddWithValue("@Version", operation.Version);
                operationCommand.Parameters.AddWithValue("@DocumentName", operation.DocumentName);
                operationCommand.Parameters.AddWithValue("@OperationId", operation.OperationId);
                operationCommand.Parameters.AddWithValue("@HttpMethod", operation.HttpMethod);
                operationCommand.Parameters.AddWithValue("@Path", operation.Path);
                operationCommand.Parameters.AddWithValue("@Summary", operation.Summary);
                operationCommand.Parameters.AddWithValue("@Description", operation.Description);
                operationCommand.Parameters.AddWithValue("@TagsJson", operation.TagsJson);
                operationCommand.Parameters.AddWithValue("@ParametersJson", operation.ParametersJson);
                operationCommand.Parameters.AddWithValue("@RequestBodyJson", operation.RequestBodyJson);
                operationCommand.Parameters.AddWithValue("@ResponsesJson", operation.ResponsesJson);
                operationCommand.Parameters.AddWithValue("@SecurityJson", operation.SecurityJson);
                operationCommand.Parameters.AddWithValue("@RoutePath", operation.RoutePath);
                operationCommand.Parameters.AddWithValue("@CreatedUtc", nowUtc);
                operationCommand.Parameters.AddWithValue("@UpdatedUtc", nowUtc);
                operationCommand.ExecuteNonQuery();
            }

            transaction.Commit();
        }

        /// <summary>
        /// Replaces captured C# source files for one generated package version.
        /// </summary>
        /// <param name="sqliteDatabasePath">The SQLite database path that receives source file snapshots.</param>
        /// <param name="packageId">The package identifier whose source files are being replaced.</param>
        /// <param name="version">The package version whose source files are being replaced.</param>
        /// <param name="sourceFiles">The captured source files to persist.</param>
        public static void ReplaceGeneratedSourceFiles(
            string sqliteDatabasePath,
            string packageId,
            string version,
            IEnumerable<DocumentationSourceFileItem> sourceFiles
        )
        {
            if (sourceFiles is null) throw new ArgumentNullException(nameof(sourceFiles));

            EnsureTableCreated(sqliteDatabasePath);

            DocumentationSourceFileItem[] items = sourceFiles.ToArray();
            string nowUtc = DateTime.UtcNow.ToString("O");

            using var connection = new SqliteConnection($"Data Source={sqliteDatabasePath}");
            connection.Open();

            using SqliteTransaction transaction = connection.BeginTransaction();

            using (SqliteCommand deleteCommand = connection.CreateCommand())
            {
                deleteCommand.Transaction = transaction;
                deleteCommand.CommandText = """
                                            DELETE FROM DocumentationSourceFiles
                                            WHERE PackageId = @PackageId
                                              AND Version = @Version
                                            """;
                deleteCommand.Parameters.AddWithValue("@PackageId", packageId);
                deleteCommand.Parameters.AddWithValue("@Version", version);
                deleteCommand.ExecuteNonQuery();
            }

            const string sql = """
                               INSERT INTO DocumentationSourceFiles
                               (PackageId, Version, ProjectFilePath, ProjectDirectoryPath, FilePath, RelativeFilePath,
                                FileName, PrimaryNamespaceName, NamespaceNamesJson, TypeNamesJson, Content, ContentHash,
                                CreatedUtc, UpdatedUtc)
                               VALUES
                               (@PackageId, @Version, @ProjectFilePath, @ProjectDirectoryPath, @FilePath, @RelativeFilePath,
                                @FileName, @PrimaryNamespaceName, @NamespaceNamesJson, @TypeNamesJson, @Content, @ContentHash,
                                @CreatedUtc, @UpdatedUtc)
                               """;

            using SqliteCommand insertCommand = connection.CreateCommand();
            insertCommand.Transaction = transaction;
            insertCommand.CommandText = sql;

            foreach (DocumentationSourceFileItem item in items)
            {
                insertCommand.Parameters.Clear();
                insertCommand.Parameters.AddWithValue("@PackageId", item.PackageId);
                insertCommand.Parameters.AddWithValue("@Version", item.Version);
                insertCommand.Parameters.AddWithValue("@ProjectFilePath", item.ProjectFilePath);
                insertCommand.Parameters.AddWithValue("@ProjectDirectoryPath", item.ProjectDirectoryPath);
                insertCommand.Parameters.AddWithValue("@FilePath", item.FilePath);
                insertCommand.Parameters.AddWithValue("@RelativeFilePath", item.RelativeFilePath);
                insertCommand.Parameters.AddWithValue("@FileName", item.FileName);
                insertCommand.Parameters.AddWithValue("@PrimaryNamespaceName", item.PrimaryNamespaceName);
                insertCommand.Parameters.AddWithValue("@NamespaceNamesJson", JsonSerializer.Serialize(item.NamespaceNames));
                insertCommand.Parameters.AddWithValue("@TypeNamesJson", JsonSerializer.Serialize(item.TypeNames));
                insertCommand.Parameters.AddWithValue("@Content", item.Content);
                insertCommand.Parameters.AddWithValue("@ContentHash", item.ContentHash);
                insertCommand.Parameters.AddWithValue("@CreatedUtc", nowUtc);
                insertCommand.Parameters.AddWithValue("@UpdatedUtc", nowUtc);
                insertCommand.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        
        /// <summary>
        /// Saves a project context file entry to SQLite.
        /// </summary>
        /// <param name="sqliteDatabasePath">The sqliteDatabasePath value used by the documentation generation operation.</param>
        /// <param name="packageId">The packageId value used by the documentation generation operation.</param>
        /// <param name="version">The version value used by the documentation generation operation.</param>
        /// <param name="projectFilePath">The projectFilePath value used by the documentation generation operation.</param>
        /// <param name="projectDirectoryPath">The projectDirectoryPath value used by the documentation generation operation.</param>
        /// <param name="filePath">The filePath value used by the documentation generation operation.</param>
        /// <param name="fileName">The fileName value used by the documentation generation operation.</param>
        /// <param name="contextType">The contextType value used by the documentation generation operation.</param>
        /// <param name="sourceFolderType">The sourceFolderType value used by the documentation generation operation.</param>
        /// <param name="directoryDepth">The directoryDepth value used by the documentation generation operation.</param>
        /// <param name="content">The content value used by the documentation generation operation.</param>
        public static void SaveProjectContextFile(
    string sqliteDatabasePath,
    string packageId,
    string version,
    string projectFilePath,
    string projectDirectoryPath,
    string filePath,
    string fileName,
    string contextType,
    string sourceFolderType,
    int directoryDepth,
    string content
)
{
    EnsureTableCreated(sqliteDatabasePath);

    string nowUtc = DateTime.UtcNow.ToString("O");

    using var connection = new SqliteConnection($"Data Source={sqliteDatabasePath}");
    connection.Open();

    const string sql = @"
    INSERT INTO DocumentationProjectContextFiles
    (PackageId, Version, ProjectFilePath, ProjectDirectoryPath,
     FilePath, FileName, ContextType, SourceFolderType, DirectoryDepth,
     Content, CreatedUtc, UpdatedUtc)
    VALUES
    (@PackageId, @Version, @ProjectFilePath, @ProjectDirectoryPath,
     @FilePath, @FileName, @ContextType, @SourceFolderType, @DirectoryDepth,
     @Content, @CreatedUtc, @UpdatedUtc)
    ON CONFLICT(PackageId, Version, FilePath) DO UPDATE SET
        FileName = excluded.FileName,
        ContextType = excluded.ContextType,
        SourceFolderType = excluded.SourceFolderType,
        DirectoryDepth = excluded.DirectoryDepth,
        Content = excluded.Content,
        UpdatedUtc = excluded.UpdatedUtc;
    ";

    using var command = new SqliteCommand(sql, connection);

    command.Parameters.AddWithValue("@PackageId", packageId);
    command.Parameters.AddWithValue("@Version", version);
    command.Parameters.AddWithValue("@ProjectFilePath", projectFilePath);
    command.Parameters.AddWithValue("@ProjectDirectoryPath", projectDirectoryPath);
    command.Parameters.AddWithValue("@FilePath", filePath);
    command.Parameters.AddWithValue("@FileName", fileName);
    command.Parameters.AddWithValue("@ContextType", contextType);
    command.Parameters.AddWithValue("@SourceFolderType", sourceFolderType);
    command.Parameters.AddWithValue("@DirectoryDepth", directoryDepth);
    command.Parameters.AddWithValue("@Content", content);
    command.Parameters.AddWithValue("@CreatedUtc", nowUtc);
    command.Parameters.AddWithValue("@UpdatedUtc", nowUtc);

    command.ExecuteNonQuery();
}

        /// <summary>
        /// Saves source content associated with a generated documentation object.
        /// </summary>
        /// <param name="sqliteDatabasePath">The sqliteDatabasePath value used by the documentation generation operation.</param>
        /// <param name="packageId">The packageId value used by the documentation generation operation.</param>
        /// <param name="version">The version value used by the documentation generation operation.</param>
        /// <param name="namespaceName">The namespaceName value used by the documentation generation operation.</param>
        /// <param name="objectName">The objectName value used by the documentation generation operation.</param>
        /// <param name="objectType">The objectType value used by the documentation generation operation.</param>
        /// <param name="sourceCode">The sourceCode value used by the documentation generation operation.</param>
        /// <param name="fileCount">The fileCount value used by the documentation generation operation.</param>
        public static void SaveObjectSource(
            string sqliteDatabasePath,
            string packageId,
            string version,
            string namespaceName,
            string objectName,
            string objectType,
            string sourceCode,
            int fileCount
        )
        {
            EnsureTableCreated(sqliteDatabasePath);

            string nowUtc = DateTime.UtcNow.ToString("O");
            using var connection = new SqliteConnection($"Data Source={sqliteDatabasePath}");
            connection.Open();
            string sql = @"
            INSERT INTO DocumentationObjectSources
            (PackageId, Version, NamespaceName, ObjectName, ObjectType, SourceCode, SourceFileCount, CreatedUtc, UpdatedUtc)
            VALUES (@PackageId, @Version, @NamespaceName, @ObjectName, @ObjectType, @SourceCode, @FileCount, @CreatedUtc, @UpdatedUtc)
            ON CONFLICT(PackageId, Version, NamespaceName, ObjectName, ObjectType) DO UPDATE SET
                SourceCode = excluded.SourceCode,
                SourceFileCount = excluded.SourceFileCount,
                UpdatedUtc = excluded.UpdatedUtc;
            ";
            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@PackageId", packageId);
            command.Parameters.AddWithValue("@Version", version);
            command.Parameters.AddWithValue("@NamespaceName", namespaceName);
            command.Parameters.AddWithValue("@ObjectName", objectName);
            command.Parameters.AddWithValue("@ObjectType", objectType);
            command.Parameters.AddWithValue("@SourceCode", sourceCode);
            command.Parameters.AddWithValue("@FileCount", fileCount);
            command.Parameters.AddWithValue("@CreatedUtc", nowUtc);
            command.Parameters.AddWithValue("@UpdatedUtc", nowUtc);
            command.ExecuteNonQuery();
        }

        #endregion
    }
}
