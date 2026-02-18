using Xbim.Api.Client;

namespace Xbim.WexBlazor.Services.Abstractions.Server;

/// <summary>
/// Service interface for file operations.
/// <para>
/// Requires Xbim.Server connectivity. Implementations typically wrap the generated
/// <see cref="IXbimApiClient"/> to provide a higher-level API.
/// </para>
/// </summary>
public interface IFilesService
{
    /// <summary>
    /// Reserves an upload session for a new file.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="request">The upload reservation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The upload session details.</returns>
    Task<ReserveUploadResponse> ReserveUploadAsync(Guid projectId, ReserveUploadRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an upload session's current status.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="sessionId">The upload session ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The upload session.</returns>
    Task<UploadSessionDto> GetUploadSessionAsync(Guid projectId, Guid sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads file content to a reserved session.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="sessionId">The upload session ID.</param>
    /// <param name="file">The file to upload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The upload response.</returns>
    Task<UploadContentResponse> UploadContentAsync(Guid projectId, Guid sessionId, FileParameter file, CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits an upload session, creating the file record.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="sessionId">The upload session ID.</param>
    /// <param name="request">Optional commit request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The commit response with the created file.</returns>
    Task<CommitUploadResponse> CommitUploadAsync(Guid projectId, Guid sessionId, CommitUploadRequest? request = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a file by ID.
    /// </summary>
    /// <param name="fileId">The file ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The file, or null if not found.</returns>
    Task<FileDto?> GetAsync(Guid fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists files in a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="kind">Optional filter by file kind.</param>
    /// <param name="category">Optional filter by file category.</param>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged list of files.</returns>
    Task<FileDtoPagedList> ListAsync(Guid projectId, FileKind? kind = null, FileCategory? category = null, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads file content.
    /// </summary>
    /// <param name="fileId">The file ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A response containing the file content stream.</returns>
    Task<FileResponse> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft deletes a file.
    /// </summary>
    /// <param name="fileId">The file ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The deleted file.</returns>
    Task<FileDto> DeleteAsync(Guid fileId, CancellationToken cancellationToken = default);
}
