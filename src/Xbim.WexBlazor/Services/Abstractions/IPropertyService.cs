using Xbim.WexBlazor.Models;

namespace Xbim.WexBlazor.Services.Abstractions;

/// <summary>
/// Service interface for managing property sources and retrieving element properties.
/// This is a standalone/viewer-local service that aggregates properties from multiple sources.
/// </summary>
public interface IPropertyService : IDisposable
{
    /// <summary>
    /// Event raised when properties are retrieved.
    /// </summary>
    event Action<ElementProperties>? OnPropertiesRetrieved;

    /// <summary>
    /// Event raised when property sources change (added or removed).
    /// </summary>
    event Action? OnSourcesChanged;

    /// <summary>
    /// Gets all registered property sources.
    /// </summary>
    IReadOnlyList<IPropertySource> Sources { get; }

    /// <summary>
    /// Registers a property source.
    /// </summary>
    /// <param name="source">The property source to register.</param>
    void RegisterSource(IPropertySource source);

    /// <summary>
    /// Unregisters a property source by ID.
    /// </summary>
    /// <param name="sourceId">ID of the source to unregister.</param>
    void UnregisterSource(string sourceId);

    /// <summary>
    /// Gets a property source by ID.
    /// </summary>
    /// <param name="sourceId">The source ID.</param>
    /// <returns>The property source, or null if not found.</returns>
    IPropertySource? GetSource(string sourceId);

    /// <summary>
    /// Gets all sources that support a specific model.
    /// </summary>
    /// <param name="modelId">The model ID.</param>
    /// <returns>Enumerable of sources that support the model.</returns>
    IEnumerable<IPropertySource> GetSourcesForModel(int modelId);

    /// <summary>
    /// Gets properties for an element from all applicable sources.
    /// </summary>
    /// <param name="elementId">Element ID (IFC entity label).</param>
    /// <param name="modelId">Model ID in the viewer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Combined properties from all sources, or null if not found.</returns>
    Task<ElementProperties?> GetPropertiesAsync(
        int elementId,
        int modelId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets properties for an element using a query.
    /// </summary>
    /// <param name="query">Property query parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Combined properties from all sources, or null if not found.</returns>
    Task<ElementProperties?> GetPropertiesAsync(
        PropertyQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets properties for multiple elements.
    /// </summary>
    /// <param name="elements">Collection of element IDs and model IDs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Dictionary mapping element IDs to their properties.</returns>
    Task<Dictionary<int, ElementProperties>> GetPropertiesBatchAsync(
        IEnumerable<(int ElementId, int ModelId)> elements,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all sources for a specific model.
    /// </summary>
    /// <param name="modelId">The model ID.</param>
    void ClearSourcesForModel(int modelId);
}
