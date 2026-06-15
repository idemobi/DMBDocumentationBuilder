#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace DMBDocumentationTest
{
    /// <summary>
    ///     Covers common REST resource operations for OpenAPI documentation generation tests.
    /// </summary>
    [ApiController]
    [Route("api/v{version}/players")]
    [Produces("application/json")]
    public sealed class DocumentationTestPlayersController : ControllerBase
    {
        #region Static fields and properties

        private static readonly DocumentationTestPlayerDto SamplePlayer = new(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Ada Lovelace",
            "ada@example.test",
            DocumentationTestPlayerStatus.Active,
            ["founder", "analytics"],
            DateTimeOffset.Parse("2026-05-21T00:00:00Z"));

        #endregion

        #region Instance methods

        /// <summary>
        ///     Creates a player from a JSON request body.
        /// </summary>
        /// <param name="version">The API version route token.</param>
        /// <param name="request">The player creation request body.</param>
        /// <returns>The created player and a location header.</returns>
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(DocumentationTestPlayerDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<DocumentationTestPlayerDto> CreatePlayer(
            [FromRoute] string version,
            [FromBody] CreateDocumentationTestPlayerRequest request
        )
        {
            DocumentationTestPlayerDto created = new(
                Guid.NewGuid(),
                request.DisplayName,
                request.Email,
                DocumentationTestPlayerStatus.Active,
                request.Tags,
                DateTimeOffset.UtcNow);

            return CreatedAtAction(
                nameof(GetPlayer),
                new { version, playerId = created.Id },
                created);
        }

        /// <summary>
        ///     Deletes a player resource.
        /// </summary>
        /// <param name="playerId">The player identifier route token.</param>
        /// <returns>No content when the delete operation succeeds.</returns>
        [HttpDelete("{playerId:guid}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public IActionResult DeletePlayer([FromRoute] Guid playerId)
        {
            _ = playerId;
            return NoContent();
        }

        /// <summary>
        ///     Gets one player by identifier.
        /// </summary>
        /// <param name="version">The API version route token.</param>
        /// <param name="playerId">The player identifier route token.</param>
        /// <returns>The matching player, or a not-found problem response.</returns>
        [HttpGet("{playerId:guid}")]
        [ProducesResponseType(typeof(DocumentationTestPlayerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public ActionResult<DocumentationTestPlayerDto> GetPlayer(
            [FromRoute] string version,
            [FromRoute] Guid playerId
        )
        {
            if (playerId != SamplePlayer.Id)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Player not found",
                    Detail = $"No player exists in API version {version} for id {playerId}.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok(SamplePlayer);
        }

        /// <summary>
        ///     Lists players with query string filtering and pagination.
        /// </summary>
        /// <param name="version">The API version route token.</param>
        /// <param name="query">The query string search and paging parameters.</param>
        /// <param name="correlationId">The optional request correlation identifier.</param>
        /// <param name="clientVersion">The optional client version header.</param>
        /// <param name="cancellationToken">A token that cancels the asynchronous operation.</param>
        /// <returns>A paged player response.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(DocumentationTestPagedResponse<DocumentationTestPlayerDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<DocumentationTestPagedResponse<DocumentationTestPlayerDto>>> ListPlayers(
            [FromRoute] string version,
            [FromQuery] DocumentationTestPlayerSearchQuery query,
            [FromHeader(Name = "X-Correlation-Id")]
            string? correlationId,
            [FromHeader(Name = "X-Client-Version")]
            string? clientVersion,
            CancellationToken cancellationToken
        )
        {
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();
            _ = clientVersion;

            DocumentationTestPlayerDto player = SamplePlayer with
            {
                Tags = string.IsNullOrWhiteSpace(correlationId)
                    ? SamplePlayer.Tags
                    : [.. SamplePlayer.Tags, correlationId]
            };

            return Ok(new DocumentationTestPagedResponse<DocumentationTestPlayerDto>(
                [player],
                query.Page,
                query.PageSize,
                1));
        }

        /// <summary>
        ///     Applies a partial update to a player resource.
        /// </summary>
        /// <param name="playerId">The player identifier route token.</param>
        /// <param name="request">The partial update request body.</param>
        /// <returns>No content when the patch is accepted.</returns>
        [HttpPatch("{playerId:guid}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult PatchPlayer(
            [FromRoute] Guid playerId,
            [FromBody] PatchDocumentationTestPlayerRequest request
        )
        {
            _ = playerId;
            _ = request;
            return NoContent();
        }

        /// <summary>
        ///     Replaces a player resource with a full JSON request body.
        /// </summary>
        /// <param name="version">The API version route token.</param>
        /// <param name="playerId">The player identifier route token.</param>
        /// <param name="request">The replacement player request body.</param>
        /// <returns>The replaced player resource.</returns>
        [HttpPut("{playerId:guid}")]
        [Authorize(Policy = "DocumentationTestWriter")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(DocumentationTestPlayerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public ActionResult<DocumentationTestPlayerDto> ReplacePlayer(
            [FromRoute] string version,
            [FromRoute] Guid playerId,
            [FromBody] UpdateDocumentationTestPlayerRequest request
        )
        {
            DocumentationTestPlayerDto updated = new(
                playerId,
                request.DisplayName,
                "updated@example.test",
                request.Status,
                request.Tags,
                DateTimeOffset.UtcNow);

            return Ok(updated);
        }

        #endregion
    }
}