#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace DMBDocumentationTest.Api
{
    /// <summary>
    ///     Covers health, error, and explicit status code responses for OpenAPI documentation tests.
    /// </summary>
    [ApiController]
    [Route("api/diagnostics")]
    [Produces("application/json")]
    public sealed class DocumentationTestDiagnosticsController : ControllerBase
    {
        #region Instance methods

        /// <summary>
        ///     Returns a typed error response used to document non-problem error payloads.
        /// </summary>
        /// <param name="code">The error code route token.</param>
        /// <returns>A typed error response.</returns>
        [HttpGet("errors/{code}")]
        [ProducesResponseType(typeof(DocumentationTestErrorResponse), StatusCodes.Status400BadRequest)]
        public ActionResult<DocumentationTestErrorResponse> GetError([FromRoute] string code)
        {
            return BadRequest(new DocumentationTestErrorResponse(
                code,
                "The requested documentation test error was returned intentionally.",
                HttpContext.TraceIdentifier));
        }

        /// <summary>
        ///     Returns a standard problem details response.
        /// </summary>
        /// <param name="authorization">The bearer authorization header value.</param>
        /// <returns>A problem details response.</returns>
        [HttpGet("problem")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public ActionResult<ProblemDetails> GetProblem([FromHeader(Name = "Authorization")] string? authorization)
        {
            _ = authorization;

            return Conflict(new ProblemDetails
            {
                Title = "Documentation test conflict",
                Detail = "This endpoint intentionally returns a problem details payload.",
                Status = StatusCodes.Status409Conflict
            });
        }

        /// <summary>
        ///     Returns a no-body health response.
        /// </summary>
        /// <returns>No content when the test service is healthy.</returns>
        [HttpGet("health")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Health()
        {
            return NoContent();
        }

        #endregion
    }
}