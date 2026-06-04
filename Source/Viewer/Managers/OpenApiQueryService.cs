#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Data.Sqlite;

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Provides read-only SQLite queries for imported OpenAPI documents and operations.
    /// </summary>
    public sealed class OpenApiQueryService
    {
        #region Static methods

        private static bool ColumnExists(SqliteConnection connection, string tableName, string columnName)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"PRAGMA table_info({tableName})";

            using SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                if (string.Equals(reader["name"]?.ToString(), columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static string FormatOperation(DocumentationOpenApiOperationQueryResult operation)
        {
            StringBuilder text = new();
            text.AppendLine($"# {operation.OperationId}");
            text.AppendLine($"{operation.HttpMethod} {operation.Path}");
            text.AppendLine($"Package: {operation.PackageId}");
            text.AppendLine($"Version: {operation.Version}");
            text.AppendLine($"Document: {operation.DocumentName}");
            text.AppendLine($"Summary: {operation.Summary}");
            text.AppendLine($"Description: {operation.Description}");
            text.AppendLine($"Tags: {operation.TagsJson}");
            text.AppendLine();
            text.AppendLine("Security:");
            text.AppendLine(DocumentationMcpTextFormatter.LimitText(operation.SecurityJson, 3000));
            text.AppendLine();
            text.AppendLine("Parameters:");
            text.AppendLine(DocumentationMcpTextFormatter.LimitText(operation.ParametersJson, 6000));
            text.AppendLine();
            text.AppendLine("Request body:");
            text.AppendLine(DocumentationMcpTextFormatter.LimitText(operation.RequestBodyJson, 6000));
            text.AppendLine();
            text.AppendLine("Responses:");
            text.AppendLine(DocumentationMcpTextFormatter.LimitText(operation.ResponsesJson, 10000));
            return text.ToString();
        }

        private static string GetSecurityJsonProjection(SqliteConnection connection)
        {
            return ColumnExists(connection, "DocumentationOpenApiOperations", "SecurityJson")
                ? GetSecurityJsonProjection(true)
                : GetSecurityJsonProjection(false);
        }

        private static string GetSecurityJsonProjection(bool hasSecurityJsonColumn)
        {
            return hasSecurityJsonColumn
                ? "SecurityJson"
                : "'[]' AS SecurityJson";
        }

        private static DocumentationOpenApiDocumentQueryResult ReadDocument(SqliteDataReader reader)
        {
            return new DocumentationOpenApiDocumentQueryResult
            {
                PackageId = reader["PackageId"]?.ToString() ?? string.Empty,
                Version = reader["Version"]?.ToString() ?? string.Empty,
                DocumentName = reader["DocumentName"]?.ToString() ?? string.Empty,
                Title = reader["Title"]?.ToString() ?? string.Empty,
                Description = reader["Description"]?.ToString() ?? string.Empty,
                JsonContent = reader["JsonContent"]?.ToString() ?? string.Empty,
                RoutePath = reader["RoutePath"]?.ToString() ?? string.Empty
            };
        }

        private static DocumentationOpenApiOperationQueryResult ReadOperation(SqliteDataReader reader)
        {
            return new DocumentationOpenApiOperationQueryResult
            {
                PackageId = reader["PackageId"]?.ToString() ?? string.Empty,
                Version = reader["Version"]?.ToString() ?? string.Empty,
                DocumentName = reader["DocumentName"]?.ToString() ?? string.Empty,
                OperationId = reader["OperationId"]?.ToString() ?? string.Empty,
                HttpMethod = reader["HttpMethod"]?.ToString() ?? string.Empty,
                Path = reader["Path"]?.ToString() ?? string.Empty,
                Summary = reader["Summary"]?.ToString() ?? string.Empty,
                Description = reader["Description"]?.ToString() ?? string.Empty,
                TagsJson = reader["TagsJson"]?.ToString() ?? "[]",
                ParametersJson = reader["ParametersJson"]?.ToString() ?? "[]",
                RequestBodyJson = reader["RequestBodyJson"]?.ToString() ?? "{}",
                ResponsesJson = reader["ResponsesJson"]?.ToString() ?? "{}",
                SecurityJson = reader["SecurityJson"]?.ToString() ?? "[]",
                RoutePath = reader["RoutePath"]?.ToString() ?? string.Empty
            };
        }

        private static IReadOnlyList<DocumentationOpenApiOperationQueryResult> ReadOperations(SqliteCommand command)
        {
            using SqliteDataReader reader = command.ExecuteReader();
            List<DocumentationOpenApiOperationQueryResult> operations = [];

            while (reader.Read())
            {
                operations.Add(ReadOperation(reader));
            }

            return operations;
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
        ///     Initializes a new instance of the <see cref="OpenApiQueryService" /> class.
        /// </summary>
        /// <param name="sqliteDatabasePath">Absolute or relative path to the generated documentation SQLite database.</param>
        public OpenApiQueryService(string sqliteDatabasePath)
        {
            _sqliteDatabasePath = sqliteDatabasePath;
        }

        #endregion

        #region Instance methods

        /// <summary>
        ///     Formats one OpenAPI operation for MCP responses.
        /// </summary>
        /// <param name="packageId">Package identifier that owns the operation.</param>
        /// <param name="version">Package version that owns the operation.</param>
        /// <param name="documentName">Stable OpenAPI document name.</param>
        /// <param name="operationId">Stable operation identifier.</param>
        /// <returns>An operation detail block or an explanatory not-found message.</returns>
        public string GetApiOperation(string packageId, string version, string documentName, string operationId)
        {
            DocumentationOpenApiOperationQueryResult? operation = GetOperation(packageId, version, documentName, operationId);

            return operation is null
                ? $"OpenAPI operation '{operationId}' not found for package '{packageId}' version '{version}' document '{documentName}'."
                : FormatOperation(operation);
        }

        /// <summary>
        ///     Gets one OpenAPI document by package, version, and document name.
        /// </summary>
        /// <param name="packageId">Package identifier that owns the document.</param>
        /// <param name="version">Package version that owns the document.</param>
        /// <param name="documentName">Stable OpenAPI document name.</param>
        /// <returns>The matching document, or <see langword="null" /> when no document exists.</returns>
        public DocumentationOpenApiDocumentQueryResult? GetDocument(string packageId, string version, string documentName)
        {
            using SqliteConnection connection = OpenConnection();

            if (!TableExists(connection, "DocumentationOpenApiDocuments"))
            {
                return null;
            }

            const string sql = """
                               SELECT PackageId, Version, DocumentName, Title, Description, JsonContent, RoutePath
                               FROM DocumentationOpenApiDocuments
                               WHERE PackageId = @PackageId
                                 AND Version = @Version
                                 AND DocumentName = @DocumentName
                               LIMIT 1
                               """;

            using SqliteCommand command = new(sql, connection);
            command.Parameters.AddWithValue("@PackageId", packageId);
            command.Parameters.AddWithValue("@Version", version);
            command.Parameters.AddWithValue("@DocumentName", documentName);

            using SqliteDataReader reader = command.ExecuteReader();
            return reader.Read() ? ReadDocument(reader) : null;
        }

        /// <summary>
        ///     Gets the raw OpenAPI document as an MCP-friendly text block.
        /// </summary>
        /// <param name="packageId">Package identifier that owns the document.</param>
        /// <param name="version">Package version that owns the document.</param>
        /// <param name="documentName">Stable OpenAPI document name.</param>
        /// <returns>The raw OpenAPI JSON or an explanatory not-found message.</returns>
        public string GetOpenApiDocument(string packageId, string version, string documentName)
        {
            DocumentationOpenApiDocumentQueryResult? document = GetDocument(packageId, version, documentName);

            if (document is null)
            {
                return $"OpenAPI document '{documentName}' not found for package '{packageId}' version '{version}'.";
            }

            return DocumentationMcpTextFormatter.LimitText(document.JsonContent, 50000);
        }

        /// <summary>
        ///     Gets one OpenAPI operation by package, version, document name, and operation identifier.
        /// </summary>
        /// <param name="packageId">Package identifier that owns the operation.</param>
        /// <param name="version">Package version that owns the operation.</param>
        /// <param name="documentName">Stable OpenAPI document name.</param>
        /// <param name="operationId">Stable operation identifier.</param>
        /// <returns>The matching operation, or <see langword="null" /> when no operation exists.</returns>
        public DocumentationOpenApiOperationQueryResult? GetOperation(
            string packageId,
            string version,
            string documentName,
            string operationId
        )
        {
            using SqliteConnection connection = OpenConnection();

            if (!TableExists(connection, "DocumentationOpenApiOperations"))
            {
                return null;
            }

            string securityProjection = GetSecurityJsonProjection(connection);
            string sql = $"""
                          SELECT PackageId, Version, DocumentName, OperationId, HttpMethod, Path, Summary, Description,
                                 TagsJson, ParametersJson, RequestBodyJson, ResponsesJson, {securityProjection}, RoutePath
                          FROM DocumentationOpenApiOperations
                          WHERE PackageId = @PackageId
                            AND Version = @Version
                            AND DocumentName = @DocumentName
                            AND OperationId = @OperationId
                          LIMIT 1
                          """;

            using SqliteCommand command = new(sql, connection);
            command.Parameters.AddWithValue("@PackageId", packageId);
            command.Parameters.AddWithValue("@Version", version);
            command.Parameters.AddWithValue("@DocumentName", documentName);
            command.Parameters.AddWithValue("@OperationId", operationId);

            using SqliteDataReader reader = command.ExecuteReader();
            return reader.Read() ? ReadOperation(reader) : null;
        }

        /// <summary>
        ///     Lists OpenAPI operations as an MCP-friendly text block.
        /// </summary>
        /// <param name="packageId">Package identifier that owns the document.</param>
        /// <param name="version">Package version that owns the document.</param>
        /// <param name="documentName">Stable OpenAPI document name.</param>
        /// <param name="tag">Optional tag filter.</param>
        /// <returns>A formatted operation list.</returns>
        public string ListApiOperations(string packageId, string version, string documentName, string? tag = null)
        {
            IReadOnlyList<DocumentationOpenApiOperationQueryResult> operations = ListOperations(packageId, version, documentName, tag);

            if (operations.Count == 0)
            {
                return $"No OpenAPI operations found for package '{packageId}' version '{version}' document '{documentName}'.";
            }

            return string.Join(
                "\n",
                operations.Select(operation => $"- {operation.OperationId}: {operation.HttpMethod} {operation.Path} - {operation.Summary}"));
        }

        /// <summary>
        ///     Lists OpenAPI operations for one document.
        /// </summary>
        /// <param name="packageId">Package identifier that owns the document.</param>
        /// <param name="version">Package version that owns the document.</param>
        /// <param name="documentName">Stable OpenAPI document name.</param>
        /// <param name="tag">Optional tag filter.</param>
        /// <returns>The matching operations ordered by path and method.</returns>
        public IReadOnlyList<DocumentationOpenApiOperationQueryResult> ListOperations(
            string packageId,
            string version,
            string documentName,
            string? tag = null
        )
        {
            using SqliteConnection connection = OpenConnection();

            if (!TableExists(connection, "DocumentationOpenApiOperations"))
            {
                return [];
            }

            bool hasSecurityJsonColumn = ColumnExists(connection, "DocumentationOpenApiOperations", "SecurityJson");
            string securityProjection = GetSecurityJsonProjection(hasSecurityJsonColumn);
            string sql = $"""
                          SELECT PackageId, Version, DocumentName, OperationId, HttpMethod, Path, Summary, Description,
                                 TagsJson, ParametersJson, RequestBodyJson, ResponsesJson, {securityProjection}, RoutePath
                          FROM DocumentationOpenApiOperations
                          WHERE PackageId = @PackageId
                            AND Version = @Version
                            AND DocumentName = @DocumentName
                          """;

            if (!string.IsNullOrWhiteSpace(tag))
            {
                sql += " AND TagsJson LIKE @Tag";
            }

            sql += " ORDER BY Path, HttpMethod";

            using SqliteCommand command = new(sql, connection);
            command.Parameters.AddWithValue("@PackageId", packageId);
            command.Parameters.AddWithValue("@Version", version);
            command.Parameters.AddWithValue("@DocumentName", documentName);

            if (!string.IsNullOrWhiteSpace(tag))
            {
                command.Parameters.AddWithValue("@Tag", $"%\"{tag}\"%");
            }

            return ReadOperations(command);
        }

        private SqliteConnection OpenConnection()
        {
            SqliteConnection connection = new($"Data Source={_sqliteDatabasePath}");
            connection.Open();
            return connection;
        }

        /// <summary>
        ///     Searches OpenAPI operations by path, method, summary, description, tags, or operation identifier.
        /// </summary>
        /// <param name="query">Search text.</param>
        /// <param name="packageId">Optional package identifier filter.</param>
        /// <param name="version">Optional package version filter.</param>
        /// <returns>A formatted search result list.</returns>
        public string SearchApiOperations(string query, string? packageId = null, string? version = null)
        {
            IReadOnlyList<DocumentationOpenApiOperationQueryResult> operations = SearchOperationRecords(query, packageId, version);

            if (operations.Count == 0)
            {
                return $"No OpenAPI operation found for '{query}'.";
            }

            return string.Join("\n\n", operations.Select(FormatOperation));
        }

        private IReadOnlyList<DocumentationOpenApiOperationQueryResult> SearchOperationRecords(
            string query,
            string? packageId,
            string? version
        )
        {
            using SqliteConnection connection = OpenConnection();

            if (!TableExists(connection, "DocumentationOpenApiOperations"))
            {
                return [];
            }

            bool hasSecurityJsonColumn = ColumnExists(connection, "DocumentationOpenApiOperations", "SecurityJson");
            string securityProjection = GetSecurityJsonProjection(hasSecurityJsonColumn);
            string securityPredicate = hasSecurityJsonColumn
                ? " OR SecurityJson LIKE @Query"
                : string.Empty;
            string sql = $"""
                          SELECT PackageId, Version, DocumentName, OperationId, HttpMethod, Path, Summary, Description,
                                 TagsJson, ParametersJson, RequestBodyJson, ResponsesJson, {securityProjection}, RoutePath
                          FROM DocumentationOpenApiOperations
                          WHERE (
                              OperationId LIKE @Query
                              OR HttpMethod LIKE @Query
                              OR Path LIKE @Query
                              OR Summary LIKE @Query
                              OR Description LIKE @Query
                              OR TagsJson LIKE @Query
                              {securityPredicate}
                          )
                          """;

            if (!string.IsNullOrWhiteSpace(packageId))
            {
                sql += " AND PackageId = @PackageId";
            }

            if (!string.IsNullOrWhiteSpace(version))
            {
                sql += " AND Version = @Version";
            }

            sql += " ORDER BY PackageId, Version DESC, Path, HttpMethod LIMIT 20";

            using SqliteCommand command = new(sql, connection);
            command.Parameters.AddWithValue("@Query", $"%{query}%");

            if (!string.IsNullOrWhiteSpace(packageId))
            {
                command.Parameters.AddWithValue("@PackageId", packageId);
            }

            if (!string.IsNullOrWhiteSpace(version))
            {
                command.Parameters.AddWithValue("@Version", version);
            }

            return ReadOperations(command);
        }

        #endregion
    }
}