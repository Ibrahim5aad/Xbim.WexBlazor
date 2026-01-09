namespace Xbim.WexBlazor.Models;

/// <summary>
/// Represents a model loaded in the viewer
/// </summary>
public class LoadedModel
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
/// Position of the model manager panel
/// </summary>
public enum ModelManagerPosition
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}

