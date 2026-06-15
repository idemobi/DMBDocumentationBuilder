#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using Microsoft.CodeAnalysis;

#endregion

namespace DMBDocumentationBuilder
{
    internal static class DocumentationDependencyGraphExtractor
    {
        #region Static methods

        private static void AddDependencyIfMissing(
            IList<DocumentationDependencyEdgeItem> list,
            DocumentationDependencyEdgeItem item
        )
        {
            if (list.Any(x =>
                    string.Equals(x.SourceNamespaceName, item.SourceNamespaceName, StringComparison.Ordinal) &&
                    string.Equals(x.SourceName, item.SourceName, StringComparison.Ordinal) &&
                    string.Equals(x.TargetNamespaceName, item.TargetNamespaceName, StringComparison.Ordinal) &&
                    string.Equals(x.TargetName, item.TargetName, StringComparison.Ordinal) &&
                    string.Equals(x.RelationshipKind, item.RelationshipKind, StringComparison.Ordinal)))
            {
                return;
            }

            list.Add(item);
        }

        private static void AddTypeDependency(
            IList<DocumentationDependencyEdgeItem> result,
            INamedTypeSymbol sourceType,
            INamedTypeSymbol? targetType,
            string relationshipKind,
            DocumentationTypeRegistryItem sourceItem,
            IReadOnlyDictionary<string, DocumentationTypeRegistryItem> documentedTypeIndex
        )
        {
            if (targetType is null) return;
            if (targetType.SpecialType == SpecialType.System_Object) return;

            INamedTypeSymbol sourceOriginal = sourceType.OriginalDefinition;
            INamedTypeSymbol targetOriginal = targetType.OriginalDefinition;

            string sourceKey = DocumentationTypeRegistry.BuildTypeKey(sourceOriginal);
            string targetKey = DocumentationTypeRegistry.BuildTypeKey(targetOriginal);
            if (string.Equals(sourceKey, targetKey, StringComparison.Ordinal)) return;

            if (!documentedTypeIndex.TryGetValue(targetKey, out DocumentationTypeRegistryItem? targetItem)) return;

            AddDependencyIfMissing(result, new DocumentationDependencyEdgeItem
            {
                SourceGroupName = sourceItem.GroupName,
                SourcePackageId = sourceItem.PackageId,
                SourceVersion = sourceItem.Version,
                SourceNamespaceName = sourceItem.NamespaceName,
                SourceName = sourceItem.ObjectName,
                SourceKindLabel = GetKindLabel(sourceOriginal),
                RelationshipKind = relationshipKind,
                TargetGroupName = targetItem.GroupName,
                TargetPackageId = targetItem.PackageId,
                TargetVersion = targetItem.Version,
                TargetNamespaceName = targetItem.NamespaceName,
                TargetName = targetItem.ObjectName,
                TargetKindLabel = GetKindLabel(targetOriginal)
            });
        }

        private static void AddUsedTypeDependencies(
            IList<DocumentationDependencyEdgeItem> result,
            INamedTypeSymbol sourceType,
            ITypeSymbol? typeSymbol,
            DocumentationTypeRegistryItem sourceItem,
            IReadOnlyDictionary<string, DocumentationTypeRegistryItem> documentedTypeIndex
        )
        {
            foreach (INamedTypeSymbol namedType in EnumerateNamedTypes(typeSymbol))
            {
                AddTypeDependency(result, sourceType, namedType, "Uses", sourceItem, documentedTypeIndex);
            }
        }

