#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Represents one granular documentation member read from the generated documentation database.
    /// </summary>
    public sealed class DocumentationMemberQueryResult
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the member accessibility keyword.
        /// </summary>
        public string Accessibility { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the HTML example documentation.
        /// </summary>
        public string ExampleHtml { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the exception documentation JSON.
        /// </summary>
        public string ExceptionsJson { get; init; } = "[]";

        /// <summary>
        ///     Gets the extension method declaring namespace.
        /// </summary>
        public string ExtensionNamespaceName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the extension method declaring type.
        /// </summary>
        public string ExtensionTypeName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets a value indicating whether the member is obsolete.
        /// </summary>
        public bool IsObsolete { get; init; }

        /// <summary>
        ///     Gets the member kind, such as <c>Method</c> or <c>ExtensionMethod</c>.
        /// </summary>
        public string MemberKind { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the member display name.
        /// </summary>
        public string MemberName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the obsolete message.
        /// </summary>
        public string ObsoleteMessage { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the parameter documentation JSON.
        /// </summary>
        public string ParametersJson { get; init; } = "[]";

        /// <summary>
        ///     Gets the HTML remarks documentation.
        /// </summary>
        public string RemarksHtml { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the HTML returns documentation.
        /// </summary>
        public string ReturnsHtml { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the member signature.
        /// </summary>
        public string Signature { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the sort order within the documented object.
        /// </summary>
        public int SortOrder { get; init; }

        /// <summary>
        ///     Gets the HTML summary documentation.
        /// </summary>
        public string SummaryHtml { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the HTML value documentation.
        /// </summary>
        public string ValueHtml { get; init; } = string.Empty;

        #endregion
    }
}