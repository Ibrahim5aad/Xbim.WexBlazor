namespace Xbim.WexBlazor;

/// <summary>
/// Exception thrown when a standalone-only component is used in PlatformConnected mode.
/// <para>
/// This exception indicates that a component designed for standalone viewer applications
/// (such as <c>FileLoaderPanel</c>) is being used in an application configured with
/// <c>AddXbimBlazorPlatformConnected()</c>.
/// </para>
/// </summary>
public class StandaloneOnlyComponentException : InvalidOperationException
{
    private const string DefaultMessageTemplate =
        "'{0}' is Standalone-only. Use Workspace -> Project -> Model -> Version navigation instead. " +
        "In PlatformConnected mode, use server-backed components for file management. " +
        "If you need file loading in standalone mode, use 'AddXbimBlazorStandalone()' or 'AddXbimBlazorServer()' " +
        "instead of 'AddXbimBlazorPlatformConnected()'.";

    /// <summary>
    /// Gets the name of the component that was incorrectly used.
    /// </summary>
    public string ComponentName { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="StandaloneOnlyComponentException"/> with the component name.
    /// </summary>
    /// <param name="componentName">The name of the component that was used incorrectly.</param>
    public StandaloneOnlyComponentException(string componentName)
        : base(string.Format(DefaultMessageTemplate, componentName))
    {
        ComponentName = componentName;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="StandaloneOnlyComponentException"/> with a custom message.
    /// </summary>
    /// <param name="componentName">The name of the component that was used incorrectly.</param>
    /// <param name="message">The custom error message.</param>
    public StandaloneOnlyComponentException(string componentName, string message)
        : base(message)
    {
        ComponentName = componentName;
    }
}
