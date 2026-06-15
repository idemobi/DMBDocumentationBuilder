#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using Microsoft.CodeAnalysis;

#endregion

namespace DMBDocumentationBuilder
{
    internal static class DocumentationSymbolDisplayFormat
    {
        #region Static fields and properties

        /// <summary>
        ///     Gets the Roslyn symbol display format used for generated member signatures.
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