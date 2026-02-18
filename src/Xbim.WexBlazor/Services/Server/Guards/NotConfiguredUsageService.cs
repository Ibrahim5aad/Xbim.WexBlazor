using Xbim.WexBlazor.Services.Abstractions.Server;

namespace Xbim.WexBlazor.Services.Server.Guards;

/// <summary>
/// Guard implementation of <see cref="IUsageService"/> that throws
/// <see cref="ServerServiceNotConfiguredException"/> on any method call.
/// <para>
/// This implementation is registered in standalone mode to provide clear error messages
/// when server-only functionality is accidentally used.
/// </para>
/// </summary>
internal sealed class NotConfiguredUsageService : IUsageService
{
    private static ServerServiceNotConfiguredException CreateException() =>
        new(typeof(IUsageService));

    /// <inheritdoc />
    public Task GetWorkspaceUsageAsync(Guid workspaceId, CancellationToken cancellationToken = default)
        => throw CreateException();

    /// <inheritdoc />
    public Task GetProjectUsageAsync(Guid projectId, CancellationToken cancellationToken = default)
        => throw CreateException();
}