        /// <summary>
        ///     Builds every dependency edge between documented types in the supplied compilation.
        /// </summary>
        /// <param name="compilation">The compilation that contains documented source types.</param>
        /// <param name="documentedTypeIndex">The documented type index used to resolve source and target metadata.</param>
        /// <returns>The dependency edges discovered between documented types.</returns>
        public static IReadOnlyList<DocumentationDependencyEdgeItem> BuildAllDependencyEdges(
            Compilation compilation,
            IReadOnlyDictionary<string, DocumentationTypeRegistryItem> documentedTypeIndex
        )
        {
            if (compilation is null) throw new ArgumentNullException(nameof(compilation));
            if (documentedTypeIndex is null) throw new ArgumentNullException(nameof(documentedTypeIndex));

            List<DocumentationDependencyEdgeItem> result = [];

            foreach (INamedTypeSymbol typeSymbol in EnumerateNamespaceTypes(compilation.Assembly.GlobalNamespace))
            {
                INamedTypeSymbol sourceOriginal = typeSymbol.OriginalDefinition;
                string sourceKey = DocumentationTypeRegistry.BuildTypeKey(sourceOriginal);

                if (!documentedTypeIndex.TryGetValue(sourceKey, out DocumentationTypeRegistryItem? sourceItem))
                {
                    continue;
                }

                foreach (DocumentationDependencyEdgeItem edge in BuildDependencyEdges(sourceOriginal, sourceItem, documentedTypeIndex))
                {
                    AddDependencyIfMissing(result, edge);
                }
            }

            return result
                .OrderBy(x => x.SourceNamespaceName, StringComparer.Ordinal)
                .ThenBy(x => x.SourceName, StringComparer.Ordinal)
                .ThenBy(x => x.RelationshipKind, StringComparer.Ordinal)
                .ThenBy(x => x.TargetNamespaceName, StringComparer.Ordinal)
                .ThenBy(x => x.TargetName, StringComparer.Ordinal)
                .ToArray();
        }

        private static IReadOnlyList<DocumentationDependencyEdgeItem> BuildDependencyEdges(
            INamedTypeSymbol sourceType,
            DocumentationTypeRegistryItem sourceItem,
            IReadOnlyDictionary<string, DocumentationTypeRegistryItem> documentedTypeIndex
        )
        {
            List<DocumentationDependencyEdgeItem> result = [];

            AddTypeDependency(result, sourceType, sourceType.BaseType, "Inherits", sourceItem, documentedTypeIndex);

            foreach (INamedTypeSymbol implementedInterface in sourceType.Interfaces)
            {
                string relationshipKind = sourceType.TypeKind == TypeKind.Interface ? "Extends" : "Implements";
                AddTypeDependency(result, sourceType, implementedInterface, relationshipKind, sourceItem, documentedTypeIndex);
            }

            foreach (ISymbol member in sourceType.GetMembers().Where(member => !member.IsImplicitlyDeclared))
            {
                switch (member)
                {
                    case IEventSymbol eventSymbol:
                        AddUsedTypeDependencies(result, sourceType, eventSymbol.Type, sourceItem, documentedTypeIndex);
                    break;

                    case IFieldSymbol fieldSymbol when fieldSymbol.AssociatedSymbol is null:
                        AddUsedTypeDependencies(result, sourceType, fieldSymbol.Type, sourceItem, documentedTypeIndex);
                    break;

                    case IMethodSymbol methodSymbol:
                        AddUsedTypeDependencies(result, sourceType, methodSymbol.ReturnType, sourceItem, documentedTypeIndex);

                        foreach (IParameterSymbol parameter in methodSymbol.Parameters)
                        {
                            AddUsedTypeDependencies(result, sourceType, parameter.Type, sourceItem, documentedTypeIndex);
                        }

                        foreach (ITypeParameterSymbol typeParameter in methodSymbol.TypeParameters)
                        {
                            foreach (ITypeSymbol constraintType in typeParameter.ConstraintTypes)
                            {
                                AddUsedTypeDependencies(result, sourceType, constraintType, sourceItem, documentedTypeIndex);
                            }
                        }

                    break;

                    case IPropertySymbol propertySymbol:
                        AddUsedTypeDependencies(result, sourceType, propertySymbol.Type, sourceItem, documentedTypeIndex);
                    break;
                }
            }

            foreach (ITypeParameterSymbol typeParameter in sourceType.TypeParameters)
            {
                foreach (ITypeSymbol constraintType in typeParameter.ConstraintTypes)
                {
                    AddUsedTypeDependencies(result, sourceType, constraintType, sourceItem, documentedTypeIndex);
                }
            }

            return result;
        }

