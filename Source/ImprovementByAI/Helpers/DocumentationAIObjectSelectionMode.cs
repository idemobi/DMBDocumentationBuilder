#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationImprovementByAI
{
    /// <summary>
    ///     Defines how documentation objects are selected before AI improvement is generated.
    /// </summary>
    public enum DocumentationAIObjectSelectionMode
    {
        /// <summary>
        ///     Selects only objects that belong to the latest generated documentation version for each package or unversioned
        ///     group object.
        /// </summary>
        LatestVersion,

        /// <summary>
        ///     Selects all documentation objects in database identifier order.
        /// </summary>
        All
    }
}