#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Text;
using Microsoft.Data.Sqlite;

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Provides read-only SQLite queries for captured C# source files exposed through MCP.
    /// </summary>
    /// <remarks>
    ///     The service reads generated source snapshots from `DocumentationSourceFiles` and never accesses the live
    ///     filesystem.
    /// </remarks>
    public sealed class SourceFileQueryService
    {
        #region Static methods

        private static string NormalizeRelativePath(string relativeFilePath)
        {
            return relativeFilePath.Replace('\\', '/').TrimStart('/');
        }

        private static SourceFileQueryResult ReadSourceFile(SqliteDataReader reader)
        {
            return new SourceFileQueryResult
            {
                PackageId = reader["PackageId"]?.ToString() ?? string.Empty,
                Version = reader["Version"]?.ToString() ?? string.Empty,
                RelativeFilePath = reader["RelativeFilePath"]?.ToString() ?? string.Empty,
                FileName = reader["FileName"]?.ToString() ?? string.Empty,
                PrimaryNamespaceName = reader["PrimaryNamespaceName"]?.ToString() ?? string.Empty,
                NamespaceNamesJson = reader["NamespaceNamesJson"]?.ToString() ?? "[]",
                TypeNamesJson = reader["TypeNamesJson"]?.ToString() ?? "[]",
                Content = reader["Content"]?.ToString() ?? string.Empty,
                ContentHash = reader["ContentHash"]?.ToString() ?? string.Empty
            };
        }

        private static IReadOnlyList<SourceFileQueryResult> ReadSourceFiles(SqliteCommand command)
        {
            using SqliteDataReader reader = command.ExecuteReader();
            List<SourceFileQueryResult> files = [];

            while (reader.Read())
            {
                files.Add(ReadSourceFile(reader));
            }

            return files;
        }

        private static bool TableExists(SqliteConnection connection, string tableName)
        {
            using SqliteCommand command = new(
                """
                SELECT 1
                FROM sqlite_master
                WHERE type = 'table'
                  AND name = @TableName
                LIMIT 1;
                """,
                connection);
            command.Parameters.AddWithValue("@TableName", tableName);

            object? result = command.ExecuteScalar();

            return result is not null;
        }

        #endregion

        #region Instance fields and properties

        private readonly string _sqliteDatabasePath;

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SourceFileQueryService" /> class.
        /// </summary>
        /// <param name="sqliteDatabasePath">Absolute or relative path to the generated documentation SQLite database.</param>
        public SourceFileQueryService(string sqliteDatabasePath)
        {
            _sqliteDatabasePath = sqliteDatabasePath;
        }

        #endregion

        #region Instance methods

        /// <summary>
        ///     Builds a compact source and documentation context for coding assistance.
        /// </summary>
        /// <param name="query">Search text used to find related source files.</param>
        /// <param name="packageId">Package identifier whose source files should be searched.</param>
        /// <param name="version">Package version whose source files should be searched.</param>
        /// <returns>A formatted coding context bundle.</returns>
        public string BuildCodingContext(string query, string packageId, string version)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return "A non-empty query is required to build a coding context.";
            }

            IReadOnlyList<SourceFileQueryResult> files = SearchSourceFileRecords(query, packageId, version, 5);

            if (files.Count == 0)
            {
                return $"No source files found for '{query}' in package '{packageId}' version '{version}'.";
            }

            List<string> blocks = [];

            foreach (SourceFileQueryResult file in files)
            {
                StringBuilder block = new();
                block.AppendLine($"# Source File: {file.RelativeFilePath}");
                block.AppendLine($"Package: {file.PackageId}");
                block.AppendLine($"Version: {file.Version}");
                block.AppendLine($"Primary namespace: {file.PrimaryNamespaceName}");
                block.AppendLine($"Types: {file.TypeNamesJson}");
                block.AppendLine($"Hash: {file.ContentHash}");
                block.AppendLine();
                block.AppendLine(DocumentationMcpTextFormatter.LimitText(file.Content, 12000));
                blocks.Add(block.ToString().Trim());
            }

            return string.Join("\n\n", blocks);
        }

        private SourceFileQueryResult? FindSourceFile(string packageId, string version, string relativeFilePath)
        {
            using SqliteConnection connection = OpenConnection();

            if (!TableExists(connection, "DocumentationSourceFiles"))
            {
                return null;
            }

            const string sql = """
                               SELECT PackageId, Version, RelativeFilePath, FileName, PrimaryNamespaceName,
                                      NamespaceNamesJson, TypeNamesJson, Content, ContentHash
                               FROM DocumentationSourceFiles
                               WHERE PackageId = @PackageId
                                 AND Version = @Version
                                 AND RelativeFilePath = @RelativeFilePath
                               LIMIT 1
                               """;

            using SqliteCommand command = new(sql, connection);
            command.Parameters.AddWithValue("@PackageId", packageId);
            command.Parameters.AddWithValue("@Version", version);
            command.Parameters.AddWithValue("@RelativeFilePath", NormalizeRelativePath(relativeFilePath));

            using SqliteDataReader reader = command.ExecuteReader();
            return reader.Read() ? ReadSourceFile(reader) : null;
        }

        /// <summary>
        ///     Gets one captured C# source file by relative path.
        /// </summary>
        /// <param name="packageId">Package identifier that owns the source file.</param>
        /// <param name="version">Package version that owns the source file.</param>
        /// <param name="relativeFilePath">Source file path relative to the documented project directory.</param>
        /// <returns>A formatted source file block, or an explanatory message when no source file exists.</returns>
        public string GetSourceFile(string packageId, string version, string relativeFilePath)
        {
            SourceFileQueryResult? file = FindSourceFile(packageId, version, relativeFilePath);

            if (file is null)
            {
                return $"Source file '{relativeFilePath}' not found for package '{packageId}' version '{version}'.";
            }

            StringBuilder text = new();
            text.AppendLine($"Source file found: {file.RelativeFilePath}");
            text.AppendLine($"Package: {file.PackageId}");
            text.AppendLine($"Version: {file.Version}");
            text.AppendLine($"Primary namespace: {file.PrimaryNamespaceName}");
            text.AppendLine($"Namespaces: {file.NamespaceNamesJson}");
            text.AppendLine($"Types: {file.TypeNamesJson}");
            text.AppendLine($"Hash: {file.ContentHash}");
            text.AppendLine();
            text.AppendLine("Source code:");
            text.AppendLine(DocumentationMcpTextFormatter.LimitText(file.Content, 30000));

            return text.ToString();
        }

        private IReadOnlyList<SourceFileQueryResult> ListSourceFileRecords(string packageId, string version, string? namespaceName)
        {
            using SqliteConnection connection = OpenConnection();

            if (!TableExists(connection, "DocumentationSourceFiles"))
            {
                return [];
            }

            string sql = """
                         SELECT PackageId, Version, RelativeFilePath, FileName, PrimaryNamespaceName,
                                NamespaceNamesJson, TypeNamesJson, Content, ContentHash
                         FROM DocumentationSourceFiles
                         WHERE PackageId = @PackageId
                           AND Version = @Version
                         """;

            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                sql += " AND NamespaceNamesJson LIKE @NamespaceName";
            }

            sql += " ORDER BY RelativeFilePath LIMIT 500";

            using SqliteCommand command = new(sql, connection);
            command.Parameters.AddWithValue("@PackageId", packageId);
            command.Parameters.AddWithValue("@Version", version);

            if (!string.IsNullOrWhiteSpace(namespaceName))
            {
                command.Parameters.AddWithValue("@NamespaceName", $"%\"{namespaceName}\"%");
            }

            return ReadSourceFiles(command);
        }

        /// <summary>
        ///     Lists captured C# source files for one package version.
        /// </summary>
        /// <param name="packageId">Package identifier whose source files should be listed.</param>
        /// <param name="version">Package version whose source files should be listed.</param>
        /// <param name="namespaceName">Optional namespace filter.</param>
        /// <returns>A formatted file list, or an explanatory message when no source file exists.</returns>
        public string ListSourceFiles(string packageId, string version, string? namespaceName = null)
        {
            IReadOnlyList<SourceFileQueryResult> files = ListSourceFileRecords(packageId, version, namespaceName);

            if (files.Count == 0)
            {
                return $"No source files found for package '{packageId}' version '{version}'.";
            }

            return string.Join(
                "\n",
                files.Select(file => $"- {file.RelativeFilePath} | namespace={file.PrimaryNamespaceName} | types={file.TypeNamesJson}"));
        }

        private SqliteConnection OpenConnection()
        {
            SqliteConnection connection = new($"Data Source={_sqliteDatabasePath}");
            connection.Open();
            return connection;
        }

        /// <summary>
        ///     Searches captured C# source files by path, type metadata, namespace metadata, or content.
        /// </summary>
        /// <param name="query">Search text used in SQLite `LIKE` filters.</param>
        /// <param name="packageId">Package identifier whose source files should be searched.</param>
        /// <param name="version">Optional package version filter.</param>
        /// <returns>A formatted search result list with excerpts.</returns>
        public string SearchSourceCode(string query, string packageId, string? version = null)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return "A non-empty query is required to search captured source code.";
            }

            IReadOnlyList<SourceFileQueryResult> files = SearchSourceFileRecords(query, packageId, version, 20);

            if (files.Count == 0)
            {
                return string.IsNullOrWhiteSpace(version)
                    ? $"No source file result found for '{query}' in package '{packageId}'."
                    : $"No source file result found for '{query}' in package '{packageId}' version '{version}'.";
            }

            List<string> blocks = [];

            foreach (SourceFileQueryResult file in files)
            {
                StringBuilder block = new();
                block.AppendLine($"# {file.RelativeFilePath}");
                block.AppendLine($"Package: {file.PackageId}");
                block.AppendLine($"Version: {file.Version}");
                block.AppendLine($"Primary namespace: {file.PrimaryNamespaceName}");
                block.AppendLine($"Types: {file.TypeNamesJson}");
                block.AppendLine();
                block.AppendLine(DocumentationMcpTextFormatter.LimitText(file.Content, 4000));
                blocks.Add(block.ToString().Trim());
            }

            return string.Join("\n\n", blocks);
        }

        private IReadOnlyList<SourceFileQueryResult> SearchSourceFileRecords(
            string query,
            string packageId,
            string? version,
            int limit
        )
        {
            using SqliteConnection connection = OpenConnection();

            if (!TableExists(connection, "DocumentationSourceFiles"))
            {
                return [];
            }

            string sql = """
                         SELECT PackageId, Version, RelativeFilePath, FileName, PrimaryNamespaceName,
                                NamespaceNamesJson, TypeNamesJson, Content, ContentHash
                         FROM DocumentationSourceFiles
                         WHERE PackageId = @PackageId
                           AND (
                               RelativeFilePath LIKE @Query
                               OR FileName LIKE @Query
                               OR PrimaryNamespaceName LIKE @Query
                               OR NamespaceNamesJson LIKE @Query
                               OR TypeNamesJson LIKE @Query
                               OR Content LIKE @Query
                           )
                         """;

            if (!string.IsNullOrWhiteSpace(version))
            {
                sql += " AND Version = @Version";
            }

            sql += " ORDER BY Version DESC, RelativeFilePath LIMIT @Limit";

            using SqliteCommand command = new(sql, connection);
            command.Parameters.AddWithValue("@PackageId", packageId);
            command.Parameters.AddWithValue("@Query", $"%{query}%");
            command.Parameters.AddWithValue("@Limit", limit);

            if (!string.IsNullOrWhiteSpace(version))
            {
                command.Parameters.AddWithValue("@Version", version);
            }

            return ReadSourceFiles(command);
        }

        #endregion
    }
}