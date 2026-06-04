#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System;
using System.Text;
using Microsoft.Data.Sqlite;

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Provides read-only SQLite queries for source-code snapshots attached to generated documentation objects.
    /// </summary>
    public sealed class SourceCodeQueryService
    {
        #region Instance fields and properties

        private readonly string _sqliteDatabasePath;

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SourceCodeQueryService" /> class.
        /// </summary>
        /// <param name="sqliteDatabasePath">Absolute or relative path to the generated documentation SQLite database.</param>
        public SourceCodeQueryService(string sqliteDatabasePath)
        {
            _sqliteDatabasePath = sqliteDatabasePath;
        }

        #endregion

        #region Instance methods

        /// <summary>
        ///     Finds the stored source-code snapshot for one documented object.
        /// </summary>
        /// <param name="objectName">Exact documented object name.</param>
        /// <param name="packageId">Package identifier that owns the documented object.</param>
        /// <param name="version">Package version that owns the documented object.</param>
        /// <param name="namespaceName">Namespace that contains the documented object.</param>
        /// <param name="objectType">Documented object type.</param>
        /// <returns>The matching source-code snapshot, or <see langword="null" /> when no source record exists.</returns>
        public SourceCodeQueryResult? FindSourceCode(
            string objectName,
            string packageId,
            string version,
            string namespaceName,
            string objectType
        )
        {
            using SqliteConnection connection = OpenConnection();

            const string sql = """
                               SELECT SourceCode, SourceFileCount
                               FROM DocumentationObjectSources
                               WHERE PackageId = @PackageId
                                 AND Version = @Version
                                 AND NamespaceName = @NamespaceName
                                 AND ObjectName = @ObjectName
                                 AND ObjectType = @ObjectType
                               LIMIT 1
                               """;

            using SqliteCommand command = new(sql, connection);
            command.Parameters.AddWithValue("@PackageId", packageId);
            command.Parameters.AddWithValue("@Version", version);
            command.Parameters.AddWithValue("@NamespaceName", namespaceName);
            command.Parameters.AddWithValue("@ObjectName", objectName);
            command.Parameters.AddWithValue("@ObjectType", objectType);

            using SqliteDataReader reader = command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return new SourceCodeQueryResult
            {
                PackageId = packageId,
                Version = version,
                NamespaceName = namespaceName,
                ObjectName = objectName,
                ObjectType = objectType,
                SourceFileCount = Convert.ToInt32(reader["SourceFileCount"]),
                SourceCode = reader["SourceCode"]?.ToString() ?? string.Empty
            };
        }

        /// <summary>
        ///     Gets the stored source-code snapshot as an MCP-friendly text block.
        /// </summary>
        /// <param name="objectName">Exact documented object name.</param>
        /// <param name="packageId">Package identifier that owns the documented object.</param>
        /// <param name="version">Package version that owns the documented object.</param>
        /// <param name="namespaceName">Namespace that contains the documented object.</param>
        /// <param name="objectType">Documented object type.</param>
        /// <returns>A formatted source-code block, or an explanatory message when no source record exists.</returns>
        public string GetSourceCode(
            string objectName,
            string packageId,
            string version,
            string namespaceName,
            string objectType
        )
        {
            SourceCodeQueryResult? result = FindSourceCode(
                objectName,
                packageId,
                version,
                namespaceName,
                objectType);

            if (result is null)
            {
                return "Source code not found.";
            }

            StringBuilder text = new();
            text.AppendLine($"Source code found for {result.NamespaceName}.{result.ObjectName} ({result.ObjectType})");
            text.AppendLine();
            text.AppendLine($"Package: {result.PackageId}");
            text.AppendLine($"Version: {result.Version}");
            text.AppendLine($"Namespace: {result.NamespaceName}");
            text.AppendLine($"Object: {result.ObjectName}");
            text.AppendLine($"Type: {result.ObjectType}");
            text.AppendLine($"Source file count: {result.SourceFileCount}");
            text.AppendLine();
            text.AppendLine("Source code:");
            text.AppendLine(DocumentationMcpTextFormatter.LimitText(result.SourceCode, 20000));

            return text.ToString();
        }

        private SqliteConnection OpenConnection()
        {
            SqliteConnection connection = new($"Data Source={_sqliteDatabasePath}");
            connection.Open();
            return connection;
        }

        #endregion
    }
}