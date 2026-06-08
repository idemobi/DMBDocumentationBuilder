#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace DMBDocumentationImprovementByAI
{
    internal static class DocumentationAIObjectSelector
    {
        #region Static fields and properties

        private static readonly DocumentationAIVersionComparer VersionComparer = new();

        #endregion

        #region Static methods

        private static string BuildLogicalVersionKey(
            string packageId,
            string namespaceName,
            string objectName,
            string objectType
        )
        {
            if (!string.IsNullOrWhiteSpace(packageId))
            {
                return $"package:{packageId}";
            }

            return $"object:{objectType}\u001F{namespaceName}\u001F{objectName}";
        }

        private static IEnumerable<T> SelectLatestVersionRows<T>(
            IReadOnlyList<T> rows,
            Func<T, string> packageIdSelector,
            Func<T, string> versionSelector,
            Func<T, string> namespaceNameSelector,
            Func<T, string> objectNameSelector,
            Func<T, string> objectTypeSelector
        )
        {
            Dictionary<string, string> latestVersionsByKey = new(StringComparer.OrdinalIgnoreCase);

            foreach (T row in rows)
            {
                string version = versionSelector(row);

                if (string.IsNullOrWhiteSpace(version))
                {
                    continue;
                }

                string key = BuildLogicalVersionKey(
                    packageIdSelector(row),
                    namespaceNameSelector(row),
                    objectNameSelector(row),
                    objectTypeSelector(row));

                if (!latestVersionsByKey.TryGetValue(key, out string? currentLatestVersion) ||
                    VersionComparer.Compare(version, currentLatestVersion) > 0)
                {
                    latestVersionsByKey[key] = version;
                }
            }

            foreach (T row in rows)
            {
                string key = BuildLogicalVersionKey(
                    packageIdSelector(row),
                    namespaceNameSelector(row),
                    objectNameSelector(row),
                    objectTypeSelector(row));

                if (latestVersionsByKey.TryGetValue(key, out string? latestVersion) &&
                    string.Equals(versionSelector(row), latestVersion, StringComparison.OrdinalIgnoreCase))
                {
                    yield return row;
                }
            }
        }

        public static List<T> SelectRows<T>(
            IEnumerable<T> sourceRows,
            DocumentationAIObjectSelectionMode selectionMode,
            int maxObjectsToProcess,
            Func<T, long> idSelector,
            Func<T, string> packageIdSelector,
            Func<T, string> versionSelector,
            Func<T, string> namespaceNameSelector,
            Func<T, string> objectNameSelector,
            Func<T, string> objectTypeSelector
        )
        {
            List<T> orderedRows = sourceRows
                .OrderBy(idSelector)
                .ToList();

            IEnumerable<T> selectedRows = selectionMode switch
            {
                DocumentationAIObjectSelectionMode.All => orderedRows,
                DocumentationAIObjectSelectionMode.LatestVersion => SelectLatestVersionRows(
                    orderedRows,
                    packageIdSelector,
                    versionSelector,
                    namespaceNameSelector,
                    objectNameSelector,
                    objectTypeSelector),
                _ => throw new ArgumentOutOfRangeException(nameof(selectionMode), selectionMode, "Unsupported object selection mode.")
            };

            if (maxObjectsToProcess > 0)
            {
                selectedRows = selectedRows.Take(maxObjectsToProcess);
            }

            return selectedRows.ToList();
        }

        #endregion
    }
}
