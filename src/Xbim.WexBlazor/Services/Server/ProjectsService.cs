using Microsoft.Extensions.Logging;
using Xbim.WexBlazor.Services.Abstractions.Server;
using Xbim.Api.Client;

namespace Xbim.WexBlazor.Services.Server;

/// <summary>
/// Server-backed implementation of <see cref="IProjectsService"/>.
/// <para>
/// Wraps the generated <see cref="IXbimApiClient"/> to provide project operations.
/// All API errors are wrapped in <see cref="XbimServiceException"/> for predictable error handling.
/// </para>
/// </summary>
public class ProjectsService : IProjectsService
{
    private readonly IXbimApiClient _client;
    private readonly ILogger<ProjectsService>? _logger;

    /// <summary>
    /// Creates a new ProjectsService.
    /// </summary>
    /// <param name="client">The Xbim API client.</param>
    /// <param name="logger">Optional logger.</param>
    public ProjectsService(IXbimApiClient client, ILogger<ProjectsService>? logger = null)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ProjectDto> CreateAsync(Guid workspaceId, CreateProjectRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            _logger?.LogDebug("Creating project in workspace {WorkspaceId} with name: {Name}", workspaceId, request.Name);
            var result = await _client.CreateProjectAsync(workspaceId, request, cancellationToken);
            _logger?.LogInformation("Created project {ProjectId} in workspace {WorkspaceId}", result.Id, workspaceId);
            return result;
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to create project in workspace {WorkspaceId}: {StatusCode}", workspaceId, ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }

    /// <inheritdoc />
    public async Task<ProjectDto?> GetAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogDebug("Getting project {ProjectId}", projectId);
            return await _client.GetProjectAsync(projectId, cancellationToken);
        }
        catch (XbimApiException ex) when (ex.StatusCode == 404)
        {
            _logger?.LogDebug("Project {ProjectId} not found", projectId);
            return null;
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to get project {ProjectId}: {StatusCode}", projectId, ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }

    /// <inheritdoc />
    public async Task<ProjectDtoPagedList> ListAsync(Guid workspaceId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogDebug("Listing projects in workspace {WorkspaceId}, page {Page}", workspaceId, page);
            return await _client.ListProjectsAsync(workspaceId, page, pageSize, cancellationToken);
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to list projects in workspace {WorkspaceId}: {StatusCode}", workspaceId, ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }

    /// <inheritdoc />
    public async Task<ProjectDto> UpdateAsync(Guid projectId, UpdateProjectRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            _logger?.LogDebug("Updating project {ProjectId}", projectId);
            var result = await _client.UpdateProjectAsync(projectId, request, cancellationToken);
            _logger?.LogInformation("Updated project {ProjectId}", projectId);
            return result;
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to update project {ProjectId}: {StatusCode}", projectId, ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }
}
