#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents the DocumentationInterfaceMethodItem type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationInterfaceMethodItem
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the Accessibility value used by generated documentation.
        /// </summary>
        public string Accessibility { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the IsAbstract value used by generated documentation.
        /// </summary>
        public bool IsAbstract { get; init; }

        /// <summary>
        ///     Gets or sets the IsObsolete value used by generated documentation.
        /// </summary>
        public bool IsObsolete { get; init; }

        /// <summary>
        ///     Gets or sets the IsOverride value used by generated documentation.
        /// </summary>
        public bool IsOverride { get; init; }

        /// <summary>
        ///     Gets or sets the IsSealed value used by generated documentation.
        /// </summary>
        public bool IsSealed { get; init; }

        /// <summary>
        ///     Gets or sets the IsStatic value used by generated documentation.
        /// </summary>
        public bool IsStatic { get; init; }

        /// <summary>
        ///     Gets or sets the IsVirtual value used by generated documentation.
        /// </summary>
        public bool IsVirtual { get; init; }

        /// <summary>
        ///     Gets or sets the MethodName value used by generated documentation.
        /// </summary>
        public string MethodName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the ObsoleteMessage value used by generated documentation.
        /// </summary>
        public string ObsoleteMessage { get; init; } = string.Empty;

        /// <summary>
        ///     Gets or sets the Signature value used by generated documentation.
        /// </summary>
        public string Signature { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the collection used by DocumentationBuilder generation.
        /// </summary>
        public DocumentationXmlModel XmlDoc { get; init; } = new();

        #endregion
    }
}