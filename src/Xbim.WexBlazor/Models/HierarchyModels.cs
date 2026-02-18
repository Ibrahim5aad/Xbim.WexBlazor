namespace Xbim.WexBlazor.Models;

public class HierarchyNode
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int? ProductType { get; set; }
    public int? ModelId { get; set; }
    public int ProductCount { get; set; }
    public bool IsExpanded { get; set; }
    public bool IsSelected { get; set; }
    public List<HierarchyNode> Children { get; set; } = new();
    public object? Tag { get; set; }
}

public class ProductTypeInfo
{
    public int TypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int[] ProductIds { get; set; } = Array.Empty<int>();
    public int ModelId { get; set; }
}

public class ModelHierarchy
{
    public int ModelId { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public List<ProductTypeInfo> ProductTypes { get; set; } = new();
    public HierarchyNode? SpatialStructure { get; set; }
    public bool HasSpatialStructure => SpatialStructure != null;
}

public static class ProductTypeNames
{
    private static readonly Dictionary<int, (string Name, string Icon)> _typeInfo = new()
    {
        // Spatial Elements
        { 170, ("Spatial Structure Element", "bi-diagram-3") },
        { 169, ("Building", "bi-building") },
        { 459, ("Building Storey", "bi-layers") },
        { 349, ("Site", "bi-geo-alt") },
        { 454, ("Space", "bi-box") },
        
        // Building Elements
        { 26, ("Building Element", "bi-bricks") },
        { 452, ("Wall", "bi-square") },
        { 453, ("Wall (Standard Case)", "bi-square") },
        { 1314, ("Wall (Elemented Case)", "bi-square") },
        { 213, ("Door", "bi-door-open") },
        { 1151, ("Door (Standard Case)", "bi-door-open") },
        { 667, ("Window", "bi-window") },
        { 1316, ("Window (Standard Case)", "bi-window") },
        { 99, ("Slab", "bi-square-fill") },
        { 1268, ("Slab (Elemented Case)", "bi-square-fill") },
        { 1269, ("Slab (Standard Case)", "bi-square-fill") },
        { 347, ("Roof", "bi-house") },
        { 346, ("Stair", "bi-ladder") },
        { 25, ("Stair Flight", "bi-ladder") },
        { 348, ("Ramp Flight", "bi-ladder") },
        { 414, ("Ramp", "bi-ladder") },
        { 350, ("Railing", "bi-grip-horizontal") },
        { 351, ("Plate", "bi-square") },
        { 1224, ("Plate (Standard Case)", "bi-square") },
        { 171, ("Beam", "bi-dash-lg") },
        { 1104, ("Beam (Standard Case)", "bi-dash-lg") },
        { 383, ("Column", "bi-bar-chart") },
        { 1126, ("Column (Standard Case)", "bi-bar-chart") },
        { 310, ("Member", "bi-dash-lg") },
        { 1214, ("Member (Standard Case)", "bi-dash-lg") },
        { 382, ("Covering", "bi-layers") },
        { 456, ("Curtain Wall", "bi-grid") },
        { 120, ("Footing", "bi-box") },
        { 572, ("Pile", "bi-box") },
        { 560, ("Building Element Proxy", "bi-box") },
        { 1120, ("Chimney", "bi-box") },
        { 1265, ("Shading Device", "bi-box") },
        
        // Openings
        { 498, ("Opening Element", "bi-dash-square") },
        { 1217, ("Opening (Standard Case)", "bi-dash-square") },
        
        // Furnishing
        { 253, ("Furnishing Element", "bi-lamp") },
        { 1184, ("Furniture", "bi-lamp") },
        { 1291, ("System Furniture Element", "bi-lamp") },
        
        // Distribution Elements (MEP)
        { 44, ("Distribution Element", "bi-gear") },
        { 45, ("Distribution Flow Element", "bi-gear") },
        { 46, ("Flow Terminal", "bi-gear") },
        { 121, ("Flow Controller", "bi-sliders") },
        { 175, ("Energy Conversion Device", "bi-lightning") },
        { 467, ("Flow Fitting", "bi-diagram-2") },
        { 502, ("Flow Moving Device", "bi-fan") },
        { 574, ("Flow Segment", "bi-dash-lg") },
        { 371, ("Flow Storage Device", "bi-box") },
        { 425, ("Flow Treatment Device", "bi-funnel") },
        { 180, ("Distribution Chamber Element", "bi-box") },
        { 468, ("Distribution Control Element", "bi-cpu") },
        
        // Structural Elements
        { 226, ("Structural Item", "bi-grid-3x3") },
        { 225, ("Structural Member", "bi-grid-3x3") },
        { 265, ("Structural Connection", "bi-diagram-2") },
        
        // Transport
        { 416, ("Transport Element", "bi-arrow-left-right") },
        
        // Annotations
        { 634, ("Annotation", "bi-card-text") },
        
        // Other
        { 20, ("Product", "bi-box") },
        { 19, ("Element", "bi-box") },
        { 18, ("Element Assembly", "bi-boxes") },
        { 168, ("Virtual Element", "bi-box") },
        { 447, ("Proxy", "bi-box") },
        { 564, ("Grid", "bi-grid-3x3") },
        { 178, ("Distribution Port", "bi-circle") },
        { 179, ("Port", "bi-circle") },
        { 1122, ("Civil Element", "bi-signpost") },
        { 1185, ("Geographic Element", "bi-globe") },
    };

    public static string GetTypeName(int typeId)
    {
        return _typeInfo.TryGetValue(typeId, out var info) ? info.Name : $"Type {typeId}";
    }

    public static string GetTypeIcon(int typeId)
    {
        return _typeInfo.TryGetValue(typeId, out var info) ? info.Icon : "bi-box";
    }

    public static string GetDisplayName(int typeId)
    {
        var name = GetTypeName(typeId);
        return name.Replace("(Standard Case)", "").Replace("(Elemented Case)", "").Trim();
    }
}
