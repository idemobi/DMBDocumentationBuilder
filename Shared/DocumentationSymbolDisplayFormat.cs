#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationSymbolDisplayFormat.cs create at 2026/04/12 12:04:31
// ©2024-2026 idéMobi SARL FRANCE

#endregion

#region

using Microsoft.CodeAnalysis;

#endregion

namespace DMBDocumentationBuilder.Shared
{
    internal static class DocumentationSymbolDisplayFormat
    {
        #region Static fields and properties

        /// <summary>
        /// Gets the Roslyn symbol display format used for generated member signatures.
        /// </summary>
        public static readonly SymbolDisplayFormat SignatureFormat = SymbolDisplayFormat.MinimallyQualifiedFormat
            .WithMemberOptions(
                (SymbolDisplayFormat.MinimallyQualifiedFormat.MemberOptions
                 & ~SymbolDisplayMemberOptions.IncludeContainingType)
                | SymbolDisplayMemberOptions.IncludeAccessibility
                | SymbolDisplayMemberOptions.IncludeModifiers);

        #endregion
    }
}
