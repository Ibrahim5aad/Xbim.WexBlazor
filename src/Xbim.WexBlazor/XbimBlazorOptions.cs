using Xbim.WexBlazor.Models;

namespace Xbim.WexBlazor;

/// <summary>
/// Configuration options for Xbim.WexBlazor services.
/// </summary>
public class XbimBlazorOptions
{
    /// <summary>
    /// Gets or sets the initial viewer theme. Defaults to <see cref="ViewerTheme.Dark"/>.
    /// </summary>
    public ViewerTheme InitialTheme { get; set; } = ViewerTheme.Dark;

    /// <summary>
    /// Gets or sets the accent color for light theme. Defaults to "#0969da".
    /// </summary>
    public string LightAccentColor { get; set; } = "#0969da";

    /// <summary>
    /// Gets or sets the accent color for dark theme. Defaults to "#1e7e34".
    /// </summary>
    public string DarkAccentColor { get; set; } = "#1e7e34";

    /// <summary>
    /// Gets or sets the background color for light theme. Defaults to "#ffffff".
    /// </summary>
    public string LightBackgroundColor { get; set; } = "#ffffff";

    /// <summary>
    /// Gets or sets the background color for dark theme. Defaults to "#404040".
    /// </summary>
    public string DarkBackgroundColor { get; set; } = "#404040";

    /// <summary>
    /// Gets or sets the FileLoaderPanel configuration options.
    /// <para>
    /// Configure this to customize the behavior of the FileLoaderPanel component,
    /// including demo models, default paths, and feature toggles.
    /// </para>
    /// </summary>
    public FileLoaderPanelOptions FileLoaderPanel { get; set; } = new();
}

/// <summary>
/// Configuration options for the FileLoaderPanel component.
/// </summary>
public class FileLoaderPanelOptions
{
    /// <summary>
    /// Gets or sets the list of demo models to display in the FileLoaderPanel.
    /// </summary>
    public List<DemoModelConfig> DemoModels { get; set; } = new();

