using Xbim.WexBlazor.Services.Abstractions.Server;
using Xbim.Api.Client;

namespace Xbim.WexBlazor.Services.Server.Guards;

/// <summary>
/// Guard implementation of <see cref="IWorkspacesService"/> that throws
/// <see cref="ServerServiceNotConfiguredException"/> on any method call.
/// <para>
/// This implementation is registered in standalone mode to provide clear error messages
/// when server-only functionality is accidentally used.
/// </para>
/// </summary>
internal sealed class NotConfiguredWorkspacesService : IWorkspacesService
{
    private static ServerServiceNotConfiguredException CreateException() =>
        new(typeof(IWorkspacesService));

    /// <inheritdoc />
    public Task<WorkspaceDto> CreateAsync(CreateWorkspaceRequest request, CancellationToken cancellationToken = default)
        => throw CreateException();

    /// <inheritdoc />
    public Task<WorkspaceDto?> GetAsync(Guid workspaceId, CancellationToken cancellationToken = default)
        => throw CreateException();

    /// <inheritdoc />
    public Task<WorkspaceDtoPagedList> ListAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        => throw CreateException();

    /// <inheritdoc />
    public Task<WorkspaceDto> UpdateAsync(Guid workspaceId, UpdateWorkspaceRequest request, CancellationToken cancellationToken = default)
        => throw CreateException();
}
