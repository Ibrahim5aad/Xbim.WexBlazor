using Xbim.Server.Client;

namespace Xbim.WexBlazor.Services.Abstractions.Server;

/// <summary>
/// Service interface for model and model version operations.
/// <para>
/// Requires Xbim.Server connectivity. Implementations typically wrap the generated
/// <see cref="IXbimApiClient"/> to provide a higher-level API.
/// </para>
/// </summary>
public interface IModelsService
{
    /// <summary>
    /// Creates a new model in a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="request">The model creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created model.</returns>
    Task<ModelDto> CreateAsync(Guid projectId, CreateModelRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a model by ID.
    /// </summary>
    /// <param name="modelId">The model ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The model, or null if not found.</returns>
    Task<ModelDto?> GetAsync(Guid modelId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists models in a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged list of models.</returns>
    Task<ModelDtoPagedList> ListAsync(Guid projectId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new version of a model.
    /// </summary>
    /// <param name="modelId">The model ID.</param>
    /// <param name="request">The version creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created model version.</returns>
    Task<ModelVersionDto> CreateVersionAsync(Guid modelId, CreateModelVersionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a model version by ID.
    /// </summary>
    /// <param name="versionId">The version ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The model version, or null if not found.</returns>
    Task<ModelVersionDto?> GetVersionAsync(Guid versionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists versions of a model.
    /// </summary>
    /// <param name="modelId">The model ID.</param>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged list of model versions.</returns>
    Task<ModelVersionDtoPagedList> ListVersionsAsync(Guid modelId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the WexBIM data for a model version.
    /// </summary>
    /// <param name="versionId">The version ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A response containing the WexBIM data stream.</returns>
    Task<FileResponse> GetWexBimAsync(Guid versionId, CancellationToken cancellationToken = default);
}
