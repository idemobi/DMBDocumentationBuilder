#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

namespace DMBDocumentationImprovementByGroq
{
    /// <summary>
    ///     Provides helper methods for Groq model identifiers and defaults.
    /// </summary>
    public static class GroqModelHelper
    {
        #region Public methods

        /// <summary>
        ///     Executes the ToModelString operation for Groq documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static string ToModelString(this GroqModel model)
        {
            return model switch
            {
                GroqModel.Llama31_8B_Instant => "llama-3.1-8b-instant",
                GroqModel.Llama31_70B_Versatile => "llama-3.1-70b-versatile",

                GroqModel.Mixtral_8x7B => "mixtral-8x7b-32768",

                _ => throw new ArgumentOutOfRangeException(nameof(model), model, null)
            };
        }

        /// <summary>
        ///     Executes the IsFastModel operation for Groq documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static bool IsFastModel(this GroqModel model)
        {
            return model switch
            {
                GroqModel.Llama31_8B_Instant => true,
                _ => false
            };
        }

        /// <summary>
        ///     Executes the IsHighQualityModel operation for Groq documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static bool IsHighQualityModel(this GroqModel model)
        {
            return model switch
            {
                GroqModel.Llama31_70B_Versatile => true,
                _ => false
            };
        }

        /// <summary>
        ///     Executes the GetRecommendedDefault operation for Groq documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static GroqModel GetRecommendedDefault()
        {
            return GroqModel.Llama31_8B_Instant;
        }

        /// <summary>
        ///     Executes the GetFallbackModel operation for Groq documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static GroqModel GetFallbackModel(this GroqModel model)
        {
            return model switch
            {
                GroqModel.Llama31_70B_Versatile => GroqModel.Llama31_8B_Instant,
                _ => model
            };
        }

        #endregion
    }
}