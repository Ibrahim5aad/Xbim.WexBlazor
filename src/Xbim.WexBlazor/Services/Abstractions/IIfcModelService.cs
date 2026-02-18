using Xbim.Common;

namespace Xbim.WexBlazor.Services.Abstractions;

/// <summary>
/// Service interface for processing IFC files and converting them to WexBIM format.
/// <para>
/// This service uses native code (xBIM libraries) and only works in server-side scenarios
/// (Blazor Server, ASP.NET Core). It is not compatible with Blazor WebAssembly.
/// </para>
/// </summary>
/// <remarks>
/// Use FileLoaderPanel for loading pre-converted WexBIM files in standalone scenarios.
/// </remarks>
public interface IIfcModelService : IDisposable
{
    /// <summary>
    /// Opens an IFC file and generates WexBIM data for visualization.
    /// </summary>
    /// <param name="ifcFilePath">Path to the IFC file.</param>
    /// <param name="progress">Optional progress callback.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Processing result with WexBIM data and IModel.</returns>
    Task<IfcProcessingResult> ProcessIfcFileAsync(
        string ifcFilePath,
        IProgress<IfcProcessingProgress>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens an IFC file from a stream and generates WexBIM data.
    /// </summary>
    /// <param name="ifcStream">Stream containing IFC data.</param>
    /// <param name="fileName">Original file name (for format detection).</param>
    /// <param name="progress">Optional progress callback.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Processing result with WexBIM data and IModel.</returns>
    Task<IfcProcessingResult> ProcessIfcStreamAsync(
        Stream ifcStream,
        string fileName,
        IProgress<IfcProcessingProgress>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens an IFC file from a byte array and generates WexBIM data.
    /// </summary>
    /// <param name="ifcData">Byte array containing IFC data.</param>
    /// <param name="fileName">Original file name.</param>
    /// <param name="progress">Optional progress callback.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Processing result with WexBIM data and IModel.</returns>
    Task<IfcProcessingResult> ProcessIfcBytesAsync(
        byte[] ifcData,
        string fileName,
        IProgress<IfcProcessingProgress>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a model for tracking (useful for managing multiple models).
    /// </summary>
    /// <param name="model">The IFC model to register.</param>
    /// <returns>The assigned model ID.</returns>
    int RegisterModel(IModel model);

    /// <summary>
    /// Gets a registered model by ID.
    /// </summary>
    /// <param name="modelId">The model ID.</param>
    /// <returns>The model, or null if not found.</returns>
    IModel? GetModel(int modelId);

    /// <summary>
    /// Unregisters and disposes a model.
    /// </summary>
    /// <param name="modelId">The model ID.</param>
    void UnregisterModel(int modelId);
}
