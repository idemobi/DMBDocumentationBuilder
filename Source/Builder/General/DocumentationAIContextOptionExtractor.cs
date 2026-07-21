#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Text.Json;

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Extracts versioned AI context options from <c>AIContextOptions/*.json</c> files near a documented project.
    /// </summary>
    public static class DocumentationAIContextOptionExtractor
    {
        #region Constants

        private const string ContextOptionsDirectoryName = "AIContextOptions";

        #endregion

        #region Static methods

        /// <summary>
        ///     Extracts AI context options for a documented project.
        /// </summary>
        /// <param name="project">The documented project descriptor.</param>
        /// <param name="groupName">The documentation group that owns the documented project.</param>
        /// <param name="maxParentDepth">The number of parent folders to inspect after the project directory.</param>
        /// <returns>The extracted AI context options.</returns>
        public static DocumentationAIContextOptionModel Extract(
            DocumentationProjectDescriptor project,
            string groupName,
            int maxParentDepth = 4
        )
        {
            if (project is null) throw new ArgumentNullException(nameof(project));
            if (string.IsNullOrWhiteSpace(project.ProjectFilePath)) throw new ArgumentException("Project file path is required.", nameof(project));
            if (!File.Exists(project.ProjectFilePath)) throw new FileNotFoundException("Project file not found.", project.ProjectFilePath);

            string projectDirectory = Path.GetDirectoryName(project.ProjectFilePath)
                                      ?? throw new InvalidOperationException("Unable to resolve project directory.");

            DocumentationAIContextOptionModel result = new()
            {
                ProjectFilePath = project.ProjectFilePath,
                ProjectDirectoryPath = projectDirectory,
                GroupName = groupName,
                PackageId = project.PackageId,
                Version = project.Version
            };

            HashSet<string> addedRuleNames = new(StringComparer.OrdinalIgnoreCase);
            DirectoryInfo? currentDirectory = new(projectDirectory);
            int depth = 0;

            while (currentDirectory is not null && depth <= maxParentDepth)
            {
                string contextOptionsDirectoryPath = Path.Combine(currentDirectory.FullName, ContextOptionsDirectoryName);

                if (Directory.Exists(contextOptionsDirectoryPath))
                {
                    foreach (DocumentationAIContextOptionFileModel option in ExtractDirectory(contextOptionsDirectoryPath, addedRuleNames))
                    {
                        result.Files.Add(option);
                    }
                }

                currentDirectory = currentDirectory.Parent;
                depth++;
            }

            result.Files.Sort(static (left, right) =>
            {
                int sortComparison = left.SortOrder.CompareTo(right.SortOrder);
                return sortComparison != 0
                    ? sortComparison
                    : string.Compare(left.RuleName, right.RuleName, StringComparison.OrdinalIgnoreCase);
            });

            return result;
        }

        private static IEnumerable<DocumentationAIContextOptionFileModel> ExtractDirectory(
            string contextOptionsDirectoryPath,
            ISet<string> addedRuleNames
        )
        {
            foreach (string filePath in Directory
                         .GetFiles(contextOptionsDirectoryPath, "*.json", SearchOption.TopDirectoryOnly)
                         .OrderBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase))
            {
                DocumentationAIContextOptionFileModel option = ReadOption(filePath);

                if (!addedRuleNames.Add(option.RuleName))
                {
                    continue;
                }

                yield return option;
            }
        }

        private static DocumentationAIContextOptionFileModel ReadOption(string filePath)
        {
            using FileStream stream = File.OpenRead(filePath);
            using JsonDocument document = JsonDocument.Parse(stream, new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
            });

            JsonElement root = document.RootElement;
            string ruleName = ReadString(root, "ruleName");

            if (string.IsNullOrWhiteSpace(ruleName))
            {
                ruleName = Path.GetFileNameWithoutExtension(filePath);
            }

            string contextText = ReadString(root, "contextText");

            if (string.IsNullOrWhiteSpace(contextText))
            {
                throw new InvalidOperationException($"AI context option '{filePath}' must define a non-empty contextText value.");
            }

            string title = ReadString(root, "title");

            return new DocumentationAIContextOptionFileModel
            {
                FilePath = filePath,
                RuleName = ruleName,
                Title = string.IsNullOrWhiteSpace(title)
                    ? ruleName
                    : title,
                Description = ReadString(root, "description"),
                ScenarioName = ReadString(root, "scenarioName"),
                ProjectStyles = ReadStringArray(root, "projectStyles"),
                Tags = ReadStringArray(root, "tags"),
                ContextText = contextText,
                SortOrder = ReadInt32(root, "sortOrder")
            };
        }

        private static int ReadInt32(JsonElement root, string propertyName)
        {
            return root.TryGetProperty(propertyName, out JsonElement property) &&
                   property.ValueKind == JsonValueKind.Number &&
                   property.TryGetInt32(out int value)
                ? value
                : 0;
        }

        private static string ReadString(JsonElement root, string propertyName)
        {
            return root.TryGetProperty(propertyName, out JsonElement property) &&
                   property.ValueKind == JsonValueKind.String
                ? property.GetString() ?? string.Empty
                : string.Empty;
        }

        private static IReadOnlyList<string> ReadStringArray(JsonElement root, string propertyName)
        {
            if (!root.TryGetProperty(propertyName, out JsonElement property) ||
                property.ValueKind != JsonValueKind.Array)
            {
                return [];
            }

            return property
                .EnumerateArray()
                .Where(item => item.ValueKind == JsonValueKind.String)
                .Select(item => item.GetString() ?? string.Empty)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToList();
        }

        #endregion
    }
}
