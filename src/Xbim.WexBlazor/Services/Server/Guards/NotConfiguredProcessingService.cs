using Xbim.WexBlazor.Services.Abstractions.Server;
using Xbim.Server.Client;

namespace Xbim.WexBlazor.Services.Server.Guards;

/// <summary>
/// Guard implementation of <see cref="IProcessingService"/> that throws
/// <see cref="ServerServiceNotConfiguredException"/> on any method call.
/// <para>
/// This implementation is registered in standalone mode to provide clear error messages
/// when server-only functionality is accidentally used.
/// </para>
/// </summary>
internal sealed class NotConfiguredProcessingService : IProcessingService
{
    private static ServerServiceNotConfiguredException CreateException() =>
        new(typeof(IProcessingService));

    /// <inheritdoc />
    public event Action<ModelVersionStatusChangedEventArgs>? OnStatusChanged
    {
        add => throw CreateException();
        remove => throw CreateException();
    }

    /// <inheritdoc />
    public Task<ModelVersionDto?> GetStatusAsync(Guid versionId, CancellationToken cancellationToken = default)
        => throw CreateException();

    /// <inheritdoc />
    public void StartWatching(Guid versionId, int pollingIntervalMs = 2000)
        => throw CreateException();

    /// <inheritdoc />
    public void StopWatching(Guid versionId)
        => throw CreateException();

    /// <inheritdoc />
    public void StopWatchingAll()
        => throw CreateException();

    /// <inheritdoc />
    public IReadOnlyCollection<Guid> WatchedVersions => throw CreateException();
}
