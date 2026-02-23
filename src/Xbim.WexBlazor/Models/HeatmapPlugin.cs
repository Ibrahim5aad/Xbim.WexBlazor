using System.Text.Json.Serialization;

namespace Xbim.WexBlazor.Models;

/// <summary>
/// Heatmap plugin for data visualization on model elements.
/// After adding this plugin to the viewer via AddPluginAsync, use the viewer's heatmap methods
/// (AddHeatmapChannelAsync, AddHeatmapSourceAsync, RenderHeatmapChannelAsync)
/// to configure and display data.
/// </summary>
public class HeatmapPlugin : ViewerPlugin
{
    public override string PluginType => "Heatmap";
}

/// <summary>
/// The type of heatmap channel, matching the JavaScript ChannelType enum
/// </summary>
public enum HeatmapChannelType
{
    Continuous,
    Discrete,
    ValueRanges,
    Constant
}

/// <summary>
/// Base class for all heatmap channel types.
/// A channel defines how data values are visually represented (color mapping).
/// </summary>
public abstract class HeatmapChannel
{
    /// <summary>
    /// The type of channel (determines the rendering strategy)
    /// </summary>
    public abstract HeatmapChannelType ChannelType { get; }

    /// <summary>
    /// A user-defined unique identifier for the channel
    /// </summary>
    public string ChannelId { get; set; } = string.Empty;

    /// <summary>
    /// The data type of the channel values (e.g., "number", "string")
    /// </summary>
    public string DataType { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the channel
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// A brief description of the channel
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The data property being visualized
    /// </summary>
    public string Property { get; set; } = string.Empty;

    /// <summary>
    /// The unit of measurement for the channel values
    /// </summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>
    /// Whether this channel is enabled
    /// </summary>
    public bool IsEnabled { get; set; } = true;
}

/// <summary>
/// A continuous heatmap channel for representing a range of continuous values
/// with interpolated color gradients.
/// </summary>
public class ContinuousHeatmapChannel : HeatmapChannel
{
    public override HeatmapChannelType ChannelType => HeatmapChannelType.Continuous;

    /// <summary>
    /// Minimum value for the data range
    /// </summary>
    public double Min { get; set; }

    /// <summary>
    /// Maximum value for the data range
    /// </summary>
    public double Max { get; set; }

    /// <summary>
    /// Hex color gradient stops used to interpolate the visualization
    /// (e.g., ["#0000FF", "#00FF00", "#FF0000"] for blue-green-red)
    /// </summary>
    public string[] ColorGradient { get; set; } = Array.Empty<string>();
}

/// <summary>
/// A discrete heatmap channel for representing a set of distinct values,
/// each mapped to a specific color.
/// </summary>
public class DiscreteHeatmapChannel : HeatmapChannel
{
    public override HeatmapChannelType ChannelType => HeatmapChannelType.Discrete;

    /// <summary>
    /// Mapping of discrete values to their hex colors
    /// (e.g., {"Good": "#00FF00", "Warning": "#FFFF00", "Bad": "#FF0000"})
    /// </summary>
    public Dictionary<string, string> Values { get; set; } = new();
}

/// <summary>
/// A value ranges heatmap channel for representing numeric ranges,
/// each with a distinct color and label.
/// </summary>
public class ValueRangesHeatmapChannel : HeatmapChannel
{
    public override HeatmapChannelType ChannelType => HeatmapChannelType.ValueRanges;

    /// <summary>
    /// List of value ranges with their color mappings
    /// </summary>
    public List<HeatmapValueRange> Ranges { get; set; } = new();
}

/// <summary>
/// A constant color channel that applies a single color to all associated elements.
/// </summary>
public class ConstantColorHeatmapChannel : HeatmapChannel
{
    public override HeatmapChannelType ChannelType => HeatmapChannelType.Constant;

    /// <summary>
    /// Hex color string (e.g., "#FF0000")
    /// </summary>
    public string Color { get; set; } = string.Empty;
}

/// <summary>
/// Defines a numeric range with an associated color for ValueRangesHeatmapChannel
/// </summary>
public class HeatmapValueRange
{
    /// <summary>
    /// Minimum value of the range
    /// </summary>
    public double Min { get; set; }

    /// <summary>
    /// Maximum value of the range
    /// </summary>
    public double Max { get; set; }

    /// <summary>
    /// Hex color string for this range
    /// </summary>
    public string Color { get; set; } = string.Empty;

    /// <summary>
    /// Display label for this range
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Render priority (higher priority ranges are rendered on top)
    /// </summary>
    public int Priority { get; set; }
}

/// <summary>
/// Identifies a product (element) within a specific model.
/// Used by heatmap sources and icons to reference model elements.
/// </summary>
public class ProductIdentity
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("model")]
    public int Model { get; set; }
}

/// <summary>
/// A heatmap data source that provides values to a channel for specific products.
/// Sources feed data into channels which determine the visual representation.
/// </summary>
public class HeatmapSource
{
    /// <summary>
    /// Unique identifier for this source
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Products (elements) associated with this source
    /// </summary>
    public List<ProductIdentity> Products { get; set; } = new();

    /// <summary>
    /// The channel ID this source feeds data to
    /// </summary>
    public string ChannelId { get; set; } = string.Empty;

    /// <summary>
    /// The value for this source. Type depends on the channel type:
    /// Continuous/ValueRanges: number (double), Discrete: string, Constant: any
    /// </summary>
    public object? Value { get; set; }
}
