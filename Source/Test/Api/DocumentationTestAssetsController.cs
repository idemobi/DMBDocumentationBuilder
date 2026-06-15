#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace DMBDocumentationTest
{
    /// <summary>
    ///     Covers file upload, file download, and binary response metadata for OpenAPI documentation tests.
    /// </summary>
    [ApiController]
    [Route("api/assets")]
    public sealed class DocumentationTestAssetsController : ControllerBase
    {
        #region Instance methods

        /// <summary>
        ///     Downloads a generated text asset as a file response.
        /// </summary>
        /// <param name="assetId">The asset identifier route token.</param>
        /// <returns>A file response containing the generated asset content.</returns>
        [HttpGet("{assetId:guid}/download")]
        [Produces("text/plain")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public FileContentResult DownloadAsset([FromRoute] Guid assetId)
        {
            byte[] content = Encoding.UTF8.GetBytes($"Documentation test asset {assetId}");
            return File(content, "text/plain", $"asset-{assetId:N}.txt");
        }

        /// <summary>
        ///     Uploads one asset using a multipart form request.
        /// </summary>
        /// <param name="request">The multipart upload request.</param>
        /// <returns>The uploaded asset metadata.</returns>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DocumentationTestAssetUploadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<DocumentationTestAssetUploadResponse> UploadAsset(
            [FromForm] DocumentationTestAssetUploadRequest request
        )
        {
            if (request.File is null)
            {
                return BadRequest(new ValidationProblemDetails
                {
                    Title = "Missing file"
                });
            }

            DocumentationTestAssetUploadResponse response = new(
                Guid.NewGuid(),
                request.File.FileName,
                request.File.ContentType,
                request.File.Length);

            return Ok(response);
        }

        #endregion
    }
}