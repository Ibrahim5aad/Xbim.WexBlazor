using Xbim.Server.Client;

namespace Xbim.WexBlazor.Services.Abstractions.Server;

/// <summary>
/// Service interface for workspace operations.
/// <para>
/// Requires Xbim.Server connectivity. Implementations typically wrap the generated
/// <see cref="IXbimApiClient"/> to provide a higher-level API.
/// </para>
/// </summary>
public interface IWorkspacesService
{
    /// <summary>
    /// Creates a new workspace.
    /// </summary>
    /// <param name="request">The workspace creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created workspace.</returns>
    Task<WorkspaceDto> CreateAsync(CreateWorkspaceRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a workspace by ID.
    /// </summary>
    /// <param name="workspaceId">The workspace ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The workspace, or null if not found.</returns>
    Task<WorkspaceDto?> GetAsync(Guid workspaceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists workspaces the current user has access to.
    /// </summary>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged list of workspaces.</returns>
    Task<WorkspaceDtoPagedList> ListAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a workspace.
    /// </summary>
    /// <param name="workspaceId">The workspace ID.</param>
    /// <param name="request">The update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated workspace.</returns>
    Task<WorkspaceDto> UpdateAsync(Guid workspaceId, UpdateWorkspaceRequest request, CancellationToken cancellationToken = default);
}
