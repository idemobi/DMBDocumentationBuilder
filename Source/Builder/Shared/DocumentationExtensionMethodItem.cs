#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents the DocumentationExtensionMethodItem type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationExtensionMethodItem
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the Accessibility value used by generated documentation.
        /// </summary>
        public string Accessibility { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the ExtensionNamespaceName value used by generated documentation.
        /// </summary>
        public string ExtensionNamespaceName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the ExtensionTypeName value used by generated documentation.
        /// </summary>
        public string ExtensionTypeName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the MethodName value used by generated documentation.
        /// </summary>
        public string MethodName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the Signature value used by generated documentation.
        /// </summary>
        public string Signature { get; init; } = string.Empty;

        #endregion
    }
}