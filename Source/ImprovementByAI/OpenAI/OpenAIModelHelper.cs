#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationImprovementByOpenAI
{
    /// <summary>
    ///     Provides helper methods for OpenAI model identifiers and defaults.
    /// </summary>
    public static class OpenAIModelHelper
    {
        #region Public methods

        /// <summary>
        ///     Executes the ToModelString operation for OpenAI documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static string ToModelString(this OpenAIModel model)
        {
            return model switch
            {
                OpenAIModel.Gpt54 => "gpt-5.4",
                OpenAIModel.Gpt54Mini => "gpt-5.4-mini",
                OpenAIModel.Gpt54Nano => "gpt-5.4-nano",
                _ => throw new ArgumentOutOfRangeException(nameof(model), model, null)
            };
        }

        /// <summary>
        ///     Executes the GetFallbackModel operation for OpenAI documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static OpenAIModel GetFallbackModel(this OpenAIModel model)
        {
            return model switch
            {
                OpenAIModel.Gpt54 => OpenAIModel.Gpt54Mini,
                OpenAIModel.Gpt54Mini => OpenAIModel.Gpt54Nano,
                _ => model
            };
        }

        /// <summary>
        ///     Executes the GetRecommendedDefault operation for OpenAI documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static OpenAIModel GetRecommendedDefault()
        {
            return OpenAIModel.Gpt54Mini;
        }

        #endregion
    }
}