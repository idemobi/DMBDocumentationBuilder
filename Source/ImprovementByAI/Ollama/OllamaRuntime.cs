#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DMBDocumentationImprovementByAI;
using Microsoft.Data.Sqlite;

#endregion

namespace DMBDocumentationImprovementByOllama
{
    /// <summary>
    ///     Runs Ollama documentation improvement against a generated documentation database.
    /// </summary>
    public static class OllamaRuntime
    {
        #region Private methods

        private static string GetAIDatabasePath(string mainDatabasePath, OllamaModel model)
        {
            string mainDirectory = Path.GetDirectoryName(mainDatabasePath)
                                   ?? throw new InvalidOperationException("Unable to resolve database directory.");

            string fileName = $"Ollama_{model}.db";

            return Path.Combine(mainDirectory, "AI", "Ollama", fileName);
        }

        private static void EnsureOllamaTable(SqliteConnection connection)
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
            DocumentationAIResultSchema.EnsureMetadataColumns(connection);
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

        /// <summary>
        ///     Runs Ollama documentation improvement asynchronously.
        /// </summary>
        /// <returns>A task that completes when documentation improvement finishes.</returns>
        public static async Task RunAsync(OllamaOptions options)
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
                EnsureOllamaTable(bootstrapConnection);
            }

            RegisterRenderSource(
                mainDatabasePath,
                provider: "Ollama",
                model: options.Model.ToString(),
                aiDatabasePath: aiDatabasePath);

            using var cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            using var processManager = new OllamaProcessManager();
            await processManager.EnsureServerRunningAsync(options.StartOllamaServerIfNeeded, cancellationToken);

            OllamaModel currentModel = options.Model;
            string currentModelName = currentModel.ToModelString();

            Console.WriteLine($"[OLLAMA] Main DB: {mainDatabasePath}");
            Console.WriteLine($"[OLLAMA] AI DB:   {aiDatabasePath}");
            Console.WriteLine($"[OLLAMA] Model:   {currentModelName}");

            var generator = new OllamaTextGenerator(currentModelName);
            await generator.PreloadModelAsync(cancellationToken);

            var database = new DocumentationDatabaseImprover(mainDatabasePath, aiDatabasePath);

            List<DocumentationObjectRow> rows = await database.LoadObjectsAsync(options.MaxObjectsToProcess, options.ObjectSelectionMode, cancellationToken);
            Dictionary<long, string> existingHashes = await database.LoadOllamaHashesAsync(cancellationToken);

            Console.WriteLine($"[OLLAMA] {rows.Count} object(s) loaded.");

            int i = 0;

            foreach (DocumentationObjectRow row in rows)
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

                    existingHashes.TryGetValue(row.Id, out string? existingHash);

