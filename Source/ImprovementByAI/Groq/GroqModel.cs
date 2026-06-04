#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationImprovementByGroq
{
    /// <summary>
    ///     Identifies the Groq model used to improve generated documentation with AI output.
    /// </summary>
    public enum GroqModel
    {
        /// <summary>The Llama31 8 B Instant model option.</summary>
        Llama31_8B_Instant,

        /// <summary>The Llama31 70 B Versatile model option.</summary>
        Llama31_70B_Versatile,

        // Ajoute si besoin plus tard
        /// <summary>The Mixtral 8x7 B model option.</summary>
        Mixtral_8x7B
    }
}