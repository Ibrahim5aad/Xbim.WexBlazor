using Microsoft.Extensions.Logging;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc;
using Xbim.IO;
using Xbim.ModelGeometry.Scene;
using Xbim.WexBlazor.Models;
using Xbim.WexBlazor.Services.Abstractions;
using Xbim.Geometry.Abstractions;

namespace Xbim.WexBlazor.Services;

/// <summary>
/// Result of processing an IFC file
/// </summary>
public class IfcProcessingResult : IDisposable
{
    /// <summary>
    /// The IFC model (for property access)
    /// </summary>
    public IModel? Model { get; set; }
    
    /// <summary>
    /// The generated wexbim data as byte array
    /// </summary>
    public byte[]? WexbimData { get; set; }
    
    /// <summary>
    /// Whether the processing was successful
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Error message if processing failed
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Processing statistics
    /// </summary>
    public IfcProcessingStats? Stats { get; set; }
    
    public void Dispose()
    {
        if (Model is IDisposable disposable)
        {
            disposable.Dispose();
        }
        WexbimData = null;
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Statistics about the IFC processing
/// </summary>
public class IfcProcessingStats
{
    public int TotalProducts { get; set; }
    public int ProcessedProducts { get; set; }
    public int FailedProducts { get; set; }
    public TimeSpan ProcessingTime { get; set; }
    public long WexbimSize { get; set; }
}

/// <summary>
/// Progress information during IFC processing
/// </summary>
public class IfcProcessingProgress
{
    public string Stage { get; set; } = string.Empty;
    public int PercentComplete { get; set; }
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Service for opening IFC files and converting them to wexbim format.
/// Note: This service uses native code and only works in server-side scenarios (Blazor Server, ASP.NET Core).
/// </summary>
public class IfcModelService : IIfcModelService
{
    private readonly ILogger<IfcModelService>? _logger;
    private readonly Dictionary<int, IModel> _openModels = new();
    private int _modelIdCounter = 0;
    private bool _disposed;
    
    public IfcModelService(ILogger<IfcModelService>? logger = null)
    {
        _logger = logger;
        // Note: xBIM geometry services must be configured at startup via:
        // XbimServices.Current.ConfigureServices(s => s.AddXbimToolkit(c => c.AddGeometryServices()));
    }
    
    /// <summary>
    /// Opens an IFC file and generates wexbim data for visualization
    /// </summary>
    /// <param name="ifcFilePath">Path to the IFC file</param>
    /// <param name="progress">Optional progress callback</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Processing result with wexbim data and IModel</returns>
    public async Task<IfcProcessingResult> ProcessIfcFileAsync(
        string ifcFilePath,
        IProgress<IfcProcessingProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var result = new IfcProcessingResult();
        var startTime = DateTime.UtcNow;
        
        try
        {
            progress?.Report(new IfcProcessingProgress
            {
                Stage = "Opening",
                PercentComplete = 0,
                Message = "Opening IFC file..."
            });
            
            // Open the IFC file
            var model = await Task.Run(() => IfcStore.Open(ifcFilePath), cancellationToken);
            
            if (model == null)
            {
                result.Success = false;
                result.ErrorMessage = "Failed to open IFC file";
                return result;
            }
            
            result.Model = model;
            
            progress?.Report(new IfcProcessingProgress
            {
                Stage = "Processing",
                PercentComplete = 20,
                Message = "Processing geometry..."
            });
            
            // Generate wexbim data
            var wexbimData = await GenerateWexbimAsync(model, progress, cancellationToken);
            
            if (wexbimData == null || wexbimData.Length == 0)
            {
                result.Success = false;
                result.ErrorMessage = "Failed to generate wexbim geometry";
                return result;
            }
            
            result.WexbimData = wexbimData;
            result.Success = true;
            result.Stats = new IfcProcessingStats
            {
                ProcessingTime = DateTime.UtcNow - startTime,
                WexbimSize = wexbimData.Length,
                TotalProducts = (int)model.Instances.Count
            };
            
            progress?.Report(new IfcProcessingProgress
            {
                Stage = "Complete",
                PercentComplete = 100,
                Message = "Processing complete"
            });
            
            _logger?.LogInformation("Successfully processed IFC file: {Path}, wexbim size: {Size} bytes",
                ifcFilePath, wexbimData.Length);
            
            return result;
        }
        catch (OperationCanceledException)
        {
            result.Success = false;
            result.ErrorMessage = "Processing was cancelled";
            result.Model?.Dispose();
            result.Model = null;
            return result;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error processing IFC file: {Path}", ifcFilePath);
            result.Success = false;
            result.ErrorMessage = $"Error processing IFC file: {ex.Message}";
            result.Model?.Dispose();
            result.Model = null;
            return result;
        }
    }
    
    /// <summary>
    /// Opens an IFC file from a stream and generates wexbim data
    /// </summary>
    /// <param name="ifcStream">Stream containing IFC data</param>
    /// <param name="fileName">Original file name (for format detection)</param>
    /// <param name="progress">Optional progress callback</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Processing result with wexbim data and IModel</returns>
    public async Task<IfcProcessingResult> ProcessIfcStreamAsync(
        Stream ifcStream,
        string fileName,
        IProgress<IfcProcessingProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var result = new IfcProcessingResult();
        var startTime = DateTime.UtcNow;
        string? tempPath = null;
        
        try
        {
            progress?.Report(new IfcProcessingProgress
            {
                Stage = "Opening",
                PercentComplete = 0,
                Message = "Opening IFC stream..."
            });
            
            // For stream-based loading, write to a temp file first
            tempPath = Path.Combine(Path.GetTempPath(), $"xbim_{Guid.NewGuid()}{Path.GetExtension(fileName)}");
            
            // Copy stream to temp file
            using (var tempFile = File.Create(tempPath))
            {
                await ifcStream.CopyToAsync(tempFile, cancellationToken);
            }
            
            // Open from temp file
            var model = await Task.Run(() => IfcStore.Open(tempPath), cancellationToken);
            
            if (model == null)
            {
                result.Success = false;
                result.ErrorMessage = "Failed to open IFC stream";
                return result;
            }
            
            result.Model = model;
            
            progress?.Report(new IfcProcessingProgress
            {
                Stage = "Processing",
                PercentComplete = 20,
                Message = "Processing geometry..."
            });
            
            // Generate wexbim data
            var wexbimData = await GenerateWexbimAsync(model, progress, cancellationToken);
            
            if (wexbimData == null || wexbimData.Length == 0)
            {
                result.Success = false;
                result.ErrorMessage = "Failed to generate wexbim geometry";
                return result;
            }
            
            result.WexbimData = wexbimData;
            result.Success = true;
            result.Stats = new IfcProcessingStats
            {
                ProcessingTime = DateTime.UtcNow - startTime,
                WexbimSize = wexbimData.Length,
                TotalProducts = (int)model.Instances.Count
            };
            
            progress?.Report(new IfcProcessingProgress
            {
                Stage = "Complete",
                PercentComplete = 100,
                Message = "Processing complete"
            });
            
            _logger?.LogInformation("Successfully processed IFC stream: {FileName}, wexbim size: {Size} bytes",
                fileName, wexbimData.Length);
            
            return result;
        }
        catch (OperationCanceledException)
        {
            result.Success = false;
            result.ErrorMessage = "Processing was cancelled";
            result.Model?.Dispose();
            result.Model = null;
            return result;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error processing IFC stream: {FileName}", fileName);
            result.Success = false;
            result.ErrorMessage = $"Error processing IFC file: {ex.Message}";
            result.Model?.Dispose();
            result.Model = null;
            return result;
        }
        finally
        {
            // Clean up temp file
            if (tempPath != null && File.Exists(tempPath))
            {
                try { File.Delete(tempPath); } catch { /* ignore */ }
            }
        }
    }
    
    /// <summary>
    /// Opens an IFC file from a byte array and generates wexbim data
    /// </summary>
    /// <param name="ifcData">Byte array containing IFC data</param>
    /// <param name="fileName">Original file name</param>
    /// <param name="progress">Optional progress callback</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Processing result with wexbim data and IModel</returns>
    public async Task<IfcProcessingResult> ProcessIfcBytesAsync(
        byte[] ifcData,
        string fileName,
        IProgress<IfcProcessingProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(ifcData);
        return await ProcessIfcStreamAsync(stream, fileName, progress, cancellationToken);
    }
    
    /// <summary>
    /// Generates wexbim data from an IFC model using the xBIM geometry engine
    /// </summary>
    private async Task<byte[]?> GenerateWexbimAsync(
        IModel model,
        IProgress<IfcProcessingProgress>? progress,
        CancellationToken cancellationToken)
    {
        return await Task.Run(() =>
        {
            try
            {
                progress?.Report(new IfcProcessingProgress
                {
                    Stage = "Geometry",
                    PercentComplete = 30,
                    Message = "Creating geometry context..."
                });
                
                var context = new Xbim3DModelContext(model, engineVersion: XGeometryEngineVersion.V6);
                
                context.CreateContext(null, true, generateBREPs: true);
                
                if (cancellationToken.IsCancellationRequested)
                    return null;
                
                progress?.Report(new IfcProcessingProgress
                {
                    Stage = "Tessellation",
                    PercentComplete = 70,
                    Message = "Generating wexbim..."
                });
                
                using var memoryStream = new MemoryStream();
                using var wexBimBinaryWriter = new BinaryWriter(memoryStream);
                
                model.SaveAsWexBim(wexBimBinaryWriter);
                
                progress?.Report(new IfcProcessingProgress
                {
                    Stage = "Finalizing",
                    PercentComplete = 95,
                    Message = "Finalizing wexbim..."
                });
                
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error generating wexbim geometry: {Message}", ex.Message);
                return null;
            }
        }, cancellationToken);
    }
    
    /// <summary>
    /// Registers a model for tracking (useful for managing multiple models)
    /// </summary>
    public int RegisterModel(IModel model)
    {
        var id = ++_modelIdCounter;
        _openModels[id] = model;
        return id;
    }
    
    /// <summary>
    /// Gets a registered model by ID
    /// </summary>
    public IModel? GetModel(int modelId)
    {
        return _openModels.TryGetValue(modelId, out var model) ? model : null;
    }
    
    /// <summary>
    /// Unregisters and disposes a model
    /// </summary>
    public void UnregisterModel(int modelId)
    {
        if (_openModels.TryGetValue(modelId, out var model))
        {
            _openModels.Remove(modelId);
            if (model is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        
        foreach (var model in _openModels.Values)
        {
            if (model is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        _openModels.Clear();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
