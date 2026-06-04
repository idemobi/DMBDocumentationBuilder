#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationImprovementByOllama
{
    /// <summary>
    ///     Identifies the Ollama model used to improve generated documentation with AI output.
    /// </summary>
    public enum OllamaModel
    {
        // Coding oriented
        /// <summary>The Qwen25 Coder7 B model option.</summary>
        Qwen25Coder7B,

        /// <summary>The Qwen25 Coder14 B model option.</summary>
        Qwen25Coder14B,

        /// <summary>The Qwen25 Coder32 B model option.</summary>
        Qwen25Coder32B,

        // General purpose
        /// <summary>The Llama31 8 B model option.</summary>
        Llama31_8B,

        /// <summary>The Gemma3 12 B model option.</summary>
        Gemma3_12B,

        /// <summary>The Gemma3 27 B model option.</summary>
        Gemma3_27B,

        // Light / fast
        /// <summary>The Phi3 Mini model option.</summary>
        Phi3Mini,

        // Experimental / fallback
        /// <summary>The Mistral7 B model option.</summary>
        Mistral7B
    }
}