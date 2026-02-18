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
    
    /// <summary>
    /// Format of the file
    /// </summary>
    public ModelFormat Format { get; set; } = ModelFormat.Wexbim;
    
    /// <summary>
    /// Determines the format from the file extension
    /// </summary>
    public static ModelFormat GetFormatFromFileName(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            ".ifc" => ModelFormat.Ifc,
            ".ifczip" => ModelFormat.IfcZip,
            ".wexbim" => ModelFormat.Wexbim,
            _ => ModelFormat.Wexbim
        };
    }
}

