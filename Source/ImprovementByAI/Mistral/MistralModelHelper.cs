#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

using System;

namespace DMBDocumentationImprovementByMistral
{
    /// <summary>
    ///     Provides helper methods for Mistral model identifiers and defaults.
    /// </summary>
    public static class MistralModelHelper
    {
        #region Public methods

        /// <summary>
        ///     Executes the ToModelString operation for Mistral documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static string ToModelString(this MistralModel model)
        {
            return model switch
            {
                MistralModel.MistralLargeLatest => "mistral-large-latest",
                MistralModel.MistralMediumLatest => "mistral-medium-latest",
                MistralModel.Ministral3BLatest => "ministral-3b-latest",
                MistralModel.Ministral8BLatest => "ministral-8b-latest",
                MistralModel.CodestralLatest => "codestral-latest",
                MistralModel.DevstralSmallLatest => "devstral-small-latest",
                _ => throw new ArgumentOutOfRangeException(nameof(model), model, null)
            };
        }

        /// <summary>
        ///     Executes the GetRecommendedDefault operation for Mistral documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static MistralModel GetRecommendedDefault()
        {
            return MistralModel.MistralMediumLatest;
        }

        /// <summary>
        ///     Executes the GetFallbackModel operation for Mistral documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static MistralModel GetFallbackModel(this MistralModel model)
        {
            return model switch
            {
                MistralModel.MistralLargeLatest => MistralModel.MistralMediumLatest,
                MistralModel.MistralMediumLatest => MistralModel.Ministral8BLatest,
                MistralModel.CodestralLatest => MistralModel.DevstralSmallLatest,
                MistralModel.Ministral8BLatest => MistralModel.Ministral3BLatest,
                _ => model
            };
        }

        /// <summary>
        ///     Executes the IsCodingModel operation for Mistral documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static bool IsCodingModel(this MistralModel model)
        {
            return model switch
            {
                MistralModel.CodestralLatest => true,
                MistralModel.DevstralSmallLatest => true,
                _ => false
            };
        }

        #endregion
    }
}