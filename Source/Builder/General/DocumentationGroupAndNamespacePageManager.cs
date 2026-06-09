#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents the DocumentationGroupAndNamespacePageManager type used by DocumentationBuilder generation.
    /// </summary>
    public static class DocumentationGroupAndNamespacePageManager
    {
        #region Static methods

        private static void AddIfMissing(
            IList<DocumentationNamespaceObjectLinkItem> list,
            DocumentationNamespaceObjectLinkItem item
        )
        {
            if (list.Any(x => string.Equals(x.Name, item.Name, StringComparison.Ordinal))) return;

            list.Add(item);
        }

        private static void AddTypeToNamespacePage(
            DocumentationNamespacePageModel namespacePage,
            INamedTypeSymbol typeSymbol,
            DocumentationNamespaceObjectLinkItem linkItem
        )
        {
            switch (typeSymbol.TypeKind)
            {
                case TypeKind.Class:
                    if (typeSymbol.IsRecord)
                        AddIfMissing(namespacePage.Records, linkItem);
                    else
                        AddIfMissing(namespacePage.Classes, linkItem);
                break;

                case TypeKind.Struct:
                    if (typeSymbol.IsRecord)
                        AddIfMissing(namespacePage.Records, linkItem);
                    else
                        AddIfMissing(namespacePage.Structs, linkItem);
                break;

                case TypeKind.Enum:
                    AddIfMissing(namespacePage.Enums, linkItem);
                break;

                case TypeKind.Interface:
                    AddIfMissing(namespacePage.Interfaces, linkItem);
                break;
            }
        }

        private static string BuildGroupRoutePath(string groupName, string version)
        {
            string routePath = $"/Documentation/ShowGroup?groupName={System.Net.WebUtility.UrlEncode(groupName)}";

            if (!string.IsNullOrWhiteSpace(version))
            {
                routePath += $"&version={System.Net.WebUtility.UrlEncode(version)}";
            }

            return routePath;
        }

        private static string CreateNamespaceLookupKey(string packageId, string version, string namespaceName)
        {
            return $"{packageId}\u001F{version}\u001F{namespaceName}";
        }

        /// <summary>
        ///     Generates documentation artifacts for the configured documentation group and project descriptors.
        /// </summary>
        /// <param name="groups">The groups value used by the documentation generation operation.</param>
        /// <param name="pageOutputDirectory">The pageOutputDirectory value used by the documentation generation operation.</param>
        /// <param name="sharedDocumentationRootDirectory">
        ///     The sharedDocumentationRootDirectory value used by the documentation
        ///     generation operation.
        /// </param>
        /// <param name="sqliteDatabasePath">The sqliteDatabasePath value used by the documentation generation operation.</param>
        public static void Generate(
            IEnumerable<DocumentationGroupDescriptor> groups,
            string pageOutputDirectory,
            string sharedDocumentationRootDirectory,
            string sqliteDatabasePath
        )
        {
            if (groups is null) throw new ArgumentNullException(nameof(groups));

            List<DocumentationGroupPageModel> groupPages = [];
            List<DocumentationNamespacePageModel> namespacePages = [];

            foreach (DocumentationGroupDescriptor group in groups)
            {
                if (string.IsNullOrWhiteSpace(group.GroupName)) continue;

                foreach (IGrouping<string, DocumentationProjectDescriptor> versionGroup in group.Projects
                             .GroupBy(project => project.Version, StringComparer.Ordinal)
                             .OrderByDescending(projectGroup => projectGroup.Key, DocumentationVersionComparer.Instance))
                {
                    DocumentationGroupPageModel groupPage = new()
                    {
                        PackageId = string.Empty,
                        Version = versionGroup.Key,
                        GroupName = group.GroupName
                    };

                    Dictionary<string, DocumentationNamespacePageModel> namespaceMap =
                        new(StringComparer.Ordinal);

                    foreach (DocumentationProjectDescriptor project in versionGroup)
                    {
                        if (string.IsNullOrWhiteSpace(project.ProjectFilePath)) continue;

                        if (!File.Exists(project.ProjectFilePath)) throw new FileNotFoundException("Project file not found.", project.ProjectFilePath);

                        string projectDirectory = Path.GetDirectoryName(project.ProjectFilePath)
                                                  ?? throw new InvalidOperationException("Unable to resolve project directory.");

                        List<string> sourceFiles = Directory
                            .GetFiles(projectDirectory, "*.cs", SearchOption.AllDirectories)
                            .Where(path => !IsInBuildDirectory(path))
                            .ToList();

                        if (sourceFiles.Count == 0) continue;

                        List<SyntaxTree> syntaxTrees = sourceFiles
                            .Select(path => CSharpSyntaxTree.ParseText(File.ReadAllText(path), path: path))
                            .ToList();

                        CSharpCompilation compilation = CSharpCompilation.Create(
                            assemblyName: project.DisplayName,
                            syntaxTrees: syntaxTrees,
                            references: GetMetadataReferencePaths(projectDirectory)
                                .Select(path => MetadataReference.CreateFromFile(path)),
                            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                        foreach (SyntaxTree syntaxTree in compilation.SyntaxTrees)
                        {
                            SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

                            foreach (BaseTypeDeclarationSyntax typeDeclaration in syntaxTree.GetRoot()
                                         .DescendantNodes()
                                         .OfType<BaseTypeDeclarationSyntax>())
                            {
                                if (semanticModel.GetDeclaredSymbol(typeDeclaration) is not INamedTypeSymbol typeSymbol) continue;

                                if (typeSymbol.IsImplicitlyDeclared) continue;

                                string namespaceName = typeSymbol.ContainingNamespace?.ToDisplayString() ?? string.Empty;
                                if (string.IsNullOrWhiteSpace(namespaceName)) continue;

                                string namespaceKey = CreateNamespaceLookupKey(project.PackageId, project.Version, namespaceName);

                                if (!namespaceMap.TryGetValue(namespaceKey, out DocumentationNamespacePageModel? namespacePage))
                                {
                                    namespacePage = new DocumentationNamespacePageModel
                                    {
                                        PackageId = project.PackageId,
                                        Version = project.Version,
                                        GroupName = group.GroupName,
                                        NamespaceName = namespaceName
                                    };

                                    namespaceMap.Add(namespaceKey, namespacePage);
                                }

                                DocumentationNamespaceObjectLinkItem linkItem = new()
                                {
                                    PackageId = project.PackageId,
                                    Version = project.Version,
                                    Name = typeSymbol.Name,
                                    KindLabel = GetKindLabel(typeSymbol)
                                };

                                AddTypeToNamespacePage(namespacePage, typeSymbol, linkItem);
                            }
                        }
                    }

                    foreach (DocumentationNamespacePageModel namespacePage in namespaceMap.Values
                                 .OrderBy(x => x.PackageId, StringComparer.Ordinal)
                                 .ThenBy(x => x.NamespaceName, StringComparer.Ordinal))
                    {
                        groupPage.NamespaceNames.Add(namespacePage.NamespaceName);
                        groupPage.NamespaceLinks.Add(new DocumentationNamespaceObjectLinkItem
                        {
                            PackageId = namespacePage.PackageId,
                            Version = namespacePage.Version,
                            Name = namespacePage.NamespaceName,
                            KindLabel = "Namespace"
                        });
                        namespacePages.Add(namespacePage);
                    }

                    groupPages.Add(groupPage);
                }
            }

            foreach (var item in groupPages)
            {
                string safeGroupName = DocumentationPathHelper.ToSafeName(item.GroupName);
                string htmlContent = DocumentationGroupAndNamespacePageRenderer.RenderGroupHtml(item, safeGroupName, sharedDocumentationRootDirectory);

                DocumentationDatabaseManager.SaveObject(
                    sqliteDatabasePath,
                    string.Empty,
                    item.Version,
                    string.Empty,
                    item.GroupName,
                    "Group",
                    item,
                    htmlContent,
                    //item.GroupName,
                    DocumentationGroupTechnicalKeywordExtractor.ExtractKeywordsAsString(item),
                    DocumentationKeywordExtractor.ExtractKeywordsAsString(htmlContent),
                    BuildGroupRoutePath(item.GroupName, item.Version));
            }

            // DocumentationGroupAndNamespacePageRenderer.RenderGroupPages(
            //     groupPages.OrderBy(x => x.GroupName, StringComparer.Ordinal),
            //     pageOutputDirectory,
            //     sharedDocumentationRootDirectory);

            foreach (var item in namespacePages)
            {
                string namespacePath = DocumentationPathHelper.NamespaceToPath(item.NamespaceName);
                string htmlContent = DocumentationGroupAndNamespacePageRenderer.RenderNamespaceHtml(item, namespacePath, sharedDocumentationRootDirectory);

                DocumentationDatabaseManager.SaveObject(
                    sqliteDatabasePath,
                    item.PackageId,
                    item.Version,
                    item.NamespaceName,
                    item.NamespaceName,
                    "Namespace",
                    item,
                    htmlContent,
                    //item.NamespaceName,
                    DocumentationNamespaceTechnicalKeywordExtractor.ExtractKeywordsAsString(item),
                    DocumentationKeywordExtractor.ExtractKeywordsAsString(htmlContent),
                    $"/Documentation/ShowNamespace?packageId={item.PackageId}&version={item.Version}&groupName={item.GroupName}&namespaceName={item.NamespaceName}");
            }

            // DocumentationGroupAndNamespacePageRenderer.RenderNamespacePages(
            //     namespacePages.OrderBy(x => x.GroupName, StringComparer.Ordinal)
            //         .ThenBy(x => x.NamespaceName, StringComparer.Ordinal),
            //     pageOutputDirectory,
            //     sharedDocumentationRootDirectory);
        }

        private static string GetKindLabel(INamedTypeSymbol typeSymbol)
        {
            return typeSymbol.TypeKind switch
            {
                TypeKind.Class when typeSymbol.IsRecord => "Record",
                TypeKind.Struct when typeSymbol.IsRecord => "Record",
                TypeKind.Class => "Class",
                TypeKind.Struct => "Struct",
                TypeKind.Enum => "Enum",
                TypeKind.Interface => "Interface",
                _ => typeSymbol.TypeKind.ToString()
            };
        }

        private static IEnumerable<string> GetMetadataReferencePaths(string projectDirectory)
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

            return paths;
        }

        private static bool IsInBuildDirectory(string path)
        {
            string normalizedPath = path.Replace('\\', '/');

            return normalizedPath.Contains("/bin/", StringComparison.OrdinalIgnoreCase)
                   || normalizedPath.Contains("/obj/", StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
