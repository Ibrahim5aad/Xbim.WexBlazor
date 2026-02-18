using Xbim.WexServer.Client;

namespace Xbim.WexBlazor.Services.Abstractions.Server;

/// <summary>
/// Service interface for storage usage operations.
/// <para>
/// Requires Xbim.WexServer connectivity. Implementations typically wrap the generated
/// <see cref="IXbimApiClient"/> to provide a higher-level API.
/// </para>
/// </summary>
public interface IUsageService
{
    /// <summary>
    /// Gets storage usage for a workspace.
    /// </summary>
    /// <param name="workspaceId">The workspace ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task GetWorkspaceUsageAsync(Guid workspaceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets storage usage for a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task GetProjectUsageAsync(Guid projectId, CancellationToken cancellationToken = default);
}
