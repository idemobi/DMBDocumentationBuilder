#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Globalization;
using System.Numerics;
using DMBDocumentationBuilder.Shared;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents the DocumentationEnumPageManager type used by DocumentationBuilder generation.
    /// </summary>
    public static class DocumentationEnumPageManager
    {
        #region Static methods

        private static string BuildAccessibility(INamedTypeSymbol enumSymbol)
        {
            return BuildAccessibility(enumSymbol.DeclaredAccessibility);
        }

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
                _ => "public"
            };
        }

        private static string BuildEnumDeclaration(INamedTypeSymbol enumSymbol)
        {
            string accessibility = BuildAccessibility(enumSymbol);
            string underlyingType = BuildUnderlyingType(enumSymbol);

            if (!string.IsNullOrWhiteSpace(underlyingType) &&
                !string.Equals(underlyingType, "int", StringComparison.Ordinal))
            {
                return $"{accessibility} enum {enumSymbol.Name} : {underlyingType}";
            }

            return $"{accessibility} enum {enumSymbol.Name}";
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

        private static string BuildUnderlyingType(INamedTypeSymbol enumSymbol)
        {
            INamedTypeSymbol? underlyingType = enumSymbol.EnumUnderlyingType;
            if (underlyingType is null) return "int";

            return underlyingType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        }

        private static long ConvertToInt64(object value)
        {
            return value switch
            {
                byte x => x,
                sbyte x => x,
                short x => x,
                ushort x => x,
                int x => x,
                uint x => x,
                long x => x,
                ulong x => unchecked((long)x),
                _ => Convert.ToInt64(value, CultureInfo.InvariantCulture)
            };
        }

        private static ulong ConvertToUInt64(object value)
        {
            return value switch
            {
                byte x => x,
                sbyte x => unchecked((ulong)x),
                short x => unchecked((ulong)x),
                ushort x => x,
                int x => unchecked((ulong)x),
                uint x => x,
                long x => unchecked((ulong)x),
                ulong x => x,
                _ => Convert.ToUInt64(value, CultureInfo.InvariantCulture)
            };
        }

        private static string FormatBinaryValue(object? value)
        {
            if (value is null) return string.Empty;

            ulong number = ConvertToUInt64(value);

            if (number == 0) return "0b0";

            return "0b" + Convert.ToString((long)number, 2);
        }

        private static string FormatBitShiftValue(object? value)
        {
            if (value is null) return string.Empty;

            ulong number = ConvertToUInt64(value);

            if (number == 0) return "0";

            if ((number & (number - 1)) != 0) return string.Empty;

            int shift = BitOperations.TrailingZeroCount(number);
            return $"1 << {shift}";
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

        private static string FormatEnumValue(object? value)
        {
            if (value is null) return string.Empty;

            return value switch
            {
                bool b => b ? "true" : "false",
                char c => $"'{c}'",
                string s => $"\"{s}\"",
                _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty
            };
        }

        private static string FormatHexValue(object? value)
        {
            if (value is null) return string.Empty;

            ulong number = ConvertToUInt64(value);
            return "0x" + number.ToString("X", CultureInfo.InvariantCulture);
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
            Console.WriteLine("Generating enum documentation pages...");
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

            List<DocumentationEnumPageModel> pages = [];

            foreach (DocumentationGroupDescriptor group in groups)
            {
                foreach (DocumentationProjectDescriptor project in group.Projects)
                {
                    Console.WriteLine($"Generating enum documentation pages for project: {project.DisplayName}");

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

                        IEnumerable<EnumDeclarationSyntax> enumDeclarations = syntaxTree.GetRoot()
                            .DescendantNodes()
                            .OfType<EnumDeclarationSyntax>();

                        foreach (EnumDeclarationSyntax enumDeclaration in enumDeclarations)
                        {
                            if (semanticModel.GetDeclaredSymbol(enumDeclaration) is not INamedTypeSymbol enumSymbol) continue;

                            string namespaceName = enumSymbol.ContainingNamespace?.ToDisplayString() ?? string.Empty;
                            if (string.IsNullOrWhiteSpace(namespaceName)) continue;

                            bool isFlags = enumSymbol
                                .GetAttributes()
                                .Any(attr => attr.AttributeClass?.ToDisplayString() == "System.FlagsAttribute" || attr.AttributeClass?.Name == "FlagsAttribute" || attr.AttributeClass?.Name == "Flags");

                            DocumentationAttributeHelper.IsObsolete(enumSymbol, out string enumObsoleteMessage);

                            DocumentationXmlModel enumXmlDoc = DocumentationXmlCommentRenderer.Extract(
                                project.PackageId,
                                project.Version,
                                group.GroupName,
                                enumSymbol,
                                localCompilation,
                                enumSymbol);

                            DocumentationEnumPageModel model = new()
                            {
                                PackageId = project.PackageId,
                                Version = project.Version,
                                GroupName = group.GroupName,
                                Accessibility = BuildAccessibility(enumSymbol),
                                NamespaceName = namespaceName,
                                EnumName = enumSymbol.Name,
                                AssemblyName = project.DisplayName + ".dll",
                                XmlDoc = enumXmlDoc,
                                IsFlags = isFlags,
                                IsObsolete = DocumentationAttributeHelper.IsObsolete(enumSymbol, out _),
                                ObsoleteMessage = enumObsoleteMessage,
                                UnderlyingType = BuildUnderlyingType(enumSymbol),
                                Declaration = BuildEnumDeclaration(enumSymbol)
                            };

                            model.DependencyEdges.AddRange(DocumentationDependencyGraphExtractor.BuildRelatedDependencyEdges(
                                enumSymbol,
                                dependencyEdges));

                            foreach (IFieldSymbol field in enumSymbol.GetMembers().OfType<IFieldSymbol>())
                            {
                                if (field.Name == "value__") continue;

                                if (!field.IsConst) continue;

                                DocumentationAttributeHelper.IsObsolete(field, out string fieldObsoleteMessage);

                                DocumentationXmlModel valueXmlDoc = DocumentationXmlCommentRenderer.Extract(
                                    project.PackageId,
                                    project.Version,
                                    group.GroupName,
                                    field,
                                    localCompilation,
                                    enumSymbol);

                                object? constantValue = field.ConstantValue;

                                model.Values.Add(new DocumentationEnumValueItem
                                {
                                    Name = field.Name,
                                    Value = FormatEnumValue(constantValue),
                                    NumericValue = constantValue is null ? 0 : ConvertToInt64(constantValue),
                                    HexValue = isFlags ? FormatHexValue(constantValue) : string.Empty,
                                    BinaryValue = isFlags ? FormatBinaryValue(constantValue) : string.Empty,
                                    BitShiftValue = isFlags ? FormatBitShiftValue(constantValue) : string.Empty,
                                    IsObsolete = DocumentationAttributeHelper.IsObsolete(field, out _),
                                    ObsoleteMessage = fieldObsoleteMessage,
                                    XmlDoc = valueXmlDoc
                                });
                            }

                            model.ExtensionMethods.AddRange(GetExtensionMethods(globalCompilation, enumSymbol));

                            string htmlContent = DocumentationEnumPageRenderer.RenderHtmlPage(model, sharedDocumentationRootDirectory);

                            DocumentationDatabaseManager.SaveObject(
                                sqliteDatabasePath,
                                project.PackageId,
                                project.Version,
                                namespaceName,
                                enumSymbol.Name,
                                "Enum",
                                model,
                                htmlContent,
                                DocumentationEnumTechnicalKeywordExtractor.ExtractKeywordsAsString(model),
                                DocumentationKeywordExtractor.ExtractKeywordsAsString(htmlContent),
                                $"/Documentation/Show?packageId={project.PackageId}&version={project.Version}&groupName={group.GroupName}&namespaceName={namespaceName}&objectName={enumSymbol.Name}");

                            var (sourceCode, fileCount) = DocumentationVisualHelper.ExtractFullSource(enumSymbol);
                            DocumentationDatabaseManager.SaveObjectSource(
                                sqliteDatabasePath,
                                project.PackageId,
                                project.Version,
                                namespaceName,
                                enumSymbol.Name,
                                "Enum",
                                sourceCode,
                                fileCount);

                            pages.Add(model);
                        }
                    }
                }
            }

            // DocumentationEnumPageRenderer.RenderPages(
            //     pages
            //         .OrderBy(x => x.NamespaceName, StringComparer.Ordinal)
            //         .ThenBy(x => x.EnumName, StringComparer.Ordinal)
            //         .ToList(),
            //     pageOutputDirectory,
            //     sharedDocumentationRootDirectory);
        }

        private static List<DocumentationExtensionMethodItem> GetExtensionMethods(
            Compilation globalCompilation,
            INamedTypeSymbol enumSymbol
        )
        {
            List<DocumentationExtensionMethodItem> result = [];
            string enumFullName = enumSymbol.ToDisplayString();

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

                    if (!string.Equals(receiverType.ToDisplayString(), enumFullName, StringComparison.Ordinal)) continue;

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
