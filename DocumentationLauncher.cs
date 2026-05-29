#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Text.RegularExpressions;
using System.Xml.Linq;

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Coordinates DocumentationBuilder cleanup and generation for a target website project.
    /// </summary>
    public static class DocumentationLauncher
    {
        #region Static methods

        /// <summary>
        ///     Removes generated documentation artifacts from a target website project.
        /// </summary>
        /// <param name="websiteProjectRelativePath">
        ///     The relative or absolute path to the website project that hosts generated
        ///     documentation.
        /// </param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="websiteProjectRelativePath" /> is empty.</exception>
        /// <remarks>
        ///     The cleanup removes generated documentation folders and reverses generated registration snippets from the host
        ///     project.
        /// </remarks>
        public static void Clean(string websiteProjectRelativePath)
        {
            Console.WriteLine($"DocumentationLauncher : Clean {websiteProjectRelativePath}");
            if (string.IsNullOrWhiteSpace(websiteProjectRelativePath)) throw new ArgumentException("Invalid path.", nameof(websiteProjectRelativePath));

            string websiteRoot = GetWebsiteRoot(websiteProjectRelativePath);

            string docDir = Path.Combine(websiteRoot, "Documentation");
            string viewsDocumentationDir = Path.Combine(websiteRoot, "Views", "Documentation");
            string generatedDir = Path.Combine(websiteRoot, "Generated");

            DeleteDirectoryIfExists(docDir);
            DeleteDirectoryIfExists(generatedDir);
            DeleteDirectoryIfExists(viewsDocumentationDir);

            RemoveProgramCsRegistration(websiteRoot);
            RemoveCsprojDatabaseConfig(websiteRoot);
        }

        private static DocumentationProjectDescriptor CreateReferencedProjectDescriptor(string projectFilePath)
        {
            string fullPath = Path.GetFullPath(projectFilePath);

            return new DocumentationProjectDescriptor
            {
                DisplayName = Path.GetFileNameWithoutExtension(fullPath),
                ProjectFilePath = fullPath,
                PackageId = string.Empty,
                Version = string.Empty,
                ProjectReferenceMode = DocumentationProjectReferenceMode.None
            };
        }

        private static void DeleteDirectoryIfExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath)) return;

            Directory.Delete(directoryPath, recursive: true);
        }

        private static void DeleteFileIfExists(string filePath)
        {
            if (!File.Exists(filePath)) return;

            File.Delete(filePath);
        }

        private static void EnsureDatabaseCopyAlways(string websiteRoot)
        {
            string[] csprojFiles = Directory.GetFiles(websiteRoot, "*.csproj");
            if (csprojFiles.Length == 0) return;

            string csprojPath = csprojFiles[0];
            string content = File.ReadAllText(csprojPath);

            if (content.Contains("Documentation\\data.db") && content.Contains("CopyToOutputDirectory"))
            {
                if (content.Contains("<CopyToOutputDirectory>Always</CopyToOutputDirectory>") ||
                    content.Contains("<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>"))
                {
                    return;
                }
            }

            string itemGroup = "\n  <ItemGroup>\n    <None Update=\"Documentation\\data.db\">\n      <CopyToOutputDirectory>Always</CopyToOutputDirectory>\n    </None>\n  </ItemGroup>\n";

            if (content.Contains("</Project>"))
            {
                content = content.Replace("</Project>", itemGroup + "</Project>");
                File.WriteAllText(csprojPath, content);
            }
        }

        private static void EnsureProjectMetadata(IEnumerable<DocumentationGroupDescriptor> groups, string sqliteDatabasePath)
        {
            foreach (DocumentationGroupDescriptor group in groups)
            {
                foreach (DocumentationProjectDescriptor project in group.Projects)
                {
                    if (!File.Exists(project.ProjectFilePath))
                    {
                        DocumentationProjectContextModel context = DocumentationProjectContextExtractor.Extract(project);
                        foreach (var file in context.Files)
                        {
                            DocumentationDatabaseManager.SaveProjectContextFile(
                                sqliteDatabasePath,
                                context.PackageId,
                                context.Version,
                                context.ProjectFilePath,
                                context.ProjectDirectoryPath,
                                file.FilePath,
                                file.FileName,
                                file.ContextType,
                                file.SourceFolderType,
                                file.DirectoryDepth,
                                file.Content
                            );
                        }

                        string fullPath = project.ProjectFilePath;
                        string lastValidDir = "unknown";
                        string[] pathParts = fullPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                        for (int i = 0; i < pathParts.Length - 1; i++)
                        {
                            string testPath = string.Join(Path.DirectorySeparatorChar.ToString(), pathParts.Take(i + 1));
                            if (Directory.Exists(testPath))
                            {
                                lastValidDir = testPath;
                            }
                        }

                        throw new FileNotFoundException($"Project file not found. Last valid directory: {lastValidDir}", fullPath);
                    }

                    bool hasPackageId = !string.IsNullOrWhiteSpace(project.PackageId);
                    bool hasVersion = !string.IsNullOrWhiteSpace(project.Version);

                    if (hasPackageId && hasVersion) continue;

                    (string packageId, string version) = ReadProjectMetadata(project.ProjectFilePath);

                    if (!hasPackageId) project.PackageId = packageId;

                    if (!hasVersion) project.Version = version;
                }
            }
        }

        private static void EnsureSourceFileSnapshots(IEnumerable<DocumentationGroupDescriptor> groups, string sqliteDatabasePath)
        {
            foreach (DocumentationGroupDescriptor group in groups)
            {
                foreach (DocumentationProjectDescriptor project in group.Projects)
                {
                    IReadOnlyList<DocumentationSourceFileItem> sourceFiles = DocumentationSourceFileExtractor.Extract(project);

                    DocumentationDatabaseManager.ReplaceGeneratedSourceFiles(
                        sqliteDatabasePath,
                        project.PackageId,
                        project.Version,
                        sourceFiles);
                }
            }
        }

        private static string GetWebsiteRoot(string websiteProjectRelativePath)
        {
            return Path.GetFullPath(Path.Combine(
                AppContext.BaseDirectory,
                websiteProjectRelativePath));
        }

        private static string NormalizeVersion(string rawVersion)
        {
            if (string.IsNullOrWhiteSpace(rawVersion) || rawVersion.Contains("unknown"))
            {
                return "unknown";
            }

            string clean = rawVersion.Split('+')[0].Split('-')[0];

            string[] parts = clean.Split(
                '.',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (parts.Length == 0) return "1.0";
            if (parts.Length == 1) return $"{parts[0]}.0";

            return $"{parts[0]}.{parts[1]}";
        }

        private static (string PackageId, string Version) ReadProjectMetadata(string projectFilePath)
        {
            XDocument document = XDocument.Load(projectFilePath);

            static string? ReadProperty(XDocument document, string propertyName)
            {
                return document
                    .Descendants()
                    .FirstOrDefault(x => x.Name.LocalName == propertyName)
                    ?.Value
                    ?.Trim();
            }

            string projectName = Path.GetFileNameWithoutExtension(projectFilePath);

            string packageId =
                ReadProperty(document, "PackageId")
                ?? ReadProperty(document, "AssemblyName")
                ?? projectName;

            string rawVersion =
                ReadProperty(document, "PackageVersion")
                ?? ReadProperty(document, "Version")
                ?? "unknown";

            return (packageId, NormalizeVersion(rawVersion));
        }

        private static IReadOnlyList<string> ReadProjectReferences(string projectFilePath)
        {
            XDocument document = XDocument.Load(projectFilePath);

            string projectDirectory = Path.GetDirectoryName(projectFilePath)
                                      ?? throw new InvalidOperationException($"Unable to determine directory for project '{projectFilePath}'.");

            List<string> references = document
                .Descendants()
                .Where(x => x.Name.LocalName == "ProjectReference")
                .Select(x => x.Attribute("Include")?.Value?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => Path.GetFullPath(Path.Combine(projectDirectory, x!)))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return references;
        }

        private static void RemoveCsprojDatabaseConfig(string websiteRoot)
        {
            string[] csprojFiles = Directory.GetFiles(websiteRoot, "*.csproj");
            if (csprojFiles.Length == 0) return;

            string csprojPath = csprojFiles[0];
            string content = File.ReadAllText(csprojPath);

            string pattern = @"\s*<ItemGroup>\s*<None Update=""Documentation[\\/]data\.db"">.*?<\/None>\s*<\/ItemGroup>";
            string newContent = Regex.Replace(content, pattern, "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            if (newContent != content)
            {
                File.WriteAllText(csprojPath, newContent);
            }
        }

        private static void RemoveProgramCsRegistration(string websiteRoot)
        {
            string programCsPath = Path.Combine(websiteRoot, "Program.cs");
            if (!File.Exists(programCsPath)) return;

            string content = File.ReadAllText(programCsPath);
            bool modified = false;

            string[] usingsToRemove =
            {
                "using DocumentationBuilders.Generated;",
                "using DocumentationBuilders.Documentation;"
            };

            foreach (string u in usingsToRemove)
            {
                if (content.Contains(u))
                {
                    content = content.Replace(u + "\r\n", "");
                    content = content.Replace(u + "\n", "");
                    content = content.Replace(u, "");
                    modified = true;
                }
            }

            string registrationLine = "DocumentationSidebarProvider.Register();";
            if (content.Contains(registrationLine))
            {
                content = content.Replace(registrationLine + "\r\n\r\n", "");
                content = content.Replace(registrationLine + "\n\n", "");
                content = content.Replace(registrationLine + "\r\n", "");
                content = content.Replace(registrationLine + "\n", "");
                content = content.Replace(registrationLine, "");
                modified = true;
            }

            if (modified)
            {
                File.WriteAllText(programCsPath, content.TrimEnd() + Environment.NewLine);
            }
        }

        private static IReadOnlyCollection<DocumentationGroupDescriptor> ResolveProjectReferences(
            IReadOnlyCollection<DocumentationGroupDescriptor> groups
        )
        {
            List<DocumentationGroupDescriptor> resolvedGroups = new();

            foreach (DocumentationGroupDescriptor group in groups)
            {
                Dictionary<string, DocumentationProjectDescriptor> resolvedProjects = new(StringComparer.OrdinalIgnoreCase);

                foreach (DocumentationProjectDescriptor project in group.Projects)
                {
                    foreach (DocumentationProjectDescriptor resolvedProject in ResolveProjectsForDescriptor(project))
                    {
                        string fullPath = Path.GetFullPath(resolvedProject.ProjectFilePath);

                        if (!resolvedProjects.TryAdd(fullPath, resolvedProject))
                        {
                            Console.WriteLine($"[Documentation][Warning] Duplicate project ignored in group '{group.GroupName}': {fullPath}");
                        }
                    }
                }

                resolvedGroups.Add(new DocumentationGroupDescriptor
                {
                    GroupName = group.GroupName,
                    Projects = resolvedProjects.Values.ToList()
                });
            }

            return resolvedGroups;
        }

        private static IReadOnlyCollection<DocumentationProjectDescriptor> ResolveProjectsForDescriptor(DocumentationProjectDescriptor rootProject)
        {
            Dictionary<string, DocumentationProjectDescriptor> resolvedProjects = new(StringComparer.OrdinalIgnoreCase);

            void AddProject(DocumentationProjectDescriptor project, bool fromReference)
            {
                string fullPath = Path.GetFullPath(project.ProjectFilePath);

                if (!resolvedProjects.TryAdd(fullPath, project))
                {
                    Console.WriteLine($"[Documentation][Warning] Duplicate project ignored: {fullPath}");
                    return;
                }

                if (fromReference)
                {
                    Console.WriteLine($"[Documentation][Info] ProjectReference added: {fullPath}");
                }
            }

            void AddReferencedProjectRecursive(string referencedProjectPath)
            {
                string fullPath = Path.GetFullPath(referencedProjectPath);

                if (resolvedProjects.ContainsKey(fullPath))
                {
                    Console.WriteLine($"[Documentation][Info] ProjectReference ignored (already included): {fullPath}");
                    return;
                }

                DocumentationProjectDescriptor referencedProject = CreateReferencedProjectDescriptor(fullPath);
                AddProject(referencedProject, fromReference: true);

                foreach (string childReferencePath in ReadProjectReferences(fullPath))
                {
                    AddReferencedProjectRecursive(childReferencePath);
                }
            }

            AddProject(rootProject, fromReference: false);

            switch (rootProject.ProjectReferenceMode)
            {
                case DocumentationProjectReferenceMode.None:
                break;

                case DocumentationProjectReferenceMode.DirectOnly:
                    foreach (string referencedProjectPath in ReadProjectReferences(rootProject.ProjectFilePath))
                    {
                        DocumentationProjectDescriptor referencedProject = CreateReferencedProjectDescriptor(referencedProjectPath);
                        AddProject(referencedProject, fromReference: true);
                    }

                break;

                case DocumentationProjectReferenceMode.Recursive:
                    foreach (string referencedProjectPath in ReadProjectReferences(rootProject.ProjectFilePath))
                    {
                        AddReferencedProjectRecursive(referencedProjectPath);
                    }

                break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(rootProject.ProjectReferenceMode),
                        rootProject.ProjectReferenceMode,
                        "Unsupported project reference mode.");
            }

            return resolvedProjects.Values.ToList();
        }

        /// <summary>
        ///     Generates documentation pages, sidebar source, and SQLite metadata for the provided documentation groups.
        /// </summary>
        /// <param name="websiteProjectRelativePath">
        ///     The relative or absolute path to the website project that will receive
        ///     generated documentation.
        /// </param>
        /// <param name="groups">The documentation group descriptors that identify projects and grouping metadata to generate.</param>
        /// <exception cref="ArgumentException">
        ///     Thrown when <paramref name="websiteProjectRelativePath" /> is empty or when <paramref name="groups" /> is empty.
        /// </exception>
        /// <remarks>
        ///     Generation creates or updates documentation directories, the documentation SQLite database, sidebar provider
        ///     source,
        ///     group and namespace pages, and type pages for classes, records, structs, interfaces, and enums.
        /// </remarks>
        public static void Run(
            string websiteProjectRelativePath,
            IReadOnlyCollection<DocumentationGroupDescriptor> groups
        )
        {
            Console.WriteLine($"DocumentationLauncher : Run on {websiteProjectRelativePath}");
            if (string.IsNullOrWhiteSpace(websiteProjectRelativePath)) throw new ArgumentException("Invalid path.", nameof(websiteProjectRelativePath));
            if (groups is null || groups.Count == 0) throw new ArgumentException("No groups provided.", nameof(groups));

            IReadOnlyCollection<DocumentationGroupDescriptor> resolvedGroups = ResolveProjectReferences(groups);


            string websiteRoot = GetWebsiteRoot(websiteProjectRelativePath);

            string docDatabaseDir = Path.Combine(websiteRoot, "Documentation");
            string sqliteDatabasePath = Path.Combine(docDatabaseDir, "data.db");
            string customizeDir = Path.Combine(docDatabaseDir, "Customize");

            Directory.CreateDirectory(docDatabaseDir);
            Directory.CreateDirectory(customizeDir);

            if (!File.Exists(sqliteDatabasePath))
            {
                File.Create(sqliteDatabasePath).Dispose();
            }

            string viewsDocumentationDir = Path.Combine(websiteRoot, "Views", "Documentation");
            string viewsObjectsDir = Path.Combine(viewsDocumentationDir, "Objects");

            Directory.CreateDirectory(viewsDocumentationDir);
            Directory.CreateDirectory(viewsObjectsDir);

            EnsureProjectMetadata(resolvedGroups, sqliteDatabasePath);
            EnsureSourceFileSnapshots(resolvedGroups, sqliteDatabasePath);
            DocumentationIndex index = DocumentationExtractionManager.BuildIndex(resolvedGroups);

            DeleteFileIfExists(Path.Combine(docDatabaseDir, "DocumentationSidebarProvider.cs"));
            RemoveProgramCsRegistration(websiteRoot);

            DocumentationGroupAndNamespacePageManager.Generate(
                resolvedGroups,
                viewsDocumentationDir,
                customizeDir,
                sqliteDatabasePath);

            DocumentationClassPageManager.Generate(resolvedGroups, viewsObjectsDir, customizeDir, sqliteDatabasePath);
            DocumentationRecordPageManager.Generate(resolvedGroups, viewsObjectsDir, customizeDir, sqliteDatabasePath);
            DocumentationStructPageManager.Generate(resolvedGroups, viewsObjectsDir, customizeDir, sqliteDatabasePath);
            DocumentationInterfacePageManager.Generate(resolvedGroups, viewsObjectsDir, customizeDir, sqliteDatabasePath);
            DocumentationEnumPageManager.Generate(resolvedGroups, viewsObjectsDir, customizeDir, sqliteDatabasePath);
            DocumentationMarkdownPageManager.Generate(resolvedGroups, sqliteDatabasePath);
            DocumentationOpenApiPageManager.Generate(resolvedGroups, sqliteDatabasePath);

            DocumentationSidebarDatabaseGenerator.GenerateSidebarData(
                index,
                resolvedGroups,
                sqliteDatabasePath);

            EnsureDatabaseCopyAlways(websiteRoot);
            RemoveProgramCsRegistration(websiteRoot);
        }

        #endregion
    }
}