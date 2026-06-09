#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationImprovementByOllama
{
    /// <summary>
    ///     Provides helper methods for Ollama model identifiers and defaults.
    /// </summary>
    public static class OllamaModelHelper
    {
        #region Public API

        /// <summary>
        ///     Executes the ToModelString operation for Ollama documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static string ToModelString(this OllamaModel model)
        {
            return model switch
            {
                OllamaModel.Qwen25Coder7B => "qwen2.5-coder:7b",
                OllamaModel.Qwen25Coder14B => "qwen2.5-coder:14b",
                OllamaModel.Qwen25Coder32B => "qwen2.5-coder:32b",

                OllamaModel.Llama31_8B => "llama3.1:8b",

                OllamaModel.Gemma3_12B => "gemma3:12b",
                OllamaModel.Gemma3_27B => "gemma3:27b",

                OllamaModel.Phi3Mini => "phi3:mini",

                OllamaModel.Mistral7B => "mistral:7b",

                _ => throw new ArgumentOutOfRangeException(nameof(model), model, null)
            };
        }

        /// <summary>
        ///     Executes the GetEstimatedSizeInGB operation for Ollama documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static int GetEstimatedSizeInGB(this OllamaModel model)
        {
            return model switch
            {
                OllamaModel.Qwen25Coder7B => 4,
                OllamaModel.Qwen25Coder14B => 8,
                OllamaModel.Qwen25Coder32B => 20,

                OllamaModel.Llama31_8B => 5,

                OllamaModel.Gemma3_12B => 8,
                OllamaModel.Gemma3_27B => 16,

                OllamaModel.Phi3Mini => 2,

                OllamaModel.Mistral7B => 4,

                _ => 0
            };
        }

        /// <summary>
        ///     Executes the IsHeavyModel operation for Ollama documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static bool IsHeavyModel(this OllamaModel model)
        {
            return model switch
            {
                OllamaModel.Qwen25Coder32B => true,
                OllamaModel.Gemma3_27B => true,
                _ => false
            };
        }

        /// <summary>
        ///     Executes the IsCodingModel operation for Ollama documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static bool IsCodingModel(this OllamaModel model)
        {
            return model switch
            {
                OllamaModel.Qwen25Coder7B => true,
                OllamaModel.Qwen25Coder14B => true,
                OllamaModel.Qwen25Coder32B => true,
                _ => false
            };
        }

        /// <summary>
        ///     Executes the GetRecommendedDefault operation for Ollama documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static OllamaModel GetRecommendedDefault()
        {
            return OllamaModel.Qwen25Coder14B;
        }

        /// <summary>
        ///     Executes the GetFallbackModel operation for Ollama documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static OllamaModel GetFallbackModel(this OllamaModel model)
        {
            return model switch
            {
                OllamaModel.Qwen25Coder32B => OllamaModel.Qwen25Coder14B,
                OllamaModel.Qwen25Coder14B => OllamaModel.Qwen25Coder7B,
                OllamaModel.Gemma3_27B => OllamaModel.Gemma3_12B,
                OllamaModel.Gemma3_12B => OllamaModel.Llama31_8B,
                OllamaModel.Llama31_8B => OllamaModel.Mistral7B,
                _ => model
            };
        }

        #endregion
    }
}