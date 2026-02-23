using Microsoft.JSInterop;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xbim.WexBlazor.Interop;

/// <summary>
/// JavaScript interop for Xbim Viewer
/// </summary>
public class XbimViewerInterop : JsInteropBase
{
    private static readonly string ModulePath = "./_content/Xbim.WexBlazor/js/XbimViewerInterop.js";

    public XbimViewerInterop(IJSRuntime jsRuntime) 
        : base(jsRuntime, ModulePath)
    {
    }

    /// <summary>
    /// Initializes a new xBIM Viewer instance
    /// </summary>
    /// <param name="canvasId">The ID of the canvas element</param>
    /// <param name="options">Viewer configuration options</param>
    /// <returns>Viewer reference ID</returns>
    public async ValueTask<string?> InitViewerAsync(string canvasId, object? options = null)
    {
        return await InvokeAsync<string?>("initViewer", canvasId, options ?? new { });
    }

    /// <summary>
    /// Loads a wexBIM model from the specified URL
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <param name="modelUrl">URL to the wexBIM model file</param>
    /// <param name="tag">Optional tag data to associate with the model</param>
    /// <returns>Model ID if successful, null otherwise</returns>
    public async ValueTask<int?> LoadModelAsync(string viewerId, string modelUrl, object? tag = null)
    {
        return await InvokeAsync<int?>("loadModel", viewerId, modelUrl, tag ?? new { });
    }

    /// <summary>
    /// Starts the viewer rendering loop
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <returns>True if successful</returns>
    public async ValueTask<bool> StartAsync(string viewerId)
    {
        return await InvokeAsync<bool>("start", viewerId);
    }

    /// <summary>
    /// Sets the viewer background color
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <param name="color">CSS color string in hex format (e.g. "#FF0000" for red)</param>
    /// <returns>True if successful</returns>
    public async ValueTask<bool> SetBackgroundColorAsync(string viewerId, string color)
    {
        // Convert hex color to RGBA array
        int[] rgba = HexToRgba(color);
        return await InvokeAsync<bool>("setBackgroundColor", viewerId, rgba);
    }

    /// <summary>
    /// Sets the highlighting (selection) color
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <param name="color">CSS color string in hex format (e.g. "#FF0000" for red)</param>
    /// <returns>True if successful</returns>
    public async ValueTask<bool> SetHighlightingColorAsync(string viewerId, string color)
    {
        // Convert hex color to RGBA array
        int[] rgba = HexToRgba(color);
        return await InvokeAsync<bool>("setHighlightingColor", viewerId, rgba);
    }

    /// <summary>
    /// Sets the hover pick color
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <param name="color">CSS color string in hex format (e.g. "#FF0000" for red)</param>
    /// <returns>True if successful</returns>
    public async ValueTask<bool> SetHoverPickColorAsync(string viewerId, string color)
    {
        // Convert hex color to RGBA array
        int[] rgba = HexToRgba(color);
        return await InvokeAsync<bool>("setHoverPickColor", viewerId, rgba);
    }

    /// <summary>
    /// Converts a hex color string to an RGBA integer array
    /// </summary>
    /// <param name="hex">Hex color string (e.g. "#FF0000" or "#FF0000FF")</param>
    /// <returns>RGBA array [r, g, b, a]</returns>
    private int[] HexToRgba(string hex)
    {
        // Remove # if present
        hex = hex.TrimStart('#');

        int r, g, b, a = 255;

        if (hex.Length == 6)
        {
            // #RRGGBB format
            r = Convert.ToInt32(hex.Substring(0, 2), 16);
            g = Convert.ToInt32(hex.Substring(2, 2), 16);
            b = Convert.ToInt32(hex.Substring(4, 2), 16);
        }
        else if (hex.Length == 8)
        {
            // #RRGGBBAA format
            r = Convert.ToInt32(hex.Substring(0, 2), 16);
            g = Convert.ToInt32(hex.Substring(2, 2), 16);
            b = Convert.ToInt32(hex.Substring(4, 2), 16);
            a = Convert.ToInt32(hex.Substring(6, 2), 16);
        }
        else if (hex.Length == 3)
        {
            // #RGB format
            r = Convert.ToInt32(hex[0].ToString() + hex[0].ToString(), 16);
            g = Convert.ToInt32(hex[1].ToString() + hex[1].ToString(), 16);
            b = Convert.ToInt32(hex[2].ToString() + hex[2].ToString(), 16);
        }
        else
        {
            // Invalid format, default to black
            r = 0;
            g = 0;
            b = 0;
        }

        return new int[] { r, g, b, a };
    }

