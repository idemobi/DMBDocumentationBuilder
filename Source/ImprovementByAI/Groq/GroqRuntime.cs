#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

#endregion

namespace DMBDocumentationImprovementByGroq
{
    /// <summary>
    ///     Runs Groq documentation improvement against a generated documentation database.
    /// </summary>
    public static class GroqRuntime
    {
        #region Nested type: DocumentationDatabaseImprover

        #region Database

        private sealed class DocumentationDatabaseImprover
        {
            #region Static methods

            /// <summary>
            ///     Computes the hash used to detect whether AI output is stale.
            /// </summary>
            /// <returns>A stable hash for the documentation object and prompt inputs.</returns>
            public static string ComputeHash(DocumentationObjectRow row, string ctx, string s, string ss, string k)
            {
                string raw = string.Join("|",
                    row.PackageId,
                    row.Version,
                    row.NamespaceName,
                    row.ObjectName,
                    row.ObjectType,
                    row.RoutePath,
                    row.ModelInJson,
                    row.TechnicalKeywords,
                    row.Keywords,
                    ctx,
                    s,
                    ss,
                    k);

                return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(raw)));
            }

            #endregion

            #region Instance fields and properties

            private readonly string _aiDatabasePath;

            private readonly string _mainDatabasePath;

            #endregion

            #region Instance constructors and destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="DocumentationDatabaseImprover" /> class.
            /// </summary>
            public DocumentationDatabaseImprover(string mainDatabasePath, string aiDatabasePath)
            {
                _mainDatabasePath = mainDatabasePath;
                _aiDatabasePath = aiDatabasePath;
            }

            #endregion

            #region Instance methods

            public async Task<Dictionary<long, string>> LoadGroqHashesAsync(CancellationToken ct)
            {
                using var connection = new SqliteConnection($"Data Source={_aiDatabasePath}");
                await connection.OpenAsync(ct);

                EnsureGroqTable(connection);

                const string sql = """
                                   SELECT DocumentationObjectId, AIContentLastHash
                                   FROM DocumentationAIResult;
                                   """;

                using var cmd = new SqliteCommand(sql, connection);
                using var reader = await cmd.ExecuteReaderAsync(ct);

                var dict = new Dictionary<long, string>();

                while (await reader.ReadAsync(ct))
                {
                    dict[reader.GetInt64(0)] = reader.GetString(1);
                }

                return dict;
            }

            /// <summary>
            ///     Loads documentation database rows required for AI improvement.
            /// </summary>
            /// <returns>The loaded database rows.</returns>
            public async Task<List<DocumentationObjectRow>> LoadObjectsAsync(int max, CancellationToken ct)
            {
                using var connection = new SqliteConnection($"Data Source={_mainDatabasePath}");
                await connection.OpenAsync(ct);

                string sql = """
                             SELECT Id, PackageId, Version, NamespaceName, ObjectName, ObjectType, RoutePath, ModelInJson, TechnicalKeywords, Keywords
                             FROM DocumentationObjects
                             ORDER BY Id
                             """;

                if (max > 0)
                {
                    sql += $" LIMIT {max}";
                }

                using var cmd = new SqliteCommand(sql, connection);
                using var reader = await cmd.ExecuteReaderAsync(ct);

                var list = new List<DocumentationObjectRow>();

                while (await reader.ReadAsync(ct))
                {
                    list.Add(new DocumentationObjectRow
                    {
                        Id = reader.GetInt64(0),
                        PackageId = reader.GetString(1),
                        Version = reader.GetString(2),
                        NamespaceName = reader.GetString(3),
                        ObjectName = reader.GetString(4),
                        ObjectType = reader.GetString(5),
                        RoutePath = reader.GetString(6),
                        ModelInJson = reader.GetString(7),
                        TechnicalKeywords = reader.GetString(8),
                        Keywords = reader.GetString(9)
                    });
                }

                return list;
            }