                    if (!options.ForceRegenerate &&
                        string.Equals(existingHash, currentHash, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    Console.WriteLine($"[OLLAMA n° {i}] Processing {row.NamespaceName}.{row.ObjectName} ({row.ObjectType})");

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

                    await database.UpsertOllamaAsync(
                        row.Id,
                        row.PackageId,
                        row.Version,
                        row.NamespaceName,
                        row.ObjectName,
                        row.ObjectType,
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
                    Console.WriteLine($"[OLLAMA][ERROR] Failed for {row.NamespaceName}.{row.ObjectName} ({row.ObjectType})");
                    Console.WriteLine(exception.Message);
                    Console.ResetColor();
                }
            }

            if (options.StopModelWhenFinished)
            {
                await generator.StopModelAsync(cancellationToken);
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Clears stored hashes so AI output can be regenerated.
        /// </summary>
        public static void ResetHashes(string databaseRelativePath, OllamaModel model)
        {
            string mainDatabasePath = Path.GetFullPath(databaseRelativePath);
            string aiDatabasePath = GetAIDatabasePath(mainDatabasePath, model);

            if (!File.Exists(aiDatabasePath))
            {
                Console.WriteLine($"[OLLAMA] No AI database found for model '{model}'.");
                return;
            }

            using var connection = new SqliteConnection($"Data Source={aiDatabasePath}");
            connection.Open();

            EnsureOllamaTable(connection);

            const string sql = """
                               UPDATE DocumentationAIResult
                               SET AIContentLastHash = '';
                               """;

            using var command = new SqliteCommand(sql, connection);
            int affected = command.ExecuteNonQuery();

            Console.WriteLine($"[OLLAMA] Hash reset completed for model '{model}'. {affected} row(s).");
        }

        /// <summary>
        ///     Removes generated AI output for the selected model and optional object type.
        /// </summary>
        public static void Clean(string databaseRelativePath, OllamaModel model)
        {
            string mainDatabasePath = Path.GetFullPath(databaseRelativePath);
            string aiDatabasePath = GetAIDatabasePath(mainDatabasePath, model);

            if (!File.Exists(aiDatabasePath))
            {
                Console.WriteLine($"[OLLAMA] No AI database found for model '{model}'.");
                return;
            }

            using var connection = new SqliteConnection($"Data Source={aiDatabasePath}");
            connection.Open();

            EnsureOllamaTable(connection);

            const string sql = """
                               UPDATE DocumentationAIResult
                               SET
                                   AISummary = '',
                                   AISummaryShort = '',
                                   AIKeywords = '',
                                   AIContentLastHash = '',
                                   AIUpdatedAt = '',
                                   AIModel = '',
                                   AIEmbedding = NULL;
                               """;

            using var command = new SqliteCommand(sql, connection);
            int affected = command.ExecuteNonQuery();

            Console.WriteLine($"[OLLAMA] Clean completed for model '{model}'. {affected} row(s) reset.");
        }

        /// <summary>
        ///     Removes generated AI output for the selected model and optional object type.
        /// </summary>
        public static void Clean(string databaseRelativePath, OllamaModel model, string? objectType = null)
        {
            string mainDatabasePath = Path.GetFullPath(databaseRelativePath);
            string aiDatabasePath = GetAIDatabasePath(mainDatabasePath, model);

            if (!File.Exists(aiDatabasePath))
            {
                Console.WriteLine($"[OLLAMA] No AI database found for model '{model}'.");
                return;
            }

            using var aiConnection = new SqliteConnection($"Data Source={aiDatabasePath}");
            aiConnection.Open();
            EnsureOllamaTable(aiConnection);

            if (objectType is null)
            {
                Clean(databaseRelativePath, model);
                return;
            }

            string mainDbPath = Path.GetFullPath(databaseRelativePath);

            using var mainConnection = new SqliteConnection($"Data Source={mainDbPath}");
            mainConnection.Open();

            const string selectSql = """
                                     SELECT Id
                                     FROM DocumentationObjects
                                     WHERE ObjectType = @ObjectType;
                                     """;

            using var selectCommand = new SqliteCommand(selectSql, mainConnection);
            selectCommand.Parameters.AddWithValue("@ObjectType", objectType);

            List<long> ids = [];

            using (var reader = selectCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    ids.Add(reader.GetInt64(0));
                }
            }

            int affected = 0;

            foreach (long id in ids)
            {
                const string updateSql = """
                                         UPDATE DocumentationAIResult
                                         SET
                                             AISummary = '',
                                             AISummaryShort = '',
                                             AIKeywords = '',
                                             AIContentLastHash = '',
                                             AIUpdatedAt = '',
                                             AIModel = '',
                                             AIEmbedding = NULL
                                         WHERE DocumentationObjectId = @Id;
                                         """;

                using var updateCommand = new SqliteCommand(updateSql, aiConnection);
                updateCommand.Parameters.AddWithValue("@Id", id);
                affected += updateCommand.ExecuteNonQuery();
            }

            Console.WriteLine($"[OLLAMA] Clean completed for model '{model}' and objectType '{objectType}'. {affected} row(s) reset.");
        }

        /// <summary>
        ///     Runs Ollama documentation improvement synchronously.
        /// </summary>
        public static void Run(OllamaOptions options)
        {
            Console.WriteLine("Starting…");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠️ First! You need to install the model with command : \" ollama pull {options.Model.ToModelString()} \" ");
            Console.WriteLine($"⚠️ Second! You need to launch the IA with command : \" ollama run {options.Model.ToModelString()} \" ");
            Console.WriteLine($"You can remove model if necessary with command : \" ollama rm {options.Model.ToModelString()} \" ");
            Console.ResetColor();

            RunAsync(options).GetAwaiter().GetResult();

            Console.WriteLine("Finished…");
        }

