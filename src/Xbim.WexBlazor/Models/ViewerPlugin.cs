namespace Xbim.WexBlazor.Models;

/// <summary>
/// Base class for xBIM Viewer plugins
/// </summary>
public abstract class ViewerPlugin
{
    /// <summary>
    /// Unique identifier for this plugin instance
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// The type of plugin (matches JavaScript plugin class name)
    /// </summary>
    public abstract string PluginType { get; }

    /// <summary>
    /// Whether the plugin is currently stopped (not rendering)
    /// </summary>
    public bool IsStopped { get; set; }

    /// <summary>
    /// Optional configuration for the plugin
    /// </summary>
    public virtual object? GetConfiguration() => null;
}

/// <summary>
/// Interactive Section Box plugin for clipping the model
/// </summary>
public class SectionBoxPlugin : ViewerPlugin
{
    public override string PluginType => "InteractiveSectionBox";

    /// <summary>
    /// Color of the section box (hex format, e.g., "#FF0000")
    /// </summary>
    public string? BoxColor { get; set; }

    public override object? GetConfiguration()
    {
        return BoxColor != null ? new { boxColor = BoxColor } : null;
    }
}

/// <summary>
/// Navigation Cube plugin for orientation
/// </summary>
public class NavigationCubePlugin : ViewerPlugin
{
    public override string PluginType => "NavigationCube";

    /// <summary>
    ///  Size of the cube relative to the size of viewer canvas. This has to be a positive number between [0,1] Default value is 0.15. 
    /// </summary>
    public double Ratio { get; set; } = 0.15;
    
    /// <summary>
    /// Navigation cube has two transparency states. One is when user hovers over the cube and the second when the cursor is anywhere else.
    /// This is for the non-hovering state, and it should be a positive number between [0,1]. If the value is less than 1 cube will be semitransparent 
    /// when user is not hovering over. Default value is 0.3. 
    /// </summary>
    public double PassiveAlpha { get; set; } = 0.7;
    
    /// <summary>
    ///  Navigation cube has two transparency states. One is when user hovers over the cube and the second when the cursor is anywhere else.
    /// This is for the hovering state and it should be a positive number between [0,1]. If the value is less than 1 cube will be semitransparent 
    /// when user hovers over. Default value is 1.0. 
    /// </summary>
    public double ActiveAlpha { get; set; } = 1;

    public override object? GetConfiguration()
    {
        return new
        {
            ratio = Ratio,
            passiveAlpha = PassiveAlpha,
            activeAlpha = ActiveAlpha
        };
    }
}

/// <summary>
/// Grid plugin for displaying a reference grid
/// </summary>
public class GridPlugin : ViewerPlugin
{
    public override string PluginType => "Grid";
    public double Factor { get; set; } = 1.0;
    public double ZFactor { get; set; } = 0.01;
    public int NumberOfLines { get; set; } = 10;
    public double[] Color { get; set; } = new[] { 0.5, 0.5, 0.5, 1.0 };

    public override object? GetConfiguration()
    {
        return new 
        { 
            factor = Factor,
            zFactor = ZFactor,
            numberOfLines = NumberOfLines,
            colour = Color
        };
    }
}

/// <summary>
/// Interactive Clipping Plane plugin
/// </summary>
public class ClippingPlanePlugin : ViewerPlugin
{
    public override string PluginType => "InteractiveClippingPlane";


    public override object? GetConfiguration()
    {
        return new {
            stopped = IsStopped
        };
    }
}
