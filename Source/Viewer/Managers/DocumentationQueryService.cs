#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Provides read-only SQLite queries for generated documentation objects.
    /// </summary>
    /// <remarks>
    ///     The service expects a generated documentation database containing the `DocumentationObjects` table.
    ///     It does not create, migrate, or update the database.
    /// </remarks>
    public sealed class DocumentationQueryService
    {
        #region Static methods

        private static bool IsPreferredDocumentationPackage(string packageId, string objectName, string? namespaceName)
        {
            return string.Equals(packageId, objectName, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(packageId, namespaceName, StringComparison.OrdinalIgnoreCase);
        }

        private static List<string> NormalizeMemberKinds(IReadOnlyCollection<string>? memberKinds)
        {
            if (memberKinds is null)
            {
                return [];
            }

            return memberKinds
                .Where(memberKind => !string.IsNullOrWhiteSpace(memberKind))
                .Select(memberKind => memberKind.Trim())
                .Distinct(StringComparer.Ordinal)
                .ToList();
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
        ///     Initializes a new instance of the <see cref="DocumentationQueryService" /> class.
        /// </summary>
        /// <param name="sqliteDatabasePath">Absolute or relative path to the generated documentation SQLite database.</param>
        public DocumentationQueryService(string sqliteDatabasePath)
        {
            _sqliteDatabasePath = sqliteDatabasePath;
        }

        #endregion

        #region Instance methods

        /// <summary>
        ///     Finds documentation objects whose keywords reference the specified object name.
        /// </summary>
        /// <param name="objectName">Object name used as the related-object search token.</param>
        /// <param name="packageId">Optional package identifier filter.</param>
        /// <param name="version">Optional package version filter.</param>
        /// <returns>A list of related documentation objects ordered by object name.</returns>
        public List<DocumentationSearchResultItem> FindRelatedObjects(
            string objectName,
            string? packageId = null,
            string? version = null
        )
        {
            using SqliteConnection connection = OpenConnection();

            string sql = """
                         SELECT PackageId, Version, NamespaceName, ObjectName, ObjectType, RoutePath
                         FROM DocumentationObjects
                         WHERE ObjectName <> @ObjectName
                           AND (
                               TechnicalKeywords LIKE @ObjectNameLike
                               OR Keywords LIKE @ObjectNameLike
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

            sql += """

                   AND NamespaceName <> '<global namespace>'
                   AND ObjectName <> '<global namespace>'
                   ORDER BY ObjectName
                   LIMIT 20
                   """;

            using SqliteCommand command = new(sql, connection);
            command.Parameters.AddWithValue("@ObjectName", objectName);
            command.Parameters.AddWithValue("@ObjectNameLike", $"%{objectName}%");

            if (!string.IsNullOrWhiteSpace(packageId))
            {
                command.Parameters.AddWithValue("@PackageId", packageId);
            }

            if (!string.IsNullOrWhiteSpace(version))
            {
                command.Parameters.AddWithValue("@Version", version);
            }

            using SqliteDataReader reader = command.ExecuteReader();

            List<DocumentationSearchResultItem> results = [];

            while (reader.Read())
            {
                results.Add(new DocumentationSearchResultItem
                {
                    PackageId = reader["PackageId"]?.ToString() ?? string.Empty,
                    Version = reader["Version"]?.ToString() ?? string.Empty,
                    NamespaceName = reader["NamespaceName"]?.ToString() ?? string.Empty,
                    ObjectName = reader["ObjectName"]?.ToString() ?? string.Empty,
                    ObjectType = reader["ObjectType"]?.ToString() ?? string.Empty,
                    RoutePath = reader["RoutePath"]?.ToString() ?? string.Empty
                });
            }

            return results;
        }

        /// <summary>
        ///     Retrieves one generated documentation object by name and optional metadata filters.
        /// </summary>
        /// <param name="objectName">Exact documented object name.</param>
        /// <param name="packageId">Optional package identifier filter.</param>
        /// <param name="version">Optional package version filter.</param>
        /// <param name="namespaceName">Optional namespace filter.</param>
        /// <param name="objectType">Optional documented object type filter.</param>
        /// <returns>The matching documentation object, or <see langword="null" /> when no record exists.</returns>
        public DocumentationQueryResult? GetDocumentation(
            string objectName,
            string? packageId = null,
            string? version = null,
            string? namespaceName = null,
            string? objectType = null
        )
        {
            using SqliteConnection connection = OpenConnection();

            string sql = """
                         SELECT Id, PackageId, Version, NamespaceName, ObjectName, ObjectType,
                                RoutePath, HtmlContent, TechnicalKeywords, Keywords, Builder
                         FROM DocumentationObjects
                         WHERE ObjectName = @ObjectName
                         """;

            if (!string.IsNullOrWhiteSpace(objectType)) sql += " AND ObjectType = @ObjectType";

            if (!string.IsNullOrWhiteSpace(packageId)) sql += " AND PackageId = @PackageId";

            if (!string.IsNullOrWhiteSpace(version)) sql += " AND Version = @Version";

            if (!string.IsNullOrWhiteSpace(namespaceName)) sql += " AND NamespaceName = @NamespaceName";

            using SqliteCommand command = new(sql, connection);
            command.Parameters.AddWithValue("@ObjectName", objectName);

            if (!string.IsNullOrWhiteSpace(objectType)) command.Parameters.AddWithValue("@ObjectType", objectType);

            if (!string.IsNullOrWhiteSpace(packageId)) command.Parameters.AddWithValue("@PackageId", packageId);

            if (!string.IsNullOrWhiteSpace(version)) command.Parameters.AddWithValue("@Version", version);

            if (!string.IsNullOrWhiteSpace(namespaceName)) command.Parameters.AddWithValue("@NamespaceName", namespaceName);

            using SqliteDataReader reader = command.ExecuteReader();

            List<DocumentationQueryResult> results = [];

            while (reader.Read())
            {
                results.Add(new DocumentationQueryResult
                {
                    Id = reader.GetInt64(0),
                    PackageId = reader["PackageId"]?.ToString() ?? string.Empty,
                    Version = reader["Version"]?.ToString() ?? string.Empty,
                    NamespaceName = reader["NamespaceName"]?.ToString() ?? string.Empty,
                    ObjectName = reader["ObjectName"]?.ToString() ?? string.Empty,
                    ObjectType = reader["ObjectType"]?.ToString() ?? string.Empty,
                    RoutePath = reader["RoutePath"]?.ToString() ?? string.Empty,
                    HtmlContent = reader["HtmlContent"]?.ToString() ?? string.Empty,
                    TechnicalKeywords = reader["TechnicalKeywords"]?.ToString() ?? string.Empty,
                    Keywords = reader["Keywords"]?.ToString() ?? string.Empty,
                    Builder = reader["Builder"]?.ToString() ?? string.Empty
                });
            }

            return results
                .OrderByDescending(result => result.Version, DocumentationVersionComparer.Instance)
                .ThenByDescending(result => IsPreferredDocumentationPackage(result.PackageId, objectName, namespaceName))
                .FirstOrDefault();
        }

        /// <summary>
        ///     Lists generated documentation objects contained in one namespace.
        /// </summary>
        /// <param name="namespaceName">Exact namespace name to list.</param>
        /// <param name="packageId">Optional package identifier filter.</param>
        /// <param name="version">Optional package version filter.</param>
        /// <returns>A list of documentation objects ordered by object type and object name.</returns>
        public List<DocumentationSearchResultItem> ListNamespaceObjects(
            string namespaceName,
            string? packageId = null,
            string? version = null
        )
        {
            using SqliteConnection connection = OpenConnection();

            string sql = """
                         SELECT PackageId, Version, NamespaceName, ObjectName, ObjectType, RoutePath
                         FROM DocumentationObjects
                         WHERE NamespaceName = @NamespaceName
                         """;

            if (!string.IsNullOrWhiteSpace(packageId))
            {
                sql += " AND PackageId = @PackageId";
            }

            if (!string.IsNullOrWhiteSpace(version))
            {
                sql += " AND Version = @Version";
            }

            sql += """

                   AND ObjectName <> '<global namespace>'
                   ORDER BY ObjectType, ObjectName
                   LIMIT 200
                   """;

            using SqliteCommand command = new(sql, connection);
            command.Parameters.AddWithValue("@NamespaceName", namespaceName);

            if (!string.IsNullOrWhiteSpace(packageId))
            {
                command.Parameters.AddWithValue("@PackageId", packageId);
            }

            if (!string.IsNullOrWhiteSpace(version))
            {
                command.Parameters.AddWithValue("@Version", version);
            }

            using SqliteDataReader reader = command.ExecuteReader();

            List<DocumentationSearchResultItem> results = [];

            while (reader.Read())
            {
                results.Add(new DocumentationSearchResultItem
                {
                    PackageId = reader["PackageId"]?.ToString() ?? string.Empty,
                    Version = reader["Version"]?.ToString() ?? string.Empty,
                    NamespaceName = reader["NamespaceName"]?.ToString() ?? string.Empty,
                    ObjectName = reader["ObjectName"]?.ToString() ?? string.Empty,
                    ObjectType = reader["ObjectType"]?.ToString() ?? string.Empty,
                    RoutePath = reader["RoutePath"]?.ToString() ?? string.Empty
                });
            }

            return results;
        }

        /// <summary>
        ///     Lists granular members for one documented object.
        /// </summary>
        /// <param name="objectName">Exact documented object name.</param>
        /// <param name="memberKind">Optional member kind filter, such as <c>Method</c> or <c>Property</c>.</param>
        /// <param name="packageId">Optional package identifier filter.</param>
        /// <param name="version">Optional package version filter.</param>
        /// <param name="namespaceName">Optional namespace filter.</param>
        /// <param name="objectType">Optional documented object type filter.</param>
        /// <returns>A list of matching granular members ordered by sort order and signature.</returns>
        public List<DocumentationMemberQueryResult> ListObjectMembers(
            string objectName,
            string? memberKind = null,
            string? packageId = null,
            string? version = null,
            string? namespaceName = null,
            string? objectType = null
        )
        {
            IReadOnlyCollection<string>? memberKinds = string.IsNullOrWhiteSpace(memberKind)
                ? null
                : [memberKind];

            return ListObjectMembers(
                objectName,
                memberKinds,
                packageId,
                version,
                namespaceName,
                objectType);
        }

        /// <summary>
        ///     Lists granular members for one documented object.
        /// </summary>
        /// <param name="objectName">Exact documented object name.</param>
        /// <param name="memberKinds">Optional member kind filters, such as <c>Method</c> or <c>Property</c>.</param>
        /// <param name="packageId">Optional package identifier filter.</param>
        /// <param name="version">Optional package version filter.</param>
        /// <param name="namespaceName">Optional namespace filter.</param>
        /// <param name="objectType">Optional documented object type filter.</param>
        /// <returns>A list of matching granular members ordered by sort order and signature.</returns>
        public List<DocumentationMemberQueryResult> ListObjectMembers(
            string objectName,
            IReadOnlyCollection<string>? memberKinds,
            string? packageId = null,
            string? version = null,
            string? namespaceName = null,
            string? objectType = null
        )
        {
            if (string.IsNullOrWhiteSpace(objectName))
            {
                return [];
            }

            DocumentationQueryResult? documentation = GetDocumentation(
                objectName,
                packageId,
                version,
                namespaceName,
                objectType);

            if (documentation is null)
            {
                return [];
            }

            using SqliteConnection connection = OpenConnection();

            if (!TableExists(connection, "DocumentationMembers"))
            {
                return [];
            }

            string sql = """
                         SELECT MemberKind, MemberName, Signature, SummaryHtml, RemarksHtml, ReturnsHtml,
                                ValueHtml, ExampleHtml, Accessibility, IsObsolete, ObsoleteMessage,
                                ExtensionTypeName, ExtensionNamespaceName, ParametersJson, ExceptionsJson, SortOrder
                         FROM DocumentationMembers
                         WHERE PackageId = @PackageId
                           AND Version = @Version
                           AND NamespaceName = @NamespaceName
                           AND ObjectName = @ObjectName
                           AND ObjectType = @ObjectType
                         """;

            List<string> normalizedMemberKinds = NormalizeMemberKinds(memberKinds);

            if (normalizedMemberKinds.Count > 0)
            {
                string memberKindParameters = string.Join(", ", normalizedMemberKinds.Select((_, index) => $"@MemberKind{index}"));
                sql += $" AND MemberKind IN ({memberKindParameters})";
            }

            sql += " ORDER BY SortOrder, Signature";

            using SqliteCommand command = new(sql, connection);
            command.Parameters.AddWithValue("@PackageId", documentation.PackageId);
            command.Parameters.AddWithValue("@Version", documentation.Version);
            command.Parameters.AddWithValue("@NamespaceName", documentation.NamespaceName);
            command.Parameters.AddWithValue("@ObjectName", documentation.ObjectName);
            command.Parameters.AddWithValue("@ObjectType", documentation.ObjectType);

            for (int index = 0; index < normalizedMemberKinds.Count; index++)
            {
                command.Parameters.AddWithValue($"@MemberKind{index}", normalizedMemberKinds[index]);
            }

            using SqliteDataReader reader = command.ExecuteReader();

            List<DocumentationMemberQueryResult> results = [];

            while (reader.Read())
            {
                results.Add(new DocumentationMemberQueryResult
                {
                    MemberKind = reader["MemberKind"]?.ToString() ?? string.Empty,
                    MemberName = reader["MemberName"]?.ToString() ?? string.Empty,
                    Signature = reader["Signature"]?.ToString() ?? string.Empty,
                    SummaryHtml = reader["SummaryHtml"]?.ToString() ?? string.Empty,
                    RemarksHtml = reader["RemarksHtml"]?.ToString() ?? string.Empty,
                    ReturnsHtml = reader["ReturnsHtml"]?.ToString() ?? string.Empty,
                    ValueHtml = reader["ValueHtml"]?.ToString() ?? string.Empty,
                    ExampleHtml = reader["ExampleHtml"]?.ToString() ?? string.Empty,
                    Accessibility = reader["Accessibility"]?.ToString() ?? string.Empty,
                    IsObsolete = Convert.ToInt32(reader["IsObsolete"]) != 0,
                    ObsoleteMessage = reader["ObsoleteMessage"]?.ToString() ?? string.Empty,
                    ExtensionTypeName = reader["ExtensionTypeName"]?.ToString() ?? string.Empty,
                    ExtensionNamespaceName = reader["ExtensionNamespaceName"]?.ToString() ?? string.Empty,
                    ParametersJson = reader["ParametersJson"]?.ToString() ?? "[]",
                    ExceptionsJson = reader["ExceptionsJson"]?.ToString() ?? "[]",
                    SortOrder = Convert.ToInt32(reader["SortOrder"])
                });
            }

            return results;
        }

        private SqliteConnection OpenConnection()
        {
            SqliteConnection connection = new($"Data Source={_sqliteDatabasePath}");
            connection.Open();
            return connection;
        }

        /// <summary>
        ///     Searches generated documentation objects by technical keywords, text keywords, or object name.
        /// </summary>
        /// <param name="query">Search text used in SQLite `LIKE` filters.</param>
        /// <param name="packageId">Optional package identifier filter.</param>
        /// <param name="version">Optional package version filter.</param>
        /// <returns>A list of matching documentation objects ordered by object name.</returns>
        public List<DocumentationSearchResultItem> SearchDocumentation(
            string query,
            string? packageId = null,
            string? version = null
        )
        {
            using SqliteConnection connection = OpenConnection();

            string sql = """
                         SELECT PackageId, Version, NamespaceName, ObjectName, ObjectType, RoutePath
                         FROM DocumentationObjects
                         WHERE (TechnicalKeywords LIKE @Query OR Keywords LIKE @Query OR ObjectName LIKE @Query)
                         """;

            if (!string.IsNullOrWhiteSpace(packageId)) sql += " AND PackageId = @PackageId";

            if (!string.IsNullOrWhiteSpace(version)) sql += " AND Version = @Version";

            sql += """

                   AND NamespaceName <> '<global namespace>'
                   AND ObjectName <> '<global namespace>'
                   ORDER BY ObjectName
                   LIMIT 20
                   """;

            using SqliteCommand command = new(sql, connection);
            command.Parameters.AddWithValue("@Query", $"%{query}%");

            if (!string.IsNullOrWhiteSpace(packageId)) command.Parameters.AddWithValue("@PackageId", packageId);

            if (!string.IsNullOrWhiteSpace(version)) command.Parameters.AddWithValue("@Version", version);

            using SqliteDataReader reader = command.ExecuteReader();

            List<DocumentationSearchResultItem> results = [];

            while (reader.Read())
            {
                results.Add(new DocumentationSearchResultItem
                {
                    PackageId = reader["PackageId"]?.ToString() ?? string.Empty,
                    Version = reader["Version"]?.ToString() ?? string.Empty,
                    NamespaceName = reader["NamespaceName"]?.ToString() ?? string.Empty,
                    ObjectName = reader["ObjectName"]?.ToString() ?? string.Empty,
                    ObjectType = reader["ObjectType"]?.ToString() ?? string.Empty,
                    RoutePath = reader["RoutePath"]?.ToString() ?? string.Empty
                });
            }

            return results;
        }

        #endregion

        #region Nested type: DocumentationVersionComparer

        private sealed class DocumentationVersionComparer : IComparer<string>
        {
            #region Static fields and properties

            /// <summary>
            ///     Gets the shared comparer instance used to order documentation versions.
            /// </summary>
            public static readonly DocumentationVersionComparer Instance = new();

            #endregion

            #region Static methods

            private static int[] ParseVersionParts(string? value)
            {
                int[] parts = new int[4];

                if (string.IsNullOrWhiteSpace(value))
                {
                    return parts;
                }

                string normalized = value.Trim();
                int metadataIndex = normalized.IndexOf('+');

                if (metadataIndex >= 0)
                {
                    normalized = normalized[..metadataIndex];
                }

                int preReleaseIndex = normalized.IndexOf('-');

                if (preReleaseIndex >= 0)
                {
                    normalized = normalized[..preReleaseIndex];
                }

                string[] tokens = normalized.Split('.', StringSplitOptions.RemoveEmptyEntries);

                for (int index = 0; index < tokens.Length && index < parts.Length; index++)
                {
                    if (int.TryParse(tokens[index], out int parsedPart))
                    {
                        parts[index] = parsedPart;
                    }
                }

                return parts;
            }

            #endregion

            #region Instance methods

            #region From interface IComparer<string>

            /// <summary>
            ///     Compares two documentation version strings by normalized numeric version parts.
            /// </summary>
            /// <param name="x">The first version string.</param>
            /// <param name="y">The second version string.</param>
            /// <returns>A value less than, equal to, or greater than zero depending on version ordering.</returns>
            public int Compare(string? x, string? y)
            {
                if (string.Equals(x, y, StringComparison.OrdinalIgnoreCase))
                {
                    return 0;
                }

                int[] xParts = ParseVersionParts(x);
                int[] yParts = ParseVersionParts(y);

                for (int index = 0; index < xParts.Length; index++)
                {
                    int partComparison = xParts[index].CompareTo(yParts[index]);

                    if (partComparison != 0)
                    {
                        return partComparison;
                    }
                }

                return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
            }

            #endregion

            #endregion
        }

        #endregion
    }
}