    /// <summary>
    /// Zooms to fit all elements in the view
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <returns>True if successful</returns>
    public async ValueTask<bool> ZoomFitAsync(string viewerId)
    {
        return await InvokeAsync<bool>("zoomFit", viewerId);
    }

    /// <summary>
    /// Hides specific elements by their IDs
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <param name="elementIds">Array of element IDs to hide</param>
    /// <returns>True if successful</returns>
    public async ValueTask<bool> HideElementsAsync(string viewerId, int[] elementIds)
    {
        return await InvokeAsync<bool>("hideElements", viewerId, elementIds);
    }

    /// <summary>
    /// Shows specific elements by their IDs
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <param name="elementIds">Array of element IDs to show</param>
    /// <returns>True if successful</returns>
    public async ValueTask<bool> ShowElementsAsync(string viewerId, int[] elementIds)
    {
        return await InvokeAsync<bool>("showElements", viewerId, elementIds);
    }

    public async ValueTask<bool> UnhideAllElementsAsync(string viewerId)
    {
        return await InvokeAsync<bool>("unhideAllElements", viewerId);
    }


    /// <summary>
    /// Isolates specific elements (hides everything else)
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <param name="elementIds">Array of element IDs to isolate</param>
    /// <returns>True if successful</returns>
    public async ValueTask<bool> IsolateElementsAsync(string viewerId, int[] elementIds)
    {
        return await InvokeAsync<bool>("isolateElements", viewerId, elementIds);
    }

    /// <summary>
    /// Unisolates elements (shows all hidden elements)
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <returns>True if successful</returns>
    public async ValueTask<bool> UnisolateElementsAsync(string viewerId)
    {
        return await InvokeAsync<bool>("unisolateElements", viewerId);
    }

    /// <summary>
    /// Gets the list of currently isolated element IDs
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <returns>Array of isolated element IDs</returns>
    public async ValueTask<int[]> GetIsolatedElementsAsync(string viewerId)
    {
        return await InvokeAsync<int[]>("getIsolatedElements", viewerId);
    }

    /// <summary>
    /// Resets the viewer to its initial state
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <returns>True if successful</returns>
    public async ValueTask<bool> ResetAsync(string viewerId)
    {
        return await InvokeAsync<bool>("reset", viewerId);
    }

    /// <summary>
    /// Shows a specific view type
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <param name="type">The view type (use ViewerConstants.ViewType)</param>
    /// <param name="id">Optional product ID</param>
    /// <param name="model">Optional model ID</param>
    /// <param name="withAnimation">Whether to animate the transition</param>
    /// <returns>True if successful</returns>
    public async ValueTask<bool> ShowAsync(string viewerId, int type, int? id = null, int? model = null, bool withAnimation = true)
    {
        return await InvokeAsync<bool>("show", viewerId, type, id ?? (object?)null, model ?? (object?)null, withAnimation);
    }

    /// <summary>
    /// Generic method to invoke any viewer method dynamically.
    /// Use this for methods that don't have a typed wrapper yet.
    /// </summary>
    /// <typeparam name="T">Return type</typeparam>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <param name="methodName">Name of the viewer method to call</param>
    /// <param name="args">Arguments to pass to the method</param>
    /// <returns>Result of the method call</returns>
    public async ValueTask<T?> InvokeViewerMethodAsync<T>(string viewerId, string methodName, params object[] args)
    {
        var allArgs = new object[] { viewerId, methodName }.Concat(args).ToArray();
        return await InvokeAsync<T?>("invokeViewerMethod", allArgs);
    }

