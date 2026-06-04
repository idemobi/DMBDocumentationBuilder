#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

using System;

namespace DMBDocumentationImprovementByLMStudio
{
    /// <summary>
    ///     Provides helper methods for LM Studio model identifiers and defaults.
    /// </summary>
    public static class LMStudioModelHelper
    {
        #region Public methods

        /// <summary>
        ///     Executes the ToModelString operation for LMStudio documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static string ToModelString(this LMStudioModel model)
        {
            return model switch
            {
                LMStudioModel.DeepSeekCoder => "deepseek-coder",
                LMStudioModel.Llama3Instruct => "llama-3-instruct",
                LMStudioModel.Qwen2_5Coder => "qwen/qwen2.5-coder-14b",
                LMStudioModel.Gemma4 => "google/gemma-4-e4b",
                _ => throw new ArgumentOutOfRangeException(nameof(model), model, null)
            };
        }

        /// <summary>
        ///     Executes the ToStorageKey operation for LMStudio documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static string ToStorageKey(this LMStudioModel model)
        {
            return model switch
            {
                LMStudioModel.DeepSeekCoder => "DeepSeekCoder",
                LMStudioModel.Llama3Instruct => "Llama3Instruct",
                LMStudioModel.Qwen2_5Coder => "Qwen2_5Coder_14b",
                LMStudioModel.Gemma4 => "Gemma4",
                _ => throw new ArgumentOutOfRangeException(nameof(model), model, null)
            };
        }

        /// <summary>
        ///     Executes the GetRecommendedDefault operation for LMStudio documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static LMStudioModel GetRecommendedDefault()
        {
            return LMStudioModel.Gemma4;
        }

        #endregion
    }
}