using Microsoft.Extensions.Logging;
using Xbim.WexBlazor.Services.Abstractions.Server;
using Xbim.Server.Client;

namespace Xbim.WexBlazor.Services.Server;

/// <summary>
/// Server-backed implementation of <see cref="IModelsService"/>.
/// <para>
/// Wraps the generated <see cref="IXbimApiClient"/> to provide model and model version operations.
/// All API errors are wrapped in <see cref="XbimServiceException"/> for predictable error handling.
/// </para>
/// </summary>
public class ModelsService : IModelsService
{
    private readonly IXbimApiClient _client;
    private readonly ILogger<ModelsService>? _logger;

    /// <summary>
    /// Creates a new ModelsService.
    /// </summary>
    /// <param name="client">The Xbim API client.</param>
    /// <param name="logger">Optional logger.</param>
    public ModelsService(IXbimApiClient client, ILogger<ModelsService>? logger = null)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ModelDto> CreateAsync(Guid projectId, CreateModelRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            _logger?.LogDebug("Creating model in project {ProjectId} with name: {Name}", projectId, request.Name);
            var result = await _client.CreateModelAsync(projectId, request, cancellationToken);
            _logger?.LogInformation("Created model {ModelId} in project {ProjectId}", result.Id, projectId);
            return result;
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to create model in project {ProjectId}: {StatusCode}", projectId, ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }

    /// <inheritdoc />
    public async Task<ModelDto?> GetAsync(Guid modelId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogDebug("Getting model {ModelId}", modelId);
            return await _client.GetModelAsync(modelId, cancellationToken);
        }
        catch (XbimApiException ex) when (ex.StatusCode == 404)
        {
            _logger?.LogDebug("Model {ModelId} not found", modelId);
            return null;
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to get model {ModelId}: {StatusCode}", modelId, ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }

    /// <inheritdoc />
    public async Task<ModelDtoPagedList> ListAsync(Guid projectId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogDebug("Listing models in project {ProjectId}, page {Page}", projectId, page);
            return await _client.ListModelsAsync(projectId, page, pageSize, cancellationToken);
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to list models in project {ProjectId}: {StatusCode}", projectId, ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }

    /// <inheritdoc />
    public async Task<ModelVersionDto> CreateVersionAsync(Guid modelId, CreateModelVersionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            _logger?.LogDebug("Creating version for model {ModelId}", modelId);
            var result = await _client.CreateModelVersionAsync(modelId, request, cancellationToken);
            _logger?.LogInformation("Created version {VersionId} for model {ModelId}", result.Id, modelId);
            return result;
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to create version for model {ModelId}: {StatusCode}", modelId, ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }

    /// <inheritdoc />
    public async Task<ModelVersionDto?> GetVersionAsync(Guid versionId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogDebug("Getting model version {VersionId}", versionId);
            return await _client.GetModelVersionAsync(versionId, cancellationToken);
        }
        catch (XbimApiException ex) when (ex.StatusCode == 404)
        {
            _logger?.LogDebug("Model version {VersionId} not found", versionId);
            return null;
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to get model version {VersionId}: {StatusCode}", versionId, ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }

    /// <inheritdoc />
    public async Task<ModelVersionDtoPagedList> ListVersionsAsync(Guid modelId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogDebug("Listing versions for model {ModelId}, page {Page}", modelId, page);
            return await _client.ListModelVersionsAsync(modelId, page, pageSize, cancellationToken);
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to list versions for model {ModelId}: {StatusCode}", modelId, ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }

    /// <inheritdoc />
    public async Task<FileResponse> GetWexBimAsync(Guid versionId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger?.LogDebug("Getting WexBIM for version {VersionId}", versionId);
            return await _client.GetModelVersionWexBimAsync(versionId, cancellationToken);
        }
        catch (XbimApiException ex)
        {
            _logger?.LogError(ex, "Failed to get WexBIM for version {VersionId}: {StatusCode}", versionId, ex.StatusCode);
            throw XbimServiceException.FromApiException(ex);
        }
    }
}
