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
    ///     Represents the DocumentationStructPageManager type used by DocumentationBuilder generation.
    /// </summary>
    public static class DocumentationStructPageManager
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

        private static string BuildDeclaration(INamedTypeSymbol structSymbol)
        {
            string accessibility = BuildAccessibility(structSymbol.DeclaredAccessibility);

            List<string> modifiers = [];
            if (!string.IsNullOrWhiteSpace(accessibility)) modifiers.Add(accessibility);

            if (structSymbol.IsReadOnly) modifiers.Add("readonly");

            if (structSymbol.IsRefLikeType) modifiers.Add("ref");

            modifiers.Add("struct");

            string typeParameters = structSymbol.IsGenericType
                ? $"<{string.Join(", ", structSymbol.TypeParameters.Select(tp => tp.Name))}>"
                : string.Empty;

            string implementedInterfaces = string.Empty;

            if (structSymbol.Interfaces.Length > 0)
            {
                implementedInterfaces = " : " + string.Join(", ",
                    structSymbol.Interfaces.Select(x => x.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
            }

            return $"{string.Join(" ", modifiers)} {structSymbol.Name}{typeParameters}{implementedInterfaces}";
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

        private static CSharpCompilation BuildGlobalCompilation(IEnumerable<DocumentationProjectDescriptor> projects)
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
            Console.WriteLine("Generating struct documentation pages...");
            if (groups is null) throw new ArgumentNullException(nameof(groups));

            List<DocumentationProjectDescriptor> projectList = groups
                .SelectMany(group => group.Projects)
                .GroupBy(project => project.ProjectFilePath, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .ToList();

            if (projectList.Count == 0) throw new InvalidOperationException("At least one project must be provided.");

            CSharpCompilation globalCompilation = BuildGlobalCompilation(projectList);
            Dictionary<string, DocumentationTypeRegistryItem> documentedTypeIndex = DocumentationTypeRegistry.BuildDocumentedTypeIndex(globalCompilation, groups);

            List<DocumentationStructPageModel> pages = [];

            foreach (DocumentationGroupDescriptor group in groups)
            {
                foreach (DocumentationProjectDescriptor project in group.Projects)
                {
                    Console.WriteLine($"Generating struct documentation pages for project: {project.DisplayName}");
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

                        IEnumerable<StructDeclarationSyntax> structDeclarations = syntaxTree.GetRoot()
                            .DescendantNodes()
                            .OfType<StructDeclarationSyntax>();

                        foreach (StructDeclarationSyntax structDeclaration in structDeclarations)
                        {
                            if (semanticModel.GetDeclaredSymbol(structDeclaration) is not INamedTypeSymbol structSymbol) continue;

                            string namespaceName = structSymbol.ContainingNamespace?.ToDisplayString() ?? string.Empty;
                            if (string.IsNullOrWhiteSpace(namespaceName)) continue;

                            DocumentationXmlModel structXmlDoc = DocumentationXmlCommentRenderer.Extract(
                                project.PackageId,
                                project.Version,
                                group.GroupName,
                                structSymbol,
                                localCompilation,
                                structSymbol);

                            DocumentationAttributeHelper.IsObsolete(structSymbol, out string structObsoleteMessage);
                            DocumentationStructPageModel model = new()
                            {
                                PackageId = project.PackageId,
                                Version = project.Version,
                                GroupName = group.GroupName,
                                Accessibility = BuildAccessibility(structSymbol.DeclaredAccessibility),
                                NamespaceName = namespaceName,
                                StructName = structSymbol.Name,
                                AssemblyName = project.DisplayName + ".dll",
                                XmlDoc = structXmlDoc,
                                Declaration = BuildDeclaration(structSymbol),
                                IsReadOnly = structSymbol.IsReadOnly,
                                IsRefLike = structSymbol.IsRefLikeType,
                                IsObsolete = DocumentationAttributeHelper.IsObsolete(structSymbol, out _),
                                ObsoleteMessage = structObsoleteMessage
                            };

                            model.DependencyEdges.AddRange(DocumentationDependencyGraphExtractor.BuildDependencyEdges(
                                structSymbol,
                                project,
                                group,
                                documentedTypeIndex));

                            foreach (INamedTypeSymbol implementedInterface in structSymbol.Interfaces
                                         .OrderBy(x => x.ToDisplayString(), StringComparer.Ordinal))
                            {
                                model.ImplementedInterfaces.Add(
                                    implementedInterface.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
                            }

                            foreach (IFieldSymbol field in structSymbol.GetMembers().OfType<IFieldSymbol>())
                            {
                                if (field.IsImplicitlyDeclared) continue;

                                if (field.AssociatedSymbol is not null) continue;

                                DocumentationAttributeHelper.IsObsolete(field, out string fieldObsoleteMessage);
                                DocumentationXmlModel fieldXmlDoc = DocumentationXmlCommentRenderer.Extract(
                                    project.PackageId,
                                    project.Version,
                                    group.GroupName,
                                    field,
                                    localCompilation,
                                    structSymbol);

                                model.Fields.Add(new DocumentationStructFieldItem
                                {
                                    Accessibility = BuildAccessibility(field.DeclaredAccessibility),
                                    FieldName = field.Name,
                                    Signature = DocumentationMemberSignatureFormatter.FormatField(field),
                                    XmlDoc = fieldXmlDoc,
                                    IsStatic = field.IsStatic,
                                    IsConst = field.IsConst,
                                    IsReadOnly = field.IsReadOnly,
                                    IsObsolete = DocumentationAttributeHelper.IsObsolete(field, out _),
                                    ObsoleteMessage = fieldObsoleteMessage
                                });
                            }

                            foreach (IPropertySymbol property in structSymbol.GetMembers().OfType<IPropertySymbol>())
                            {
                                DocumentationAttributeHelper.IsObsolete(property, out string propertyObsoleteMessage);
                                DocumentationXmlModel propertyXmlDoc = DocumentationXmlCommentRenderer.Extract(
                                    project.PackageId,
                                    project.Version,
                                    group.GroupName,
                                    property,
                                    localCompilation,
                                    structSymbol);

                                model.Properties.Add(new DocumentationStructPropertyItem
                                {
                                    Accessibility = BuildAccessibility(property.DeclaredAccessibility),
                                    PropertyName = property.Name,
                                    Signature = DocumentationMemberSignatureFormatter.FormatProperty(property),
                                    XmlDoc = propertyXmlDoc,
                                    IsStatic = property.IsStatic,
                                    IsVirtual = property.IsVirtual,
                                    IsOverride = property.IsOverride,
                                    IsSealed = property.IsSealed,
                                    IsReadOnly = property.SetMethod is null,
                                    IsObsolete = DocumentationAttributeHelper.IsObsolete(property, out _),
                                    ObsoleteMessage = propertyObsoleteMessage
                                });
                            }

                            foreach (IMethodSymbol method in structSymbol.GetMembers().OfType<IMethodSymbol>())
                            {
                                if (method.MethodKind != MethodKind.Ordinary) continue;

                                DocumentationAttributeHelper.IsObsolete(method, out string methodObsoleteMessage);
                                DocumentationXmlModel methodXmlDoc = DocumentationXmlCommentRenderer.Extract(
                                    project.PackageId,
                                    project.Version,
                                    group.GroupName,
                                    method,
                                    localCompilation,
                                    structSymbol);

                                model.Methods.Add(new DocumentationStructMethodItem
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

                            model.ExtensionMethods.AddRange(GetExtensionMethods(globalCompilation, structSymbol));

                            string htmlContent = DocumentationStructPageRenderer.RenderHtmlPage(model, sharedDocumentationRootDirectory);

                            DocumentationDatabaseManager.SaveObject(
                                sqliteDatabasePath,
                                project.PackageId,
                                project.Version,
                                namespaceName,
                                structSymbol.Name,
                                "Struct",
                                model,
                                htmlContent,
                                DocumentationStructTechnicalKeywordExtractor.ExtractKeywordsAsString(model),
                                DocumentationKeywordExtractor.ExtractKeywordsAsString(htmlContent),
                                $"/Documentation/Show?packageId={project.PackageId}&version={project.Version}&groupName={group.GroupName}&namespaceName={namespaceName}&objectName={structSymbol.Name}");

                            var (sourceCode, fileCount) = DocumentationVisualHelper.ExtractFullSource(structSymbol);
                            DocumentationDatabaseManager.SaveObjectSource(
                                sqliteDatabasePath,
                                project.PackageId,
                                project.Version,
                                namespaceName,
                                structSymbol.Name,
                                "Struct",
                                sourceCode,
                                fileCount);

                            pages.Add(model);
                        }
                    }
                }
            }

            // DocumentationStructPageRenderer.RenderPages(
            //     pages
            //         .OrderBy(x => x.NamespaceName, StringComparer.Ordinal)
            //         .ThenBy(x => x.StructName, StringComparer.Ordinal)
            //         .ToList(),
            //     pageOutputDirectory,
            //     sharedDocumentationRootDirectory);
        }

        private static List<DocumentationExtensionMethodItem> GetExtensionMethods(
            Compilation globalCompilation,
            INamedTypeSymbol structSymbol
        )
        {
            List<DocumentationExtensionMethodItem> result = [];
            string structFullName = structSymbol.ToDisplayString();

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
                        structFullName,
                        StringComparison.Ordinal);

                    if (!match && receiverType is ITypeParameterSymbol typeParameter)
                    {
                        foreach (ITypeSymbol constraintType in typeParameter.ConstraintTypes)
                        {
                            if (string.Equals(constraintType.ToDisplayString(), structFullName, StringComparison.Ordinal))
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