        #endregion

        #region Nested types

        private sealed class OllamaProcessManager : IDisposable
        {
            #region Instance fields and properties

            private readonly HttpClient _httpClient;
            private Process? _startedProcess;

            #endregion

            #region Instance constructors and destructors

            #region Constructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="OllamaProcessManager" /> class.
            /// </summary>
            public OllamaProcessManager()
            {
                _httpClient = new HttpClient
                {
                    BaseAddress = new Uri("http://localhost:11434/")
                };
            }

            #endregion

            #endregion

            #region Instance methods

            #region Private methods

            private async Task<bool> IsServerRunningAsync(CancellationToken cancellationToken)
            {
                try
                {
                    using HttpResponseMessage response = await _httpClient.GetAsync("api/tags", cancellationToken);
                    return response.IsSuccessStatusCode;
                }
                catch
                {
                    return false;
                }
            }

            #endregion

            #endregion

            #region Public methods

            /// <summary>
            ///     Ensures that the Ollama server is available before generation starts.
            /// </summary>
            /// <returns>A task that completes when the server is available.</returns>
            public async Task EnsureServerRunningAsync(bool startIfNeeded, CancellationToken cancellationToken)
            {
                if (await IsServerRunningAsync(cancellationToken))
                {
                    return;
                }

                if (!startIfNeeded)
                {
                    throw new InvalidOperationException("Ollama server is not running.");
                }

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "ollama",
                    Arguments = "serve",
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                _startedProcess = Process.Start(processStartInfo)
                                  ?? throw new InvalidOperationException("Unable to start 'ollama serve'.");

                for (int i = 0; i < 50; i++)
                {
                    if (await IsServerRunningAsync(cancellationToken))
                    {
                        return;
                    }

                    await Task.Delay(300, cancellationToken);
                }

                throw new TimeoutException("Ollama server did not start in time.");
            }

            /// <summary>
            ///     Releases resources owned by the helper instance.
            /// </summary>
            public void Dispose()
            {
                _httpClient.Dispose();

                try
                {
                    if (_startedProcess is { HasExited: false })
                    {
                        _startedProcess.Kill(entireProcessTree: true);
                    }
                }
                catch
                {
                }

                _startedProcess?.Dispose();
            }

            #endregion
        }

        private sealed class OllamaTextGenerator
        {
            #region Instance fields and properties

            private readonly HttpClient _httpClient;
            private readonly string _modelName;

            #endregion

            #region Instance constructors and destructors

            #region Constructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="OllamaTextGenerator" /> class.
            /// </summary>
            public OllamaTextGenerator(string modelName)
            {
                _modelName = modelName;
                _httpClient = new HttpClient
                {
                    BaseAddress = new Uri("http://localhost:11434/"),
                    Timeout = Timeout.InfiniteTimeSpan
                };
            }

            #endregion

            #endregion

            #region Public methods

            /// <summary>
            ///     Preloads the selected Ollama model before generation.
            /// </summary>
            /// <returns>A task that completes when the model has been loaded.</returns>
            public async Task PreloadModelAsync(CancellationToken cancellationToken)
            {
                var payload = new
                {
                    model = _modelName,
                    prompt = "Reply with OK only.",
                    stream = false,
                    keep_alive = -1
                };

                using var content = CreateJsonContent(payload);
                using HttpResponseMessage response = await _httpClient.PostAsync("api/generate", content, cancellationToken);
                response.EnsureSuccessStatusCode();
            }

