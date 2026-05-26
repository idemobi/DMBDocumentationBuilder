#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationExtractionManager.cs create at 2026/04/12 12:04:31
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents the DocumentationExtractionManager type used by DocumentationBuilder generation.
    /// </summary>
    public static class DocumentationExtractionManager
    {
        #region Static methods

        private static void AddUnique(List<string> items, string value)
        {
            if (!items.Contains(value, StringComparer.Ordinal)) items.Add(value);
        }

        /// <summary>
        /// Builds the project, namespace, and group index used by generated documentation navigation.
        /// </summary>
        /// <param name="groups">The groups value used by the documentation generation operation.</param>
        /// <returns>The BuildIndex result produced by DocumentationBuilder generation.</returns>
        public static DocumentationIndex BuildIndex(IEnumerable<DocumentationGroupDescriptor> groups)
        {
            if (groups is null) throw new ArgumentNullException(nameof(groups));

            List<DocumentationGroupDescriptor> groupList = groups.ToList();
            if (groupList.Count == 0) throw new InvalidOperationException("At least one documentation group must be provided.");

            DocumentationIndex index = new();

            foreach (DocumentationGroupDescriptor group in groupList)
            {
                DocumentationGroupItem groupItem = new()
                {
                    GroupName = group.GroupName
                };

                foreach (DocumentationProjectDescriptor project in group.Projects)
                {
                    DocumentationProjectItem projectItem = BuildProjectItem(project);
                    groupItem.Projects.Add(projectItem);
                }

                index.Groups.Add(groupItem);
            }

            return index;
        }

        private static DocumentationProjectItem BuildProjectItem(DocumentationProjectDescriptor project)
        {
            if (!File.Exists(project.ProjectFilePath)) throw new FileNotFoundException("Project file not found.", project.ProjectFilePath);

            string projectDirectory = Path.GetDirectoryName(project.ProjectFilePath)
                                      ?? throw new InvalidOperationException("Unable to resolve project directory.");

            List<string> sourceFiles = GetProjectSourceFiles(project);

            List<SyntaxTree> syntaxTrees = sourceFiles
                .Select(path => CSharpSyntaxTree.ParseText(File.ReadAllText(path), path: path))
                .ToList();

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: project.DisplayName,
                syntaxTrees: syntaxTrees,
                references: GetMetadataReferences(projectDirectory),
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            Dictionary<string, DocumentationNamespaceItem> namespaces = new(StringComparer.Ordinal);

            foreach (SyntaxTree syntaxTree in compilation.SyntaxTrees)
            {
                SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                foreach (TypeDeclarationSyntax declaration in syntaxTree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>())
                {
                    INamedTypeSymbol? symbol = semanticModel.GetDeclaredSymbol(declaration);
                    if (symbol is null) continue;

                    string namespaceName = symbol.ContainingNamespace?.ToDisplayString() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(namespaceName)) continue;

                    if (!namespaces.TryGetValue(namespaceName, out DocumentationNamespaceItem? namespaceItem))
                    {
                        namespaceItem = new DocumentationNamespaceItem
                        {
                            NamespaceName = namespaceName
                        };

                        namespaces.Add(namespaceName, namespaceItem);
                    }

                    if (declaration is RecordDeclarationSyntax)
                    {
                        AddUnique(namespaceItem.Records, symbol.Name);
                        continue;
                    }

                    switch (symbol.TypeKind)
                    {
                        case TypeKind.Class:
                            AddUnique(namespaceItem.Classes, symbol.Name);
                        break;

                        case TypeKind.Interface:
                            AddUnique(namespaceItem.Interfaces, symbol.Name);
                        break;

                        case TypeKind.Struct:
                            AddUnique(namespaceItem.Structs, symbol.Name);
                        break;
                    }
                }

                foreach (EnumDeclarationSyntax declaration in syntaxTree.GetRoot().DescendantNodes().OfType<EnumDeclarationSyntax>())
                {
                    INamedTypeSymbol? symbol = semanticModel.GetDeclaredSymbol(declaration);
                    if (symbol is null) continue;

                    string namespaceName = symbol.ContainingNamespace?.ToDisplayString() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(namespaceName)) continue;

                    if (!namespaces.TryGetValue(namespaceName, out DocumentationNamespaceItem? namespaceItem))
                    {
                        namespaceItem = new DocumentationNamespaceItem
                        {
                            NamespaceName = namespaceName
                        };

                        namespaces.Add(namespaceName, namespaceItem);
                    }

                    AddUnique(namespaceItem.Enums, symbol.Name);
                }
            }

            DocumentationProjectItem projectItem = new()
            {
                ProjectName = project.DisplayName,
                ProjectFilePath = project.ProjectFilePath,
                PackageId = project.PackageId,
                Version = project.Version
            };

            foreach (DocumentationNamespaceItem item in namespaces.Values.OrderBy(x => x.NamespaceName, StringComparer.Ordinal))
            {
                item.Classes.Sort(StringComparer.Ordinal);
                item.Interfaces.Sort(StringComparer.Ordinal);
                item.Records.Sort(StringComparer.Ordinal);
                item.Structs.Sort(StringComparer.Ordinal);
                item.Enums.Sort(StringComparer.Ordinal);

                projectItem.Namespaces.Add(item);
            }

            return projectItem;
        }

        private static Regex BuildWildcardRegex(string pattern)
        {
            string normalizedPattern = Path.GetFullPath(pattern).Replace('\\', '/');
            string escaped = Regex.Escape(normalizedPattern)
                .Replace(@"\*\*", "§§DOUBLESTAR§§")
                .Replace(@"\*", @"[^/]*")
                .Replace(@"\?", @"[^/]");

            escaped = escaped.Replace("§§DOUBLESTAR§§", @".*");

            return new Regex("^" + escaped + "$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        private static bool ContainsWildcard(string path)
        {
            return path.Contains('*') || path.Contains('?');
        }

        private static IEnumerable<MetadataReference> GetMetadataReferences(string projectDirectory)
        {
            HashSet<string> paths = new(StringComparer.OrdinalIgnoreCase);

            void AddAssembly(Type type)
            {
                if (!string.IsNullOrWhiteSpace(type.Assembly.Location)) paths.Add(type.Assembly.Location);
            }

            AddAssembly(typeof(object));
            AddAssembly(typeof(Enumerable));
            AddAssembly(typeof(List<>));
            AddAssembly(typeof(Console));

            string? runtimeDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);
            if (!string.IsNullOrWhiteSpace(runtimeDirectory) && Directory.Exists(runtimeDirectory))
            {
                foreach (string dll in Directory.GetFiles(runtimeDirectory, "*.dll")) paths.Add(dll);
            }

            foreach (string dll in Directory.GetFiles(projectDirectory, "*.dll", SearchOption.AllDirectories))
            {
                if (IsInBuildDirectory(dll)) paths.Add(dll);
            }

            return paths.Select(path => MetadataReference.CreateFromFile(path));
        }

        /// <summary>
        /// Resolves the C# source files compiled by a documented project.
        /// </summary>
        /// <param name="project">The documented project whose source files should be resolved.</param>
        /// <returns>The ordered source file paths used by documentation extraction.</returns>
        public static List<string> GetProjectSourceFiles(DocumentationProjectDescriptor project)
        {
            if (project is null) throw new ArgumentNullException(nameof(project));

            string projectFilePath = Path.GetFullPath(project.ProjectFilePath);
            string projectDirectory = Path.GetDirectoryName(projectFilePath)
                                      ?? throw new InvalidOperationException("Unable to resolve project directory.");

            XDocument document = XDocument.Load(projectFilePath);

            List<XElement> compileElements = document
                .Descendants()
                .Where(x => x.Name.LocalName == "Compile")
                .ToList();

            HashSet<string> includedFiles = new(StringComparer.OrdinalIgnoreCase);
            List<string> removePatterns = new();

            foreach (XElement compileElement in compileElements)
            {
                string? includeValue = compileElement.Attribute("Include")?.Value?.Trim();
                string? removeValue = compileElement.Attribute("Remove")?.Value?.Trim();

                if (!string.IsNullOrWhiteSpace(includeValue))
                {
                    foreach (string resolvedFilePath in ResolveCompilePattern(projectDirectory, includeValue))
                    {
                        if (IsInBuildDirectory(resolvedFilePath))
                        {
                            continue;
                        }

                        bool isLocalProjectFile = IsUnderDirectory(resolvedFilePath, projectDirectory);

                        if (project.CompileItemMode == DocumentationCompileItemMode.LocalProjectFilesOnly && !isLocalProjectFile)
                        {
                            Console.WriteLine($"[Documentation][Info] Linked compile item ignored: {resolvedFilePath}");
                            continue;
                        }

                        includedFiles.Add(Path.GetFullPath(resolvedFilePath));
                    }
                }

                if (!string.IsNullOrWhiteSpace(removeValue))
                {
                    removePatterns.Add(removeValue);
                }
            }

            if (removePatterns.Count > 0 && includedFiles.Count > 0)
            {
                List<string> filesToRemove = new();

                foreach (string filePath in includedFiles)
                {
                    if (MatchesAnyRemovePattern(projectDirectory, filePath, removePatterns))
                    {
                        filesToRemove.Add(filePath);
                    }
                }

                foreach (string filePath in filesToRemove)
                {
                    includedFiles.Remove(filePath);
                    Console.WriteLine($"[Documentation][Info] Compile item removed: {filePath}");
                }
            }

            if (includedFiles.Count > 0)
            {
                return includedFiles
                    .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }

            Console.WriteLine($"[Documentation][Warning] No usable <Compile Include=\"...\"> items found in '{projectFilePath}'. Fallback to directory scan.");

            return Directory
                .GetFiles(projectDirectory, "*.cs", SearchOption.AllDirectories)
                .Where(path => !IsInBuildDirectory(path))
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static string GetSearchRootFromPattern(string absolutePattern)
        {
            string normalized = absolutePattern.Replace('\\', '/');
            int wildcardIndex = normalized.IndexOfAny(new[] { '*', '?' });

            if (wildcardIndex < 0)
            {
                string? directory = Path.GetDirectoryName(absolutePattern);
                return string.IsNullOrWhiteSpace(directory) ? absolutePattern : directory;
            }

            int slashIndex = normalized.LastIndexOf('/', wildcardIndex);
            if (slashIndex < 0)
            {
                return Path.GetPathRoot(absolutePattern) ?? Directory.GetCurrentDirectory();
            }

            string root = normalized[..slashIndex].Replace('/', Path.DirectorySeparatorChar);
            return string.IsNullOrWhiteSpace(root)
                ? Path.GetPathRoot(absolutePattern) ?? Directory.GetCurrentDirectory()
                : root;
        }

        private static bool IsInBuildDirectory(string path)
        {
            string normalizedPath = path.Replace('\\', '/');

            return normalizedPath.Contains("/bin/", StringComparison.OrdinalIgnoreCase)
                   || normalizedPath.Contains("/obj/", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsUnderDirectory(string filePath, string directoryPath)
        {
            string fullFilePath = Path.GetFullPath(filePath)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            string fullDirectoryPath = Path.GetFullPath(directoryPath)
                                           .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                                       + Path.DirectorySeparatorChar;

            return fullFilePath.StartsWith(fullDirectoryPath, StringComparison.OrdinalIgnoreCase);
        }

        private static bool MatchesAnyRemovePattern(string projectDirectory, string filePath, IEnumerable<string> removePatterns)
        {
            string fullFilePath = Path.GetFullPath(filePath);

            foreach (string removePattern in removePatterns)
            {
                if (MatchesCompilePattern(projectDirectory, fullFilePath, removePattern))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool MatchesCompilePattern(string projectDirectory, string filePath, string pattern)
        {
            string normalizedPattern = pattern.Replace('\\', Path.DirectorySeparatorChar)
                .Replace('/', Path.DirectorySeparatorChar);

            if (!ContainsWildcard(normalizedPattern))
            {
                string fullPatternPath = Path.GetFullPath(Path.Combine(projectDirectory, normalizedPattern));
                return string.Equals(
                    Path.GetFullPath(filePath),
                    fullPatternPath,
                    StringComparison.OrdinalIgnoreCase);
            }

            string absolutePattern = Path.GetFullPath(Path.Combine(projectDirectory, normalizedPattern));
            Regex regex = BuildWildcardRegex(absolutePattern);

            return regex.IsMatch(Path.GetFullPath(filePath));
        }

        private static IEnumerable<string> ResolveCompilePattern(string projectDirectory, string pattern)
        {
            string normalizedPattern = pattern.Replace('\\', Path.DirectorySeparatorChar)
                .Replace('/', Path.DirectorySeparatorChar);

            if (!ContainsWildcard(normalizedPattern))
            {
                string fullPath = Path.GetFullPath(Path.Combine(projectDirectory, normalizedPattern));

                if (File.Exists(fullPath))
                {
                    yield return fullPath;
                }

                yield break;
            }

            string absolutePattern = Path.GetFullPath(Path.Combine(projectDirectory, normalizedPattern));
            string searchRoot = GetSearchRootFromPattern(absolutePattern);

            if (!Directory.Exists(searchRoot))
            {
                yield break;
            }

            Regex regex = BuildWildcardRegex(absolutePattern);

            foreach (string filePath in Directory.EnumerateFiles(searchRoot, "*.cs", SearchOption.AllDirectories))
            {
                string fullPath = Path.GetFullPath(filePath);

                if (regex.IsMatch(fullPath))
                {
                    yield return fullPath;
                }
            }
        }

        #endregion
    }
}