    /// <summary>
    /// Highlights/selects specific elements by their IDs (replaces current selection)
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <param name="elementIds">Array of element IDs to highlight</param>
    /// <param name="modelId">Optional model ID</param>
    /// <returns>True if successful</returns>
    public async ValueTask<bool> HighlightElementsAsync(string viewerId, int[] elementIds, int? modelId = null)
    {
        if (modelId.HasValue)
            return await InvokeAsync<bool>("highlightElements", viewerId, elementIds, modelId.Value);
        return await InvokeAsync<bool>("highlightElements", viewerId, elementIds);
    }

    /// <summary>
    /// Unhighlights (restores to normal style) the specified elements
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <param name="elementIds">Array of element IDs to unhighlight</param>
    /// <param name="modelId">Optional model ID</param>
    /// <returns>True if successful</returns>
    public async ValueTask<bool> UnhighlightElementsAsync(string viewerId, int[] elementIds, int? modelId = null)
    {
        if (modelId.HasValue)
            return await InvokeAsync<bool>("unhighlightElements", viewerId, elementIds, modelId.Value);
        return await InvokeAsync<bool>("unhighlightElements", viewerId, elementIds);
    }

    /// <summary>
    /// Checks if an element is currently highlighted
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <param name="elementId">The element ID to check</param>
    /// <param name="modelId">Optional model ID</param>
    /// <returns>True if the element is highlighted</returns>
    public async ValueTask<bool> IsElementHighlightedAsync(string viewerId, int elementId, int? modelId = null)
    {
        if (modelId.HasValue)
            return await InvokeAsync<bool>("isElementHighlighted", viewerId, elementId, modelId.Value);
        return await InvokeAsync<bool>("isElementHighlighted", viewerId, elementId);
    }

    /// <summary>
    /// Adds elements to the current selection
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <param name="elementIds">Array of element IDs to add to selection</param>
    /// <param name="modelId">Optional model ID</param>
    /// <returns>True if successful</returns>
    public async ValueTask<bool> AddToSelectionAsync(string viewerId, int[] elementIds, int? modelId = null)
    {
        if (modelId.HasValue)
            return await InvokeAsync<bool>("addToSelection", viewerId, elementIds, modelId.Value);
        return await InvokeAsync<bool>("addToSelection", viewerId, elementIds);
    }

    /// <summary>
    /// Removes elements from the current selection
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <param name="elementIds">Array of element IDs to remove from selection</param>
    /// <param name="modelId">Optional model ID</param>
    /// <returns>True if successful</returns>
    public async ValueTask<bool> RemoveFromSelectionAsync(string viewerId, int[] elementIds, int? modelId = null)
    {
        if (modelId.HasValue)
            return await InvokeAsync<bool>("removeFromSelection", viewerId, elementIds, modelId.Value);
        return await InvokeAsync<bool>("removeFromSelection", viewerId, elementIds);
    }

    /// <summary>
    /// Clears all selected/highlighted elements
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <returns>True if successful</returns>
    public async ValueTask<bool> ClearSelectionAsync(string viewerId)
    {
        return await InvokeAsync<bool>("clearSelection", viewerId);
    }

    /// <summary>
    /// Gets all currently selected/highlighted elements
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <returns>Array of selected elements with their IDs and model IDs</returns>
    public async ValueTask<SelectedElement[]> GetSelectedElementsAsync(string viewerId)
    {
        return await InvokeAsync<SelectedElement[]>("getSelectedElements", viewerId) ?? Array.Empty<SelectedElement>();
    }

    public async ValueTask<ProductTypeResult[]> GetModelProductTypesAsync(string viewerId, int? modelId = null)
    {
        if (modelId.HasValue)
            return await InvokeAsync<ProductTypeResult[]>("getModelProductTypes", viewerId, modelId.Value) ?? Array.Empty<ProductTypeResult>();
        return await InvokeAsync<ProductTypeResult[]>("getModelProductTypes", viewerId) ?? Array.Empty<ProductTypeResult>();
    }

