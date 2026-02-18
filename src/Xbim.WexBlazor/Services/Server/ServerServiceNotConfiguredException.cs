namespace Xbim.WexBlazor.Services.Server;

/// <summary>
/// Exception thrown when a server-only service is used without proper server configuration.
/// <para>
/// This exception indicates that a component or code is attempting to use a server-backed
/// service (such as <c>IWorkspacesService</c>, <c>IProjectsService</c>, etc.) but the
/// application was configured with <c>AddXbimBlazorStandalone()</c> instead of
/// <c>AddXbimBlazorPlatformConnected()</c>.
/// </para>
/// </summary>
public class ServerServiceNotConfiguredException : InvalidOperationException
{
    private const string DefaultMessageTemplate =
        "The service '{0}' requires Xbim.WexServer connectivity, but the application is configured in standalone mode. " +
        "To use server-backed services, call 'AddXbimBlazorPlatformConnected(baseUrl)' instead of 'AddXbimBlazorStandalone()' " +
        "during service registration.";

    /// <summary>
    /// Gets the name of the service that was not configured.
    /// </summary>
    public string ServiceName { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ServerServiceNotConfiguredException"/> with the service type.
    /// </summary>
    /// <param name="serviceType">The type of the service that was not configured.</param>
    public ServerServiceNotConfiguredException(Type serviceType)
        : base(string.Format(DefaultMessageTemplate, serviceType.Name))
    {
        ServiceName = serviceType.Name;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ServerServiceNotConfiguredException"/> with the service name.
    /// </summary>
    /// <param name="serviceName">The name of the service that was not configured.</param>
    public ServerServiceNotConfiguredException(string serviceName)
        : base(string.Format(DefaultMessageTemplate, serviceName))
    {
        ServiceName = serviceName;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ServerServiceNotConfiguredException"/> with a custom message.
    /// </summary>
    /// <param name="serviceName">The name of the service that was not configured.</param>
    /// <param name="message">The custom error message.</param>
    public ServerServiceNotConfiguredException(string serviceName, string message)
        : base(message)
    {
        ServiceName = serviceName;
    }
}
