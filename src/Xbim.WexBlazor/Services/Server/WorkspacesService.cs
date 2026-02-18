using Microsoft.Extensions.Logging;
using Xbim.WexBlazor.Services.Abstractions.Server;
using Xbim.Api.Client;

namespace Xbim.WexBlazor.Services.Server;

/// <summary>
/// Server-backed implementation of <see cref="IWorkspacesService"/>.
/// <para>
/// Wraps the generated <see cref="IXbimApiClient"/> to provide workspace operations.
/// All API errors are wrapped in <see cref="XbimServiceException"/> for predictable error handling.
/// </para>
/// </summary>
public class WorkspacesService : IWorkspacesService
{
    private readonly IXbimApiClient _client;
    private readonly ILogger<WorkspacesService>? _logger;

    /// <summary>
    /// Creates a new WorkspacesService.
    /// </summary>
    /// <param name="client">The Xbim API client.</param>
    /// <param name="logger">Optional logger.</param>
    public WorkspacesService(IXbimApiClient client, ILogger<WorkspacesService>? logger = null)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<WorkspaceDto> CreateAsync(CreateWorkspaceRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            _logger?.LogDebug("Creating workspace with name: {Name}", request.Name);
            var result = await _client.CreateWorkspaceAsync(request, cancellationToken);
            _logger?.LogInformation("Created workspace {WorkspaceId} with name: {Name}", result.Id, result.Name);
            return result;
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to create workspace: {StatusCode}", ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }

    /// <inheritdoc />
    public async Task<WorkspaceDto?> GetAsync(Guid workspaceId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogDebug("Getting workspace {WorkspaceId}", workspaceId);
            return await _client.GetWorkspaceAsync(workspaceId, cancellationToken);
        }
        catch (XbimApiException ex) when (ex.StatusCode == 404)
        {
            _logger?.LogDebug("Workspace {WorkspaceId} not found", workspaceId);
            return null;
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to get workspace {WorkspaceId}: {StatusCode}", workspaceId, ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }

    /// <inheritdoc />
    public async Task<WorkspaceDtoPagedList> ListAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogDebug("Listing workspaces page {Page} with size {PageSize}", page, pageSize);
            return await _client.ListWorkspacesAsync(page, pageSize, cancellationToken);
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to list workspaces: {StatusCode}", ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }

    /// <inheritdoc />
    public async Task<WorkspaceDto> UpdateAsync(Guid workspaceId, UpdateWorkspaceRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            _logger?.LogDebug("Updating workspace {WorkspaceId}", workspaceId);
            var result = await _client.UpdateWorkspaceAsync(workspaceId, request, cancellationToken);
            _logger?.LogInformation("Updated workspace {WorkspaceId}", workspaceId);
            return result;
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to update workspace {WorkspaceId}: {StatusCode}", workspaceId, ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }
}
