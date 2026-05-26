#region Copyright

// Game-Data-Forge Solution
// Written by CONTART Jean-François & BOULOGNE Quentin
// DMBDocumentationBuilder.csproj DocumentationExtensionMethodItem.cs create at 2026/04/12 12:04:31
// ©2024-2026 idéMobi SARL FRANCE

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    /// Represents the DocumentationExtensionMethodItem type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationExtensionMethodItem
    {
        #region Instance fields and properties

        /// <summary>
        /// Gets or sets the Accessibility value used by generated documentation.
        /// </summary>
        public string Accessibility { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the ExtensionNamespaceName value used by generated documentation.
        /// </summary>
        public string ExtensionNamespaceName { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the ExtensionTypeName value used by generated documentation.
        /// </summary>
        public string ExtensionTypeName { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the MethodName value used by generated documentation.
        /// </summary>
        public string MethodName { get; init; } = string.Empty;
        /// <summary>
        /// Gets or sets the Signature value used by generated documentation.
        /// </summary>
        public string Signature { get; init; } = string.Empty;

        #endregion
    }
}