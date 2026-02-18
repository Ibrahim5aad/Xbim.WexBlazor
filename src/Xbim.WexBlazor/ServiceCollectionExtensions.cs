using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xbim.WexBlazor.Models;
using Xbim.WexBlazor.Services;
using Xbim.WexBlazor.Services.Abstractions;
using Xbim.WexBlazor.Services.Abstractions.Server;
using Xbim.WexBlazor.Services.Server;
using Xbim.WexBlazor.Services.Server.Guards;
using Xbim.Api.Client;

namespace Xbim.WexBlazor;

/// <summary>
/// Extension methods for configuring Xbim.WexBlazor services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds core Xbim.WexBlazor services for standalone viewer applications.
    /// <para>
    /// This registration is suitable for applications that:
    /// <list type="bullet">
    ///   <item>Load WexBIM files via FileLoaderPanel (from URLs, static assets, or local files)</item>
    ///   <item>Do not require Xbim.Server connectivity</item>
    ///   <item>Do not need IFC file processing (use pre-converted WexBIM files)</item>
    /// </list>
    /// </para>
    /// <para>
    /// Services registered:
    /// <list type="bullet">
    ///   <item><see cref="ThemeService"/> - Theme management (singleton)</item>
    ///   <item><see cref="IPropertyService"/> / <see cref="PropertyService"/> - Property aggregation (singleton)</item>
    ///   <item><see cref="IfcHierarchyService"/> - Hierarchy generation (singleton)</item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddXbimBlazorStandalone(this IServiceCollection services)
    {
        return services.AddXbimBlazorStandalone(_ => { });
    }

    /// <summary>
    /// Adds core Xbim.WexBlazor services for standalone viewer applications with custom configuration.
    /// <para>
    /// This registration is suitable for applications that:
    /// <list type="bullet">
    ///   <item>Load WexBIM files via FileLoaderPanel (from URLs, static assets, or local files)</item>
    ///   <item>Do not require Xbim.Server connectivity</item>
    ///   <item>Do not need IFC file processing (use pre-converted WexBIM files)</item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">An action to configure the <see cref="XbimBlazorOptions"/>.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddXbimBlazorStandalone(
        this IServiceCollection services,
        Action<XbimBlazorOptions> configure)
    {
        var options = new XbimBlazorOptions();
        configure(options);

        // Store options for later use
        services.TryAddSingleton(options);

        // Register ThemeService with configured options
        var themeService = new ThemeService();
        themeService.SetTheme(options.InitialTheme);
        themeService.SetAccentColors(options.LightAccentColor, options.DarkAccentColor);
        themeService.SetBackgroundColors(options.LightBackgroundColor, options.DarkBackgroundColor);
        services.TryAddSingleton(themeService);

        // Register PropertyService as both interface and concrete type for backward compatibility
        services.TryAddSingleton<PropertyService>();
        services.TryAddSingleton<IPropertyService>(sp => sp.GetRequiredService<PropertyService>());

        // Register IfcHierarchyService
        services.TryAddSingleton<IfcHierarchyService>();

        // Register the standalone hosting mode provider
        services.TryAddSingleton<IXbimHostingModeProvider, StandaloneHostingModeProvider>();

        // Register guard implementations for server-only services.
        // These throw ServerServiceNotConfiguredException with actionable messages when used,
        // preventing ambiguous null reference errors and guiding users to proper configuration.
        services.TryAddSingleton<IWorkspacesService, NotConfiguredWorkspacesService>();
        services.TryAddSingleton<IProjectsService, NotConfiguredProjectsService>();
        services.TryAddSingleton<IFilesService, NotConfiguredFilesService>();
        services.TryAddSingleton<IModelsService, NotConfiguredModelsService>();
        services.TryAddSingleton<IUsageService, NotConfiguredUsageService>();
        services.TryAddSingleton<IProcessingService, NotConfiguredProcessingService>();

        return services;
    }

    /// <summary>
    /// Adds Xbim.WexBlazor services for standalone viewer applications using configuration binding.
    /// <para>
    /// This method binds configuration from the "Xbim:Standalone" section in appsettings.json.
    /// </para>
    /// <para>
    /// Example appsettings.json:
    /// <code>
    /// {
    ///   "Xbim": {
    ///     "Standalone": {
    ///       "Theme": {
    ///         "InitialTheme": "Dark",
    ///         "LightAccentColor": "#0969da",
    ///         "DarkAccentColor": "#1e7e34"
    ///       },
    ///       "FileLoaderPanel": {
    ///         "AllowIfcFiles": true,
    ///         "AutoCloseOnLoad": true,
    ///         "DemoModels": [
    ///           { "Name": "Sample House", "Path": "models/SampleHouse.wexbim" }
    ///         ]
    ///       }
    ///     }
    ///   }
    /// }
    /// </code>
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration root.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddXbimBlazorStandalone(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(XbimStandaloneOptions.SectionName);
        var standaloneOptions = new XbimStandaloneOptions();
        section.Bind(standaloneOptions);

        // Store standalone options for retrieval
        services.TryAddSingleton(standaloneOptions);

        return services.AddXbimBlazorStandalone(options =>
        {
            // Apply theme settings
            options.InitialTheme = standaloneOptions.Theme.GetViewerTheme();
            options.LightAccentColor = standaloneOptions.Theme.LightAccentColor;
            options.DarkAccentColor = standaloneOptions.Theme.DarkAccentColor;
            options.LightBackgroundColor = standaloneOptions.Theme.LightBackgroundColor;
            options.DarkBackgroundColor = standaloneOptions.Theme.DarkBackgroundColor;

            // Apply FileLoaderPanel settings
            options.FileLoaderPanel = standaloneOptions.FileLoaderPanel;
        });
    }

    /// <summary>
    /// Adds Xbim.WexBlazor services for Blazor Server applications using configuration binding.
    /// <para>
    /// This method binds configuration from the "Xbim:Standalone" section in appsettings.json,
    /// and adds IFC processing capabilities for server-side scenarios.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration root.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddXbimBlazorServer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(XbimStandaloneOptions.SectionName);
        var standaloneOptions = new XbimStandaloneOptions();
        section.Bind(standaloneOptions);

        // Store standalone options for retrieval
        services.TryAddSingleton(standaloneOptions);

        return services.AddXbimBlazorServer(options =>
        {
            // Apply theme settings
            options.InitialTheme = standaloneOptions.Theme.GetViewerTheme();
            options.LightAccentColor = standaloneOptions.Theme.LightAccentColor;
            options.DarkAccentColor = standaloneOptions.Theme.DarkAccentColor;
            options.LightBackgroundColor = standaloneOptions.Theme.LightBackgroundColor;
            options.DarkBackgroundColor = standaloneOptions.Theme.DarkBackgroundColor;

            // Apply FileLoaderPanel settings
            options.FileLoaderPanel = standaloneOptions.FileLoaderPanel;
        });
    }

    /// <summary>
    /// Adds Xbim.WexBlazor services for Blazor Server applications with IFC processing capabilities.
    /// <para>
    /// This registration extends <see cref="AddXbimBlazorStandalone"/> with:
    /// <list type="bullet">
    ///   <item><see cref="IIfcModelService"/> / <see cref="IfcModelService"/> - Server-side IFC to WexBIM conversion</item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Note:</strong> <see cref="IfcModelService"/> uses native xBIM libraries and only works
    /// in server-side scenarios (Blazor Server, ASP.NET Core). It is not compatible with Blazor WebAssembly.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddXbimBlazorServer(this IServiceCollection services)
    {
        return services.AddXbimBlazorServer(_ => { });
    }

    /// <summary>
    /// Adds Xbim.WexBlazor services for Blazor Server applications with IFC processing capabilities.
    /// <para>
    /// This registration extends <see cref="AddXbimBlazorStandalone"/> with:
    /// <list type="bullet">
    ///   <item><see cref="IIfcModelService"/> / <see cref="IfcModelService"/> - Server-side IFC to WexBIM conversion</item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Note:</strong> <see cref="IfcModelService"/> uses native xBIM libraries and only works
    /// in server-side scenarios (Blazor Server, ASP.NET Core). It is not compatible with Blazor WebAssembly.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">An action to configure the <see cref="XbimBlazorOptions"/>.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddXbimBlazorServer(
        this IServiceCollection services,
        Action<XbimBlazorOptions> configure)
    {
        // Add standalone services first
        services.AddXbimBlazorStandalone(configure);

        // Add server-specific services as both interface and concrete type for backward compatibility
        services.TryAddSingleton<IfcModelService>();
        services.TryAddSingleton<IIfcModelService>(sp => sp.GetRequiredService<IfcModelService>());

        return services;
    }

    /// <summary>
    /// Adds Xbim.WexBlazor services with default configuration.
    /// <para>
    /// This is an alias for <see cref="AddXbimBlazorStandalone()"/> for backward compatibility.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddXbimBlazor(this IServiceCollection services)
    {
        return services.AddXbimBlazorStandalone();
    }

    /// <summary>
    /// Adds Xbim.WexBlazor services with custom configuration.
    /// <para>
    /// This is an alias for <see cref="AddXbimBlazorStandalone(IServiceCollection, Action{XbimBlazorOptions})"/>
    /// for backward compatibility.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">An action to configure the <see cref="XbimBlazorOptions"/>.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddXbimBlazor(
        this IServiceCollection services,
        Action<XbimBlazorOptions> configure)
    {
        return services.AddXbimBlazorStandalone(configure);
    }

    /// <summary>
    /// Adds Xbim.WexBlazor services with server connectivity for full application functionality.
    /// <para>
    /// This registration includes:
    /// <list type="bullet">
    ///   <item>All standalone services (themes, properties)</item>
    ///   <item>Server-backed services for workspaces, projects, files, models, usage, and processing</item>
    ///   <item>Xbim API client with optional authentication</item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Prerequisites:</strong>
    /// <list type="bullet">
    ///   <item>A running Xbim.Server instance</item>
    ///   <item>Valid base URL configuration</item>
    ///   <item>Optional: Authentication token provider for secured endpoints</item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseUrl">The base URL of the Xbim API server.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if baseUrl is null or empty.</exception>
    public static IServiceCollection AddXbimBlazorPlatformConnected(
        this IServiceCollection services,
        string baseUrl)
    {
        return services.AddXbimBlazorPlatformConnected(baseUrl, _ => { });
    }

    /// <summary>
    /// Adds Xbim.WexBlazor services with server connectivity and custom Blazor options.
    /// <para>
    /// This registration includes:
    /// <list type="bullet">
    ///   <item>All standalone services (themes, properties)</item>
    ///   <item>Server-backed services for workspaces, projects, files, models, usage, and processing</item>
    ///   <item>Xbim API client with optional authentication</item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseUrl">The base URL of the Xbim API server.</param>
    /// <param name="configureBlazor">An action to configure the <see cref="XbimBlazorOptions"/>.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if baseUrl is null or empty.</exception>
    public static IServiceCollection AddXbimBlazorPlatformConnected(
        this IServiceCollection services,
        string baseUrl,
        Action<XbimBlazorOptions> configureBlazor)
    {
        return services.AddXbimBlazorPlatformConnected(baseUrl, configureBlazor, _ => { });
    }

    /// <summary>
    /// Adds Xbim.WexBlazor services with server connectivity and authentication.
    /// <para>
    /// This registration includes:
    /// <list type="bullet">
    ///   <item>All standalone services (themes, properties)</item>
    ///   <item>Server-backed services for workspaces, projects, files, models, usage, and processing</item>
    ///   <item>Xbim API client with token-based authentication</item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseUrl">The base URL of the Xbim API server.</param>
    /// <param name="tokenProvider">The token provider for authentication.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if baseUrl is null or empty.</exception>
    public static IServiceCollection AddXbimBlazorPlatformConnected(
        this IServiceCollection services,
        string baseUrl,
        IAuthTokenProvider tokenProvider)
    {
        return services.AddXbimBlazorPlatformConnected(baseUrl, _ => { }, options =>
        {
            options.TokenProvider = tokenProvider;
        });
    }

    /// <summary>
    /// Adds Xbim.WexBlazor services with server connectivity and a token factory function.
    /// <para>
    /// This registration includes:
    /// <list type="bullet">
    ///   <item>All standalone services (themes, properties)</item>
    ///   <item>Server-backed services for workspaces, projects, files, models, usage, and processing</item>
    ///   <item>Xbim API client with token-based authentication</item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseUrl">The base URL of the Xbim API server.</param>
    /// <param name="tokenFactory">A function that provides authentication tokens.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if baseUrl is null or empty.</exception>
    public static IServiceCollection AddXbimBlazorPlatformConnected(
        this IServiceCollection services,
        string baseUrl,
        Func<Task<string?>> tokenFactory)
    {
        return services.AddXbimBlazorPlatformConnected(baseUrl, _ => { }, options =>
        {
            options.TokenFactory = _ => tokenFactory();
        });
    }

    /// <summary>
    /// Adds Xbim.WexBlazor services with server connectivity and full configuration control.
    /// <para>
    /// This registration includes:
    /// <list type="bullet">
    ///   <item>All standalone services (themes, properties)</item>
    ///   <item>Server-backed services for workspaces, projects, files, models, usage, and processing</item>
    ///   <item>Xbim API client with configurable authentication</item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseUrl">The base URL of the Xbim API server.</param>
    /// <param name="configureBlazor">An action to configure the <see cref="XbimBlazorOptions"/>.</param>
    /// <param name="configureClient">An action to configure the <see cref="XbimClientOptions"/>.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="ArgumentException">Thrown if baseUrl is null or empty.</exception>
    public static IServiceCollection AddXbimBlazorPlatformConnected(
        this IServiceCollection services,
        string baseUrl,
        Action<XbimBlazorOptions> configureBlazor,
        Action<XbimClientOptions> configureClient)
    {
        if (string.IsNullOrEmpty(baseUrl))
            throw new ArgumentException("BaseUrl must be provided.", nameof(baseUrl));

        // Add standalone services first
        services.AddXbimBlazorStandalone(configureBlazor);

        // Add the Xbim API client
        services.AddXbimClient(options =>
        {
            options.BaseUrl = baseUrl;
            configureClient(options);
        });

        // Replace the hosting mode provider to indicate PlatformConnected mode.
        // This enables components to detect the mode and adjust behavior accordingly.
        services.Replace(ServiceDescriptor.Singleton<IXbimHostingModeProvider, PlatformConnectedHostingModeProvider>());

        // Register server-backed services, replacing any guard implementations from standalone mode.
        // Using Replace ensures that the real implementations override the guards that throw
        // ServerServiceNotConfiguredException.
        services.Replace(ServiceDescriptor.Singleton<IWorkspacesService, WorkspacesService>());
        services.Replace(ServiceDescriptor.Singleton<IProjectsService, ProjectsService>());
        services.Replace(ServiceDescriptor.Singleton<IFilesService, FilesService>());
        services.Replace(ServiceDescriptor.Singleton<IModelsService, ModelsService>());
        services.Replace(ServiceDescriptor.Singleton<IUsageService, UsageService>());
        services.Replace(ServiceDescriptor.Singleton<IProcessingService, ProcessingService>());

        return services;
    }

    /// <summary>
    /// Adds Xbim.WexBlazor services with server connectivity using configuration binding.
    /// <para>
    /// This method binds configuration from the "Xbim:Server" section in appsettings.json.
    /// </para>
    /// <para>
    /// Example appsettings.json:
    /// <code>
    /// {
    ///   "Xbim": {
    ///     "Server": {
    ///       "BaseUrl": "https://api.Xbim.example.com",
    ///       "RequireAuthentication": true,
    ///       "TimeoutSeconds": 30
    ///     }
    ///   }
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// For authentication, provide a token factory via the <paramref name="configureClient"/> action,
    /// or implement <see cref="IAuthTokenProvider"/> and register it in the service collection before calling this method.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration root.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when server configuration is missing or invalid.</exception>
    public static IServiceCollection AddXbimBlazorPlatformConnected(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddXbimBlazorPlatformConnected(configuration, _ => { }, _ => { });
    }

    /// <summary>
    /// Adds Xbim.WexBlazor services with server connectivity using configuration binding and custom Blazor options.
    /// <para>
    /// This method binds server configuration from the "Xbim:Server" section and standalone configuration
    /// from the "Xbim:Standalone" section in appsettings.json.
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration root.</param>
    /// <param name="configureBlazor">An action to configure additional Blazor options.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when server configuration is missing or invalid.</exception>
    public static IServiceCollection AddXbimBlazorPlatformConnected(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<XbimBlazorOptions> configureBlazor)
    {
        return services.AddXbimBlazorPlatformConnected(configuration, configureBlazor, _ => { });
    }

    /// <summary>
    /// Adds Xbim.WexBlazor services with server connectivity using configuration binding and full control.
    /// <para>
    /// This method binds server configuration from the "Xbim:Server" section and standalone configuration
    /// from the "Xbim:Standalone" section in appsettings.json.
    /// </para>
    /// <para>
    /// Example appsettings.json:
    /// <code>
    /// {
    ///   "Xbim": {
    ///     "Server": {
    ///       "BaseUrl": "https://api.Xbim.example.com",
    ///       "RequireAuthentication": true,
    ///       "TimeoutSeconds": 30
    ///     },
    ///     "Standalone": {
    ///       "Theme": { "InitialTheme": "Dark" },
    ///       "FileLoaderPanel": { "AllowIfcFiles": false }
    ///     }
    ///   }
    /// }
    /// </code>
    /// </para>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration root.</param>
    /// <param name="configureBlazor">An action to configure additional Blazor options.</param>
    /// <param name="configureClient">An action to configure the Xbim client options (e.g., authentication).</param>
    /// <returns>The service collection for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when server configuration is missing or invalid.</exception>
    public static IServiceCollection AddXbimBlazorPlatformConnected(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<XbimBlazorOptions> configureBlazor,
        Action<XbimClientOptions> configureClient)
    {
        // Bind and validate server options
        var serverSection = configuration.GetSection(XbimServerOptions.SectionName);
        var serverOptions = new XbimServerOptions();
        serverSection.Bind(serverOptions);

        // Validate configuration at startup - fail fast with actionable message
        serverOptions.Validate();

        // Store server options for retrieval by components
        services.TryAddSingleton(serverOptions);

        // Bind standalone options for theme and FileLoaderPanel configuration
        var standaloneSection = configuration.GetSection(XbimStandaloneOptions.SectionName);
        var standaloneOptions = new XbimStandaloneOptions();
        standaloneSection.Bind(standaloneOptions);
        services.TryAddSingleton(standaloneOptions);

        // Configure Blazor options from configuration + custom action
        Action<XbimBlazorOptions> combinedConfigure = options =>
        {
            // Apply theme settings from configuration
            options.InitialTheme = standaloneOptions.Theme.GetViewerTheme();
            options.LightAccentColor = standaloneOptions.Theme.LightAccentColor;
            options.DarkAccentColor = standaloneOptions.Theme.DarkAccentColor;
            options.LightBackgroundColor = standaloneOptions.Theme.LightBackgroundColor;
            options.DarkBackgroundColor = standaloneOptions.Theme.DarkBackgroundColor;

            // Apply FileLoaderPanel settings
            options.FileLoaderPanel = standaloneOptions.FileLoaderPanel;

            // Apply any custom configuration
            configureBlazor(options);
        };

        // Configure client options from configuration + custom action
        Action<XbimClientOptions> combinedClientConfigure = options =>
        {
            options.BaseUrl = serverOptions.BaseUrl!;
            // Note: TimeoutSeconds is stored in XbimServerOptions for reference,
            // but HttpClient timeout is configured by the host (AddXbimClient handles defaults)

            // Apply any custom configuration
            configureClient(options);
        };

        return services.AddXbimBlazorPlatformConnected(
            serverOptions.BaseUrl!,
            combinedConfigure,
            combinedClientConfigure);
    }
}
