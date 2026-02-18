using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.WexBlazor.Models;

namespace Xbim.WexBlazor.Services;

/// <summary>
/// Property source that reads properties from IFC models using xBIM Essentials
/// </summary>
public class IfcPropertySource : PropertySourceBase
{
    private readonly IModel _model;
    private bool _disposed;
    
    public override string SourceType => "IFC";
    
    public override bool IsAvailable => !_disposed && _model != null;
    
    /// <summary>
    /// Creates a new IFC property source
    /// </summary>
    /// <param name="model">The xBIM IModel to read properties from</param>
    /// <param name="modelId">The viewer model ID this source is associated with</param>
    /// <param name="name">Optional display name</param>
    public IfcPropertySource(IModel model, int modelId, string? name = null) 
        : base(null, name ?? "IFC Properties")
    {
        _model = model ?? throw new ArgumentNullException(nameof(model));
        SetSupportedModels(modelId);
    }
    
    /// <summary>
    /// Gets properties for an element from the IFC model
    /// </summary>
    public override Task<ElementProperties?> GetPropertiesAsync(
        PropertyQuery query, 
        CancellationToken cancellationToken = default)
    {
        if (_disposed || !SupportsModel(query.ModelId))
            return Task.FromResult<ElementProperties?>(null);
            
        try
        {
            // Get the IFC entity by its label (the product ID in wexbim is the IFC entity label)
            var entity = _model.Instances[query.ElementId];
            
            if (entity == null)
                return Task.FromResult<ElementProperties?>(null);
            
            var result = new ElementProperties
            {
                ElementId = query.ElementId,
                ModelId = query.ModelId
            };
            
            // Get basic info if it's a product
            if (entity is IIfcProduct product)
            {
                result.Name = GetLabelValue(product.Name);
                result.GlobalId = product.GlobalId.ToString();
                result.TypeName = entity.ExpressType.Name;
                
                // Add identity group
                var identityGroup = new PropertyGroup
                {
                    Name = "Identity",
                    Source = SourceType,
                    IsExpanded = true,
                    Properties = new List<PropertyValue>
                    {
                        new() { Name = "Entity Label", Value = query.ElementId.ToString(), ValueType = "integer" },
                        new() { Name = "Global ID", Value = product.GlobalId.ToString(), ValueType = "string" },
                        new() { Name = "Name", Value = GetLabelValue(product.Name), ValueType = "string" },
                        new() { Name = "Type", Value = entity.ExpressType.Name, ValueType = "string" },
                        new() { Name = "Description", Value = GetTextValue(product.Description), ValueType = "string" },
                        new() { Name = "Object Type", Value = GetLabelValue(product.ObjectType), ValueType = "string" }
                    }
                };
                result.Groups.Add(identityGroup);
                
                // Get property sets
                if (query.IncludeTypeProperties || query.PropertySetNames == null)
                {
                    var propertySets = GetPropertySets(product, query.PropertySetNames);
                    result.Groups.AddRange(propertySets);
                }
                
                // Get quantity sets
                if (query.IncludeQuantitySets)
                {
                    var quantitySets = GetQuantitySets(product);
                    result.Groups.AddRange(quantitySets);
                }
                
                // Get type properties
                if (query.IncludeTypeProperties)
                {
                    var typeProperties = GetTypeProperties(product);
                    result.Groups.AddRange(typeProperties);
                }
            }
            else if (entity is IIfcRoot root)
            {
                result.Name = GetLabelValue(root.Name);
                result.GlobalId = root.GlobalId.ToString();
                result.TypeName = entity.ExpressType.Name;
                
                // Add basic identity for non-product entities
                var identityGroup = new PropertyGroup
                {
                    Name = "Identity",
                    Source = SourceType,
                    Properties = new List<PropertyValue>
                    {
                        new() { Name = "Entity Label", Value = query.ElementId.ToString(), ValueType = "integer" },
                        new() { Name = "Global ID", Value = root.GlobalId.ToString(), ValueType = "string" },
                        new() { Name = "Name", Value = GetLabelValue(root.Name), ValueType = "string" },
                        new() { Name = "Type", Value = entity.ExpressType.Name, ValueType = "string" }
                    }
                };
                result.Groups.Add(identityGroup);
            }
            else
            {
                result.TypeName = entity.ExpressType.Name;
                
                var identityGroup = new PropertyGroup
                {
                    Name = "Identity",
                    Source = SourceType,
                    Properties = new List<PropertyValue>
                    {
                        new() { Name = "Entity Label", Value = query.ElementId.ToString(), ValueType = "integer" },
                        new() { Name = "Type", Value = entity.ExpressType.Name, ValueType = "string" }
                    }
                };
                result.Groups.Add(identityGroup);
            }
            
            return Task.FromResult<ElementProperties?>(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting IFC properties for element {query.ElementId}: {ex.Message}");
            return Task.FromResult<ElementProperties?>(null);
        }
    }
    
    /// <summary>
    /// Gets property sets for a product
    /// </summary>
    private List<PropertyGroup> GetPropertySets(IIfcProduct product, List<string>? filterNames = null)
    {
        var groups = new List<PropertyGroup>();
        
        // Get property sets through IsDefinedBy relationship
        var relDefines = product.IsDefinedBy
            .OfType<IIfcRelDefinesByProperties>()
            .ToList();
        
        foreach (var rel in relDefines)
        {
            if (rel.RelatingPropertyDefinition is IIfcPropertySet pset)
            {
                var psetName = GetLabelValue(pset.Name) ?? "Property Set";
                
                // Filter by name if specified
                if (filterNames != null && !filterNames.Contains(psetName))
                    continue;
                    
                var group = new PropertyGroup
                {
                    Name = psetName,
                    Source = SourceType,
                    Properties = new List<PropertyValue>()
                };
                
                foreach (var prop in pset.HasProperties)
                {
                    var propValue = ExtractPropertyValue(prop);
                    if (propValue != null)
                    {
                        group.Properties.Add(propValue);
                    }
                }
                
                if (group.Properties.Any())
                {
                    groups.Add(group);
                }
            }
        }
        
        return groups;
    }
    
    /// <summary>
    /// Gets quantity sets for a product
    /// </summary>
    private List<PropertyGroup> GetQuantitySets(IIfcProduct product)
    {
        var groups = new List<PropertyGroup>();
        
        var relDefines = product.IsDefinedBy
            .OfType<IIfcRelDefinesByProperties>()
            .ToList();
        
        foreach (var rel in relDefines)
        {
            if (rel.RelatingPropertyDefinition is IIfcElementQuantity qset)
            {
                var group = new PropertyGroup
                {
                    Name = GetLabelValue(qset.Name) ?? "Quantities",
                    Source = SourceType,
                    Properties = new List<PropertyValue>()
                };
                
                foreach (var quantity in qset.Quantities)
                {
                    var propValue = ExtractQuantityValue(quantity);
                    if (propValue != null)
                    {
                        group.Properties.Add(propValue);
                    }
                }
                
                if (group.Properties.Any())
                {
                    groups.Add(group);
                }
            }
        }
        
        return groups;
    }
    
    /// <summary>
    /// Gets type properties for a product
    /// </summary>
    private List<PropertyGroup> GetTypeProperties(IIfcProduct product)
    {
        var groups = new List<PropertyGroup>();
        
        // Get type object through IsTypedBy relationship
        var relTypes = product.IsTypedBy?.FirstOrDefault();
        
        if (relTypes?.RelatingType is IIfcTypeObject typeObject)
        {
            // Add type identity
            var typeIdentity = new PropertyGroup
            {
                Name = "Type Information",
                Source = SourceType,
                Properties = new List<PropertyValue>
                {
                    new() { Name = "Type Name", Value = GetLabelValue(typeObject.Name), ValueType = "string" },
                    new() { Name = "Type", Value = typeObject.ExpressType.Name, ValueType = "string" }
                }
            };
            groups.Add(typeIdentity);
            
            // Get type property sets
            if (typeObject.HasPropertySets != null)
            {
                foreach (var pset in typeObject.HasPropertySets.OfType<IIfcPropertySet>())
                {
                    var group = new PropertyGroup
                    {
                        Name = $"{GetLabelValue(pset.Name)} (Type)",
                        Source = SourceType,
                        Properties = new List<PropertyValue>()
                    };
                    
                    foreach (var prop in pset.HasProperties)
                    {
                        var propValue = ExtractPropertyValue(prop);
                        if (propValue != null)
                        {
                            group.Properties.Add(propValue);
                        }
                    }
                    
                    if (group.Properties.Any())
                    {
                        groups.Add(group);
                    }
                }
            }
        }
        
        return groups;
    }
    
    /// <summary>
    /// Extracts a property value from an IFC property
    /// </summary>
    private PropertyValue? ExtractPropertyValue(IIfcProperty property)
    {
        try
        {
            var propertyName = GetIdentifierValue(property.Name) ?? "Unknown";
            
            switch (property)
            {
                case IIfcPropertySingleValue singleValue:
                    return new PropertyValue
                    {
                        Name = propertyName,
                        Value = singleValue.NominalValue?.ToString(),
                        ValueType = GetValueType(singleValue.NominalValue),
                        Unit = singleValue.Unit?.ToString()
                    };
                    
                case IIfcPropertyEnumeratedValue enumValue:
                    var values = enumValue.EnumerationValues?.Select(v => v.ToString()).ToList();
                    return new PropertyValue
                    {
                        Name = propertyName,
                        Value = values != null ? string.Join(", ", values) : null,
                        ValueType = "enumeration"
                    };
                    
                case IIfcPropertyBoundedValue boundedValue:
                    var lower = boundedValue.LowerBoundValue?.ToString() ?? "?";
                    var upper = boundedValue.UpperBoundValue?.ToString() ?? "?";
                    return new PropertyValue
                    {
                        Name = propertyName,
                        Value = $"{lower} - {upper}",
                        ValueType = "range",
                        Unit = boundedValue.Unit?.ToString()
                    };
                    
                case IIfcPropertyListValue listValue:
                    var listValues = listValue.ListValues?.Select(v => v.ToString()).ToList();
                    return new PropertyValue
                    {
                        Name = propertyName,
                        Value = listValues != null ? string.Join(", ", listValues) : null,
                        ValueType = "list"
                    };
                    
                case IIfcPropertyTableValue:
                    return new PropertyValue
                    {
                        Name = propertyName,
                        Value = "[Table]",
                        ValueType = "table"
                    };
                    
                case IIfcComplexProperty complexProp:
                    return new PropertyValue
                    {
                        Name = propertyName,
                        Value = $"[{complexProp.HasProperties.Count()} properties]",
                        ValueType = "complex"
                    };
                    
                default:
                    return new PropertyValue
                    {
                        Name = propertyName,
                        Value = property.ToString(),
                        ValueType = "unknown"
                    };
            }
        }
        catch
        {
            return null;
        }
    }
    
    /// <summary>
    /// Extracts a value from an IFC quantity
    /// </summary>
    private PropertyValue? ExtractQuantityValue(IIfcPhysicalQuantity quantity)
    {
        try
        {
            var quantityName = GetLabelValue(quantity.Name) ?? "Unknown";
            
            switch (quantity)
            {
                case IIfcQuantityLength length:
                    return new PropertyValue
                    {
                        Name = quantityName,
                        Value = length.LengthValue.ToString(),
                        ValueType = "double",
                        Unit = length.Unit?.ToString() ?? "m"
                    };
                    
                case IIfcQuantityArea area:
                    return new PropertyValue
                    {
                        Name = quantityName,
                        Value = area.AreaValue.ToString(),
                        ValueType = "double",
                        Unit = area.Unit?.ToString() ?? "m²"
                    };
                    
                case IIfcQuantityVolume volume:
                    return new PropertyValue
                    {
                        Name = quantityName,
                        Value = volume.VolumeValue.ToString(),
                        ValueType = "double",
                        Unit = volume.Unit?.ToString() ?? "m³"
                    };
                    
                case IIfcQuantityCount count:
                    return new PropertyValue
                    {
                        Name = quantityName,
                        Value = count.CountValue.ToString(),
                        ValueType = "integer"
                    };
                    
                case IIfcQuantityWeight weight:
                    return new PropertyValue
                    {
                        Name = quantityName,
                        Value = weight.WeightValue.ToString(),
                        ValueType = "double",
                        Unit = weight.Unit?.ToString() ?? "kg"
                    };
                    
                case IIfcQuantityTime time:
                    return new PropertyValue
                    {
                        Name = quantityName,
                        Value = time.TimeValue.ToString(),
                        ValueType = "double",
                        Unit = time.Unit?.ToString() ?? "s"
                    };
                    
                default:
                    return new PropertyValue
                    {
                        Name = quantityName,
                        Value = quantity.ToString(),
                        ValueType = "unknown"
                    };
            }
        }
        catch
        {
            return null;
        }
    }
    
    /// <summary>
    /// Gets the value type string from an IFC value
    /// </summary>
    private string GetValueType(IIfcValue? value)
    {
        if (value == null) return "null";
        
        // Check the underlying value type
        var underlyingValue = value.Value;
        
        return underlyingValue switch
        {
            bool => "boolean",
            int or long => "integer",
            double or float => "double",
            string => "string",
            _ => "string"
        };
    }
    
    /// <summary>
    /// Safely gets a string from an IfcLabel (which is a simple value type)
    /// </summary>
    private string? GetLabelValue(object? label)
    {
        if (label == null) return null;
        var str = label.ToString();
        return string.IsNullOrEmpty(str) ? null : str;
    }
    
    /// <summary>
    /// Safely gets a string from an IfcText (which is a simple value type)
    /// </summary>
    private string? GetTextValue(object? text)
    {
        if (text == null) return null;
        var str = text.ToString();
        return string.IsNullOrEmpty(str) ? null : str;
    }
    
    /// <summary>
    /// Safely gets a string from an IfcIdentifier (which is a simple value type)
    /// </summary>
    private string? GetIdentifierValue(object? identifier)
    {
        if (identifier == null) return null;
        var str = identifier.ToString();
        return string.IsNullOrEmpty(str) ? null : str;
    }
    
    public override void Dispose()
    {
        _disposed = true;
        // Note: We don't dispose the IModel as it's owned by the caller
        base.Dispose();
    }
}
