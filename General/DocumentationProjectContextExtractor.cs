#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using System.Text;

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents the DocumentationProjectContextExtractor type used by DocumentationBuilder generation.
    /// </summary>
    public static class DocumentationProjectContextExtractor
    {
        #region Static fields and properties

        private static readonly string[] KContextDirectoryNames =
        [
            ".ai",
            ".aiassistant"
        ];

        private static readonly string[] KPriorityFileNames =
        [
            "rules.md",
            "architecture.md",
            "project.md",
            "domain.md",
            "coding-style.md",
            "glossary.md",
            "readme.md"
        ];

        #endregion

        #region Static methods

        /// <summary>
        /// Extracts structured documentation metadata from XML comments or project context inputs.
        /// </summary>
        /// <param name="project">The project value used by the documentation generation operation.</param>
        /// <param name="maxParentDepth">The maxParentDepth value used by the documentation generation operation.</param>
        /// <returns>The Extract result produced by DocumentationBuilder generation.</returns>
        public static DocumentationProjectContextModel Extract(
            DocumentationProjectDescriptor project,
            int maxParentDepth = 4
        )
        {
            if (project is null) throw new ArgumentNullException(nameof(project));

            if (string.IsNullOrWhiteSpace(project.ProjectFilePath))
                throw new ArgumentException("Project file path is required.", nameof(project));

            if (!File.Exists(project.ProjectFilePath))
                throw new FileNotFoundException("Project file not found.", project.ProjectFilePath);

            string projectDirectory = Path.GetDirectoryName(project.ProjectFilePath)
                                      ?? throw new InvalidOperationException("Unable to resolve project directory.");

            DocumentationProjectContextModel result = new()
            {
                ProjectFilePath = project.ProjectFilePath,
                ProjectDirectoryPath = projectDirectory,
                PackageId = project.PackageId,
                Version = project.Version
            };

            HashSet<string> addedFiles = new(StringComparer.OrdinalIgnoreCase);

            DirectoryInfo? currentDirectory = new(projectDirectory);
            int depth = 0;

            while (currentDirectory is not null && depth <= maxParentDepth)
            {
                foreach (string contextDirectoryName in KContextDirectoryNames)
                {
                    string contextDirectoryPath = Path.Combine(currentDirectory.FullName, contextDirectoryName);

                    if (!Directory.Exists(contextDirectoryPath))
                        continue;

                    foreach (DocumentationProjectContextFileModel file in ExtractContextFiles(
                                 contextDirectoryPath,
                                 contextDirectoryName,
                                 depth,
                                 addedFiles))
                    {
                        result.Files.Add(file);
                    }
                }

                currentDirectory = currentDirectory.Parent;
                depth++;
            }

            SortContextFiles(result.Files);

            return result;
        }

        /// <summary>
        /// Builds the merged project context text used by documentation pages and search metadata.
        /// </summary>
        /// <param name="model">The model value used by the documentation generation operation.</param>
        /// <returns>The BuildMergedContextText result produced by DocumentationBuilder generation.</returns>
        public static string BuildMergedContextText(DocumentationProjectContextModel model)
        {
            if (model is null) throw new ArgumentNullException(nameof(model));

            if (model.Files.Count == 0)
                return string.Empty;

            StringBuilder builder = new();

            foreach (DocumentationProjectContextFileModel file in model.Files)
            {
                builder.AppendLine($"# Context File: {file.FileName}");
                builder.AppendLine($"Path: {file.FilePath}");
                builder.AppendLine($"Type: {file.ContextType}");
                builder.AppendLine($"Source: {file.SourceFolderType}");
                builder.AppendLine($"Depth: {file.DirectoryDepth}");
                builder.AppendLine();
                builder.AppendLine(file.Content);

                if (!file.Content.EndsWith(Environment.NewLine, StringComparison.Ordinal))
                    builder.AppendLine();

                builder.AppendLine();
            }

            return builder.ToString().Trim();
        }

        private static string DetectContextType(string fileName)
        {
            return fileName.ToLowerInvariant() switch
            {
                "rules.md" => "Rules",
                "architecture.md" => "Architecture",
                "project.md" => "Project",
                "domain.md" => "Domain",
                "coding-style.md" => "CodingStyle",
                "glossary.md" => "Glossary",
                "readme.md" => "Readme",
                _ => "Other"
            };
        }

        private static IEnumerable<DocumentationProjectContextFileModel> ExtractContextFiles(
            string contextDirectoryPath,
            string sourceFolderType,
            int directoryDepth,
            ISet<string> addedFiles
        )
        {
            List<DocumentationProjectContextFileModel> result = [];

            foreach (string fileName in KPriorityFileNames)
            {
                string filePath = Path.Combine(contextDirectoryPath, fileName);

                if (!File.Exists(filePath))
                    continue;

                if (!addedFiles.Add(filePath))
                    continue;

                result.Add(CreateFileModel(filePath, sourceFolderType, directoryDepth));
            }

            foreach (string filePath in Directory
                         .GetFiles(contextDirectoryPath, "*.md", SearchOption.TopDirectoryOnly)
                         .OrderBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase))
            {
                if (!addedFiles.Add(filePath))
                    continue;

                result.Add(CreateFileModel(filePath, sourceFolderType, directoryDepth));
            }

            return result;
        }

        private static DocumentationProjectContextFileModel CreateFileModel(
            string filePath,
            string sourceFolderType,
            int directoryDepth
        )
        {
            string fileName = Path.GetFileName(filePath);

            return new DocumentationProjectContextFileModel
            {
                FilePath = filePath,
                FileName = fileName,
                ContextType = DetectContextType(fileName),
                SourceFolderType = sourceFolderType,
                DirectoryDepth = directoryDepth,
                Content = File.ReadAllText(filePath)
            };
        }

        private static int GetContextTypePriority(string contextType)
        {
            return contextType switch
            {
                "Rules" => 0,
                "Architecture" => 1,
                "Project" => 2,
                "Domain" => 3,
                "CodingStyle" => 4,
                "Glossary" => 5,
                "Readme" => 6,
                _ => 100
            };
        }

        private static int GetSourceFolderPriority(string sourceFolderType)
        {
            return sourceFolderType switch
            {
                ".aiassistant" => 0,
                ".ai" => 1,
                _ => 100
            };
        }

        private static void SortContextFiles(List<DocumentationProjectContextFileModel> files)
        {
            files.Sort(static (left, right) =>
            {
                int sourceComparison = GetSourceFolderPriority(left.SourceFolderType)
                    .CompareTo(GetSourceFolderPriority(right.SourceFolderType));

                if (sourceComparison != 0)
                    return sourceComparison;

                int depthComparison = left.DirectoryDepth.CompareTo(right.DirectoryDepth);
                if (depthComparison != 0)
                    return depthComparison;

                int typeComparison = GetContextTypePriority(left.ContextType)
                    .CompareTo(GetContextTypePriority(right.ContextType));

                if (typeComparison != 0)
                    return typeComparison;

                return string.Compare(left.FileName, right.FileName, StringComparison.OrdinalIgnoreCase);
            });
        }

        #endregion
    }
}