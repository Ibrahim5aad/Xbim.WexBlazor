using Xbim.WexBlazor.Models;

namespace Xbim.WexBlazor.Services;

/// <summary>
/// A custom property source that retrieves properties from a user-defined function.
/// This allows integrating with any data source (databases, APIs, in-memory data, etc.)
/// </summary>
public class CustomPropertySource : PropertySourceBase
{
    private readonly Func<PropertyQuery, CancellationToken, Task<ElementProperties?>> _propertyProvider;
    private readonly string _sourceType;
    
    public override string SourceType => _sourceType;
    
    /// <summary>
    /// Creates a custom property source
    /// </summary>
    /// <param name="propertyProvider">Function that retrieves properties for an element</param>
    /// <param name="sourceType">Type identifier for this source (e.g., "Database", "API")</param>
    /// <param name="modelIds">Model IDs this source supports (empty = all)</param>
    /// <param name="name">Display name</param>
    public CustomPropertySource(
        Func<PropertyQuery, CancellationToken, Task<ElementProperties?>> propertyProvider,
        string sourceType = "Custom",
        int[]? modelIds = null,
        string? name = null) 
        : base(null, name ?? "Custom Properties")
    {
        _propertyProvider = propertyProvider ?? throw new ArgumentNullException(nameof(propertyProvider));
        _sourceType = sourceType;
        
        if (modelIds != null && modelIds.Length > 0)
        {
            SetSupportedModels(modelIds);
        }
    }
    
    public override async Task<ElementProperties?> GetPropertiesAsync(
        PropertyQuery query, 
        CancellationToken cancellationToken = default)
    {
        return await _propertyProvider(query, cancellationToken);
    }
}

/// <summary>
/// A dictionary-based property source for simple use cases.
/// Stores properties in memory keyed by element ID.
/// </summary>
public class DictionaryPropertySource : PropertySourceBase
{
    private readonly Dictionary<int, ElementProperties> _properties = new();
    
    public override string SourceType => "Dictionary";
    
    /// <summary>
    /// Creates a dictionary-based property source
    /// </summary>
    /// <param name="modelIds">Model IDs this source supports (empty = all)</param>
    /// <param name="name">Display name</param>
    public DictionaryPropertySource(int[]? modelIds = null, string? name = null) 
        : base(null, name ?? "In-Memory Properties")
    {
        if (modelIds != null && modelIds.Length > 0)
        {
            SetSupportedModels(modelIds);
        }
    }
    
    /// <summary>
    /// Adds or updates properties for an element
    /// </summary>
    public void SetProperties(int elementId, ElementProperties properties)
    {
        _properties[elementId] = properties;
    }
    
    /// <summary>
    /// Adds a property group to an element
    /// </summary>
    public void AddPropertyGroup(int elementId, int modelId, PropertyGroup group)
    {
        if (!_properties.TryGetValue(elementId, out var props))
        {
            props = new ElementProperties
            {
                ElementId = elementId,
                ModelId = modelId
            };
            _properties[elementId] = props;
        }
        props.Groups.Add(group);
    }
    
    /// <summary>
    /// Adds a simple property to an element
    /// </summary>
    public void AddProperty(int elementId, int modelId, string groupName, string propertyName, string? value, string valueType = "string")
    {
        if (!_properties.TryGetValue(elementId, out var props))
        {
            props = new ElementProperties
            {
                ElementId = elementId,
                ModelId = modelId
            };
            _properties[elementId] = props;
        }
        
        var group = props.Groups.FirstOrDefault(g => g.Name == groupName);
        if (group == null)
        {
            group = new PropertyGroup
            {
                Name = groupName,
                Source = SourceType
            };
            props.Groups.Add(group);
        }
        
        group.Properties.Add(new PropertyValue
        {
            Name = propertyName,
            Value = value,
            ValueType = valueType
        });
    }
    
    /// <summary>
    /// Removes properties for an element
    /// </summary>
    public bool RemoveProperties(int elementId)
    {
        return _properties.Remove(elementId);
    }
    
    /// <summary>
    /// Clears all properties
    /// </summary>
    public void Clear()
    {
        _properties.Clear();
    }
    
    public override Task<ElementProperties?> GetPropertiesAsync(
        PropertyQuery query, 
        CancellationToken cancellationToken = default)
    {
        _properties.TryGetValue(query.ElementId, out var props);
        return Task.FromResult(props);
    }
}
