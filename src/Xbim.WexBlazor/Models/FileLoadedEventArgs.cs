namespace Xbim.WexBlazor.Models;

/// <summary>
/// Event arguments for file loaded events
/// </summary>
public class FileLoadedEventArgs
{
    /// <summary>
    /// The file data as a byte array
    /// </summary>
    public byte[] FileData { get; set; } = Array.Empty<byte>();
    
    /// <summary>
    /// Original filename
    /// </summary>
    public string FileName { get; set; } = string.Empty;
    
    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }
}

