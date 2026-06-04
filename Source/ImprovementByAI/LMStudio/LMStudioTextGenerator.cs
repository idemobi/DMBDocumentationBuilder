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

namespace DMBDocumentationImprovementByLMStudio
{
    internal sealed class LMStudioTextGenerator
    {
        #region Static methods

        #region Private methods

        private static string Cleanup(string? value)
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

        private readonly HttpClient _httpClient;
        private readonly string _modelName;

        #endregion

        #region Instance constructors and destructors

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LMStudioTextGenerator" /> class.
        /// </summary>
        public LMStudioTextGenerator(LMStudioModel model, string baseUrl)
        {
            _modelName = model.ToModelString();

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = Timeout.InfiniteTimeSpan
            };

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", "lm-studio");
        }

        #endregion

        #endregion

        #region Instance methods

        #region Public methods

        /// <summary>
        ///     Generates text with the configured LMStudio model.
        /// </summary>
        /// <returns>The generated text returned by the model.</returns>
        public async Task<string> GenerateTextAsync(
            string prompt,
            TimeSpan timeout,
            CancellationToken cancellationToken
        )
        {
            using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            linkedCancellation.CancelAfter(timeout);

            var payload = new
            {
                model = _modelName,
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
                linkedCancellation.Token);

            response.EnsureSuccessStatusCode();

            string raw = await response.Content.ReadAsStringAsync(linkedCancellation.Token);

            using JsonDocument doc = JsonDocument.Parse(raw);

            string? result = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return Cleanup(result);
        }

        #endregion

        #endregion
    }
}