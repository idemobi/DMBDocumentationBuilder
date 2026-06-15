#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace DMBDocumentationTest
{
    /// <summary>
    ///     Covers nested route resources, route constraints, and query flags for OpenAPI documentation tests.
    /// </summary>
    [ApiController]
    [Route("api/organizations/{organizationId:guid}/projects")]
    [Produces("application/json")]
    public sealed class DocumentationTestProjectsController : ControllerBase
    {
        #region Instance methods

        /// <summary>
        ///     Creates a nested project resource.
        /// </summary>
        /// <param name="organizationId">The organization identifier route token.</param>
        /// <param name="request">The project creation request body.</param>
        /// <returns>The created project.</returns>
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(DocumentationTestProjectDto), StatusCodes.Status201Created)]
        public ActionResult<DocumentationTestProjectDto> CreateProject(
            [FromRoute] Guid organizationId,
            [FromBody] CreateDocumentationTestProjectRequest request
        )
        {
            DocumentationTestProjectDto created = new(
                organizationId,
                Guid.NewGuid(),
                request.Name,
                request.IsArchived);

            return CreatedAtRoute(
                "DocumentationTestGetProject",
                new { organizationId, projectId = created.ProjectId },
                created);
        }

        /// <summary>
        ///     Gets one nested project resource.
        /// </summary>
        /// <param name="organizationId">The organization identifier route token.</param>
        /// <param name="projectId">The project identifier route token.</param>
        /// <returns>The matching project.</returns>
        [HttpGet("{projectId:guid}", Name = "DocumentationTestGetProject")]
        [ProducesResponseType(typeof(DocumentationTestProjectDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public ActionResult<DocumentationTestProjectDto> GetProject(
            [FromRoute] Guid organizationId,
            [FromRoute] Guid projectId
        )
        {
            return Ok(new DocumentationTestProjectDto(
                organizationId,
                projectId,
                "Nested project",
                false));
        }

        /// <summary>
        ///     Lists projects that belong to an organization.
        /// </summary>
        /// <param name="organizationId">The organization identifier route token.</param>
        /// <param name="includeArchived">A value indicating whether archived projects should be included.</param>
        /// <returns>The projects owned by the organization.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<DocumentationTestProjectDto>), StatusCodes.Status200OK)]
        public ActionResult<IReadOnlyList<DocumentationTestProjectDto>> ListProjects(
            [FromRoute] Guid organizationId,
            [FromQuery] bool includeArchived = false
        )
        {
            DocumentationTestProjectDto project = new(
                organizationId,
                Guid.Parse("22222222-2222-2222-2222-222222222222"),
                includeArchived ? "Archived sample" : "Active sample",
                includeArchived);

            return Ok(new[] { project });
        }

        #endregion
    }
}