#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents one granular documentation member row stored in the generated documentation database.
    /// </summary>
    public sealed class DocumentationMemberDatabaseItem
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
        ///     Gets the exception documentation serialized for this member.
        /// </summary>
        public string ExceptionsJson { get; init; } = "[]";

        /// <summary>
        ///     Gets the namespace that contains the extension method declaring type.
        /// </summary>
        public string ExtensionNamespaceName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the extension method declaring type name.
        /// </summary>
        public string ExtensionTypeName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets a value indicating whether the member is abstract.
        /// </summary>
        public bool IsAbstract { get; init; }

        /// <summary>
        ///     Gets a value indicating whether the member is constant.
        /// </summary>
        public bool IsConst { get; init; }

        /// <summary>
        ///     Gets a value indicating whether the member is obsolete.
        /// </summary>
        public bool IsObsolete { get; init; }

        /// <summary>
        ///     Gets a value indicating whether the member overrides an inherited member.
        /// </summary>
        public bool IsOverride { get; init; }

        /// <summary>
        ///     Gets a value indicating whether the member is read-only.
        /// </summary>
        public bool IsReadOnly { get; init; }

        /// <summary>
        ///     Gets a value indicating whether the member is sealed.
        /// </summary>
        public bool IsSealed { get; init; }

        /// <summary>
        ///     Gets a value indicating whether the member is static.
        /// </summary>
        public bool IsStatic { get; init; }

        /// <summary>
        ///     Gets a value indicating whether the member is virtual.
        /// </summary>
        public bool IsVirtual { get; init; }

        /// <summary>
        ///     Gets the stable member key within the documented object.
        /// </summary>
        public required string MemberKey { get; init; }

        /// <summary>
        ///     Gets the member kind, such as <c>Constructor</c>, <c>Method</c>, or <c>ExtensionMethod</c>.
        /// </summary>
        public required string MemberKind { get; init; }

        /// <summary>
        ///     Gets the member display name.
        /// </summary>
        public required string MemberName { get; init; }

        /// <summary>
        ///     Gets the obsolete message when the member is obsolete.
        /// </summary>
        public string ObsoleteMessage { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the parameter documentation serialized for this member.
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
        public required string Signature { get; init; }

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