            /// <summary>
            ///     Inserts or updates Groq AI output for a documentation object.
            /// </summary>
            /// <returns>A task that completes when the row has been stored.</returns>
            public async Task UpsertGroqAsync(
                long id,
                string route,
                string summary,
                string shortSummary,
                string keywords,
                string hash,
                string model,
                CancellationToken ct
            )
            {
                using var connection = new SqliteConnection($"Data Source={_aiDatabasePath}");
                await connection.OpenAsync(ct);

                EnsureGroqTable(connection);

                const string sql = @"
INSERT INTO DocumentationAIResult
(DocumentationObjectId, RoutePath, AISummary, AISummaryShort, AIKeywords, AIContentLastHash, AIUpdatedAt, AIModel)
VALUES
(@Id, @Route, @S, @SS, @K, @H, @D, @M)
ON CONFLICT(DocumentationObjectId) DO UPDATE SET
    RoutePath = excluded.RoutePath,
    AISummary = excluded.AISummary,
    AISummaryShort = excluded.AISummaryShort,
    AIKeywords = excluded.AIKeywords,
    AIContentLastHash = excluded.AIContentLastHash,
    AIUpdatedAt = excluded.AIUpdatedAt,
    AIModel = excluded.AIModel;
";

                using var cmd = new SqliteCommand(sql, connection);

                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Route", route);
                cmd.Parameters.AddWithValue("@S", summary);
                cmd.Parameters.AddWithValue("@SS", shortSummary);
                cmd.Parameters.AddWithValue("@K", keywords);
                cmd.Parameters.AddWithValue("@H", hash);
                cmd.Parameters.AddWithValue("@D", DateTime.UtcNow.ToString("O"));
                cmd.Parameters.AddWithValue("@M", model);

                await cmd.ExecuteNonQueryAsync(ct);
            }

