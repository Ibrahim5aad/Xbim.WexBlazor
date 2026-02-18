using Xbim.Common;

namespace Xbim.WexBlazor.Models;

/// <summary>
/// Represents a model loaded in the viewer
/// </summary>
public class LoadedModel : IDisposable
{
    /// <summary>
    /// Unique identifier for this model in the viewer
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Display name of the model (usually the filename)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Source URL or path of the model
    /// </summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Type of source (File, URL, Blob)
    /// </summary>
    public ModelSourceType SourceType { get; set; }

    /// <summary>
    /// Whether the model is currently started (rendering/visible)
    /// </summary>
    public bool IsStarted { get; set; } = true;

    /// <summary>
    /// File size in bytes (if known)
    /// </summary>
    public long? SizeBytes { get; set; }

    /// <summary>
    /// When the model was loaded
    /// </summary>
    public DateTime LoadedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Blob URL if created (for cleanup)
    /// </summary>
    public string? BlobUrl { get; set; }

    /// <summary>
    /// Custom tag data associated with the model
    /// </summary>
    public object? Tag { get; set; }
    
    /// <summary>
    /// The IFC model (for property access). Only available when loaded from IFC file.
    /// </summary>
    public IModel? IfcModel { get; set; }
    
    /// <summary>
    /// Whether this model has property data available (IFC model loaded)
    /// </summary>
    public bool HasProperties => IfcModel != null;
    
    /// <summary>
    /// Original file format (IFC, wexbim, etc.)
    /// </summary>
    public ModelFormat OriginalFormat { get; set; } = ModelFormat.Wexbim;
    
    public void Dispose()
    {
        if (IfcModel is IDisposable disposable)
        {
            disposable.Dispose();
        }
        IfcModel = null;
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Type of model source
/// </summary>
public enum ModelSourceType
{
    LocalFile,
    Url,
    Blob
}

/// <summary>
/// Original format of the model file
/// </summary>
public enum ModelFormat
{
    /// <summary>
    /// wexbim format (geometry only)
    /// </summary>
    Wexbim,
    
    /// <summary>
    /// IFC format (full BIM data)
    /// </summary>
    Ifc,
    
    /// <summary>
    /// Compressed IFC format
    /// </summary>
    IfcZip
}

/// <summary>
/// Position of the model manager panel
/// </summary>
public enum ModelManagerPosition
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}

/// <summary>
/// Type of model change event
/// </summary>
public enum ModelChangeType
{
    Loaded,
    Unloaded,
    Started,
    Stopped
}

/// <summary>
/// Event args for model state changes
/// </summary>
public class ModelChangedEventArgs : EventArgs
{
    public LoadedModel Model { get; }
    public ModelChangeType ChangeType { get; }
    public IReadOnlyDictionary<int, LoadedModel> AllModels { get; }
    
    public ModelChangedEventArgs(LoadedModel model, ModelChangeType changeType, IReadOnlyDictionary<int, LoadedModel> allModels)
    {
        Model = model;
        ChangeType = changeType;
        AllModels = allModels;
    }
}
