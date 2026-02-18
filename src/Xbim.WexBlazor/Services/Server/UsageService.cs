using Microsoft.Extensions.Logging;
using Xbim.WexBlazor.Services.Abstractions.Server;
using Xbim.Server.Client;

namespace Xbim.WexBlazor.Services.Server;

/// <summary>
/// Server-backed implementation of <see cref="IUsageService"/>.
/// <para>
/// Wraps the generated <see cref="IXbimApiClient"/> to provide storage usage operations.
/// All API errors are wrapped in <see cref="XbimServiceException"/> for predictable error handling.
/// </para>
/// </summary>
public class UsageService : IUsageService
{
    private readonly IXbimApiClient _client;
    private readonly ILogger<UsageService>? _logger;

    /// <summary>
    /// Creates a new UsageService.
    /// </summary>
    /// <param name="client">The Xbim API client.</param>
    /// <param name="logger">Optional logger.</param>
    public UsageService(IXbimApiClient client, ILogger<UsageService>? logger = null)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task GetWorkspaceUsageAsync(Guid workspaceId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogDebug("Getting usage for workspace {WorkspaceId}", workspaceId);
            await _client.GetWorkspaceUsageAsync(workspaceId, cancellationToken);
            _logger?.LogDebug("Retrieved usage for workspace {WorkspaceId}", workspaceId);
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to get usage for workspace {WorkspaceId}: {StatusCode}", workspaceId, ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }

    /// <inheritdoc />
    public async Task GetProjectUsageAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogDebug("Getting usage for project {ProjectId}", projectId);
            await _client.GetProjectUsageAsync(projectId, cancellationToken);
            _logger?.LogDebug("Retrieved usage for project {ProjectId}", projectId);
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to get usage for project {ProjectId}: {StatusCode}", projectId, ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }
}
