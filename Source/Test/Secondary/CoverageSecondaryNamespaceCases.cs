#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationTest.Secondary
{
    /// <summary>
    ///     Validates that DocumentationViewer navigation displays multiple namespaces for the coverage project.
    /// </summary>
    public sealed class CoverageSecondaryNamespaceCases
    {
        #region Instance constructors and destructors

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CoverageSecondaryNamespaceCases" /> class.
        /// </summary>
        public CoverageSecondaryNamespaceCases()
        {
        }

        #endregion

        #endregion

        #region Instance methods

        /// <summary>
        ///     Returns a secondary namespace marker.
        /// </summary>
        /// <returns>The secondary namespace marker.</returns>
        public string GetMarker()
        {
            return "secondary";
        }

        #endregion
    }
}