    /// <summary>
    /// Gets or sets whether to allow loading IFC files. Defaults to true.
    /// <para>
    /// When enabled, the FileLoaderPanel will accept .ifc and .ifczip files for processing.
    /// Requires IFC processing capability (use <c>AddXbimBlazorServer</c> for Blazor Server).
    /// </para>
    /// </summary>
    public bool AllowIfcFiles { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to allow custom HTTP headers for URL loading. Defaults to true.
    /// </summary>
    public bool AllowCustomHeaders { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to automatically close the panel after loading a model. Defaults to true.
    /// </summary>
    public bool AutoCloseOnLoad { get; set; } = true;

    /// <summary>
    /// Gets or sets the default URL to pre-populate in the URL input field.
    /// </summary>
    public string? DefaultUrl { get; set; }

    /// <summary>
    /// Adds a demo model configuration.
    /// </summary>
    /// <param name="name">The display name of the demo model.</param>
    /// <param name="path">The path or URL to the model file.</param>
    /// <param name="description">Optional description of the demo model.</param>
    /// <returns>This instance for chaining.</returns>
    public FileLoaderPanelOptions AddDemoModel(string name, string path, string? description = null)
    {
        DemoModels.Add(new DemoModelConfig { Name = name, Path = path, Description = description });
        return this;
    }

    /// <summary>
    /// Converts the demo model configurations to the component-friendly <see cref="Models.DemoModel"/> list.
    /// </summary>
    /// <returns>A list of <see cref="Models.DemoModel"/> instances.</returns>
    public List<Models.DemoModel> ToDemoModelList()
    {
        return DemoModels.Select(c => new Models.DemoModel
        {
            Name = c.Name,
            Path = c.Path,
            Description = c.Description
        }).ToList();
    }
}

/// <summary>
/// Configuration for a demo model in the FileLoaderPanel.
/// </summary>
public class DemoModelConfig
{
    /// <summary>
    /// Gets or sets the display name of the demo model.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path or URL to the model file.
    /// <para>
    /// Supports:
    /// <list type="bullet">
    ///   <item>Relative paths within wwwroot (e.g., "models/house.wexbim")</item>
    ///   <item>Paths with ~/ prefix for wwwroot (e.g., "~/models/house.wexbim")</item>
    ///   <item>Absolute URLs (e.g., "https://cdn.example.com/model.wexbim")</item>
    /// </list>
    /// </para>
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional description for the demo model.
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// Configuration options for Xbim.WexServer connectivity in PlatformConnected mode.
/// <para>
/// Bind this from the "Xbim:Server" configuration section.
/// </para>
/// </summary>
public class XbimServerOptions
{
    /// <summary>
    /// The configuration section path for server options.
    /// </summary>
    public const string SectionName = "Xbim:Server";

    /// <summary>
    /// Gets or sets the base URL of the Xbim.WexServer API.
    /// <para>
    /// Required for PlatformConnected mode. Example: "https://api.Xbim.example.com"
    /// </para>
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets whether to require authentication. Defaults to true.
    /// <para>
    /// When true, requests to the server will include authentication tokens.
    /// </para>
    /// </summary>
    public bool RequireAuthentication { get; set; } = true;

    /// <summary>
    /// Gets or sets the timeout in seconds for API requests. Defaults to 30 seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Validates the server options and throws if misconfigured.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when configuration is invalid.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(BaseUrl))
        {
            throw new InvalidOperationException(
                $"Xbim.WexServer configuration is invalid: 'BaseUrl' is required. " +
                $"Configure the '{SectionName}:BaseUrl' setting in appsettings.json or call " +
                $"AddWexBlazorPlatformConnected(baseUrl) with a valid URL.");
        }

        if (!Uri.TryCreate(BaseUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException(
                $"Xbim.WexServer configuration is invalid: 'BaseUrl' must be a valid HTTP or HTTPS URL. " +
                $"Current value: '{BaseUrl}'. " +
                $"Example: \"https://api.Xbim.example.com\"");
        }
    }
}

/// <summary>
/// Configuration options for standalone mode.
/// <para>
/// Bind this from the "Xbim:Standalone" configuration section.
/// </para>
/// </summary>
public class XbimStandaloneOptions
{
    /// <summary>
    /// The configuration section path for standalone options.
    /// </summary>
    public const string SectionName = "Xbim:Standalone";

    /// <summary>
    /// Gets or sets the theme configuration.
    /// </summary>
    public ThemeOptions Theme { get; set; } = new();

    /// <summary>
    /// Gets or sets the FileLoaderPanel configuration.
    /// </summary>
    public FileLoaderPanelOptions FileLoaderPanel { get; set; } = new();
}

/// <summary>
/// Theme configuration options for binding from configuration.
/// </summary>
public class ThemeOptions
{
    /// <summary>
    /// Gets or sets the initial theme. Valid values: "Dark", "Light". Defaults to "Dark".
    /// </summary>
    public string InitialTheme { get; set; } = "Dark";

    /// <summary>
    /// Gets or sets the accent color for light theme.
    /// </summary>
    public string LightAccentColor { get; set; } = "#0969da";

    /// <summary>
    /// Gets or sets the accent color for dark theme.
    /// </summary>
    public string DarkAccentColor { get; set; } = "#1e7e34";

    /// <summary>
    /// Gets or sets the background color for light theme.
    /// </summary>
    public string LightBackgroundColor { get; set; } = "#ffffff";

    /// <summary>
    /// Gets or sets the background color for dark theme.
    /// </summary>
    public string DarkBackgroundColor { get; set; } = "#404040";

    /// <summary>
    /// Converts the theme string to a <see cref="ViewerTheme"/> enum value.
    /// </summary>
    public ViewerTheme GetViewerTheme()
    {
        return InitialTheme?.Equals("Light", StringComparison.OrdinalIgnoreCase) == true
            ? ViewerTheme.Light
            : ViewerTheme.Dark;
    }
}

