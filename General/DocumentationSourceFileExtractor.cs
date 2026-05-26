#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationSourceFileExtractor.cs create at 2026/05/21 00:00:00
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using System.Security.Cryptography;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Extracts C# source file snapshots for DocumentationViewer MCP source-code tools.
    /// </summary>
    public static class DocumentationSourceFileExtractor
    {
        #region Static methods

        /// <summary>
        /// Extracts source file metadata and content for one documented project.
        /// </summary>
        /// <param name="project">The documented project whose C# compile items should be captured.</param>
        /// <returns>The source file snapshots ordered by relative file path.</returns>
        public static IReadOnlyList<DocumentationSourceFileItem> Extract(DocumentationProjectDescriptor project)
        {
            if (project is null) throw new ArgumentNullException(nameof(project));
            if (!File.Exists(project.ProjectFilePath)) throw new FileNotFoundException("Project file not found.", project.ProjectFilePath);

            string projectFilePath = Path.GetFullPath(project.ProjectFilePath);
            string projectDirectoryPath = Path.GetDirectoryName(projectFilePath)
                                          ?? throw new InvalidOperationException("Unable to resolve project directory.");

            List<DocumentationSourceFileItem> items = [];

            foreach (string filePath in DocumentationExtractionManager.GetProjectSourceFiles(project))
            {
                string fullPath = Path.GetFullPath(filePath);
                string content = File.ReadAllText(fullPath);
                SyntaxNode root = CSharpSyntaxTree.ParseText(content, path: fullPath).GetRoot();
                IReadOnlyList<string> namespaces = ExtractNamespaceNames(root);
                IReadOnlyList<string> typeNames = ExtractTypeNames(root);

                items.Add(new DocumentationSourceFileItem
                {
                    PackageId = project.PackageId,
                    Version = project.Version,
                    ProjectFilePath = projectFilePath,
                    ProjectDirectoryPath = projectDirectoryPath,
                    FilePath = fullPath,
                    RelativeFilePath = NormalizeRelativePath(Path.GetRelativePath(projectDirectoryPath, fullPath)),
                    FileName = Path.GetFileName(fullPath),
                    PrimaryNamespaceName = namespaces.FirstOrDefault() ?? string.Empty,
                    NamespaceNames = namespaces,
                    TypeNames = typeNames,
                    Content = content,
                    ContentHash = ComputeSha256(content)
                });
            }

            return items
                .OrderBy(item => item.RelativeFilePath, StringComparer.Ordinal)
                .ToArray();
        }

        private static string ComputeSha256(string content)
        {
            byte[] hash = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(content));
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        private static IReadOnlyList<string> ExtractNamespaceNames(SyntaxNode root)
        {
            return root.DescendantNodes()
                .OfType<BaseNamespaceDeclarationSyntax>()
                .Select(declaration => declaration.Name.ToString())
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.Ordinal)
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();
        }

        private static IReadOnlyList<string> ExtractTypeNames(SyntaxNode root)
        {
            return root.DescendantNodes()
                .OfType<BaseTypeDeclarationSyntax>()
                .Select(declaration => declaration.Identifier.ValueText)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.Ordinal)
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();
        }

        private static string NormalizeRelativePath(string relativeFilePath)
        {
            return relativeFilePath.Replace('\\', '/');
        }

        #endregion
    }
}
