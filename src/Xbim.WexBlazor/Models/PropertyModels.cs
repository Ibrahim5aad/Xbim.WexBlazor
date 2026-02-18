namespace Xbim.WexBlazor.Models;

/// <summary>
/// Represents a single property value
/// </summary>
public class PropertyValue
{
    /// <summary>
    /// Property name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Property value as string
    /// </summary>
    public string? Value { get; set; }
    
    /// <summary>
    /// Original value type (e.g., "string", "double", "boolean", "integer")
    /// </summary>
    public string ValueType { get; set; } = "string";
    
    /// <summary>
    /// Unit of measurement if applicable
    /// </summary>
    public string? Unit { get; set; }
}

/// <summary>
/// Represents a group of related properties (e.g., a PropertySet in IFC)
/// </summary>
public class PropertyGroup
{
    /// <summary>
    /// Group name (e.g., PropertySet name)
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Source of the property group (e.g., "IFC", "Database", "API")
    /// </summary>
    public string Source { get; set; } = string.Empty;
    
    /// <summary>
    /// Properties in this group
    /// </summary>
    public List<PropertyValue> Properties { get; set; } = new();
    
    /// <summary>
    /// Whether this group is expanded in the UI
    /// </summary>
    public bool IsExpanded { get; set; } = true;
}

/// <summary>
/// Represents all properties for an element
/// </summary>
public class ElementProperties
{
    /// <summary>
    /// The element ID (IFC entity label)
    /// </summary>
    public int ElementId { get; set; }
    
    /// <summary>
    /// The model ID in the viewer
    /// </summary>
    public int ModelId { get; set; }
    
    /// <summary>
    /// Element name if available
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Element type (e.g., "IfcWall", "IfcDoor")
    /// </summary>
    public string? TypeName { get; set; }
    
    /// <summary>
    /// Global ID (GUID) if available
    /// </summary>
    public string? GlobalId { get; set; }
    
    /// <summary>
    /// Property groups
    /// </summary>
    public List<PropertyGroup> Groups { get; set; } = new();
}

/// <summary>
/// Query for retrieving element properties
/// </summary>
public class PropertyQuery
{
    /// <summary>
    /// Element ID (IFC entity label / product ID)
    /// </summary>
    public int ElementId { get; set; }
    
    /// <summary>
    /// Model ID in the viewer
    /// </summary>
    public int ModelId { get; set; }
    
    /// <summary>
    /// Optional: specific property set names to retrieve
    /// </summary>
    public List<string>? PropertySetNames { get; set; }
    
    /// <summary>
    /// Whether to include type properties
    /// </summary>
    public bool IncludeTypeProperties { get; set; } = true;
    
    /// <summary>
    /// Whether to include quantity sets
    /// </summary>
    public bool IncludeQuantitySets { get; set; } = true;
}

/// <summary>
/// Configuration for a property source
/// </summary>
public class PropertySourceConfig
{
    /// <summary>
    /// Unique identifier for this source
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Display name for the source
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Type of source (e.g., "IFC", "Database", "API")
    /// </summary>
    public string SourceType { get; set; } = string.Empty;
    
    /// <summary>
    /// Model IDs this source applies to (empty = all models)
    /// </summary>
    public List<int> ModelIds { get; set; } = new();
    
    /// <summary>
    /// Whether this source is enabled
    /// </summary>
    public bool IsEnabled { get; set; } = true;
    
    /// <summary>
    /// Priority for property retrieval (lower = higher priority)
    /// </summary>
    public int Priority { get; set; } = 0;
}