    public async ValueTask<int?> GetProductTypeAsync(string viewerId, int productId, int? modelId = null)
    {
        if (modelId.HasValue)
            return await InvokeAsync<int?>("getProductType", viewerId, productId, modelId.Value);
        return await InvokeAsync<int?>("getProductType", viewerId, productId);
    }

    public async ValueTask<int[]> GetProductsOfTypeAsync(string viewerId, int typeId, int? modelId = null)
    {
        if (modelId.HasValue)
            return await InvokeAsync<int[]>("getProductsOfType", viewerId, typeId, modelId.Value) ?? Array.Empty<int>();
        return await InvokeAsync<int[]>("getProductsOfType", viewerId, typeId) ?? Array.Empty<int>();
    }

    public async ValueTask<ProductTypeCountResult[]> GetAllProductTypesAsync(string viewerId)
    {
        return await InvokeAsync<ProductTypeCountResult[]>("getAllProductTypes", viewerId) ?? Array.Empty<ProductTypeCountResult>();
    }

    /// <summary>
    /// Registers an event listener for viewer events
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <param name="eventName">Name of the event (e.g., "pick", "click", "loaded")</param>
    /// <param name="dotNetHelper">DotNetObjectReference for callbacks</param>
    /// <returns>True if successful</returns>
    public async ValueTask<bool> AddEventListenerAsync<T>(string viewerId, string eventName, DotNetObjectReference<T> dotNetHelper) where T : class
    {
        return await InvokeAsync<bool>("addEventListener", viewerId, eventName, dotNetHelper);
    }

    /// <summary>
    /// Unregisters an event listener
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <param name="eventName">Name of the event to unregister</param>
    /// <returns>True if successful</returns>
    public async ValueTask<bool> RemoveEventListenerAsync(string viewerId, string eventName)
    {
        return await InvokeAsync<bool>("removeEventListener", viewerId, eventName);
    }

    /// <summary>
    /// Disposes a viewer instance
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <returns>True if successful</returns>
    public async ValueTask<bool> DisposeViewerAsync(string viewerId)
    {
        return await InvokeAsync<bool>("disposeViewer", viewerId);
    }

    // Model Management Methods

    /// <summary>
    /// Unloads a specific model from the viewer
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <param name="modelId">The model ID to unload</param>
    /// <returns>True if successful</returns>
    public async ValueTask<bool> UnloadModelAsync(string viewerId, int modelId)
    {
        return await InvokeAsync<bool>("unloadModel", viewerId, modelId);
    }

    /// <summary>
    /// Gets the list of loaded models from the viewer
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <returns>List of loaded model information</returns>
    public async ValueTask<List<ModelInfo>> GetLoadedModelsAsync(string viewerId)
    {
        return await InvokeAsync<List<ModelInfo>>("getLoadedModels", viewerId) ?? new List<ModelInfo>();
    }

    /// <summary>
    /// Sets the visibility of a specific model
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <param name="modelId">The model ID</param>
    /// <param name="visible">True to show, false to hide</param>
    /// <returns>True if successful</returns>
    public async ValueTask<bool> SetModelVisibilityAsync(string viewerId, int modelId, bool visible)
    {
        return await InvokeAsync<bool>("setModelVisibility", viewerId, modelId, visible);
    }

    /// <summary>
    /// Debug helper to get all products and model information (for troubleshooting)
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <returns>Debug information as JSON</returns>
    public async ValueTask<string> DebugGetAllProductsAsync(string viewerId)
    {
        var result = await InvokeAsync<object>("debugGetAllProducts", viewerId);
        return System.Text.Json.JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    }

    // Plugin Management Methods

    /// <summary>
    /// Adds a plugin to the viewer
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <param name="pluginId">Unique identifier for this plugin instance</param>
    /// <param name="pluginType">The type of plugin (JavaScript class name)</param>
    /// <param name="config">Optional plugin configuration</param>
    /// <returns>True if successful</returns>
    public async ValueTask<bool> AddPluginAsync(string viewerId, string pluginId, string pluginType, object? config = null)
    {
        return await InvokeAsync<bool>("addPlugin", viewerId, pluginId, pluginType, config ?? new { });
    }

