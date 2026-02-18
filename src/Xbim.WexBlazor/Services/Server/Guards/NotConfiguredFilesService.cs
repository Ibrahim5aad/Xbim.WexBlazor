using Xbim.WexBlazor.Services.Abstractions.Server;
using Xbim.WexServer.Client;

namespace Xbim.WexBlazor.Services.Server.Guards;

/// <summary>
/// Guard implementation of <see cref="IFilesService"/> that throws
/// <see cref="ServerServiceNotConfiguredException"/> on any method call.
/// <para>
/// This implementation is registered in standalone mode to provide clear error messages
/// when server-only functionality is accidentally used.
/// </para>
/// </summary>
internal sealed class NotConfiguredFilesService : IFilesService
{
    private static ServerServiceNotConfiguredException CreateException() =>
        new(typeof(IFilesService));

    /// <inheritdoc />
    public Task<ReserveUploadResponse> ReserveUploadAsync(Guid projectId, ReserveUploadRequest request, CancellationToken cancellationToken = default)
        => throw CreateException();

    /// <inheritdoc />
    public Task<UploadSessionDto> GetUploadSessionAsync(Guid projectId, Guid sessionId, CancellationToken cancellationToken = default)
        => throw CreateException();

    /// <inheritdoc />
    public Task<UploadContentResponse> UploadContentAsync(Guid projectId, Guid sessionId, FileParameter file, CancellationToken cancellationToken = default)
        => throw CreateException();

    /// <inheritdoc />
    public Task<CommitUploadResponse> CommitUploadAsync(Guid projectId, Guid sessionId, CommitUploadRequest? request = null, CancellationToken cancellationToken = default)
        => throw CreateException();

    /// <inheritdoc />
    public Task<FileDto?> GetAsync(Guid fileId, CancellationToken cancellationToken = default)
        => throw CreateException();

    /// <inheritdoc />
    public Task<FileDtoPagedList> ListAsync(Guid projectId, FileKind? kind = null, FileCategory? category = null, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        => throw CreateException();

    /// <inheritdoc />
    public Task<FileResponse> DownloadAsync(Guid fileId, CancellationToken cancellationToken = default)
        => throw CreateException();

    /// <inheritdoc />
    public Task<FileDto> DeleteAsync(Guid fileId, CancellationToken cancellationToken = default)
        => throw CreateException();
}
