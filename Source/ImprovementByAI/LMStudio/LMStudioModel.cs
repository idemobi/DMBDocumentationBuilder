#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationImprovementByLMStudio
{
    /// <summary>
    ///     Identifies the LMStudio model used to improve generated documentation with AI output.
    /// </summary>
    public enum LMStudioModel
    {
        /// <summary>The Custom model option.</summary>
        Custom,

        /// <summary>The Gemma4 model option.</summary>
        Gemma4,

        /// <summary>The Deep Seek Coder model option.</summary>
        DeepSeekCoder,

        /// <summary>The Llama3 Instruct model option.</summary>
        Llama3Instruct,

        /// <summary>The Qwen2 5 Coder model option.</summary>
        Qwen2_5Coder
    }
}