    /// <summary>
    /// Removes a plugin from the viewer
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <param name="pluginId">The plugin identifier</param>
    /// <returns>True if successful</returns>
    public async ValueTask<bool> RemovePluginAsync(string viewerId, string pluginId)
    {
        return await InvokeAsync<bool>("removePlugin", viewerId, pluginId);
    }

    /// <summary>
    /// Sets the stopped state of a plugin
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <param name="pluginId">The plugin identifier</param>
    /// <param name="stopped">True to stop, false to start</param>
    /// <returns>True if successful</returns>
    public async ValueTask<bool> SetPluginStoppedAsync(string viewerId, string pluginId, bool stopped)
    {
        return await InvokeAsync<bool>("setPluginStopped", viewerId, pluginId, stopped);
    }

    /// <summary>
    /// Gets list of active plugins
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <returns>List of active plugin information</returns>
    public async ValueTask<List<PluginInfo>> GetActivePluginsAsync(string viewerId)
    {
        return await InvokeAsync<List<PluginInfo>>("getActivePlugins", viewerId) ?? new List<PluginInfo>();
    }

    /// <summary>
    /// Removes the clipping plane from the viewer
    /// </summary>
    /// <param name="viewerId">The viewer reference ID</param>
    /// <returns>True if successful</returns>
    public async ValueTask<bool> UnclipAsync(string viewerId)
    {
        return await InvokeAsync<bool>("unclip", viewerId);
    }

    public async ValueTask<bool> CreateSectionBoxAsync(string viewerId, string pluginId)
    {
        return await InvokeAsync<bool>("createSectionBox", viewerId, pluginId);
    }

    public async ValueTask<bool> ClearSectionBoxAsync(string viewerId, string pluginId)
    {
        return await InvokeAsync<bool>("clearSectionBox", viewerId, pluginId);
    }

    // ============================================================
    // Heatmap Plugin Methods
    // ============================================================

    /// <summary>
    /// Adds a channel to the Heatmap plugin.
    /// The plugin must already be added to the viewer via AddPluginAsync.
    /// </summary>
    public async ValueTask<bool> AddHeatmapChannelAsync(string viewerId, string pluginId, object channelConfig)
    {
        return await InvokeAsync<bool>("addHeatmapChannel", viewerId, pluginId, channelConfig);
    }

    /// <summary>
    /// Adds a data source to the Heatmap plugin.
    /// </summary>
    public async ValueTask<bool> AddHeatmapSourceAsync(string viewerId, string pluginId, object sourceConfig)
    {
        return await InvokeAsync<bool>("addHeatmapSource", viewerId, pluginId, sourceConfig);
    }

    /// <summary>
    /// Renders (applies visualization for) a specific heatmap channel.
    /// </summary>
    public async ValueTask<bool> RenderHeatmapChannelAsync(string viewerId, string pluginId, string channelId)
    {
        return await InvokeAsync<bool>("renderHeatmapChannel", viewerId, pluginId, channelId);
    }

    /// <summary>
    /// Renders (applies visualization for) a specific heatmap source.
    /// </summary>
    public async ValueTask<bool> RenderHeatmapSourceAsync(string viewerId, string pluginId, string sourceId)
    {
        return await InvokeAsync<bool>("renderHeatmapSource", viewerId, pluginId, sourceId);
    }

    /// <summary>
    /// Updates an existing heatmap source's value in-place (without removing/re-adding).
    /// Call RenderHeatmapChannelAsync afterwards to refresh the visualization.
    /// </summary>
    public async ValueTask<bool> UpdateHeatmapSourceValueAsync(string viewerId, string pluginId, string sourceId, object value)
    {
        return await InvokeAsync<bool>("updateHeatmapSourceValue", viewerId, pluginId, sourceId, value);
    }

    // ============================================================
    // Icons Plugin Methods
    // ============================================================

    /// <summary>
    /// Adds an icon to the Icons plugin.
    /// </summary>
    public async ValueTask<bool> AddIconAsync(string viewerId, string pluginId, object iconConfig)
    {
        return await InvokeAsync<bool>("addIcon", viewerId, pluginId, iconConfig);
    }