            /// <summary>
            ///     Generates text with the configured Ollama model.
            /// </summary>
            /// <returns>The generated text returned by the model.</returns>
            public async Task<string> GenerateTextAsync(
                string prompt,
                TimeSpan requestTimeout,
                CancellationToken cancellationToken
            )
            {
                using var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                linkedCancellationTokenSource.CancelAfter(requestTimeout);

                var payload = new
                {
                    model = _modelName,
                    prompt,
                    stream = false,
                    keep_alive = -1
                };

                using var content = CreateJsonContent(payload);
                using HttpResponseMessage response = await _httpClient.PostAsync("api/generate", content, linkedCancellationTokenSource.Token);
                response.EnsureSuccessStatusCode();

                string rawResponse = await response.Content.ReadAsStringAsync(linkedCancellationTokenSource.Token);

                using JsonDocument jsonDocument = JsonDocument.Parse(rawResponse);

                string? result = jsonDocument.RootElement.TryGetProperty("response", out JsonElement responseElement)
                    ? responseElement.GetString()
                    : null;

                return CleanupModelText(result);
            }

            /// <summary>
            ///     Stops the selected Ollama model after generation.
            /// </summary>
            /// <returns>A task that completes when the model has been stopped.</returns>
            public async Task StopModelAsync(CancellationToken cancellationToken)
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "ollama",
                    Arguments = $"stop {_modelName}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using Process? process = Process.Start(processStartInfo);

                if (process is null)
                {
                    return;
                }

                await process.WaitForExitAsync(cancellationToken);
            }

            #endregion

            #region Private methods

            private static StringContent CreateJsonContent(object payload)
            {
                string json = JsonSerializer.Serialize(payload);
                return new StringContent(json, Encoding.UTF8, "application/json");
            }

