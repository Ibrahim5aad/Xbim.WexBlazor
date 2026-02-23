namespace Xbim.WexBlazor.Models;

/// <summary>
/// Icons plugin for displaying data visualization icons anchored to model elements.
/// After adding this plugin to the viewer via AddPluginAsync, use the viewer's icon methods
/// (AddIconAsync, UpdateIconsLocationsAsync, SetFloatingDetailsStateAsync)
/// to manage icons.
/// </summary>
public class IconsPlugin : ViewerPlugin
{
    public override string PluginType => "Icons";
}

/// <summary>
/// Represents an icon to display in the 3D viewer, anchored to model elements.
/// </summary>
public class ViewerIcon
{
    /// <summary>
    /// Name of the icon (used as display label)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// A brief description of the icon
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Optional value readout text displayed on the icon
    /// </summary>
    public string? ValueReadout { get; set; }

    /// <summary>
    /// Products this icon is associated with.
    /// If Location is null, the icon is placed at the centroid of these products.
    /// </summary>
    public List<ProductIdentity>? Products { get; set; }

    /// <summary>
    /// Base64-encoded image data for the icon.
    /// If null, a default icon is used.
    /// </summary>
    public string? ImageData { get; set; }

    /// <summary>
    /// Optional XYZ location as [x, y, z].
    /// If null, the centroid of the associated product bounding box is used.
    /// </summary>
    public float[]? Location { get; set; }

    /// <summary>
    /// Optional icon width in pixels. If null, the default width is used.
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Optional icon height in pixels. If null, the default height is used.
    /// </summary>
    public int? Height { get; set; }
}
