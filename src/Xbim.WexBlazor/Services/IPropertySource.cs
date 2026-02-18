using Xbim.WexBlazor.Models;

namespace Xbim.WexBlazor.Services;

/// <summary>
/// Interface for property sources that can provide element properties
/// </summary>
public interface IPropertySource : IDisposable
{
    /// <summary>
    /// Unique identifier for this source instance
    /// </summary>
    string Id { get; }
    
    /// <summary>
    /// Display name of the source
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Type of source (e.g., "IFC", "Database", "API")
    /// </summary>
    string SourceType { get; }
    
    /// <summary>
    /// Whether this source is currently available
    /// </summary>
    bool IsAvailable { get; }
    
    /// <summary>
    /// Model IDs this source can provide properties for (empty = all)
    /// </summary>
    IReadOnlyList<int> SupportedModelIds { get; }
    
    /// <summary>
    /// Gets properties for a single element
    /// </summary>
    /// <param name="query">Property query parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Element properties or null if not found</returns>
    Task<ElementProperties?> GetPropertiesAsync(PropertyQuery query, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets properties for multiple elements
    /// </summary>
    /// <param name="queries">Property query parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary of element ID to properties</returns>
    Task<Dictionary<int, ElementProperties>> GetPropertiesBatchAsync(
        IEnumerable<PropertyQuery> queries, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if this source can provide properties for a specific model
    /// </summary>
    /// <param name="modelId">Model ID to check</param>
    /// <returns>True if this source supports the model</returns>
    bool SupportsModel(int modelId);
}

/// <summary>
/// Base implementation for property sources
/// </summary>
public abstract class PropertySourceBase : IPropertySource
{
    public string Id { get; }
    public string Name { get; protected set; }
    public abstract string SourceType { get; }
    public virtual bool IsAvailable => true;
    
    protected readonly List<int> _supportedModelIds = new();
    public IReadOnlyList<int> SupportedModelIds => _supportedModelIds;
    
    protected PropertySourceBase(string? id = null, string? name = null)
    {
        Id = id ?? Guid.NewGuid().ToString();
        Name = name ?? GetType().Name;
    }
    
    public abstract Task<ElementProperties?> GetPropertiesAsync(
        PropertyQuery query, 
        CancellationToken cancellationToken = default);
    
    public virtual async Task<Dictionary<int, ElementProperties>> GetPropertiesBatchAsync(
        IEnumerable<PropertyQuery> queries, 
        CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<int, ElementProperties>();
        
        foreach (var query in queries)
        {
            if (cancellationToken.IsCancellationRequested)
                break;
                
            var props = await GetPropertiesAsync(query, cancellationToken);
            if (props != null)
            {
                result[query.ElementId] = props;
            }
        }
        
        return result;
    }
    
    public virtual bool SupportsModel(int modelId)
    {
        return _supportedModelIds.Count == 0 || _supportedModelIds.Contains(modelId);
    }
    
    /// <summary>
    /// Associates this source with specific model IDs
    /// </summary>
    public void SetSupportedModels(params int[] modelIds)
    {
        _supportedModelIds.Clear();
        _supportedModelIds.AddRange(modelIds);
    }
    
    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
