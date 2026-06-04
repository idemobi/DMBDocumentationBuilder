#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

using System;

namespace DMBDocumentationImprovementByClaude
{
    /// <summary>
    ///     Provides helper methods for Claude model identifiers and defaults.
    /// </summary>
    public static class ClaudeModelHelper
    {
        #region Public methods

        /// <summary>
        ///     Executes the ToModelString operation for Claude documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static string ToModelString(this ClaudeModel model)
        {
            return model switch
            {
                ClaudeModel.ClaudeOpus46 => "claude-opus-4-6",
                ClaudeModel.ClaudeSonnet46 => "claude-sonnet-4-6",
                ClaudeModel.ClaudeHaiku45 => "claude-haiku-4-5",
                _ => throw new ArgumentOutOfRangeException(nameof(model), model, null)
            };
        }

        /// <summary>
        ///     Executes the GetRecommendedDefault operation for Claude documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static ClaudeModel GetRecommendedDefault()
        {
            return ClaudeModel.ClaudeSonnet46;
        }

        /// <summary>
        ///     Executes the GetFallbackModel operation for Claude documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static ClaudeModel GetFallbackModel(this ClaudeModel model)
        {
            return model switch
            {
                ClaudeModel.ClaudeOpus46 => ClaudeModel.ClaudeSonnet46,
                ClaudeModel.ClaudeSonnet46 => ClaudeModel.ClaudeHaiku45,
                _ => model
            };
        }

        /// <summary>
        ///     Executes the IsFastModel operation for Claude documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static bool IsFastModel(this ClaudeModel model)
        {
            return model switch
            {
                ClaudeModel.ClaudeHaiku45 => true,
                _ => false
            };
        }

        /// <summary>
        ///     Executes the IsHighQualityModel operation for Claude documentation improvement.
        /// </summary>
        /// <returns>The operation result.</returns>
        public static bool IsHighQualityModel(this ClaudeModel model)
        {
            return model switch
            {
                ClaudeModel.ClaudeOpus46 => true,
                ClaudeModel.ClaudeSonnet46 => true,
                _ => false
            };
        }

        #endregion
    }
}