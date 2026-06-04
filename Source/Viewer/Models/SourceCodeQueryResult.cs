#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Represents stored source code associated with one documented object.
    /// </summary>
    public sealed class SourceCodeQueryResult
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the namespace that contains the documented object.
        /// </summary>
        public string NamespaceName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the documented object name.
        /// </summary>
        public string ObjectName { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the documented object type.
        /// </summary>
        public string ObjectType { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the package identifier that owns the source-code snapshot.
        /// </summary>
        public string PackageId { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the stored source code snapshot for the documented object.
        /// </summary>
        public string SourceCode { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the number of source files represented by the stored source-code snapshot.
        /// </summary>
        public int SourceFileCount { get; init; }

        /// <summary>
        ///     Gets the documented package version.
        /// </summary>
        public string Version { get; init; } = string.Empty;

        #endregion
    }
}