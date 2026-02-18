using Microsoft.Extensions.Logging;
using Xbim.WexBlazor.Services.Abstractions.Server;
using Xbim.WexServer.Client;

namespace Xbim.WexBlazor.Services.Server;

/// <summary>
/// Server-backed implementation of <see cref="IFilesService"/>.
/// <para>
/// Wraps the generated <see cref="IXbimApiClient"/> to provide file operations.
/// All API errors are wrapped in <see cref="XbimServiceException"/> for predictable error handling.
/// </para>
/// </summary>
public class FilesService : IFilesService
{
    private readonly IXbimApiClient _client;
    private readonly ILogger<FilesService>? _logger;

    /// <summary>
    /// Creates a new FilesService.
    /// </summary>
    /// <param name="client">The Xbim API client.</param>
    /// <param name="logger">Optional logger.</param>
    public FilesService(IXbimApiClient client, ILogger<FilesService>? logger = null)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ReserveUploadResponse> ReserveUploadAsync(Guid projectId, ReserveUploadRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            _logger?.LogDebug("Reserving upload for project {ProjectId}, file: {FileName}", projectId, request.FileName);
            var result = await _client.ReserveUploadAsync(projectId, request, cancellationToken);
            _logger?.LogInformation("Reserved upload session {SessionId} for project {ProjectId}", result.Session?.Id, projectId);
            return result;
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to reserve upload for project {ProjectId}: {StatusCode}", projectId, ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }

    /// <inheritdoc />
    public async Task<UploadSessionDto> GetUploadSessionAsync(Guid projectId, Guid sessionId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogDebug("Getting upload session {SessionId}", sessionId);
            return await _client.GetUploadSessionAsync(projectId, sessionId, cancellationToken);
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to get upload session {SessionId}: {StatusCode}", sessionId, ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }

    /// <inheritdoc />
    public async Task<UploadContentResponse> UploadContentAsync(Guid projectId, Guid sessionId, FileParameter file, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        try
        {
            _logger?.LogDebug("Uploading content to session {SessionId}, file: {FileName}", sessionId, file.FileName);
            var result = await _client.UploadContentAsync(projectId, sessionId, file, cancellationToken);
            _logger?.LogInformation("Uploaded {BytesUploaded} bytes to session {SessionId}", result.BytesUploaded, sessionId);
            return result;
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to upload content to session {SessionId}: {StatusCode}", sessionId, ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }

    /// <inheritdoc />
    public async Task<CommitUploadResponse> CommitUploadAsync(Guid projectId, Guid sessionId, CommitUploadRequest? request = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogDebug("Committing upload session {SessionId}", sessionId);
            var result = await _client.CommitUploadAsync(projectId, sessionId, request, cancellationToken);
            _logger?.LogInformation("Committed upload session {SessionId}, created file {FileId}", sessionId, result.File?.Id);
            return result;
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to commit upload session {SessionId}: {StatusCode}", sessionId, ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }

    /// <inheritdoc />
    public async Task<FileDto?> GetAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogDebug("Getting file {FileId}", fileId);
            return await _client.GetFileAsync(fileId, cancellationToken);
        }
        catch (XbimApiException ex) when (ex.StatusCode == 404)
        {
            _logger?.LogDebug("File {FileId} not found", fileId);
            return null;
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to get file {FileId}: {StatusCode}", fileId, ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }

    /// <inheritdoc />
    public async Task<FileDtoPagedList> ListAsync(Guid projectId, FileKind? kind = null, FileCategory? category = null, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogDebug("Listing files in project {ProjectId}, kind: {Kind}, category: {Category}, page: {Page}",
                projectId, kind, category, page);
            return await _client.ListFilesAsync(projectId, kind, category, page, pageSize, cancellationToken);
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to list files in project {ProjectId}: {StatusCode}", projectId, ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }

    /// <inheritdoc />
    public async Task<FileResponse> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogDebug("Downloading file {FileId}", fileId);
            return await _client.GetFileContentAsync(fileId, cancellationToken);
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to download file {FileId}: {StatusCode}", fileId, ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }

    /// <inheritdoc />
    public async Task<FileDto> DeleteAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogDebug("Deleting file {FileId}", fileId);
            var result = await _client.DeleteFileAsync(fileId, cancellationToken);
            _logger?.LogInformation("Deleted file {FileId}", fileId);
            return result;
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to delete file {FileId}: {StatusCode}", fileId, ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }
}
