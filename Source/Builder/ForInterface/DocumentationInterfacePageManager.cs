#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Globalization;
using DMBDocumentationBuilder.Shared;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents the DocumentationInterfacePageManager type used by DocumentationBuilder generation.
    /// </summary>
    public static class DocumentationInterfacePageManager
    {
        #region Static methods

        private static string BuildAccessibility(Accessibility accessibility)
        {
            return accessibility switch
            {
                Accessibility.Public => "public",
                Accessibility.Internal => "internal",
                Accessibility.Private => "private",
                Accessibility.Protected => "protected",
                Accessibility.ProtectedAndInternal => "private protected",
                Accessibility.ProtectedOrInternal => "protected internal",
                _ => string.Empty
            };
        }

        private static string BuildDeclaration(INamedTypeSymbol interfaceSymbol)
        {
            string accessibility = BuildAccessibility(interfaceSymbol.DeclaredAccessibility);

            string typeParameters = interfaceSymbol.IsGenericType
                ? $"<{string.Join(", ", interfaceSymbol.TypeParameters.Select(tp => tp.Name))}>"
                : string.Empty;

            string baseInterfaces = string.Empty;

            if (interfaceSymbol.Interfaces.Length > 0)
            {
                baseInterfaces = " : " + string.Join(", ",
                    interfaceSymbol.Interfaces.Select(x => x.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
            }

            return $"{accessibility} interface {interfaceSymbol.Name}{typeParameters}{baseInterfaces}".Trim();
        }

        private static string BuildExtensionMethodSignature(IMethodSymbol methodSymbol)
        {
            string returnType = methodSymbol.ReturnType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
            string typeParameters = methodSymbol.IsGenericMethod
                ? $"<{string.Join(", ", methodSymbol.TypeParameters.Select(tp => tp.Name))}>"
                : string.Empty;

            string parameters = string.Join(", ",
                methodSymbol.Parameters.Select((p, i) => FormatParameter(p, includeThisModifier: i == 0)));

            string accessibility = BuildAccessibility(methodSymbol.DeclaredAccessibility);
            string staticModifier = methodSymbol.IsStatic ? " static" : string.Empty;

            return $"{accessibility}{staticModifier} {returnType} {methodSymbol.Name}{typeParameters}({parameters})".Trim();
        }

        private static CSharpCompilation BuildGlobalCompilation(
            IEnumerable<DocumentationProjectDescriptor> projects
        )
        {
            List<SyntaxTree> syntaxTrees = [];
            HashSet<string> metadataPaths = new(StringComparer.OrdinalIgnoreCase);

            foreach (DocumentationProjectDescriptor project in projects)
            {
                if (!File.Exists(project.ProjectFilePath)) throw new FileNotFoundException("Project file not found.", project.ProjectFilePath);

                string projectDirectory = Path.GetDirectoryName(project.ProjectFilePath)
                                          ?? throw new InvalidOperationException("Unable to resolve project directory.");

                foreach (string sourceFile in Directory
                             .GetFiles(projectDirectory, "*.cs", SearchOption.AllDirectories)
                             .Where(path => !IsInBuildDirectory(path)))
                {
                    syntaxTrees.Add(CSharpSyntaxTree.ParseText(File.ReadAllText(sourceFile), path: sourceFile));
                }

                foreach (string referencePath in GetMetadataReferencePaths(projectDirectory)) metadataPaths.Add(referencePath);
            }

            return CSharpCompilation.Create(
                assemblyName: "DocumentationGlobalCompilation",
                syntaxTrees: syntaxTrees,
                references: metadataPaths.Select(path => MetadataReference.CreateFromFile(path)),
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        }

        private static string FormatDefaultValue(IParameterSymbol parameter)
        {
            object? value = parameter.ExplicitDefaultValue;

            if (value is null) return "null";

            if (parameter.Type.TypeKind == TypeKind.Enum)
            {
                string enumTypeName = parameter.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

                foreach (IFieldSymbol field in parameter.Type.GetMembers().OfType<IFieldSymbol>())
                {
                    if (!field.IsConst) continue;

                    if (Equals(field.ConstantValue, value)) return $"{enumTypeName}.{field.Name}";
                }
            }

            return value switch
            {
                string s => $"\"{s}\"",
                char c => $"'{c}'",
                bool b => b ? "true" : "false",
                _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? "null"
            };
        }

        private static string FormatParameter(IParameterSymbol parameter, bool includeThisModifier)
        {
            List<string> parts = [];

            if (includeThisModifier) parts.Add("this");

            if (parameter.IsParams) parts.Add("params");

            switch (parameter.RefKind)
            {
                case RefKind.Ref:
                    parts.Add("ref");
                break;
                case RefKind.Out:
                    parts.Add("out");
                break;
                case RefKind.In:
                    parts.Add("in");
                break;
            }

            parts.Add(parameter.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            parts.Add(parameter.Name);

            string text = string.Join(" ", parts);

            if (parameter.HasExplicitDefaultValue) text += $" = {FormatDefaultValue(parameter)}";

            return text;
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
            Console.WriteLine("Generating interface documentation pages...");
            if (groups is null) throw new ArgumentNullException(nameof(groups));

            List<DocumentationProjectDescriptor> projectList = groups
                .SelectMany(group => group.Projects)
                .GroupBy(project => project.ProjectFilePath, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .ToList();

            if (projectList.Count == 0) throw new InvalidOperationException("At least one project must be provided.");

            CSharpCompilation globalCompilation = BuildGlobalCompilation(projectList);
            Dictionary<string, DocumentationTypeRegistryItem> documentedTypeIndex = DocumentationTypeRegistry.BuildDocumentedTypeIndex(globalCompilation, groups);
            IReadOnlyList<DocumentationDependencyEdgeItem> dependencyEdges = DocumentationDependencyGraphExtractor.BuildAllDependencyEdges(globalCompilation, documentedTypeIndex);

            List<DocumentationInterfacePageModel> pages = [];

            foreach (DocumentationGroupDescriptor group in groups)
            {
                foreach (DocumentationProjectDescriptor project in group.Projects)
                {
                    Console.WriteLine($"Generating interface documentation pages for project: {project.DisplayName}");
                    if (!File.Exists(project.ProjectFilePath)) throw new FileNotFoundException("Project file not found.", project.ProjectFilePath);

                    string projectDirectory = Path.GetDirectoryName(project.ProjectFilePath)
                                              ?? throw new InvalidOperationException("Unable to resolve project directory.");

                    List<string> sourceFiles = Directory
                        .GetFiles(projectDirectory, "*.cs", SearchOption.AllDirectories)
                        .Where(path => !IsInBuildDirectory(path))
                        .ToList();

                    List<SyntaxTree> syntaxTrees = sourceFiles
                        .Select(path => CSharpSyntaxTree.ParseText(File.ReadAllText(path), path: path))
                        .ToList();

                    if (syntaxTrees.Count == 0) continue;

                    CSharpCompilation localCompilation = CSharpCompilation.Create(
                        assemblyName: project.DisplayName,
                        syntaxTrees: syntaxTrees,
                        references: GetMetadataReferencePaths(projectDirectory)
                            .Select(path => MetadataReference.CreateFromFile(path)),
                        options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                    foreach (SyntaxTree syntaxTree in localCompilation.SyntaxTrees)
                    {
                        SemanticModel semanticModel = localCompilation.GetSemanticModel(syntaxTree);

                        IEnumerable<InterfaceDeclarationSyntax> interfaceDeclarations = syntaxTree.GetRoot()
                            .DescendantNodes()
                            .OfType<InterfaceDeclarationSyntax>();

                        foreach (InterfaceDeclarationSyntax interfaceDeclaration in interfaceDeclarations)
                        {
                            if (semanticModel.GetDeclaredSymbol(interfaceDeclaration) is not INamedTypeSymbol interfaceSymbol) continue;

                            string namespaceName = interfaceSymbol.ContainingNamespace?.ToDisplayString() ?? string.Empty;
                            if (string.IsNullOrWhiteSpace(namespaceName)) continue;

                            DocumentationXmlModel interfaceXmlDoc = DocumentationXmlCommentRenderer.Extract(
                                project.PackageId,
                                project.Version,
                                group.GroupName,
                                interfaceSymbol,
                                localCompilation,
                                interfaceSymbol);

                            DocumentationAttributeHelper.IsObsolete(interfaceSymbol, out string interfaceObsoleteMessage);
                            DocumentationInterfacePageModel model = new()
                            {
                                PackageId = project.PackageId,
                                Version = project.Version,
                                GroupName = group.GroupName,
                                Accessibility = BuildAccessibility(interfaceSymbol.DeclaredAccessibility),
                                NamespaceName = namespaceName,
                                InterfaceName = interfaceSymbol.Name,
                                AssemblyName = project.DisplayName + ".dll",
                                XmlDoc = interfaceXmlDoc,
                                Declaration = BuildDeclaration(interfaceSymbol),
                                IsObsolete = DocumentationAttributeHelper.IsObsolete(interfaceSymbol, out _),
                                ObsoleteMessage = interfaceObsoleteMessage
                            };

                            model.DependencyEdges.AddRange(DocumentationDependencyGraphExtractor.BuildRelatedDependencyEdges(
                                interfaceSymbol,
                                dependencyEdges));

                            foreach (IPropertySymbol property in interfaceSymbol.GetMembers().OfType<IPropertySymbol>())
                            {
                                DocumentationAttributeHelper.IsObsolete(property, out string propertyObsoleteMessage);
                                DocumentationXmlModel propertyXmlDoc = DocumentationXmlCommentRenderer.Extract(
                                    project.PackageId,
                                    project.Version,
                                    group.GroupName,
                                    property,
                                    localCompilation,
                                    interfaceSymbol);

                                model.Properties.Add(new DocumentationInterfacePropertyItem
                                {
                                    Accessibility = BuildAccessibility(property.DeclaredAccessibility),
                                    PropertyName = property.Name,
                                    Signature = DocumentationMemberSignatureFormatter.FormatProperty(property),
                                    XmlDoc = propertyXmlDoc,
                                    IsStatic = property.IsStatic,
                                    IsAbstract = property.IsAbstract,
                                    IsVirtual = property.IsVirtual,
                                    IsOverride = property.IsOverride,
                                    IsSealed = property.IsSealed,
                                    IsReadOnly = property.SetMethod is null,
                                    IsObsolete = DocumentationAttributeHelper.IsObsolete(property, out _),
                                    ObsoleteMessage = propertyObsoleteMessage
                                });
                            }

                            foreach (IEventSymbol eventSymbol in interfaceSymbol.GetMembers().OfType<IEventSymbol>())
                            {
                                DocumentationAttributeHelper.IsObsolete(eventSymbol, out string eventObsoleteMessage);
                                DocumentationXmlModel eventXmlDoc = DocumentationXmlCommentRenderer.Extract(
                                    project.PackageId,
                                    project.Version,
                                    group.GroupName,
                                    eventSymbol,
                                    localCompilation,
                                    interfaceSymbol);

                                model.Events.Add(new DocumentationInterfaceEventItem
                                {
                                    Accessibility = BuildAccessibility(eventSymbol.DeclaredAccessibility),
                                    EventName = eventSymbol.Name,
                                    Signature = DocumentationMemberSignatureFormatter.FormatEvent(eventSymbol),
                                    XmlDoc = eventXmlDoc,
                                    IsStatic = eventSymbol.IsStatic,
                                    IsObsolete = DocumentationAttributeHelper.IsObsolete(eventSymbol, out _),
                                    ObsoleteMessage = eventObsoleteMessage
                                });
                            }

                            foreach (IMethodSymbol method in interfaceSymbol.GetMembers().OfType<IMethodSymbol>())
                            {
                                if (method.MethodKind != MethodKind.Ordinary) continue;

                                DocumentationAttributeHelper.IsObsolete(method, out string methodObsoleteMessage);
                                DocumentationXmlModel methodXmlDoc = DocumentationXmlCommentRenderer.Extract(
                                    project.PackageId,
                                    project.Version,
                                    group.GroupName,
                                    method,
                                    localCompilation,
                                    interfaceSymbol);

                                model.Methods.Add(new DocumentationInterfaceMethodItem
                                {
                                    Accessibility = BuildAccessibility(method.DeclaredAccessibility),
                                    MethodName = method.Name,
                                    Signature = DocumentationMemberSignatureFormatter.FormatMethod(method),
                                    XmlDoc = methodXmlDoc,
                                    IsStatic = method.IsStatic,
                                    IsAbstract = method.IsAbstract,
                                    IsVirtual = method.IsVirtual,
                                    IsOverride = method.IsOverride,
                                    IsSealed = method.IsSealed,
                                    IsObsolete = DocumentationAttributeHelper.IsObsolete(method, out _),
                                    ObsoleteMessage = methodObsoleteMessage
                                });
                            }

                            model.ExtensionMethods.AddRange(GetExtensionMethods(globalCompilation, interfaceSymbol));

                            string htmlContent = DocumentationInterfacePageRenderer.RenderHtmlPage(model, sharedDocumentationRootDirectory);

                            DocumentationDatabaseManager.SaveObject(
                                sqliteDatabasePath,
                                project.PackageId,
                                project.Version,
                                namespaceName,
                                interfaceSymbol.Name,
                                "Interface",
                                model,
                                htmlContent,
                                DocumentationInterfaceTechnicalKeywordExtractor.ExtractKeywordsAsString(model),
                                DocumentationKeywordExtractor.ExtractKeywordsAsString(htmlContent),
                                $"/Documentation/Show?packageId={project.PackageId}&version={project.Version}&groupName={group.GroupName}&namespaceName={namespaceName}&objectName={interfaceSymbol.Name}");

                            var (sourceCode, fileCount) = DocumentationVisualHelper.ExtractFullSource(interfaceSymbol);
                            DocumentationDatabaseManager.SaveObjectSource(
                                sqliteDatabasePath,
                                project.PackageId,
                                project.Version,
                                namespaceName,
                                interfaceSymbol.Name,
                                "Interface",
                                sourceCode,
                                fileCount);

                            pages.Add(model);
                        }
                    }
                }
            }

            // DocumentationInterfacePageRenderer.RenderPages(
            //     pages
            //         .OrderBy(x => x.NamespaceName, StringComparer.Ordinal)
            //         .ThenBy(x => x.InterfaceName, StringComparer.Ordinal)
            //         .ToList(),
            //     pageOutputDirectory,
            //     sharedDocumentationRootDirectory);
        }

        private static List<DocumentationExtensionMethodItem> GetExtensionMethods(
            Compilation globalCompilation,
            INamedTypeSymbol interfaceSymbol
        )
        {
            List<DocumentationExtensionMethodItem> result = [];
            string interfaceFullName = interfaceSymbol.ToDisplayString();

            foreach (SyntaxTree syntaxTree in globalCompilation.SyntaxTrees)
            {
                SemanticModel semanticModel = globalCompilation.GetSemanticModel(syntaxTree);

                IEnumerable<MethodDeclarationSyntax> methodDeclarations = syntaxTree.GetRoot()
                    .DescendantNodes()
                    .OfType<MethodDeclarationSyntax>();

                foreach (MethodDeclarationSyntax methodDeclaration in methodDeclarations)
                {
                    if (semanticModel.GetDeclaredSymbol(methodDeclaration) is not IMethodSymbol methodSymbol) continue;

                    if (!methodSymbol.IsExtensionMethod) continue;

                    if (methodSymbol.Parameters.Length == 0) continue;

                    ITypeSymbol receiverType = methodSymbol.Parameters[0].Type;

                    bool match = string.Equals(
                        receiverType.ToDisplayString(),
                        interfaceFullName,
                        StringComparison.Ordinal);

                    if (!match && receiverType is ITypeParameterSymbol typeParameter)
                    {
                        foreach (ITypeSymbol constraintType in typeParameter.ConstraintTypes)
                        {
                            if (string.Equals(constraintType.ToDisplayString(), interfaceFullName, StringComparison.Ordinal))
                            {
                                match = true;
                                break;
                            }
                        }
                    }

                    if (!match) continue;

                    result.Add(new DocumentationExtensionMethodItem
                    {
                        Accessibility = methodSymbol.DeclaredAccessibility switch
                        {
                            Accessibility.Public => "public",
                            Accessibility.Internal => "internal",
                            Accessibility.Private => "private",
                            Accessibility.Protected => "protected",
                            Accessibility.ProtectedAndInternal => "private protected",
                            Accessibility.ProtectedOrInternal => "protected internal",
                            _ => string.Empty
                        },
                        MethodName = methodSymbol.Name,
                        Signature = BuildExtensionMethodSignature(methodSymbol),
                        ExtensionTypeName = methodSymbol.ContainingType.Name,
                        ExtensionNamespaceName = methodSymbol.ContainingNamespace?.ToDisplayString() ?? string.Empty
                    });
                }
            }

            return result
                .GroupBy(
                    x => $"{x.ExtensionNamespaceName}::{x.ExtensionTypeName}::{x.Signature}",
                    StringComparer.Ordinal)
                .Select(g => g.First())
                .OrderBy(x => x.ExtensionTypeName, StringComparer.Ordinal)
                .ThenBy(x => x.MethodName, StringComparer.Ordinal)
                .ToList();
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
