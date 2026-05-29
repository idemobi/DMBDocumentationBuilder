#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    internal sealed class DocumentationVersionComparer : IComparer<string>
    {
        #region Static fields and properties

        /// <summary>
        ///     Gets the shared comparer instance used to order documentation versions.
        /// </summary>
        public static readonly DocumentationVersionComparer Instance = new();

        #endregion

        #region Static methods

        private static int[] ParseVersionParts(string? value)
        {
            int[] parts = new int[4];

            if (string.IsNullOrWhiteSpace(value))
            {
                return parts;
            }

            string normalized = value.Trim();
            int metadataIndex = normalized.IndexOf('+');

            if (metadataIndex >= 0)
            {
                normalized = normalized[..metadataIndex];
            }

            int prereleaseIndex = normalized.IndexOf('-');

            if (prereleaseIndex >= 0)
            {
                normalized = normalized[..prereleaseIndex];
            }

            string[] tokens = normalized.Split('.', StringSplitOptions.RemoveEmptyEntries);

            for (int index = 0; index < tokens.Length && index < parts.Length; index++)
            {
                if (int.TryParse(tokens[index], out int parsedPart))
                {
                    parts[index] = parsedPart;
                }
            }

            return parts;
        }

        #endregion

        #region Instance methods

        #region From interface IComparer<string>

        /// <summary>
        ///     Compares two documentation version labels using numeric version parts first and ordinal text as a fallback.
        /// </summary>
        /// <param name="x">The first version label to compare.</param>
        /// <param name="y">The second version label to compare.</param>
        /// <returns>A signed integer that indicates the relative order of <paramref name="x" /> and <paramref name="y" />.</returns>
        public int Compare(string? x, string? y)
        {
            if (string.Equals(x, y, StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            int[] xParts = ParseVersionParts(x);
            int[] yParts = ParseVersionParts(y);

            for (int index = 0; index < xParts.Length; index++)
            {
                int partComparison = xParts[index].CompareTo(yParts[index]);

                if (partComparison != 0)
                {
                    return partComparison;
                }
            }

            return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #endregion
    }
}