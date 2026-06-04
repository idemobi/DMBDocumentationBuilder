#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationViewer
{
    /// <summary>
    ///     Defines the granular documentation member kinds stored by DocumentationBuilder.
    /// </summary>
    public enum DocumentationMemberKind
    {
        /// <summary>
        ///     Represents class, struct, or record constructors.
        /// </summary>
        Constructor,

        /// <summary>
        ///     Represents fields.
        /// </summary>
        Field,

        /// <summary>
        ///     Represents properties.
        /// </summary>
        Property,

        /// <summary>
        ///     Represents methods declared by the documented object.
        /// </summary>
        Method,

        /// <summary>
        ///     Represents extension methods targeting the documented object.
        /// </summary>
        ExtensionMethod
    }
}