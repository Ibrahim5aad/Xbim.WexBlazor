using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Xbim.WexBlazor.Services.Abstractions.Server;
using Xbim.WexServer.Client;

namespace Xbim.WexBlazor.Services.Server;

/// <summary>
/// Server-backed implementation of <see cref="IProcessingService"/>.
/// <para>
/// Provides processing status tracking with polling support. Uses a background polling
/// mechanism to watch model versions and raise events when their processing status changes.
/// </para>
/// <para>
/// All API errors are wrapped in <see cref="XbimServiceException"/> for predictable error handling.
/// </para>
/// </summary>
public class ProcessingService : IProcessingService, IDisposable
{
    private readonly IXbimApiClient _client;
    private readonly ILogger<ProcessingService>? _logger;
    private readonly ConcurrentDictionary<Guid, WatchState> _watchedVersions = new();
    private bool _disposed;

    /// <inheritdoc />
    public event Action<ModelVersionStatusChangedEventArgs>? OnStatusChanged;

    /// <inheritdoc />
    public IReadOnlyCollection<Guid> WatchedVersions => _watchedVersions.Keys.ToArray();

    /// <summary>
    /// Creates a new ProcessingService.
    /// </summary>
    /// <param name="client">The Xbim API client.</param>
    /// <param name="logger">Optional logger.</param>
    public ProcessingService(IXbimApiClient client, ILogger<ProcessingService>? logger = null)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ModelVersionDto?> GetStatusAsync(Guid versionId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogDebug("Getting status for model version {VersionId}", versionId);
            return await _client.GetModelVersionAsync(versionId, cancellationToken);
        }
        catch (XbimApiException ex) when (ex.StatusCode == 404)
        {
            _logger?.LogDebug("Model version {VersionId} not found", versionId);
            return null;
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to get status for model version {VersionId}: {StatusCode}", versionId, ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }

    /// <inheritdoc />
    public void StartWatching(Guid versionId, int pollingIntervalMs = 2000)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (pollingIntervalMs < 100)
        {
            throw new ArgumentOutOfRangeException(nameof(pollingIntervalMs), "Polling interval must be at least 100ms.");
        }

        if (_watchedVersions.ContainsKey(versionId))
        {
            _logger?.LogDebug("Already watching model version {VersionId}", versionId);
            return;
        }

        var cts = new CancellationTokenSource();
        var state = new WatchState(cts, pollingIntervalMs);

        if (_watchedVersions.TryAdd(versionId, state))
        {
            _logger?.LogInformation("Started watching model version {VersionId} with {IntervalMs}ms polling", versionId, pollingIntervalMs);
            _ = PollStatusAsync(versionId, state);
        }
        else
        {
            cts.Dispose();
        }
    }

    /// <inheritdoc />
    public void StopWatching(Guid versionId)
    {
        if (_watchedVersions.TryRemove(versionId, out var state))
        {
            _logger?.LogInformation("Stopped watching model version {VersionId}", versionId);
            state.Cts.Cancel();
            state.Cts.Dispose();
        }
    }

    /// <inheritdoc />
    public void StopWatchingAll()
    {
        _logger?.LogInformation("Stopping all model version watchers ({Count} active)", _watchedVersions.Count);

        foreach (var versionId in _watchedVersions.Keys.ToArray())
        {
            StopWatching(versionId);
        }
    }

    private async Task PollStatusAsync(Guid versionId, WatchState state)
    {
        ProcessingStatus? lastStatus = null;

        try
        {
            // Get initial status
            var version = await GetStatusAsync(versionId, state.Cts.Token);
            if (version != null)
            {
                lastStatus = version.Status;
            }
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Failed to get initial status for model version {VersionId}", versionId);
        }

        while (!state.Cts.Token.IsCancellationRequested && _watchedVersions.ContainsKey(versionId))
        {
            try
            {
                await Task.Delay(state.PollingIntervalMs, state.Cts.Token);

                var version = await GetStatusAsync(versionId, state.Cts.Token);
                if (version == null)
                {
                    _logger?.LogWarning("Model version {VersionId} no longer exists, stopping watcher", versionId);
                    StopWatching(versionId);
                    break;
                }

                if (version.Status != lastStatus)
                {
                    _logger?.LogDebug("Status changed for model version {VersionId}: {OldStatus} -> {NewStatus}",
                        versionId, lastStatus, version.Status);

                    var args = new ModelVersionStatusChangedEventArgs
                    {
                        VersionId = versionId,
                        PreviousStatus = lastStatus ?? ProcessingStatus._0,
                        NewStatus = version.Status ?? ProcessingStatus._0,
                        ErrorMessage = version.ErrorMessage
                    };

                    lastStatus = version.Status;

                    // Raise event
                    try
                    {
                        OnStatusChanged?.Invoke(args);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Error in OnStatusChanged handler for model version {VersionId}", versionId);
                    }

                    // Stop watching if processing is complete (Ready or Failed)
                    // ProcessingStatus: _0 = Pending, _1 = Processing, _2 = Ready, _3 = Failed
                    if (version.Status == ProcessingStatus._2 || version.Status == ProcessingStatus._3)
                    {
                        _logger?.LogInformation("Processing complete for model version {VersionId} with status {Status}, stopping watcher",
                            versionId, version.Status);
                        StopWatching(versionId);
                        break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (XbimServiceException ex) when (ex.IsUnauthorized || ex.IsForbidden)
            {
                _logger?.LogWarning("Access denied while polling model version {VersionId}, stopping watcher", versionId);
                StopWatching(versionId);
                break;
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Error polling status for model version {VersionId}, will retry", versionId);
                // Continue polling on transient errors
            }
        }
    }

    /// <summary>
    /// Disposes the service, stopping all watchers.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        StopWatchingAll();
        GC.SuppressFinalize(this);
    }

    private sealed class WatchState
    {
        public CancellationTokenSource Cts { get; }
        public int PollingIntervalMs { get; }

        public WatchState(CancellationTokenSource cts, int pollingIntervalMs)
        {
            Cts = cts;
            PollingIntervalMs = pollingIntervalMs;
        }
    }
}