            private static string CleanupModelText(string? text)
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return string.Empty;
                }

                string result = text.Trim();

                if (result.StartsWith("```", StringComparison.Ordinal))
                {
                    result = result.Trim('`').Trim();
                }

                result = result.Replace("\r\n", "\n", StringComparison.Ordinal);
                result = result.Replace("\r", "\n", StringComparison.Ordinal);

                while (result.Contains("\n\n\n", StringComparison.Ordinal))
                {
                    result = result.Replace("\n\n\n", "\n\n", StringComparison.Ordinal);
                }

                return result.Trim();
            }

            #endregion
        }

        private sealed class DocumentationDatabaseImprover
        {
            #region Instance fields and properties

            private readonly string _aiDatabasePath;

            private readonly string _mainDatabasePath;

            #endregion

            #region Instance constructors and destructors

            #region Constructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="DocumentationDatabaseImprover" /> class.
            /// </summary>
            public DocumentationDatabaseImprover(string mainDatabasePath, string aiDatabasePath)
            {
                _mainDatabasePath = mainDatabasePath;
                _aiDatabasePath = aiDatabasePath;
            }

            #endregion

            #endregion

            #region Public methods

            /// <summary>
            ///     Loads documentation database rows required for AI improvement.
            /// </summary>
            /// <returns>The loaded database rows.</returns>
            public async Task<List<DocumentationObjectRow>> LoadObjectsAsync(int max, DocumentationAIObjectSelectionMode selectionMode, CancellationToken ct)
            {
                using var connection = new SqliteConnection($"Data Source={_mainDatabasePath}");
                await connection.OpenAsync(ct);

                string sql = """
                             SELECT Id, NamespaceName, ObjectName, ObjectType, RoutePath, ModelInJson, TechnicalKeywords, Keywords, PackageId, Version
                             FROM DocumentationObjects
                             ORDER BY Id
                             """;

                using var cmd = new SqliteCommand(sql, connection);
                using var reader = await cmd.ExecuteReaderAsync(ct);

                var list = new List<DocumentationObjectRow>();

                while (await reader.ReadAsync(ct))
                {
                    list.Add(new DocumentationObjectRow
                    {
                        Id = reader.GetInt64(0),
                        NamespaceName = reader.GetString(1),
                        ObjectName = reader.GetString(2),
                        ObjectType = reader.GetString(3),
                        RoutePath = reader.GetString(4),
                        ModelInJson = reader.GetString(5),
                        TechnicalKeywords = reader.GetString(6),
                        Keywords = reader.GetString(7),
                        PackageId = reader.GetString(8),
                        Version = reader.GetString(9)
                    });
                }

                return DocumentationAIObjectSelector.SelectRows(
                    list,
                    selectionMode,
                    max,
                    row => row.Id,
                    row => row.PackageId,
                    row => row.Version,
                    row => row.NamespaceName,
                    row => row.ObjectName,
                    row => row.ObjectType);
            }

            public async Task<Dictionary<long, string>> LoadOllamaHashesAsync(CancellationToken ct)
            {
                using var connection = new SqliteConnection($"Data Source={_aiDatabasePath}");
                await connection.OpenAsync(ct);

                EnsureOllamaTable(connection);

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
            ///     Inserts or updates Ollama AI output for a documentation object.
            /// </summary>
            /// <returns>A task that completes when the row has been stored.</returns>
            public async Task UpsertOllamaAsync(
                long id,
                string packageId,
                string version,
                string namespaceName,
                string objectName,
                string objectType,
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

                EnsureOllamaTable(connection);

                const string sql = @"
INSERT INTO DocumentationAIResult
(DocumentationObjectId, PackageId, Version, NamespaceName, ObjectName, ObjectType, RoutePath, AISummary, AISummaryShort, AIKeywords, AIContentLastHash, AIUpdatedAt, AIModel)
VALUES
(@Id, @PackageId, @Version, @NamespaceName, @ObjectName, @ObjectType, @Route, @S, @SS, @K, @H, @D, @M)
ON CONFLICT(DocumentationObjectId) DO UPDATE SET
    PackageId = excluded.PackageId,
    Version = excluded.Version,
    NamespaceName = excluded.NamespaceName,
    ObjectName = excluded.ObjectName,
    ObjectType = excluded.ObjectType,
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
                cmd.Parameters.AddWithValue("@PackageId", packageId);
                cmd.Parameters.AddWithValue("@Version", version);
                cmd.Parameters.AddWithValue("@NamespaceName", namespaceName);
                cmd.Parameters.AddWithValue("@ObjectName", objectName);
                cmd.Parameters.AddWithValue("@ObjectType", objectType);
                cmd.Parameters.AddWithValue("@Route", route);
                cmd.Parameters.AddWithValue("@S", summary);
                cmd.Parameters.AddWithValue("@SS", shortSummary);
                cmd.Parameters.AddWithValue("@K", keywords);
                cmd.Parameters.AddWithValue("@H", hash);
                cmd.Parameters.AddWithValue("@D", DateTime.UtcNow.ToString("O"));
                cmd.Parameters.AddWithValue("@M", model);

                await cmd.ExecuteNonQueryAsync(ct);
            }

            /// <summary>
            ///     Computes the hash used to detect whether AI output is stale.
            /// </summary>
            /// <returns>A stable hash for the documentation object and prompt inputs.</returns>
            public static string ComputeHash(
                DocumentationObjectRow row,
                string projectContextPrompt,
                string summaryPrompt,
                string shortSummaryPrompt,
                string keywordsPrompt
            )
            {
                string raw = string.Join("\n", new[]
                {
                    row.PackageId,
                    row.Version,
                    row.NamespaceName,
                    row.ObjectName,
                    row.ObjectType,
                    row.RoutePath,
                    row.ModelInJson,
                    row.TechnicalKeywords,
                    row.Keywords,
                    projectContextPrompt,
                    summaryPrompt,
                    shortSummaryPrompt,
                    keywordsPrompt
                });

                byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
                return Convert.ToHexString(hash);
            }

            #endregion
        }

        private static class DocumentationPromptFactory
        {
            #region Private methods

            private static string BuildSourceContext(
                DocumentationObjectRow row,
                string projectContextPrompt,
                int maxModelJsonLength
            )
            {
                return $"""
                        PROJECT CONTEXT
                        {projectContextPrompt}

                        DOCUMENTATION OBJECT
                        PackageId: {row.PackageId}
                        Version: {row.Version}
                        Namespace: {row.NamespaceName}
                        ObjectName: {row.ObjectName}
                        ObjectType: {row.ObjectType}
                        RoutePath: {row.RoutePath}

                        TECHNICAL KEYWORDS
                        {row.TechnicalKeywords}

                        FUNCTIONAL KEYWORDS
                        {row.Keywords}

                        SERIALIZED MODEL
                        {LimitText(row.ModelInJson, maxModelJsonLength)}
                        """;
            }

            private static string LimitText(string value, int maxLength)
            {
                if (string.IsNullOrEmpty(value))
                {
                    return string.Empty;
                }

                if (maxLength <= 0 || value.Length <= maxLength)
                {
                    return value;
                }

                return value[..maxLength] + "\n...[TRUNCATED]";
            }

            #endregion

            #region Public methods

            /// <summary>
            ///     Builds the build summary prompt for AI generation.
            /// </summary>
            /// <returns>The prompt text sent to the AI model.</returns>
            public static string BuildSummaryPrompt(
                DocumentationObjectRow row,
                string projectContextPrompt,
                string customPrompt,
                int maxModelJsonLength
            )
            {
                return $"""
                        You are writing a rich technical summary for a generated documentation page.

                        Rules:
                        - Return plain text only
                        - No markdown
                        - No bullet list
                        - 3 to 6 sentences
                        - Explain the role of the object
                        - Explain how it is likely used
                        - Be precise and conservative
                        - Avoid generic filler
                        - Do not invent APIs not visible in the input

                        Additional instructions:
                        {customPrompt}

                        {BuildSourceContext(row, projectContextPrompt, maxModelJsonLength)}
                        """;
            }

            /// <summary>
            ///     Builds the build short summary prompt for AI generation.
            /// </summary>
            /// <returns>The prompt text sent to the AI model.</returns>
            public static string BuildShortSummaryPrompt(
                DocumentationObjectRow row,
                string projectContextPrompt,
                string customPrompt,
                int maxModelJsonLength
            )
            {
                return $"""
                        You are writing a compact technical summary for a generated documentation page.

                        Rules:
                        - Return plain text only
                        - Exactly one sentence
                        - Maximum 220 characters
                        - Neutral technical tone
                        - Focus on the primary responsibility of the object

                        Additional instructions:
                        {customPrompt}

                        {BuildSourceContext(row, projectContextPrompt, maxModelJsonLength)}
                        """;
            }

            /// <summary>
            ///     Builds the build keywords prompt for AI generation.
            /// </summary>
            /// <returns>The prompt text sent to the AI model.</returns>
            public static string BuildKeywordsPrompt(
                DocumentationObjectRow row,
                string projectContextPrompt,
                string customPrompt,
                int maxModelJsonLength
            )
            {
                return $"""
                        You are extracting AI-oriented technical keywords for a generated documentation page.

                        Rules:
                        - Return plain text only
                        - Return a single comma-separated line
                        - Between 5 and 15 keywords
                        - No numbering
                        - No explanations
                        - No markdown
                        - Prefer stable technical terms
                        - Avoid empty generic words

                        Additional instructions:
                        {customPrompt}

                        {BuildSourceContext(row, projectContextPrompt, maxModelJsonLength)}
                        """;
            }

            #endregion
        }

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
    }
}