        /// <summary>
        ///     Filters a dependency graph to the incoming and outgoing edges related to one documented type.
        /// </summary>
        /// <param name="typeSymbol">The documented type whose neighborhood should be selected.</param>
        /// <param name="dependencyEdges">The complete dependency graph edges.</param>
        /// <returns>The incoming and outgoing dependency edges related to the type.</returns>
        public static IReadOnlyList<DocumentationDependencyEdgeItem> BuildRelatedDependencyEdges(
            INamedTypeSymbol typeSymbol,
            IReadOnlyCollection<DocumentationDependencyEdgeItem> dependencyEdges
        )
        {
            if (typeSymbol is null) throw new ArgumentNullException(nameof(typeSymbol));
            if (dependencyEdges is null) throw new ArgumentNullException(nameof(dependencyEdges));

            INamedTypeSymbol original = typeSymbol.OriginalDefinition;
            string namespaceName = original.ContainingNamespace?.ToDisplayString() ?? string.Empty;
            string objectName = original.Name;

            return dependencyEdges
                .Where(edge =>
                    (string.Equals(edge.SourceNamespaceName, namespaceName, StringComparison.Ordinal) &&
                     string.Equals(edge.SourceName, objectName, StringComparison.Ordinal)) ||
                    (string.Equals(edge.TargetNamespaceName, namespaceName, StringComparison.Ordinal) &&
                     string.Equals(edge.TargetName, objectName, StringComparison.Ordinal)))
                .OrderBy(x => x.TargetNamespaceName, StringComparer.Ordinal)
                .ThenBy(x => x.TargetName, StringComparer.Ordinal)
                .ThenBy(x => x.SourceNamespaceName, StringComparer.Ordinal)
                .ThenBy(x => x.SourceName, StringComparer.Ordinal)
                .ThenBy(x => x.RelationshipKind, StringComparer.Ordinal)
                .ToArray();
        }

        private static IEnumerable<INamedTypeSymbol> EnumerateNamedTypes(ITypeSymbol? typeSymbol)
        {
            switch (typeSymbol)
            {
                case null:
                break;

                case IArrayTypeSymbol arrayTypeSymbol:
                    foreach (INamedTypeSymbol nestedType in EnumerateNamedTypes(arrayTypeSymbol.ElementType))
                    {
                        yield return nestedType;
                    }

                break;

                case IPointerTypeSymbol pointerTypeSymbol:
                    foreach (INamedTypeSymbol nestedType in EnumerateNamedTypes(pointerTypeSymbol.PointedAtType))
                    {
                        yield return nestedType;
                    }

                break;

                case INamedTypeSymbol namedTypeSymbol:
                    yield return namedTypeSymbol.OriginalDefinition;

                    foreach (ITypeSymbol typeArgument in namedTypeSymbol.TypeArguments)
                    {
                        foreach (INamedTypeSymbol nestedType in EnumerateNamedTypes(typeArgument))
                        {
                            yield return nestedType;
                        }
                    }

                break;
            }
        }

        private static IEnumerable<INamedTypeSymbol> EnumerateNamespaceTypes(INamespaceSymbol namespaceSymbol)
        {
            foreach (INamespaceSymbol childNamespace in namespaceSymbol.GetNamespaceMembers())
            {
                foreach (INamedTypeSymbol childType in EnumerateNamespaceTypes(childNamespace))
                {
                    yield return childType;
                }
            }

            foreach (INamedTypeSymbol type in namespaceSymbol.GetTypeMembers())
            {
                foreach (INamedTypeSymbol childType in EnumerateTypeAndNestedTypes(type))
                {
                    yield return childType;
                }
            }
        }

        private static IEnumerable<INamedTypeSymbol> EnumerateTypeAndNestedTypes(INamedTypeSymbol typeSymbol)
        {
            yield return typeSymbol;

            foreach (INamedTypeSymbol nestedType in typeSymbol.GetTypeMembers())
            {
                foreach (INamedTypeSymbol childType in EnumerateTypeAndNestedTypes(nestedType))
                {
                    yield return childType;
                }
            }
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

        #endregion
    }
}