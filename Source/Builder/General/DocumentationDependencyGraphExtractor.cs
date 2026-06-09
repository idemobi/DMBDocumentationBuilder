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

        /// <summary>
        ///     Builds dependency edges from one documented source type to other documented types.
        /// </summary>
        /// <param name="sourceType">The documented source type.</param>
        /// <param name="project">The project metadata that owns the source type.</param>
        /// <param name="group">The documentation group metadata that owns the source type.</param>
        /// <param name="documentedTypeIndex">The documented type index used to resolve target metadata.</param>
        /// <returns>The dependency edges discovered for the source type.</returns>
        public static IReadOnlyList<DocumentationDependencyEdgeItem> BuildDependencyEdges(
            INamedTypeSymbol sourceType,
            DocumentationProjectDescriptor project,
            DocumentationGroupDescriptor group,
            IReadOnlyDictionary<string, DocumentationTypeRegistryItem> documentedTypeIndex
        )
        {
            if (sourceType is null) throw new ArgumentNullException(nameof(sourceType));
            if (project is null) throw new ArgumentNullException(nameof(project));
            if (group is null) throw new ArgumentNullException(nameof(group));
            if (documentedTypeIndex is null) throw new ArgumentNullException(nameof(documentedTypeIndex));

            List<DocumentationDependencyEdgeItem> result = [];

            AddTypeDependency(result, sourceType, sourceType.BaseType, "Inherits", project, group, documentedTypeIndex);

            foreach (INamedTypeSymbol implementedInterface in sourceType.Interfaces)
            {
                string relationshipKind = sourceType.TypeKind == TypeKind.Interface ? "Extends" : "Implements";
                AddTypeDependency(result, sourceType, implementedInterface, relationshipKind, project, group, documentedTypeIndex);
            }

            foreach (ISymbol member in sourceType.GetMembers().Where(member => !member.IsImplicitlyDeclared))
            {
                switch (member)
                {
                    case IEventSymbol eventSymbol:
                        AddUsedTypeDependencies(result, sourceType, eventSymbol.Type, project, group, documentedTypeIndex);
                    break;

                    case IFieldSymbol fieldSymbol when fieldSymbol.AssociatedSymbol is null:
                        AddUsedTypeDependencies(result, sourceType, fieldSymbol.Type, project, group, documentedTypeIndex);
                    break;

                    case IMethodSymbol methodSymbol:
                        AddUsedTypeDependencies(result, sourceType, methodSymbol.ReturnType, project, group, documentedTypeIndex);

                        foreach (IParameterSymbol parameter in methodSymbol.Parameters)
                        {
                            AddUsedTypeDependencies(result, sourceType, parameter.Type, project, group, documentedTypeIndex);
                        }

                        foreach (ITypeParameterSymbol typeParameter in methodSymbol.TypeParameters)
                        {
                            foreach (ITypeSymbol constraintType in typeParameter.ConstraintTypes)
                            {
                                AddUsedTypeDependencies(result, sourceType, constraintType, project, group, documentedTypeIndex);
                            }
                        }
                    break;

                    case IPropertySymbol propertySymbol:
                        AddUsedTypeDependencies(result, sourceType, propertySymbol.Type, project, group, documentedTypeIndex);
                    break;
                }
            }

            foreach (ITypeParameterSymbol typeParameter in sourceType.TypeParameters)
            {
                foreach (ITypeSymbol constraintType in typeParameter.ConstraintTypes)
                {
                    AddUsedTypeDependencies(result, sourceType, constraintType, project, group, documentedTypeIndex);
                }
            }

            return result;
        }

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
            DocumentationProjectDescriptor project,
            DocumentationGroupDescriptor group,
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
                SourceGroupName = group.GroupName,
                SourcePackageId = project.PackageId,
                SourceVersion = project.Version,
                SourceNamespaceName = sourceOriginal.ContainingNamespace?.ToDisplayString() ?? string.Empty,
                SourceName = sourceOriginal.Name,
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
            DocumentationProjectDescriptor project,
            DocumentationGroupDescriptor group,
            IReadOnlyDictionary<string, DocumentationTypeRegistryItem> documentedTypeIndex
        )
        {
            foreach (INamedTypeSymbol namedType in EnumerateNamedTypes(typeSymbol))
            {
                AddTypeDependency(result, sourceType, namedType, "Uses", project, group, documentedTypeIndex);
            }
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
