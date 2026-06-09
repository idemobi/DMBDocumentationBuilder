#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

#endregion

namespace DMBDocumentationImprovementByMistral
{
    internal sealed class MistralTextGenerator
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
        ///     Initializes a new instance of the <see cref="MistralTextGenerator" /> class.
        /// </summary>
        public MistralTextGenerator(MistralModel model, string? apiKey = null)
        {
            apiKey ??= Environment.GetEnvironmentVariable("MISTRAL_API_KEY");

            if (string.IsNullOrWhiteSpace(apiKey)) throw new InvalidOperationException("MISTRAL_API_KEY is missing.");

            _model = model.ToModelString();

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.mistral.ai/v1/")
            };

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);
        }

        #endregion

        #endregion

        #region Instance methods

        #region Public methods

        /// <summary>
        ///     Generates text with the configured Mistral model.
        /// </summary>
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