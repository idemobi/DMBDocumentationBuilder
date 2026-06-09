#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Globalization;
using System.Text.Json;
using DMBDocumentationBuilder.Shared;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents the DocumentationClassPageManager type used by DocumentationBuilder generation.
    /// </summary>
    public static class DocumentationClassPageManager
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

        private static string BuildClassDeclaration(INamedTypeSymbol classSymbol)
        {
            string accessibility = BuildAccessibility(classSymbol.DeclaredAccessibility);

            List<string> modifiers = [];
            if (!string.IsNullOrWhiteSpace(accessibility)) modifiers.Add(accessibility);

            if (classSymbol.IsStatic)
                modifiers.Add("static");
            else if (classSymbol.IsAbstract)
                modifiers.Add("abstract");
            else if (classSymbol.IsSealed) modifiers.Add("sealed");

            modifiers.Add("class");

            string typeParameters = classSymbol.IsGenericType
                ? $"<{string.Join(", ", classSymbol.TypeParameters.Select(tp => tp.Name))}>"
                : string.Empty;

            List<string> inheritanceItems = [];

            if (classSymbol.BaseType is not null &&
                classSymbol.BaseType.SpecialType != SpecialType.System_Object)
            {
                inheritanceItems.Add(classSymbol.BaseType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            }

            inheritanceItems.AddRange(
                classSymbol.Interfaces.Select(x => x.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));

            string baseTypes = inheritanceItems.Count > 0
                ? " : " + string.Join(", ", inheritanceItems.Distinct(StringComparer.Ordinal))
                : string.Empty;

            return $"{string.Join(" ", modifiers)} {classSymbol.Name}{typeParameters}{baseTypes}";
        }

        private static string BuildConstructorSignature(IMethodSymbol constructorSymbol)
        {
            string parameters = string.Join(", ",
                constructorSymbol.Parameters.Select(p => FormatParameter(p, includeThisModifier: false)));

            string accessibility = BuildAccessibility(constructorSymbol.DeclaredAccessibility);

            return $"{accessibility} {constructorSymbol.ContainingType.Name}({parameters})".Trim();
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

        private static string BuildMemberKey(string memberKind, string signature)
        {
            return $"{memberKind}:{signature}";
        }

        private static List<DocumentationMemberDatabaseItem> BuildMemberRows(DocumentationClassPageModel model)
        {
            List<DocumentationMemberDatabaseItem> members = [];
            int sortOrder = 0;

            foreach (DocumentationClassConstructorItem constructor in model.Constructors)
            {
                members.Add(new DocumentationMemberDatabaseItem
                {
                    MemberKind = "Constructor",
                    MemberKey = BuildMemberKey("Constructor", constructor.Signature),
                    MemberName = constructor.ConstructorName,
                    Signature = constructor.Signature,
                    SummaryHtml = constructor.SummaryHtml,
                    RemarksHtml = constructor.RemarksHtml,
                    ExampleHtml = constructor.ExampleHtml,
                    Accessibility = constructor.Accessibility,
                    IsObsolete = constructor.IsObsolete,
                    ObsoleteMessage = constructor.ObsoleteMessage,
                    ParametersJson = JsonSerializer.Serialize(constructor.Parameters),
                    ExceptionsJson = JsonSerializer.Serialize(constructor.Exceptions),
                    SortOrder = sortOrder++
                });
            }

            foreach (DocumentationClassFieldItem field in model.Fields)
            {
                members.Add(new DocumentationMemberDatabaseItem
                {
                    MemberKind = "Field",
                    MemberKey = BuildMemberKey("Field", field.Signature),
                    MemberName = field.FieldName,
                    Signature = field.Signature,
                    SummaryHtml = field.SummaryHtml,
                    RemarksHtml = field.RemarksHtml,
                    ExampleHtml = field.ExampleHtml,
                    Accessibility = field.Accessibility,
                    IsStatic = field.IsStatic,
                    IsConst = field.IsConst,
                    IsReadOnly = field.IsReadOnly,
                    IsObsolete = field.IsObsolete,
                    ObsoleteMessage = field.ObsoleteMessage,
                    SortOrder = sortOrder++
                });
            }

            foreach (DocumentationClassPropertyItem property in model.Properties)
            {
                members.Add(new DocumentationMemberDatabaseItem
                {
                    MemberKind = "Property",
                    MemberKey = BuildMemberKey("Property", property.Signature),
                    MemberName = property.PropertyName,
                    Signature = property.Signature,
                    SummaryHtml = property.SummaryHtml,
                    RemarksHtml = property.RemarksHtml,
                    ValueHtml = property.ValueHtml,
                    ExampleHtml = property.ExampleHtml,
                    Accessibility = property.Accessibility,
                    IsStatic = property.IsStatic,
                    IsVirtual = property.IsVirtual,
                    IsOverride = property.IsOverride,
                    IsSealed = property.IsSealed,
                    IsReadOnly = property.IsReadOnly,
                    IsObsolete = property.IsObsolete,
                    ObsoleteMessage = property.ObsoleteMessage,
                    SortOrder = sortOrder++
                });
            }

            foreach (DocumentationClassMethodItem method in model.Methods)
            {
                members.Add(new DocumentationMemberDatabaseItem
                {
                    MemberKind = "Method",
                    MemberKey = BuildMemberKey("Method", method.Signature),
                    MemberName = method.MethodName,
                    Signature = method.Signature,
                    SummaryHtml = method.SummaryHtml,
                    RemarksHtml = method.RemarksHtml,
                    ReturnsHtml = method.ReturnsHtml,
                    ExampleHtml = method.ExampleHtml,
                    Accessibility = method.Accessibility,
                    IsStatic = method.IsStatic,
                    IsAbstract = method.IsAbstract,
                    IsVirtual = method.IsVirtual,
                    IsOverride = method.IsOverride,
                    IsSealed = method.IsSealed,
                    IsObsolete = method.IsObsolete,
                    ObsoleteMessage = method.ObsoleteMessage,
                    ParametersJson = JsonSerializer.Serialize(method.Parameters),
                    ExceptionsJson = JsonSerializer.Serialize(method.Exceptions),
                    SortOrder = sortOrder++
                });
            }

            foreach (DocumentationExtensionMethodItem extensionMethod in model.ExtensionMethods)
            {
                members.Add(new DocumentationMemberDatabaseItem
                {
                    MemberKind = "ExtensionMethod",
                    MemberKey = BuildMemberKey("ExtensionMethod", $"{extensionMethod.ExtensionNamespaceName}.{extensionMethod.ExtensionTypeName}.{extensionMethod.Signature}"),
                    MemberName = extensionMethod.MethodName,
                    Signature = extensionMethod.Signature,
                    Accessibility = extensionMethod.Accessibility,
                    IsStatic = true,
                    ExtensionNamespaceName = extensionMethod.ExtensionNamespaceName,
                    ExtensionTypeName = extensionMethod.ExtensionTypeName,
                    SortOrder = sortOrder++
                });
            }

            return members;
        }

        private static DocumentationTypeLinkItem? BuildTypeLinkItem(
            INamedTypeSymbol? typeSymbol,
            IReadOnlyDictionary<string, DocumentationTypeRegistryItem> documentedTypeIndex
        )
        {
            if (typeSymbol is null) return null;

            if (typeSymbol.SpecialType == SpecialType.System_Object) return null;

            if (string.IsNullOrWhiteSpace(typeSymbol.ContainingNamespace?.ToDisplayString())) return null;

            INamedTypeSymbol targetType = typeSymbol.OriginalDefinition;
            bool isDocumented = DocumentationTypeRegistry.TryGetDocumentedType(targetType, documentedTypeIndex, out DocumentationTypeRegistryItem? registryItem);

            return new DocumentationTypeLinkItem
            {
                DisplayName = typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                NamespaceName = targetType.ContainingNamespace?.ToDisplayString() ?? string.Empty,
                ObjectName = targetType.Name,
                GroupName = registryItem?.GroupName ?? string.Empty,
                PackageId = registryItem?.PackageId ?? string.Empty,
                Version = registryItem?.Version ?? string.Empty,
                IsDocumented = isDocumented
            };
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
            Console.WriteLine("Generating class documentation pages...");
            if (groups is null) throw new ArgumentNullException(nameof(groups));

            List<DocumentationProjectDescriptor> projectList = groups
                .SelectMany(group => group.Projects)
                .GroupBy(project => project.ProjectFilePath, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .ToList();

            if (projectList.Count == 0) throw new InvalidOperationException("At least one project must be provided.");

            CSharpCompilation globalCompilation = BuildGlobalCompilation(projectList);
            Dictionary<string, DocumentationTypeRegistryItem> documentedTypeIndex = DocumentationTypeRegistry.BuildDocumentedTypeIndex(globalCompilation, groups);
            List<DocumentationClassPageModel> pages = [];

            foreach (DocumentationGroupDescriptor group in groups)
            {
                foreach (DocumentationProjectDescriptor project in group.Projects)
                {
                    Console.WriteLine($"Generating class documentation pages for project: {project.DisplayName}");
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

                        IEnumerable<ClassDeclarationSyntax> classDeclarations = syntaxTree.GetRoot()
                            .DescendantNodes()
                            .OfType<ClassDeclarationSyntax>();

                        foreach (ClassDeclarationSyntax classDeclaration in classDeclarations)
                        {
                            if (semanticModel.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol classSymbol) continue;

                            if (classSymbol.TypeKind != TypeKind.Class) continue;

                            string namespaceName = classSymbol.ContainingNamespace?.ToDisplayString() ?? string.Empty;
                            if (string.IsNullOrWhiteSpace(namespaceName)) continue;

                            DocumentationXmlModel classXmlDoc = DocumentationXmlCommentRenderer.Extract(
                                project.PackageId,
                                project.Version,
                                group.GroupName,
                                classSymbol,
                                localCompilation,
                                classSymbol);

                            DocumentationAttributeHelper.IsObsolete(classSymbol, out string classObsoleteMessage);
                            DocumentationClassPageModel model = new()
                            {
                                PackageId = project.PackageId,
                                Version = project.Version,
                                GroupName = group.GroupName,
                                Accessibility = BuildAccessibility(classSymbol.DeclaredAccessibility),
                                NamespaceName = namespaceName,
                                ClassName = classSymbol.Name,
                                AssemblyName = project.DisplayName + ".dll",
                                SummaryHtml = classXmlDoc.SummaryHtml,
                                RemarksHtml = classXmlDoc.RemarksHtml,
                                ExampleHtml = classXmlDoc.ExampleHtml,
                                Declaration = BuildClassDeclaration(classSymbol),
                                BaseType = BuildTypeLinkItem(classSymbol.BaseType, documentedTypeIndex),
                                IsStatic = classSymbol.IsStatic,
                                IsAbstract = classSymbol.IsAbstract && !classSymbol.IsStatic,
                                IsSealed = classSymbol.IsSealed && !classSymbol.IsStatic,
                                IsObsolete = DocumentationAttributeHelper.IsObsolete(classSymbol, out _),
                                ObsoleteMessage = classObsoleteMessage
                            };

                            model.SeeAlsos.AddRange((IEnumerable<DocumentationXmlLinkItem>)classXmlDoc.SeeAlsos);

                            foreach (INamedTypeSymbol implementedInterface in classSymbol.Interfaces
                                         .OrderBy(x => x.ToDisplayString(), StringComparer.Ordinal))
                            {
                                DocumentationTypeLinkItem? linkItem = BuildTypeLinkItem(implementedInterface, documentedTypeIndex);
                                if (linkItem is not null) model.ImplementedInterfaces.Add(linkItem);
                            }

                            foreach (IMethodSymbol constructor in classSymbol.InstanceConstructors
                                         .Where(x => !x.IsImplicitlyDeclared)
                                         .OrderBy(x => x.Parameters.Length))
                            {
                                DocumentationAttributeHelper.IsObsolete(constructor, out string constructorObsoleteMessage);
                                DocumentationXmlModel xmlDoc = DocumentationXmlCommentRenderer.Extract(
                                    project.PackageId,
                                    project.Version,
                                    group.GroupName,
                                    constructor,
                                    localCompilation,
                                    classSymbol);

                                DocumentationClassConstructorItem constructorItem = new()
                                {
                                    Accessibility = BuildAccessibility(constructor.DeclaredAccessibility),
                                    ConstructorName = constructor.Name,
                                    Signature = BuildConstructorSignature(constructor),
                                    SummaryHtml = xmlDoc.SummaryHtml,
                                    RemarksHtml = xmlDoc.RemarksHtml,
                                    ExampleHtml = xmlDoc.ExampleHtml,
                                    IsObsolete = DocumentationAttributeHelper.IsObsolete(constructor, out _),
                                    ObsoleteMessage = constructorObsoleteMessage
                                };

                                constructorItem.Parameters.AddRange(xmlDoc.Parameters);
                                constructorItem.Exceptions.AddRange(xmlDoc.Exceptions);

                                model.Constructors.Add(constructorItem);
                            }

                            foreach (IFieldSymbol field in classSymbol.GetMembers().OfType<IFieldSymbol>())
                            {
                                if (field.IsImplicitlyDeclared) continue;

                                if (field.AssociatedSymbol is not null) continue;

                                DocumentationAttributeHelper.IsObsolete(field, out string fieldObsoleteMessage);
                                DocumentationXmlModel xmlDoc = DocumentationXmlCommentRenderer.Extract(
                                    project.PackageId,
                                    project.Version,
                                    group.GroupName,
                                    field,
                                    localCompilation,
                                    classSymbol);

                                model.Fields.Add(new DocumentationClassFieldItem
                                {
                                    Accessibility = BuildAccessibility(field.DeclaredAccessibility),
                                    FieldName = field.Name,
                                    Signature = DocumentationMemberSignatureFormatter.FormatField(field),
                                    SummaryHtml = xmlDoc.SummaryHtml,
                                    RemarksHtml = xmlDoc.RemarksHtml,
                                    ExampleHtml = xmlDoc.ExampleHtml,
                                    IsStatic = field.IsStatic,
                                    IsConst = field.IsConst,
                                    IsReadOnly = field.IsReadOnly,
                                    IsObsolete = DocumentationAttributeHelper.IsObsolete(field, out _),
                                    ObsoleteMessage = fieldObsoleteMessage
                                });
                            }

                            foreach (IPropertySymbol property in classSymbol.GetMembers().OfType<IPropertySymbol>())
                            {
                                DocumentationAttributeHelper.IsObsolete(property, out string propertyObsoleteMessage);
                                DocumentationXmlModel xmlDoc = DocumentationXmlCommentRenderer.Extract(
                                    project.PackageId,
                                    project.Version,
                                    group.GroupName,
                                    property,
                                    localCompilation,
                                    classSymbol);

                                model.Properties.Add(new DocumentationClassPropertyItem
                                {
                                    Accessibility = BuildAccessibility(property.DeclaredAccessibility),
                                    PropertyName = property.Name,
                                    Signature = DocumentationMemberSignatureFormatter.FormatProperty(property),
                                    SummaryHtml = xmlDoc.SummaryHtml,
                                    RemarksHtml = xmlDoc.RemarksHtml,
                                    ValueHtml = xmlDoc.ValueHtml,
                                    ExampleHtml = xmlDoc.ExampleHtml,
                                    IsStatic = property.IsStatic,
                                    IsVirtual = property.IsVirtual,
                                    IsOverride = property.IsOverride,
                                    IsSealed = property.IsSealed,
                                    IsReadOnly = property.SetMethod is null,
                                    IsObsolete = DocumentationAttributeHelper.IsObsolete(property, out _),
                                    ObsoleteMessage = propertyObsoleteMessage
                                });
                            }

                            foreach (IMethodSymbol method in classSymbol.GetMembers().OfType<IMethodSymbol>())
                            {
                                if (method.MethodKind != MethodKind.Ordinary) continue;

                                DocumentationAttributeHelper.IsObsolete(method, out string methodObsoleteMessage);
                                DocumentationXmlModel xmlDoc = DocumentationXmlCommentRenderer.Extract(
                                    project.PackageId,
                                    project.Version,
                                    group.GroupName,
                                    method,
                                    localCompilation,
                                    classSymbol);

                                DocumentationClassMethodItem methodItem = new()
                                {
                                    Accessibility = BuildAccessibility(method.DeclaredAccessibility),
                                    MethodName = method.Name,
                                    Signature = DocumentationMemberSignatureFormatter.FormatMethod(method),
                                    SummaryHtml = xmlDoc.SummaryHtml,
                                    RemarksHtml = xmlDoc.RemarksHtml,
                                    ReturnsHtml = xmlDoc.ReturnsHtml,
                                    ExampleHtml = xmlDoc.ExampleHtml,
                                    IsStatic = method.IsStatic,
                                    IsAbstract = method.IsAbstract,
                                    IsVirtual = method.IsVirtual,
                                    IsOverride = method.IsOverride,
                                    IsSealed = method.IsSealed,
                                    IsObsolete = DocumentationAttributeHelper.IsObsolete(method, out _),
                                    ObsoleteMessage = methodObsoleteMessage
                                };

                                methodItem.Parameters.AddRange((IEnumerable<DocumentationXmlNamedItem>)xmlDoc.Parameters);
                                methodItem.Exceptions.AddRange((IEnumerable<DocumentationXmlNamedItem>)xmlDoc.Exceptions);

                                model.Methods.Add(methodItem);
                            }

                            model.ExtensionMethods.AddRange(GetExtensionMethods(globalCompilation, classSymbol));

                            string htmlContent = DocumentationClassPageRenderer.RenderHtmlPage(model, sharedDocumentationRootDirectory);

                            DocumentationDatabaseManager.SaveObject(
                                sqliteDatabasePath,
                                project.PackageId,
                                project.Version,
                                namespaceName,
                                classSymbol.Name,
                                "Class",
                                model,
                                htmlContent,
                                DocumentationClassTechnicalKeywordExtractor.ExtractKeywordsAsString(model),
                                DocumentationKeywordExtractor.ExtractKeywordsAsString(htmlContent),
                                $"/Documentation/Show?packageId={project.PackageId}&version={project.Version}&groupName={group.GroupName}&namespaceName={namespaceName}&objectName={classSymbol.Name}");

                            var (sourceCode, fileCount) = DocumentationVisualHelper.ExtractFullSource(classSymbol);
                            DocumentationDatabaseManager.SaveObjectSource(
                                sqliteDatabasePath,
                                project.PackageId,
                                project.Version,
                                namespaceName,
                                classSymbol.Name,
                                "Class",
                                sourceCode,
                                fileCount);

                            DocumentationDatabaseManager.ReplaceObjectMembers(
                                sqliteDatabasePath,
                                project.PackageId,
                                project.Version,
                                group.GroupName,
                                namespaceName,
                                classSymbol.Name,
                                "Class",
                                BuildMemberRows(model));

                            pages.Add(model);
                        }
                    }
                }
            }

            // DocumentationClassPageRenderer.RenderPages(
            //     pages
            //         .OrderBy(x => x.NamespaceName, StringComparer.Ordinal)
            //         .ThenBy(x => x.ClassName, StringComparer.Ordinal)
            //         .ToList(),
            //     pageOutputDirectory,
            //     sharedDocumentationRootDirectory);
        }

        private static List<DocumentationExtensionMethodItem> GetExtensionMethods(
            Compilation globalCompilation,
            INamedTypeSymbol classSymbol
        )
        {
            List<DocumentationExtensionMethodItem> result = [];
            string classFullName = classSymbol.ToDisplayString();

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
                        classFullName,
                        StringComparison.Ordinal);

                    if (!match && receiverType is ITypeParameterSymbol typeParameter)
                    {
                        foreach (ITypeSymbol constraintType in typeParameter.ConstraintTypes)
                        {
                            if (string.Equals(constraintType.ToDisplayString(), classFullName, StringComparison.Ordinal))
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