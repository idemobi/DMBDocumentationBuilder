#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationBuilder
{
    /// <summary>
    ///     Represents the DocumentationIndex type used by DocumentationBuilder generation.
    /// </summary>
    public sealed class DocumentationIndex
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the collection used by DocumentationBuilder generation.
        /// </summary>
        public List<DocumentationGroupItem> Groups { get; } = new();

        #endregion
    }
}