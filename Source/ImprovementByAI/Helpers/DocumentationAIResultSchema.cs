#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using Microsoft.Data.Sqlite;

#endregion

namespace DMBDocumentationImprovementByAI
{
    internal static class DocumentationAIResultSchema
    {
        #region Static methods

        private static void EnsureColumnExists(
            SqliteConnection connection,
            string tableName,
            string columnName,
            string definition
        )
        {
            using (var probeCommand = new SqliteCommand($"PRAGMA table_info({tableName});", connection))
                using (SqliteDataReader reader = probeCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (string.Equals(reader["name"]?.ToString(), columnName, System.StringComparison.OrdinalIgnoreCase))
                        {
                            return;
                        }
                    }
                }

            using var alterCommand = new SqliteCommand(
                $"ALTER TABLE {tableName} ADD COLUMN {columnName} {definition};",
                connection);
            alterCommand.ExecuteNonQuery();
        }

        public static void EnsureMetadataColumns(SqliteConnection connection)
        {
            EnsureColumnExists(connection, "DocumentationAIResult", "PackageId", "TEXT NOT NULL DEFAULT ''");
            EnsureColumnExists(connection, "DocumentationAIResult", "Version", "TEXT NOT NULL DEFAULT ''");
            EnsureColumnExists(connection, "DocumentationAIResult", "NamespaceName", "TEXT NOT NULL DEFAULT ''");
            EnsureColumnExists(connection, "DocumentationAIResult", "ObjectName", "TEXT NOT NULL DEFAULT ''");
            EnsureColumnExists(connection, "DocumentationAIResult", "ObjectType", "TEXT NOT NULL DEFAULT ''");

            const string indexSql = """
                                    CREATE INDEX IF NOT EXISTS IX_DocumentationAIResult_ObjectIdentity
                                    ON DocumentationAIResult (PackageId, Version, NamespaceName, ObjectName, ObjectType);
                                    """;

            using var indexCommand = new SqliteCommand(indexSql, connection);
            indexCommand.ExecuteNonQuery();
        }

        #endregion
    }
}