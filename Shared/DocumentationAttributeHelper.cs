#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationAttributeHelper.cs create at 2026/04/13 12:04:26
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using Microsoft.CodeAnalysis;

#endregion

namespace DMBDocumentationBuilder.Shared
{
    #region

    #endregion

    internal static class DocumentationAttributeHelper
    {
        #region Static methods

        /// <summary>
        /// Determines whether a Roslyn symbol carries obsolete metadata.
        /// </summary>
        /// <param name="symbol">The symbol value used by the documentation generation operation.</param>
        /// <param name="message">The message value used by the documentation generation operation.</param>
        /// <returns>The IsObsolete result produced by DocumentationBuilder generation.</returns>
        public static bool IsObsolete(ISymbol symbol, out string message)
        {
            message = string.Empty;
            AttributeData? attr = symbol.GetAttributes()
                .FirstOrDefault(a =>
                    a.AttributeClass?.ToDisplayString() == "System.ObsoleteAttribute" ||
                    a.AttributeClass?.Name == "ObsoleteAttribute" ||
                    a.AttributeClass?.Name == "Obsolete");

            if (attr == null) return false;

            message = attr.ConstructorArguments.FirstOrDefault().Value?.ToString() ?? string.Empty;
            return true;
        }

        #endregion
    }
}