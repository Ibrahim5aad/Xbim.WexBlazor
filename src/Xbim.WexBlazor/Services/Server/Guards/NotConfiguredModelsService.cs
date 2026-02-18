using Xbim.WexBlazor.Services.Abstractions.Server;
using Xbim.Server.Client;

namespace Xbim.WexBlazor.Services.Server.Guards;

/// <summary>
/// Guard implementation of <see cref="IModelsService"/> that throws
/// <see cref="ServerServiceNotConfiguredException"/> on any method call.
/// <para>
/// This implementation is registered in standalone mode to provide clear error messages
/// when server-only functionality is accidentally used.
/// </para>
/// </summary>
internal sealed class NotConfiguredModelsService : IModelsService
{
    private static ServerServiceNotConfiguredException CreateException() =>
        new(typeof(IModelsService));

    /// <inheritdoc />
    public Task<ModelDto> CreateAsync(Guid projectId, CreateModelRequest request, CancellationToken cancellationToken = default)
        => throw CreateException();

    /// <inheritdoc />
    public Task<ModelDto?> GetAsync(Guid modelId, CancellationToken cancellationToken = default)
        => throw CreateException();

    /// <inheritdoc />
    public Task<ModelDtoPagedList> ListAsync(Guid projectId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        => throw CreateException();

    /// <inheritdoc />
    public Task<ModelVersionDto> CreateVersionAsync(Guid modelId, CreateModelVersionRequest request, CancellationToken cancellationToken = default)
        => throw CreateException();

    /// <inheritdoc />
    public Task<ModelVersionDto?> GetVersionAsync(Guid versionId, CancellationToken cancellationToken = default)
        => throw CreateException();

    /// <inheritdoc />
    public Task<ModelVersionDtoPagedList> ListVersionsAsync(Guid modelId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        => throw CreateException();

    /// <inheritdoc />
    public Task<FileResponse> GetWexBimAsync(Guid versionId, CancellationToken cancellationToken = default)
        => throw CreateException();
}