    /// <summary>
    /// Updates all icon locations (recalculates from product bounding boxes).
    /// </summary>
    public async ValueTask<bool> UpdateIconsLocationsAsync(string viewerId, string pluginId)
    {
        return await InvokeAsync<bool>("updateIconsLocations", viewerId, pluginId);
    }

    /// <summary>
    /// Enables or disables floating detail tooltips on icon hover.
    /// </summary>
    public async ValueTask<bool> SetFloatingDetailsStateAsync(string viewerId, string pluginId, bool enabled)
    {
        return await InvokeAsync<bool>("setFloatingDetailsState", viewerId, pluginId, enabled);
    }

    /// <summary>
    /// Updates an existing icon's valueReadout text by its index in the plugin's icon list.
    /// </summary>
    public async ValueTask<bool> UpdateIconReadoutAsync(string viewerId, string pluginId, int iconIndex, string readout)
    {
        return await InvokeAsync<bool>("updateIconReadout", viewerId, pluginId, iconIndex, readout);
    }

    /// <summary>
    /// Locks the grid plugin to the current full model region so it doesn't
    /// resize when elements are isolated.
    /// </summary>
    public async ValueTask<bool> LockGridRegionAsync(string viewerId, string pluginId)
    {
        return await InvokeAsync<bool>("lockGridRegion", viewerId, pluginId);
    }

    /// <summary>
    /// Updates the grid plugin's colour and fixes the blend mode so that
    /// light-coloured grids render correctly on dark backgrounds.
    /// </summary>
    public async ValueTask<bool> UpdateGridColorAsync(string viewerId, string pluginId, double[] colour)
    {
        return await InvokeAsync<bool>("updateGridColor", viewerId, pluginId, colour);
    }
}

/// <summary>
/// Information about an active plugin
/// </summary>
public class PluginInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("stopped")]
    public bool Stopped { get; set; }
}

/// <summary>
/// Information about a loaded model from JavaScript
/// </summary>
public class ModelInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("tag")]
    public object? Tag { get; set; }

    [JsonPropertyName("loadedAt")]
    public DateTime LoadedAt { get; set; }
}

/// <summary>
/// Represents a selected element in the viewer
/// </summary>
public class SelectedElement
{
    public int Id { get; set; }
    public int Model { get; set; }
}

/// <summary>
/// Event data from viewer events
/// </summary>
public class ViewerEventArgs
{
    [JsonPropertyName("eventName")]
    public string EventName { get; set; } = string.Empty;
    
    [JsonPropertyName("id")]
    public int? Id { get; set; }
    
    [JsonPropertyName("model")]
    public int? Model { get; set; }
    
    [JsonPropertyName("modelId")]
    public int? ModelId { get; set; }
    
    [JsonPropertyName("x")]
    public double? X { get; set; }
    
    [JsonPropertyName("y")]
    public double? Y { get; set; }
    
    [JsonPropertyName("z")]
    public double? Z { get; set; }
    
    [JsonPropertyName("tag")]
    public object? Tag { get; set; }
    
    [JsonPropertyName("message")]
    public string? Message { get; set; }
    
    /// <summary>
    /// Gets the 3D position if available
    /// </summary>
    public (double X, double Y, double Z)? Position => 
        X.HasValue && Y.HasValue && Z.HasValue 
            ? (X.Value, Y.Value, Z.Value) 
            : null;
}

public class ProductTypeResult
{
    [JsonPropertyName("typeId")]
    public int TypeId { get; set; }
    
    [JsonPropertyName("productIds")]
    public int[] ProductIds { get; set; } = Array.Empty<int>();
    
    [JsonPropertyName("modelId")]
    public int ModelId { get; set; }
}

public class ProductTypeCountResult
{
    [JsonPropertyName("typeId")]
    public int TypeId { get; set; }
    
    [JsonPropertyName("count")]
    public int Count { get; set; }
    
    [JsonPropertyName("modelId")]
    public int ModelId { get; set; }
} 