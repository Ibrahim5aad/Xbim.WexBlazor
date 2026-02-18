using System.Collections.Concurrent;
using Xbim.WexBlazor.Models;
using Xbim.Server.Client;

namespace Xbim.WexBlazor.Services.Server;

/// <summary>
/// Property source that fetches element properties from SQL Server via the Xbim API.
/// Used in PlatformConnected mode to retrieve properties stored in the database.
/// </summary>
public class PlatformPropertySource : PropertySourceBase
{
    private readonly IXbimApiClient _client;
    private readonly ConcurrentDictionary<int, Guid> _modelIdToVersionId = new();
    private readonly ConcurrentDictionary<(int ModelId, int ElementId), ElementProperties?> _cache = new();

    public override string SourceType => "Platform";

    /// <summary>
    /// Creates a new PlatformPropertySource instance.
    /// </summary>
    /// <param name="client">The Xbim API client for making server requests.</param>
    /// <param name="name">Optional display name for this source.</param>
    public PlatformPropertySource(IXbimApiClient client, string? name = null)
        : base(null, name ?? "Platform Properties")
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    /// <summary>
    /// Registers a mapping between a viewer model ID and a server model version ID.
    /// This mapping is required before properties can be fetched for elements in that model.
    /// </summary>
    /// <param name="viewerModelId">The model ID used by the viewer (internal ID).</param>
    /// <param name="serverVersionId">The model version GUID from the server.</param>
    public void RegisterModelMapping(int viewerModelId, Guid serverVersionId)
    {
        _modelIdToVersionId[viewerModelId] = serverVersionId;

        // Add to supported models if not already present
        if (!_supportedModelIds.Contains(viewerModelId))
        {
            _supportedModelIds.Add(viewerModelId);
        }
    }

    /// <summary>
    /// Removes a model mapping and clears cached properties for that model.
    /// </summary>
    /// <param name="viewerModelId">The viewer model ID to unregister.</param>
    public void UnregisterModelMapping(int viewerModelId)
    {
        _modelIdToVersionId.TryRemove(viewerModelId, out _);
        _supportedModelIds.Remove(viewerModelId);

        // Clear cache entries for this model
        var keysToRemove = _cache.Keys.Where(k => k.ModelId == viewerModelId).ToList();
        foreach (var key in keysToRemove)
        {
            _cache.TryRemove(key, out _);
        }
    }

    /// <summary>
    /// Clears the property cache for all models or a specific model.
    /// </summary>
    /// <param name="viewerModelId">Optional model ID to clear cache for. If null, clears all.</param>
    public void ClearCache(int? viewerModelId = null)
    {
        if (viewerModelId == null)
        {
            _cache.Clear();
        }
        else
        {
            var keysToRemove = _cache.Keys.Where(k => k.ModelId == viewerModelId.Value).ToList();
            foreach (var key in keysToRemove)
            {
                _cache.TryRemove(key, out _);
            }
        }
    }

    public override async Task<ElementProperties?> GetPropertiesAsync(
        PropertyQuery query,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = (query.ModelId, query.ElementId);

        // Check cache first
        if (_cache.TryGetValue(cacheKey, out var cachedResult))
        {
            return cachedResult;
        }

        // Get the server version ID for this model
        if (!_modelIdToVersionId.TryGetValue(query.ModelId, out var versionId))
        {
            // No mapping registered for this model - return null without caching
            return null;
        }

        try
        {
            // Query the API for properties by entity label
            var result = await _client.QueryPropertiesAsync(
                modelVersionId: versionId,
                entityLabel: query.ElementId,
                page: 1,
                pageSize: 1,
                cancellationToken: cancellationToken);

            var element = result.Items?.FirstOrDefault();

            if (element == null)
            {
                // Cache null result to avoid repeated API calls for missing elements
                _cache[cacheKey] = null;
                return null;
            }

            var properties = MapToElementProperties(element, query.ModelId);

            // Cache the result
            _cache[cacheKey] = properties;

            return properties;
        }
        catch (XbimApiException ex) when (ex.StatusCode == 404)
        {
            // Element not found - cache null to avoid repeated calls
            _cache[cacheKey] = null;
            return null;
        }
    }

    /// <summary>
    /// Maps an IfcElementDto from the server to the local ElementProperties model.
    /// </summary>
    private static ElementProperties MapToElementProperties(IfcElementDto element, int modelId)
    {
        var properties = new ElementProperties
        {
            ElementId = element.EntityLabel ?? 0,
            ModelId = modelId,
            Name = element.Name,
            TypeName = element.TypeName,
            GlobalId = element.GlobalId,
            Groups = new List<PropertyGroup>()
        };

        // Map property sets
        if (element.PropertySets != null)
        {
            foreach (var pset in element.PropertySets)
            {
                var group = new PropertyGroup
                {
                    Name = pset.Name ?? "Unknown",
                    Source = pset.IsTypePropertySet == true ? "Type" : "Instance",
                    IsExpanded = true,
                    Properties = new List<PropertyValue>()
                };

                if (pset.Properties != null)
                {
                    foreach (var prop in pset.Properties)
                    {
                        group.Properties.Add(new PropertyValue
                        {
                            Name = prop.Name ?? "Unknown",
                            Value = prop.Value,
                            ValueType = prop.ValueType ?? "string",
                            Unit = prop.Unit
                        });
                    }
                }

                properties.Groups.Add(group);
            }
        }

        // Map quantity sets
        if (element.QuantitySets != null)
        {
            foreach (var qset in element.QuantitySets)
            {
                var group = new PropertyGroup
                {
                    Name = qset.Name ?? "Quantities",
                    Source = "Quantities",
                    IsExpanded = true,
                    Properties = new List<PropertyValue>()
                };

                if (qset.Quantities != null)
                {
                    foreach (var qty in qset.Quantities)
                    {
                        group.Properties.Add(new PropertyValue
                        {
                            Name = qty.Name ?? "Unknown",
                            Value = qty.Value?.ToString(),
                            ValueType = qty.ValueType ?? "double",
                            Unit = qty.Unit
                        });
                    }
                }

                properties.Groups.Add(group);
            }
        }

        return properties;
    }

    public override void Dispose()
    {
        _cache.Clear();
        _modelIdToVersionId.Clear();
        _supportedModelIds.Clear();
        base.Dispose();
    }
}
