namespace Xbim.WexBlazor;

/// <summary>
/// Indicates the current Xbim.WexBlazor hosting mode.
/// </summary>
public enum XbimHostingMode
{
    /// <summary>
    /// Standalone viewer mode. Suitable for loading WexBIM from local files, static assets, or URLs
    /// without Xbim.Server connectivity.
    /// <para>
    /// In this mode, the <c>FileLoaderPanel</c> component is available for loading models.
    /// </para>
    /// </summary>
    Standalone,

    /// <summary>
    /// Platform-connected mode. Connected to Xbim.Server for full workspace, project,
    /// file, and model management functionality.
    /// <para>
    /// In this mode, models are loaded through the workspace/project/model navigation
    /// rather than through direct file selection.
    /// </para>
    /// </summary>
    PlatformConnected
}

/// <summary>
/// Provides information about the current Xbim.WexBlazor hosting mode.
/// <para>
/// Inject this service to determine which mode the application is running in
/// and to conditionally enable or disable functionality.
/// </para>
/// </summary>
public interface IXbimHostingModeProvider
{
    /// <summary>
    /// Gets the current hosting mode.
    /// </summary>
    XbimHostingMode HostingMode { get; }

    /// <summary>
    /// Gets a value indicating whether the application is in standalone mode.
    /// </summary>
    bool IsStandalone { get; }

    /// <summary>
    /// Gets a value indicating whether the application is in platform-connected mode.
    /// </summary>
    bool IsPlatformConnected { get; }
}

/// <summary>
/// Hosting mode provider for standalone viewer applications.
/// </summary>
internal sealed class StandaloneHostingModeProvider : IXbimHostingModeProvider
{
    /// <inheritdoc />
    public XbimHostingMode HostingMode => XbimHostingMode.Standalone;

    /// <inheritdoc />
    public bool IsStandalone => true;

    /// <inheritdoc />
    public bool IsPlatformConnected => false;
}

/// <summary>
/// Hosting mode provider for platform-connected applications.
/// </summary>
internal sealed class PlatformConnectedHostingModeProvider : IXbimHostingModeProvider
{
    /// <inheritdoc />
    public XbimHostingMode HostingMode => XbimHostingMode.PlatformConnected;

    /// <inheritdoc />
    public bool IsStandalone => false;

    /// <inheritdoc />
    public bool IsPlatformConnected => true;
}
