#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using DMBBootstrapBuilder;
using DMBPageBuilder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Renders the context pack builder form and exports selected latest-version context options as a ZIP file.
    /// </summary>
    public sealed class DocumentationContextPackController : RawBootstrapController
    {
        #region Instance fields and properties

        private readonly IWebHostEnvironment _environment;

        #endregion

        #region Instance constructors and destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DocumentationContextPackController" /> class.
        /// </summary>
        /// <param name="environment">Host environment used to resolve the generated documentation database path.</param>
        public DocumentationContextPackController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        #endregion

        #region Instance methods

        /// <summary>
        ///     Downloads the selected context options as a ZIP context pack.
        /// </summary>
        /// <param name="request">The posted option fingerprint selection.</param>
        /// <returns>A ZIP file containing selected context option files and a manifest.</returns>
        [HttpPost]
        public IActionResult Download(DocumentationContextPackDownloadRequest request)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            IReadOnlyList<DocumentationContextPackOption> options = BuildOptions(
                request.GroupName,
                request.PackageId);

            HashSet<string> selectedFingerprints = new(request.SelectedFingerprints, StringComparer.OrdinalIgnoreCase);
            List<DocumentationContextPackOption> selectedOptions = options
                .Where(option => selectedFingerprints.Contains(option.Fingerprint))
                .OrderBy(option => option.Category, StringComparer.Ordinal)
                .ThenBy(option => option.SubCategory, StringComparer.Ordinal)
                .ThenBy(option => option.SortOrder)
                .ThenBy(option => option.RuleName, StringComparer.Ordinal)
                .ToList();

            if (selectedOptions.Count == 0)
            {
                return RedirectToAction(
                    nameof(Index),
                    new
                    {
                        groupName = request.GroupName,
                        packageId = request.PackageId,
                        namespaceName = request.NamespaceName
                    });
            }

            byte[] zipBytes = BuildZip(selectedOptions, request);
            string fileName = BuildZipFileName(request.PackageId, selectedOptions);

            return File(zipBytes, "application/zip", fileName);
        }

        /// <summary>
        ///     Renders the context pack builder form.
        /// </summary>
        /// <param name="groupName">Optional documentation group filter.</param>
        /// <param name="packageId">Optional package filter.</param>
        /// <param name="namespaceName">Optional namespace used to preserve the documentation sidebar scope.</param>
        /// <returns>The context pack builder form view.</returns>
        public IActionResult Index(string? groupName = null, string? packageId = null, string? namespaceName = null)
        {
            DocumentationContextPackViewModel model = BuildViewModel(groupName, packageId, namespaceName);

            AddBreadcrumb(
                new UrlActionItem().WithUrl("/").SetTitle("Home").SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_house)),
                new AspRouteActionItem("Documentation", "Index").SetTitle("Documentation").SetIcon(IconStruct.BootstrapEnum(BootStrapEnum.bi_book)),
                new AspRouteActionItem("DocumentationContextPack", nameof(Index)).SetTitle("Context pack").SetIcon(IconStruct.Bootstrap("bi-file-zip"))
            );

            SetSidebar(new SideBarComponent().AddSection(CreateSidebar(model)));
            SetTitle("Context pack");
            SetDescription("Select latest-version context options and export them as a ZIP file.");
            SetKeywords($"documentation context pack ai {model.GroupName} {model.PackageId} {model.NamespaceName}");

            return View("~/Views/DocumentationContextPack/Index.cshtml", model);
        }

        private static void AddTextEntry(ZipArchive archive, string entryName, string content)
        {
            ZipArchiveEntry entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);

            using Stream stream = entry.Open();
            using StreamWriter writer = new(stream, Encoding.UTF8);
            writer.Write(content);
        }

        private static byte[] BuildZip(
            IReadOnlyList<DocumentationContextPackOption> options,
            DocumentationContextPackDownloadRequest request
        )
        {
            using MemoryStream zipStream = new();

            using (ZipArchive archive = new(zipStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                var manifest = new
                {
                    generatedUtc = DateTime.UtcNow.ToString("O"),
                    request = new
                    {
                        request.GroupName,
                        request.PackageId,
                        request.NamespaceName
                    },
                    options = options.Select(option => new
                    {
                        option.Fingerprint,
                        option.GroupName,
                        option.PackageId,
                        option.Version,
                        option.Category,
                        option.SubCategory,
                        option.RuleName,
                        option.Title,
                        option.ProjectStyles,
                        option.Tags
                    })
                };

                AddTextEntry(
                    archive,
                    "context-pack.json",
                    JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true }));

                AddTextEntry(archive, "AI_CONTEXT_PACK.md", BuildCombinedMarkdown(options));

                foreach (DocumentationContextPackOption option in options)
                {
                    string category = ToSafeFileName(option.Category);
                    string subCategory = ToSafeFileName(option.SubCategory);
                    string ruleName = ToSafeFileName(option.RuleName);
                    string entryName = $"rules/{category}/{subCategory}-{ruleName}.md";

                    AddTextEntry(archive, entryName, BuildOptionMarkdown(option));
                }
            }

            return zipStream.ToArray();
        }

        private static string BuildCombinedMarkdown(IEnumerable<DocumentationContextPackOption> options)
        {
            StringBuilder builder = new();
            builder.AppendLine("# AI Context Pack");
            builder.AppendLine();

            foreach (DocumentationContextPackOption option in options)
            {
                builder.Append(BuildOptionMarkdown(option));
                builder.AppendLine();
            }

            return builder.ToString();
        }

        private static string BuildFingerprint(DocumentationContextPackOption option)
        {
            string raw = string.Join(
                "\u001F",
                option.GroupName,
                option.PackageId,
                option.Version,
                option.RuleName,
                option.Category,
                option.SubCategory,
                option.ContextText);

            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        private static string BuildOptionMarkdown(DocumentationContextPackOption option)
        {
            StringBuilder builder = new();
            builder.AppendLine($"# {option.Title}");
            builder.AppendLine();
            builder.AppendLine($"- Fingerprint: `{option.Fingerprint}`");
            builder.AppendLine($"- Group: `{option.GroupName}`");
            builder.AppendLine($"- Package: `{option.PackageId}`");
            builder.AppendLine($"- Version: `{option.Version}`");
            builder.AppendLine($"- Category: `{option.Category}`");
            builder.AppendLine($"- Sub category: `{option.SubCategory}`");
            builder.AppendLine();
            builder.AppendLine(option.ContextText);
            builder.AppendLine();

            return builder.ToString();
        }

        private IReadOnlyList<DocumentationContextPackOption> BuildOptions(string? groupName, string? packageId)
        {
            List<DocumentationContextPackOption> options = ReadOptions(groupName, packageId);

            return options
                .GroupBy(
                    option => $"{option.GroupName}\u001F{option.PackageId}\u001F{option.RuleName}",
                    StringComparer.OrdinalIgnoreCase)
                .Select(group => group
                    .OrderByDescending(option => option.Version, ContextPackVersionComparer.Instance)
                    .ThenBy(option => option.SortOrder)
                    .First())
                .Select(option => WithFingerprint(option, BuildFingerprint(option)))
                .OrderBy(option => option.Category, StringComparer.Ordinal)
                .ThenBy(option => option.SubCategory, StringComparer.Ordinal)
                .ThenBy(option => option.SortOrder)
                .ThenBy(option => option.RuleName, StringComparer.Ordinal)
                .ToArray();
        }

        private DocumentationContextPackViewModel BuildViewModel(string? groupName, string? packageId, string? namespaceName)
        {
            IReadOnlyList<DocumentationContextPackOption> options = BuildOptions(groupName, packageId);

            return new DocumentationContextPackViewModel
            {
                GroupName = groupName ?? string.Empty,
                PackageId = ResolvePackageId(packageId, options),
                NamespaceName = namespaceName ?? string.Empty,
                Options = options
            };
        }

        private static string BuildZipFileName(
            string packageId,
            IReadOnlyList<DocumentationContextPackOption> options
        )
        {
            string resolvedPackageId = string.IsNullOrWhiteSpace(packageId)
                ? options.Select(option => option.PackageId).FirstOrDefault() ?? "context-pack"
                : packageId;

            return $"{ToSafeFileName(resolvedPackageId)}-context-pack.zip";
        }

        private SideBarSectionComponent CreateSidebar(DocumentationContextPackViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.GroupName) &&
                !string.IsNullOrWhiteSpace(model.PackageId) &&
                !string.IsNullOrWhiteSpace(model.NamespaceName))
            {
                string version = model.Options.Select(option => option.Version).FirstOrDefault() ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(version))
                {
                    return DocumentationSidebarFactory.CreateNamespaceSidebar(
                        model.GroupName,
                        model.PackageId,
                        version,
                        model.NamespaceName);
                }
            }

            if (!string.IsNullOrWhiteSpace(model.GroupName))
            {
                return DocumentationSidebarFactory.CreateGroupSidebar(model.GroupName);
            }

            return DocumentationSidebarFactory.CreateRootSidebar();
        }

        private string GetSqliteDatabasePath()
        {
            return Path.Combine(_environment.ContentRootPath, "Documentation", "data.db");
        }

        private static bool ColumnExists(SqliteConnection connection, string tableName, string columnName)
        {
            using var command = new SqliteCommand($"PRAGMA table_info({tableName});", connection);
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

        private static IReadOnlyList<string> ReadJsonStringArray(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return [];
            }

            try
            {
                return JsonSerializer.Deserialize<List<string>>(json) ?? [];
            }
            catch (JsonException)
            {
                return [];
            }
        }

        private List<DocumentationContextPackOption> ReadOptions(string? groupName, string? packageId)
        {
            List<DocumentationContextPackOption> options = [];

            using var connection = new SqliteConnection($"Data Source={GetSqliteDatabasePath()}");
            connection.Open();

            if (!TableExists(connection, "DocumentationAIContextOptions"))
            {
                return options;
            }

            bool hasGroupNameColumn = ColumnExists(connection, "DocumentationAIContextOptions", "GroupName");
            string sql = hasGroupNameColumn
                ? """
                  SELECT GroupName, PackageId, Version, RuleName, Title, Description,
                         ScenarioName, ProjectStylesJson, TagsJson, ContextText, SortOrder
                  FROM DocumentationAIContextOptions
                  WHERE (@GroupName = '' OR GroupName = @GroupName)
                    AND (@PackageId = '' OR PackageId = @PackageId)
                  ORDER BY GroupName, PackageId, Version, SortOrder, RuleName
                  """
                : """
                  SELECT '' AS GroupName, PackageId, Version, RuleName, Title, Description,
                         ScenarioName, ProjectStylesJson, TagsJson, ContextText, SortOrder
                  FROM DocumentationAIContextOptions
                  WHERE (@PackageId = '' OR PackageId = @PackageId)
                  ORDER BY PackageId, Version, SortOrder, RuleName
                  """;

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@PackageId", packageId ?? string.Empty);

            if (hasGroupNameColumn)
            {
                command.Parameters.AddWithValue("@GroupName", groupName ?? string.Empty);
            }

            using SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string ruleName = reader["RuleName"]?.ToString() ?? string.Empty;
                string scenarioName = reader["ScenarioName"]?.ToString() ?? string.Empty;
                string title = reader["Title"]?.ToString() ?? ruleName;

                options.Add(new DocumentationContextPackOption
                {
                    GroupName = reader["GroupName"]?.ToString() ?? string.Empty,
                    PackageId = reader["PackageId"]?.ToString() ?? string.Empty,
                    Version = reader["Version"]?.ToString() ?? string.Empty,
                    RuleName = ruleName,
                    Category = ruleName,
                    SubCategory = string.IsNullOrWhiteSpace(scenarioName) ? title : scenarioName,
                    Title = title,
                    Description = reader["Description"]?.ToString() ?? string.Empty,
                    ProjectStyles = ReadJsonStringArray(reader["ProjectStylesJson"]?.ToString()),
                    Tags = ReadJsonStringArray(reader["TagsJson"]?.ToString()),
                    ContextText = reader["ContextText"]?.ToString() ?? string.Empty,
                    SortOrder = Convert.ToInt32(reader["SortOrder"])
                });
            }

            return options;
        }

        private static string ResolvePackageId(string? packageId, IEnumerable<DocumentationContextPackOption> options)
        {
            if (!string.IsNullOrWhiteSpace(packageId))
            {
                return packageId;
            }

            string[] packageIds = options
                .Select(option => option.PackageId)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return packageIds.Length == 1
                ? packageIds[0]
                : string.Empty;
        }

        private static bool TableExists(SqliteConnection connection, string tableName)
        {
            using var command = new SqliteCommand(
                "SELECT name FROM sqlite_master WHERE type = 'table' AND name = @TableName;",
                connection);
            command.Parameters.AddWithValue("@TableName", tableName);

            object? result = command.ExecuteScalar();
            return result is not null;
        }

        private static string ToSafeFileName(string value)
        {
            string safe = Regex.Replace(value, "[^a-zA-Z0-9._-]+", "-").Trim('-');
            return string.IsNullOrWhiteSpace(safe) ? "context-option" : safe;
        }

        private static DocumentationContextPackOption WithFingerprint(DocumentationContextPackOption option, string fingerprint)
        {
            return new DocumentationContextPackOption
            {
                Category = option.Category,
                ContextText = option.ContextText,
                Description = option.Description,
                Fingerprint = fingerprint,
                GroupName = option.GroupName,
                PackageId = option.PackageId,
                ProjectStyles = option.ProjectStyles,
                RuleName = option.RuleName,
                SortOrder = option.SortOrder,
                SubCategory = option.SubCategory,
                Tags = option.Tags,
                Title = option.Title,
                Version = option.Version
            };
        }

        #endregion

        #region Nested types

        private sealed class ContextPackVersionComparer : IComparer<string>
        {
            #region Static fields and properties

            public static readonly ContextPackVersionComparer Instance = new();

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

                int prereleaseIndex = normalized.IndexOf('-');

                if (prereleaseIndex >= 0)
                {
                    normalized = normalized[..prereleaseIndex];
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
        }

        #endregion
    }
}
