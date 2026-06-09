#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Text;
using System.Text.Json;

#endregion

namespace DMBDocumentationImprovementByClaude
{
    internal sealed class ClaudeTextGenerator
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
        ///     Initializes a new instance of the <see cref="ClaudeTextGenerator" /> class.
        /// </summary>
        public ClaudeTextGenerator(ClaudeModel model, string? apiKey = null)
        {
            apiKey ??= Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");

            if (string.IsNullOrWhiteSpace(apiKey)) throw new InvalidOperationException("ANTHROPIC_API_KEY is missing.");

            _model = model.ToModelString();

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.anthropic.com/v1/")
            };

            _httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
            _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        }

        #endregion

        #endregion

        #region Instance methods

        #region Public methods

        /// <summary>
        ///     Generates text with the configured Claude model.
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
                max_tokens = 512,
                temperature = 0.2,
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                }
            };

            string json = JsonSerializer.Serialize(payload);

            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await _httpClient.PostAsync(
                "messages",
                content,
                cts.Token);

            response.EnsureSuccessStatusCode();

            string raw = await response.Content.ReadAsStringAsync(cts.Token);

            using JsonDocument doc = JsonDocument.Parse(raw);

            string? result =
                doc.RootElement
                    .GetProperty("content")[0]
                    .GetProperty("text")
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