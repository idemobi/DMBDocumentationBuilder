#region Copyright

// ©2002-2026 idéMobi
// www.idemobi.com

#endregion

#region

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

#endregion

namespace DMBDocumentationTest.Api
{
    /// <summary>
    ///     Defines the lifecycle status values used by documentation test API resources.
    /// </summary>
    public enum DocumentationTestPlayerStatus
    {
        /// <summary>
        ///     Indicates that the player is active and can access projects.
        /// </summary>
        Active,

        /// <summary>
        ///     Indicates that the player is temporarily suspended.
        /// </summary>
        Suspended,

        /// <summary>
        ///     Indicates that the player record is archived.
        /// </summary>
        Archived
    }

    /// <summary>
    ///     Represents one API player resource returned by the documentation test API.
    /// </summary>
    /// <param name="Id">The stable player identifier.</param>
    /// <param name="DisplayName">The display name shown in API responses.</param>
    /// <param name="Email">The contact email address.</param>
    /// <param name="Status">The lifecycle status.</param>
    /// <param name="Tags">The tags associated with the player.</param>
    /// <param name="UpdatedUtc">The last update timestamp.</param>
    public sealed record DocumentationTestPlayerDto(
        Guid Id,
        string DisplayName,
        string Email,
        DocumentationTestPlayerStatus Status,
        IReadOnlyList<string> Tags,
        DateTimeOffset UpdatedUtc
    );

    /// <summary>
    ///     Represents the request body used to create a documentation test player.
    /// </summary>
    public sealed class CreateDocumentationTestPlayerRequest
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the display name of the new player.
        /// </summary>
        [Required]
        [StringLength(120, MinimumLength = 2)]
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the email address of the new player.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the tags assigned to the new player.
        /// </summary>
        public List<string> Tags { get; set; } = [];

        #endregion
    }

    /// <summary>
    ///     Represents the request body used to update a documentation test player.
    /// </summary>
    public sealed class UpdateDocumentationTestPlayerRequest
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the updated display name.
        /// </summary>
        [Required]
        [StringLength(120, MinimumLength = 2)]
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the updated lifecycle status.
        /// </summary>
        public DocumentationTestPlayerStatus Status { get; set; }

        /// <summary>
        ///     Gets or sets the updated tags.
        /// </summary>
        public List<string> Tags { get; set; } = [];

        #endregion
    }

    /// <summary>
    ///     Represents a JSON Patch-like request body for partial player updates.
    /// </summary>
    public sealed class PatchDocumentationTestPlayerRequest
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the optional replacement display name.
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        ///     Gets or sets the optional replacement lifecycle status.
        /// </summary>
        public DocumentationTestPlayerStatus? Status { get; set; }

        /// <summary>
        ///     Gets or sets the optional replacement tags.
        /// </summary>
        public List<string>? Tags { get; set; }

        #endregion
    }

    /// <summary>
    ///     Represents query string filters used when listing documentation test players.
    /// </summary>
    public sealed class DocumentationTestPlayerSearchQuery
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the one-based page number.
        /// </summary>
        [Range(1, 500)]
        public int Page { get; set; } = 1;

        /// <summary>
        ///     Gets or sets the number of records requested per page.
        /// </summary>
        [Range(1, 100)]
        public int PageSize { get; set; } = 25;

        /// <summary>
        ///     Gets or sets the optional full-text search value.
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        ///     Gets or sets the optional lifecycle status filter.
        /// </summary>
        public DocumentationTestPlayerStatus? Status { get; set; }

        /// <summary>
        ///     Gets or sets the optional tag filters.
        /// </summary>
        public List<string> Tags { get; set; } = [];

        #endregion
    }

    /// <summary>
    ///     Represents a paged API response.
    /// </summary>
    /// <typeparam name="TItem">The item type returned by the page.</typeparam>
    /// <param name="Items">The page items.</param>
    /// <param name="Page">The one-based page number.</param>
    /// <param name="PageSize">The page size.</param>
    /// <param name="TotalCount">The total number of records.</param>
    public sealed record DocumentationTestPagedResponse<TItem>(
        IReadOnlyList<TItem> Items,
        int Page,
        int PageSize,
        int TotalCount
    );

    /// <summary>
    ///     Represents a lightweight project resource returned by the test API.
    /// </summary>
    /// <param name="OrganizationId">The organization identifier from the route.</param>
    /// <param name="ProjectId">The project identifier from the route.</param>
    /// <param name="Name">The project display name.</param>
    /// <param name="IsArchived">A value indicating whether the project is archived.</param>
    public sealed record DocumentationTestProjectDto(
        Guid OrganizationId,
        Guid ProjectId,
        string Name,
        bool IsArchived
    );

    /// <summary>
    ///     Represents the request body used to create an organization project.
    /// </summary>
    public sealed class CreateDocumentationTestProjectRequest
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets a value indicating whether the project starts archived.
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        ///     Gets or sets the project display name.
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        #endregion
    }

    /// <summary>
    ///     Represents a multipart form upload request for one API asset.
    /// </summary>
    public sealed class DocumentationTestAssetUploadRequest
    {
        #region Instance fields and properties

        /// <summary>
        ///     Gets or sets the uploaded file.
        /// </summary>
        [Required]
        public IFormFile? File { get; set; }

        /// <summary>
        ///     Gets or sets the optional public visibility flag.
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        ///     Gets or sets the optional asset title supplied with the multipart form.
        /// </summary>
        public string? Title { get; set; }

        #endregion
    }

    /// <summary>
    ///     Represents the response returned after an asset upload.
    /// </summary>
    /// <param name="AssetId">The generated asset identifier.</param>
    /// <param name="FileName">The uploaded file name.</param>
    /// <param name="ContentType">The uploaded content type.</param>
    /// <param name="Length">The uploaded file length.</param>
    public sealed record DocumentationTestAssetUploadResponse(
        Guid AssetId,
        string FileName,
        string ContentType,
        long Length
    );

    /// <summary>
    ///     Represents an error payload used by documented API endpoints.
    /// </summary>
    /// <param name="Code">The stable error code.</param>
    /// <param name="Message">The human-readable error message.</param>
    /// <param name="TraceId">The request trace identifier.</param>
    public sealed record DocumentationTestErrorResponse(
        string Code,
        string Message,
        string TraceId
    );
}