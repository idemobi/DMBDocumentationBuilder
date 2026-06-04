#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Describes an optional AI-render database source used by documentation pages.
    /// </summary>
    public sealed class DocumentationAIRenderSource
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets the absolute or relative path to the SQLite database that stores AI-rendered documentation content.
        /// </summary>
        public string DatabasePath { get; init; } = string.Empty;

        /// <summary>
        ///     Gets a value indicating whether this AI-render source should be queried by the viewer.
        /// </summary>
        public bool IsEnabled { get; init; }

        /// <summary>
        ///     Gets the model name associated with the rendered AI documentation records.
        /// </summary>
        public string Model { get; init; } = string.Empty;

        /// <summary>
        ///     Gets the provider name associated with the rendered AI documentation records.
        /// </summary>
        public string Provider { get; init; } = string.Empty;

        #endregion
    }
}