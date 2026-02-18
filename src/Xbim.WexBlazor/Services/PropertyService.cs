using Xbim.WexBlazor.Models;
using Xbim.WexBlazor.Services.Abstractions;

namespace Xbim.WexBlazor.Services;

/// <summary>
/// Service for managing property sources and retrieving element properties.
/// </summary>
public class PropertyService : IPropertyService
{
    private readonly List<IPropertySource> _sources = new();
    private readonly object _lock = new();
    
    /// <summary>
    /// Event raised when properties are retrieved
    /// </summary>
    public event Action<ElementProperties>? OnPropertiesRetrieved;
    
    /// <summary>
    /// Event raised when property sources change
    /// </summary>
    public event Action? OnSourcesChanged;
    
    /// <summary>
    /// Gets all registered property sources
    /// </summary>
    public IReadOnlyList<IPropertySource> Sources
    {
        get
        {
            lock (_lock)
            {
                return _sources.ToList();
            }
        }
    }
    
    /// <summary>
    /// Registers a property source
    /// </summary>
    /// <param name="source">The property source to register</param>
    public void RegisterSource(IPropertySource source)
    {
        lock (_lock)
        {
            if (!_sources.Any(s => s.Id == source.Id))
            {
                _sources.Add(source);
                OnSourcesChanged?.Invoke();
            }
        }
    }
    
    /// <summary>
    /// Unregisters a property source
    /// </summary>
    /// <param name="sourceId">ID of the source to unregister</param>
    public void UnregisterSource(string sourceId)
    {
        lock (_lock)
        {
            var source = _sources.FirstOrDefault(s => s.Id == sourceId);
            if (source != null)
            {
                _sources.Remove(source);
                source.Dispose();
                OnSourcesChanged?.Invoke();
            }
        }
    }
    
    /// <summary>
    /// Gets a property source by ID
    /// </summary>
    public IPropertySource? GetSource(string sourceId)
    {
        lock (_lock)
        {
            return _sources.FirstOrDefault(s => s.Id == sourceId);
        }
    }
    
    /// <summary>
    /// Gets all sources that support a specific model
    /// </summary>
    public IEnumerable<IPropertySource> GetSourcesForModel(int modelId)
    {
        lock (_lock)
        {
            return _sources.Where(s => s.IsAvailable && s.SupportsModel(modelId)).ToList();
        }
    }
    
    /// <summary>
    /// Gets properties for an element from all applicable sources
    /// </summary>
    /// <param name="elementId">Element ID (IFC entity label)</param>
    /// <param name="modelId">Model ID in the viewer</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Combined properties from all sources</returns>
    public async Task<ElementProperties?> GetPropertiesAsync(
        int elementId, 
        int modelId, 
        CancellationToken cancellationToken = default)
    {
        var query = new PropertyQuery
        {
            ElementId = elementId,
            ModelId = modelId
        };
        
        return await GetPropertiesAsync(query, cancellationToken);
    }
    
    /// <summary>
    /// Gets properties for an element using a query
    /// </summary>
    public async Task<ElementProperties?> GetPropertiesAsync(
        PropertyQuery query, 
        CancellationToken cancellationToken = default)
    {
        var sources = GetSourcesForModel(query.ModelId);
        ElementProperties? result = null;
        
        foreach (var source in sources)
        {
            if (cancellationToken.IsCancellationRequested)
                break;
                
            try
            {
                var props = await source.GetPropertiesAsync(query, cancellationToken);
                if (props != null)
                {
                    if (result == null)
                    {
                        result = props;
                    }
                    else
                    {
                        // Merge properties from multiple sources
                        MergeProperties(result, props);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting properties from source {source.Name}: {ex.Message}");
            }
        }
        
        if (result != null)
        {
            OnPropertiesRetrieved?.Invoke(result);
        }
        
        return result;
    }
    
    /// <summary>
    /// Gets properties for multiple elements
    /// </summary>
    public async Task<Dictionary<int, ElementProperties>> GetPropertiesBatchAsync(
        IEnumerable<(int ElementId, int ModelId)> elements,
        CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<int, ElementProperties>();
        
        // Group by model ID for efficient querying
        var groupedByModel = elements.GroupBy(e => e.ModelId);
        
        foreach (var group in groupedByModel)
        {
            var modelId = group.Key;
            var sources = GetSourcesForModel(modelId);
            var queries = group.Select(e => new PropertyQuery 
            { 
                ElementId = e.ElementId, 
                ModelId = modelId 
            });
            
            foreach (var source in sources)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                    
                try
                {
                    var props = await source.GetPropertiesBatchAsync(queries, cancellationToken);
                    foreach (var kvp in props)
                    {
                        if (!result.ContainsKey(kvp.Key))
                        {
                            result[kvp.Key] = kvp.Value;
                        }
                        else
                        {
                            MergeProperties(result[kvp.Key], kvp.Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting batch properties from source {source.Name}: {ex.Message}");
                }
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// Merges properties from source into target
    /// </summary>
    private void MergeProperties(ElementProperties target, ElementProperties source)
    {
        // Merge basic info if not set
        target.Name ??= source.Name;
        target.TypeName ??= source.TypeName;
        target.GlobalId ??= source.GlobalId;
        
        // Merge property groups (avoiding duplicates by name + source)
        foreach (var group in source.Groups)
        {
            var existing = target.Groups.FirstOrDefault(g => 
                g.Name == group.Name && g.Source == group.Source);
            
            if (existing == null)
            {
                target.Groups.Add(group);
            }
            else
            {
                // Merge properties within the group
                foreach (var prop in group.Properties)
                {
                    if (!existing.Properties.Any(p => p.Name == prop.Name))
                    {
                        existing.Properties.Add(prop);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Clears all sources for a specific model
    /// </summary>
    public void ClearSourcesForModel(int modelId)
    {
        lock (_lock)
        {
            var sourcesToRemove = _sources
                .Where(s => s.SupportedModelIds.Contains(modelId) && s.SupportedModelIds.Count == 1)
                .ToList();
                
            foreach (var source in sourcesToRemove)
            {
                _sources.Remove(source);
                source.Dispose();
            }
            
            if (sourcesToRemove.Any())
            {
                OnSourcesChanged?.Invoke();
            }
        }
    }
    
    public void Dispose()
    {
        lock (_lock)
        {
            foreach (var source in _sources)
            {
                source.Dispose();
            }
            _sources.Clear();
        }
        GC.SuppressFinalize(this);
    }
}
