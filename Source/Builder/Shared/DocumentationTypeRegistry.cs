#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;

#endregion

namespace DMBDocumentationBuilder
{
    internal static class DocumentationTypeRegistry
    {
        #region Static methods

        /// <summary>
        ///     Builds an index of documented types with their owning documentation project metadata.
        /// </summary>
        /// <param name="compilation">The compilation value used by the documentation generation operation.</param>
        /// <param name="groups">The documentation groups used to resolve target package and version metadata.</param>
        /// <returns>The documented type index keyed by stable documentation type key.</returns>
        public static Dictionary<string, DocumentationTypeRegistryItem> BuildDocumentedTypeIndex(
            Compilation compilation,
            IEnumerable<DocumentationGroupDescriptor> groups
        )
        {
            List<ProjectIndexItem> projects = groups
                .SelectMany(group => group.Projects.Select(project => new ProjectIndexItem
                {
                    GroupName = group.GroupName,
                    PackageId = project.PackageId,
                    Version = project.Version,
                    ProjectDirectory = Path.GetFullPath(Path.GetDirectoryName(project.ProjectFilePath) ?? string.Empty)
                }))
                .Where(project => !string.IsNullOrWhiteSpace(project.ProjectDirectory))
                .ToList();

            Dictionary<string, DocumentationTypeRegistryItem> result = new(StringComparer.Ordinal);

            void VisitNamespace(INamespaceSymbol ns)
            {
                foreach (INamespaceSymbol childNamespace in ns.GetNamespaceMembers()) VisitNamespace(childNamespace);

                foreach (INamedTypeSymbol type in ns.GetTypeMembers()) VisitType(type);
            }

            void VisitType(INamedTypeSymbol type)
            {
                string namespaceName = type.ContainingNamespace?.ToDisplayString() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(namespaceName) &&
                    TryResolveProject(type, projects, out ProjectIndexItem? project) &&
                    project is not null)
                {
                    INamedTypeSymbol original = type.OriginalDefinition;
                    string key = BuildTypeKey(original);

                    result[key] = new DocumentationTypeRegistryItem
                    {
                        GroupName = project.GroupName,
                        PackageId = project.PackageId,
                        Version = project.Version,
                        NamespaceName = original.ContainingNamespace?.ToDisplayString() ?? string.Empty,
                        ObjectName = original.Name
                    };
                }

                foreach (INamedTypeSymbol nested in type.GetTypeMembers()) VisitType(nested);
            }

            VisitNamespace(compilation.Assembly.GlobalNamespace);

            return result;
        }

        /// <summary>
        ///     Builds the set of documented type keys available in a Roslyn compilation.
        /// </summary>
        /// <param name="compilation">The compilation value used by the documentation generation operation.</param>
        /// <returns>The BuildDocumentedTypeKeys result produced by DocumentationBuilder generation.</returns>
        public static HashSet<string> BuildDocumentedTypeKeys(Compilation compilation)
        {
            HashSet<string> result = new(StringComparer.Ordinal);

            void VisitNamespace(INamespaceSymbol ns)
            {
                foreach (INamespaceSymbol childNamespace in ns.GetNamespaceMembers()) VisitNamespace(childNamespace);

                foreach (INamedTypeSymbol type in ns.GetTypeMembers()) VisitType(type);
            }

            void VisitType(INamedTypeSymbol type)
            {
                if (!string.IsNullOrWhiteSpace(type.ContainingNamespace?.ToDisplayString())) result.Add(BuildTypeKey(type));

                foreach (INamedTypeSymbol nested in type.GetTypeMembers()) VisitType(nested);
            }

            VisitNamespace(compilation.Assembly.GlobalNamespace);

            return result;
        }

        /// <summary>
        ///     Builds the stable documentation key for a Roslyn named type symbol.
        /// </summary>
        /// <param name="typeSymbol">The typeSymbol value used by the documentation generation operation.</param>
        /// <returns>The BuildTypeKey result produced by DocumentationBuilder generation.</returns>
        public static string BuildTypeKey(INamedTypeSymbol typeSymbol)
        {
            INamedTypeSymbol original = typeSymbol.OriginalDefinition;

            return $"{original.ContainingNamespace?.ToDisplayString() ?? string.Empty}::{original.Name}";
        }

        /// <summary>
        ///     Determines whether a Roslyn named type symbol is part of the generated documentation set.
        /// </summary>
        /// <param name="typeSymbol">The typeSymbol value used by the documentation generation operation.</param>
        /// <param name="documentedTypeKeys">The documentedTypeKeys value used by the documentation generation operation.</param>
        /// <returns>The IsDocumented result produced by DocumentationBuilder generation.</returns>
        public static bool IsDocumented(INamedTypeSymbol? typeSymbol, HashSet<string> documentedTypeKeys)
        {
            if (typeSymbol is null) return false;

            return documentedTypeKeys.Contains(BuildTypeKey(typeSymbol));
        }

        private static bool IsInProjectDirectory(string filePath, string projectDirectory)
        {
            string normalizedFilePath = Path.GetFullPath(filePath);
            string normalizedProjectDirectory = Path.GetFullPath(projectDirectory)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            return normalizedFilePath.StartsWith(
                normalizedProjectDirectory + Path.DirectorySeparatorChar,
                StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Attempts to resolve one Roslyn named type to its generated documentation project metadata.
        /// </summary>
        /// <param name="typeSymbol">The type symbol to resolve.</param>
        /// <param name="documentedTypeIndex">The documented type index used for lookup.</param>
        /// <param name="item">The resolved documentation type metadata, when available.</param>
        /// <returns><c>true</c> when the type is documented; otherwise, <c>false</c>.</returns>
        public static bool TryGetDocumentedType(
            INamedTypeSymbol? typeSymbol,
            IReadOnlyDictionary<string, DocumentationTypeRegistryItem> documentedTypeIndex,
            out DocumentationTypeRegistryItem? item
        )
        {
            item = null;
            if (typeSymbol is null) return false;

            return documentedTypeIndex.TryGetValue(BuildTypeKey(typeSymbol), out item);
        }

        private static bool TryResolveProject(
            INamedTypeSymbol typeSymbol,
            IReadOnlyList<ProjectIndexItem> projects,
            out ProjectIndexItem? project
        )
        {
            project = null;

            string? sourcePath = typeSymbol.DeclaringSyntaxReferences
                .Select(reference => reference.SyntaxTree.FilePath)
                .FirstOrDefault(path => !string.IsNullOrWhiteSpace(path));

            if (string.IsNullOrWhiteSpace(sourcePath)) return false;

            project = projects.FirstOrDefault(candidate => IsInProjectDirectory(sourcePath, candidate.ProjectDirectory));

            return project is not null;
        }

        #endregion

        #region Nested type: ProjectIndexItem

        private sealed class ProjectIndexItem
        {
            #region Instance fields and properties

            public required string GroupName { get; init; }
            public required string PackageId { get; init; }
            public required string ProjectDirectory { get; init; }
            public required string Version { get; init; }

            #endregion
        }

        #endregion
    }
}