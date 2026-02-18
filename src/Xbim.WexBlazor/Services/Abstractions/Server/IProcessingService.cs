using Xbim.Server.Client;

namespace Xbim.WexBlazor.Services.Abstractions.Server;

/// <summary>
/// Service interface for tracking model version processing status.
/// <para>
/// Requires Xbim.Server connectivity. Implementations typically wrap the generated
/// <see cref="IXbimApiClient"/> to provide a higher-level API.
/// </para>
/// </summary>
public interface IProcessingService
{
    /// <summary>
    /// Event raised when a model version's processing status changes.
    /// </summary>
    event Action<ModelVersionStatusChangedEventArgs>? OnStatusChanged;

    /// <summary>
    /// Gets the current processing status of a model version.
    /// </summary>
    /// <param name="versionId">The model version ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The model version with current status.</returns>
    Task<ModelVersionDto?> GetStatusAsync(Guid versionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts watching a model version for status changes.
    /// <para>
    /// When the status changes, <see cref="OnStatusChanged"/> will be raised.
    /// </para>
    /// </summary>
    /// <param name="versionId">The model version ID.</param>
    /// <param name="pollingIntervalMs">Polling interval in milliseconds (default: 2000).</param>
    void StartWatching(Guid versionId, int pollingIntervalMs = 2000);

    /// <summary>
    /// Stops watching a model version.
    /// </summary>
    /// <param name="versionId">The model version ID.</param>
    void StopWatching(Guid versionId);

    /// <summary>
    /// Stops watching all model versions.
    /// </summary>
    void StopWatchingAll();

    /// <summary>
    /// Gets all model versions currently being watched.
    /// </summary>
    IReadOnlyCollection<Guid> WatchedVersions { get; }
}

/// <summary>
/// Event arguments for model version status changes.
/// </summary>
public class ModelVersionStatusChangedEventArgs : EventArgs
{
    /// <summary>
    /// The model version ID.
    /// </summary>
    public Guid VersionId { get; set; }

    /// <summary>
    /// The previous status.
    /// </summary>
    public ProcessingStatus PreviousStatus { get; set; }

    /// <summary>
    /// The new status.
    /// </summary>
    public ProcessingStatus NewStatus { get; set; }

    /// <summary>
    /// Error message if the new status is Failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