            #endregion
        }

        #endregion

        #endregion

        #region Nested type: DocumentationObjectRow

        private sealed class DocumentationObjectRow
        {
            #region Instance fields and properties

            /// <summary>
            ///     Gets or sets the documentation object database identifier.
            /// </summary>
            public long Id { get; set; }

            /// <summary>
            ///     Gets or sets human-readable keywords generated or stored for the documentation object.
            /// </summary>
            public string Keywords { get; set; } = "";

            /// <summary>
            ///     Gets or sets the serialized source model used as AI prompt input.
            /// </summary>
            public string ModelInJson { get; set; } = "";

            /// <summary>
            ///     Gets or sets the namespace name for the documentation object.
            /// </summary>
            public string NamespaceName { get; set; } = "";

            /// <summary>
            ///     Gets or sets the documented object name.
            /// </summary>
            public string ObjectName { get; set; } = "";

            /// <summary>
            ///     Gets or sets the documented object type.
            /// </summary>
            public string ObjectType { get; set; } = "";

            /// <summary>
            ///     Gets or sets the package identifier for the documentation object.
            /// </summary>
            public string PackageId { get; set; } = "";

            /// <summary>
            ///     Gets or sets the route path that displays the documentation object.
            /// </summary>
            public string RoutePath { get; set; } = "";

            /// <summary>
            ///     Gets or sets technical keywords extracted from the documentation object.
            /// </summary>
            public string TechnicalKeywords { get; set; } = "";

            /// <summary>
            ///     Gets or sets the package version for the documentation object.
            /// </summary>
            public string Version { get; set; } = "";

            #endregion
        }

        #endregion

        #region Nested type: DocumentationPromptFactory

        #region PromptFactory

        private static class DocumentationPromptFactory
        {
            #region Static methods

            private static string BuildBase(DocumentationObjectRow row, string ctx, string custom, int max, string rule)
            {
                return $"""
                        You are a technical documentation assistant.

                        Rules:
                        {rule}

                        Context:
                        {ctx}

                        Instructions:
                        {custom}

                        Object:
                        {row.NamespaceName}.{row.ObjectName}

                        Model:
                        {LimitText(row.ModelInJson, max)}
                        """;
            }

            /// <summary>
            ///     Builds the build keywords prompt for AI generation.
            /// </summary>
            /// <returns>The prompt text sent to the AI model.</returns>
            public static string BuildKeywordsPrompt(DocumentationObjectRow row, string ctx, string custom, int max)
                => BuildBase(row, ctx, custom, max, "comma-separated keywords");

            /// <summary>
            ///     Builds the build short summary prompt for AI generation.
            /// </summary>
            /// <returns>The prompt text sent to the AI model.</returns>
            public static string BuildShortSummaryPrompt(DocumentationObjectRow row, string ctx, string custom, int max)
                => BuildBase(row, ctx, custom, max, "one sentence max 220 chars");

            /// <summary>
            ///     Builds the build summary prompt for AI generation.
            /// </summary>
            /// <returns>The prompt text sent to the AI model.</returns>
            public static string BuildSummaryPrompt(DocumentationObjectRow row, string ctx, string custom, int max)
                => BuildBase(row, ctx, custom, max, "3 to 6 sentences");

            private static string LimitText(string value, int max)
                => value.Length <= max ? value : value[..max] + "...";

            #endregion
        }

        #endregion

        #endregion

        #region TABLE

        private static string GetAIDatabasePath(string mainDatabasePath, GroqModel model)
        {
            string mainDirectory = Path.GetDirectoryName(mainDatabasePath)
                                   ?? throw new InvalidOperationException("Unable to resolve database directory.");

            string fileName = $"Groq_{model}.db";

            return Path.Combine(mainDirectory, "AI", "Groq", fileName);
        }

        private static void EnsureGroqTable(SqliteConnection connection)
        {
            const string sql = @"
CREATE TABLE IF NOT EXISTS DocumentationAIResult
(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    DocumentationObjectId INTEGER NOT NULL,
    RoutePath TEXT NOT NULL,

    AISummary TEXT NOT NULL DEFAULT '',
    AISummaryShort TEXT NOT NULL DEFAULT '',
    AIKeywords TEXT NOT NULL DEFAULT '',
    AIContentLastHash TEXT NOT NULL DEFAULT '',
    AIUpdatedAt TEXT NOT NULL DEFAULT '',
    AIModel TEXT NOT NULL DEFAULT '',
    AIEmbedding BLOB NULL,

    UNIQUE(DocumentationObjectId)
);
";
            using var command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();
        }

        private static void EnsureRenderSourcesTable(SqliteConnection connection)
        {
            const string sql = @"
CREATE TABLE IF NOT EXISTS DocumentationAIRenderSources
(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Provider TEXT NOT NULL,
    Model TEXT NOT NULL,
    DatabasePath TEXT NOT NULL,
    IsEnabled INTEGER NOT NULL DEFAULT 1,
    CreatedUtc TEXT NOT NULL DEFAULT '',
    UpdatedUtc TEXT NOT NULL DEFAULT '',
    UNIQUE(Provider, Model)
);

CREATE INDEX IF NOT EXISTS IX_DocumentationAIRenderSources_IsEnabled
ON DocumentationAIRenderSources (IsEnabled);
";
            using var command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();
        }

        private static string GetRelativeAIDatabasePath(string mainDatabasePath, string aiDatabasePath)
        {
            string mainDatabaseDirectory = Path.GetDirectoryName(mainDatabasePath)
                                           ?? throw new InvalidOperationException("Unable to resolve main database directory.");

            return Path.GetRelativePath(mainDatabaseDirectory, aiDatabasePath);
        }

        private static void RegisterRenderSource(
            string mainDatabasePath,
            string provider,
            string model,
            string aiDatabasePath
        )
        {
            using var connection = new SqliteConnection($"Data Source={mainDatabasePath}");
            connection.Open();

            EnsureRenderSourcesTable(connection);

            string relativeDatabasePath = GetRelativeAIDatabasePath(mainDatabasePath, aiDatabasePath);

            const string sql = @"
INSERT INTO DocumentationAIRenderSources
(
    Provider,
    Model,
    DatabasePath,
    IsEnabled,
    CreatedUtc,
    UpdatedUtc
)
VALUES
(
    @Provider,
    @Model,
    @DatabasePath,
    1,
    @NowUtc,
    @NowUtc
)
ON CONFLICT(Provider, Model) DO UPDATE SET
    DatabasePath = excluded.DatabasePath,
    IsEnabled = excluded.IsEnabled,
    UpdatedUtc = excluded.UpdatedUtc;
";

            using var command = new SqliteCommand(sql, connection);

            string nowUtc = DateTime.UtcNow.ToString("O");

            command.Parameters.AddWithValue("@Provider", provider);
            command.Parameters.AddWithValue("@Model", model);
            command.Parameters.AddWithValue("@DatabasePath", relativeDatabasePath);
            command.Parameters.AddWithValue("@NowUtc", nowUtc);

            command.ExecuteNonQuery();
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Runs Groq documentation improvement synchronously.
        /// </summary>
        public static void Run(GroqOptions options)
        {
            Console.WriteLine("Starting Groq runtime...");
            RunAsync(options).GetAwaiter().GetResult();
            Console.WriteLine("Finished Groq runtime.");
        }

        /// <summary>
        ///     Runs Groq documentation improvement asynchronously.
        /// </summary>
        /// <returns>A task that completes when documentation improvement finishes.</returns>
        public static async Task RunAsync(GroqOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            if (string.IsNullOrWhiteSpace(options.DatabasePath))
            {
                throw new ArgumentException("DatabasePath is required.", nameof(options));
            }

            string mainDatabasePath = Path.GetFullPath(options.DatabasePath);
            string aiDatabasePath = GetAIDatabasePath(mainDatabasePath, options.Model);

            Directory.CreateDirectory(Path.GetDirectoryName(aiDatabasePath)!);

            using (var bootstrapConnection = new SqliteConnection($"Data Source={aiDatabasePath}"))
            {
                await bootstrapConnection.OpenAsync();
                EnsureGroqTable(bootstrapConnection);
            }

            RegisterRenderSource(
                mainDatabasePath,
                provider: "Groq",
                model: options.Model.ToString(),
                aiDatabasePath: aiDatabasePath);

            using var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            GroqModel currentModel = options.Model;
            string currentModelName = currentModel.ToModelString();

            Console.WriteLine($"[GROQ] Main DB: {mainDatabasePath}");
            Console.WriteLine($"[GROQ] AI DB:   {aiDatabasePath}");
            Console.WriteLine($"[GROQ] Model:   {currentModelName}");

            var generator = new GroqTextGenerator(currentModel, options.ApiKey);
            var databaseImprover = new DocumentationDatabaseImprover(mainDatabasePath, aiDatabasePath);

            var rows = await databaseImprover.LoadObjectsAsync(options.MaxObjectsToProcess, cancellationToken);
            var existingHashes = await databaseImprover.LoadGroqHashesAsync(cancellationToken);

            Console.WriteLine($"[GROQ] {rows.Count} object(s) loaded.");

            int i = 0;

            foreach (var row in rows)
            {
                i++;

                try
                {
                    string currentHash = DocumentationDatabaseImprover.ComputeHash(
                        row,
                        options.ProjectContextPrompt,
                        options.SummaryPrompt,
                        options.ShortSummaryPrompt,
                        options.KeywordsPrompt);

                    existingHashes.TryGetValue(row.Id, out var existingHash);

                    if (!options.ForceRegenerate &&
                        string.Equals(existingHash, currentHash, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    Console.WriteLine($"[GROQ n° {i}] Processing {row.NamespaceName}.{row.ObjectName} ({row.ObjectType})");

                    string summary = await generator.GenerateTextAsync(
                        DocumentationPromptFactory.BuildSummaryPrompt(
                            row,
                            options.ProjectContextPrompt,
                            options.SummaryPrompt,
                            options.MaxModelJsonLength),
                        options.RequestTimeout,
                        cancellationToken);

                    string shortSummary = await generator.GenerateTextAsync(
                        DocumentationPromptFactory.BuildShortSummaryPrompt(
                            row,
                            options.ProjectContextPrompt,
                            options.ShortSummaryPrompt,
                            options.MaxModelJsonLength),
                        options.RequestTimeout,
                        cancellationToken);

                    string keywords = await generator.GenerateTextAsync(
                        DocumentationPromptFactory.BuildKeywordsPrompt(
                            row,
                            options.ProjectContextPrompt,
                            options.KeywordsPrompt,
                            options.MaxModelJsonLength),
                        options.RequestTimeout,
                        cancellationToken);

                    await databaseImprover.UpsertGroqAsync(
                        row.Id,
                        row.RoutePath,
                        summary,
                        shortSummary,
                        keywords,
                        currentHash,
                        currentModelName,
                        cancellationToken);

                    existingHashes[row.Id] = currentHash;
                }
                catch (Exception exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[GROQ][ERROR] Failed for {row.NamespaceName}.{row.ObjectName} ({row.ObjectType})");
                    Console.WriteLine(exception.Message);
                    Console.ResetColor();
                }
            }
        }

        #endregion
    }
}