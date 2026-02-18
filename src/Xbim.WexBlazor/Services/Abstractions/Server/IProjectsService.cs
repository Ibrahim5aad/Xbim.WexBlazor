using Xbim.WexServer.Client;

namespace Xbim.WexBlazor.Services.Abstractions.Server;

/// <summary>
/// Service interface for project operations.
/// <para>
/// Requires Xbim.WexServer connectivity. Implementations typically wrap the generated
/// <see cref="IXbimApiClient"/> to provide a higher-level API.
/// </para>
/// </summary>
public interface IProjectsService
{
    /// <summary>
    /// Creates a new project in a workspace.
    /// </summary>
    /// <param name="workspaceId">The workspace ID.</param>
    /// <param name="request">The project creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created project.</returns>
    Task<ProjectDto> CreateAsync(Guid workspaceId, CreateProjectRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a project by ID.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The project, or null if not found.</returns>
    Task<ProjectDto?> GetAsync(Guid projectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists projects in a workspace.
    /// </summary>
    /// <param name="workspaceId">The workspace ID.</param>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged list of projects.</returns>
    Task<ProjectDtoPagedList> ListAsync(Guid workspaceId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="request">The update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated project.</returns>
    Task<ProjectDto> UpdateAsync(Guid projectId, UpdateProjectRequest request, CancellationToken cancellationToken = default);
}
