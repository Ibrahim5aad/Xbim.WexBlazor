using Xbim.WexBlazor.Services.Abstractions.Server;
using Xbim.Server.Client;

namespace Xbim.WexBlazor.Services.Server.Guards;

/// <summary>
/// Guard implementation of <see cref="IProjectsService"/> that throws
/// <see cref="ServerServiceNotConfiguredException"/> on any method call.
/// <para>
/// This implementation is registered in standalone mode to provide clear error messages
/// when server-only functionality is accidentally used.
/// </para>
/// </summary>
internal sealed class NotConfiguredProjectsService : IProjectsService
{
    private static ServerServiceNotConfiguredException CreateException() =>
        new(typeof(IProjectsService));

    /// <inheritdoc />
    public Task<ProjectDto> CreateAsync(Guid workspaceId, CreateProjectRequest request, CancellationToken cancellationToken = default)
        => throw CreateException();

    /// <inheritdoc />
    public Task<ProjectDto?> GetAsync(Guid projectId, CancellationToken cancellationToken = default)
        => throw CreateException();

    /// <inheritdoc />
    public Task<ProjectDtoPagedList> ListAsync(Guid workspaceId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        => throw CreateException();

    /// <inheritdoc />
    public Task<ProjectDto> UpdateAsync(Guid projectId, UpdateProjectRequest request, CancellationToken cancellationToken = default)
        => throw CreateException();
}
