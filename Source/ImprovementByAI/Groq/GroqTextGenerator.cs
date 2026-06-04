#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace DMBDocumentationImprovementByGroq
{
    internal sealed class GroqTextGenerator
    {
        #region Static methods

        #region Private methods

        private static string Cleanup(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            string result = value.Trim();

            if (result.StartsWith("```")) result = result.Trim('`').Trim();

            return result;
        }

        #endregion

        #endregion

        #region Instance constructors and destructors

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="GroqTextGenerator" /> class.
        /// </summary>
        /// <param name="model">The Groq model used to generate documentation text.</param>
        /// <param name="apiKey">The optional Groq API key; when omitted, <c>GROQ_API_KEY</c> is read from the environment.</param>
        public GroqTextGenerator(GroqModel model, string? apiKey = null)
        {
            apiKey ??= Environment.GetEnvironmentVariable("GROQ_API_KEY");

            if (string.IsNullOrWhiteSpace(apiKey)) throw new InvalidOperationException("GROQ_API_KEY is missing.");

            _model = model.ToModelString();

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.groq.com/openai/v1/")
            };

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);
        }

        #endregion

        #endregion

        #region Instance methods

        #region Public methods

        /// <summary>
        ///     Generates text with the configured Groq model.
        /// </summary>
        /// <param name="prompt">The prompt sent to the model.</param>
        /// <param name="timeout">The maximum duration allowed for the request.</param>
        /// <param name="cancellationToken">A token used to cancel the generation request.</param>
        /// <returns>The generated text returned by the model.</returns>
        public async Task<string> GenerateTextAsync(
            string prompt,
            TimeSpan timeout,
            CancellationToken cancellationToken
        )
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(timeout);

            var payload = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                temperature = 0.2
            };

            string json = JsonSerializer.Serialize(payload);

            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await _httpClient.PostAsync(
                "chat/completions",
                content,
                cts.Token);

            response.EnsureSuccessStatusCode();

            string raw = await response.Content.ReadAsStringAsync(cts.Token);

            using JsonDocument doc = JsonDocument.Parse(raw);

            string? result =
                doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

            return Cleanup(result);
        }

        #endregion

        #endregion

        #region Fields

        private readonly HttpClient _httpClient;
        private readonly string _model;

        #endregion
    }
}