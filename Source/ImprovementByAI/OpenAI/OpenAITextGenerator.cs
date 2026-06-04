#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenAI.Chat;

#endregion

namespace DMBDocumentationImprovementByOpenAI
{
    internal sealed class OpenAITextGenerator
    {
        #region Static methods

        #region Private methods

        private static string CleanupText(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            string result = value.Trim();

            if (result.StartsWith("```", StringComparison.Ordinal))
            {
                result = result.Trim('`').Trim();
            }

            result = result.Replace("\r\n", "\n", StringComparison.Ordinal);
            result = result.Replace("\r", "\n", StringComparison.Ordinal);

            while (result.Contains("\n\n\n", StringComparison.Ordinal))
            {
                result = result.Replace("\n\n\n", "\n\n", StringComparison.Ordinal);
            }

            return result.Trim();
        }

        #endregion

        #endregion

        #region Instance fields and properties

        private readonly ChatClient _client;
        private readonly string _modelName;

        #endregion

        #region Instance constructors and destructors

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="OpenAITextGenerator" /> class.
        /// </summary>
        public OpenAITextGenerator(OpenAIModel model, string? apiKey = null)
        {
            _modelName = model.ToModelString();

            apiKey ??= Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("OpenAI API key is missing. Set OPENAI_API_KEY or provide ApiKey in OpenAIOptions.");
            }

            _client = new ChatClient(_modelName, apiKey);
        }

        #endregion

        #endregion

        #region Instance methods

        #region Public methods

        /// <summary>
        ///     Generates text with the configured OpenAI model.
        /// </summary>
        /// <returns>The generated text returned by the model.</returns>
        public async Task<string> GenerateTextAsync(
            string prompt,
            TimeSpan requestTimeout,
            CancellationToken cancellationToken
        )
        {
            using var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            linkedCancellationTokenSource.CancelAfter(requestTimeout);

            List<ChatMessage> messages =
            [
                new UserChatMessage(prompt)
            ];

            ChatCompletion completion = await _client.CompleteChatAsync(
                messages,
                cancellationToken: linkedCancellationTokenSource.Token);

            if (completion.Content.Count == 0)
            {
                return string.Empty;
            }

            return CleanupText(completion.Content[0].Text);
        }

        #endregion

        #endregion
    }
}