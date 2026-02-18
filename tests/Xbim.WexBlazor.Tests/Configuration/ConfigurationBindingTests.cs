using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xbim.WexBlazor.Models;
using Xbim.WexBlazor.Services.Abstractions.Server;

namespace Xbim.WexBlazor.Tests.Configuration;

public class ConfigurationBindingTests
{
    [Fact]
    public void AddWexBlazorStandalone_WithConfiguration_BindsThemeOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Xbim:Standalone:Theme:InitialTheme"] = "Light",
                ["Xbim:Standalone:Theme:LightAccentColor"] = "#ff0000",
                ["Xbim:Standalone:Theme:DarkAccentColor"] = "#00ff00"
            })
            .Build();

        // Act
        services.AddWexBlazorStandalone(configuration);
        var provider = services.BuildServiceProvider();
        var options = provider.GetService<XbimBlazorOptions>();

        // Assert
        Assert.NotNull(options);
        Assert.Equal(ViewerTheme.Light, options.InitialTheme);
        Assert.Equal("#ff0000", options.LightAccentColor);
        Assert.Equal("#00ff00", options.DarkAccentColor);
    }

    [Fact]
    public void AddWexBlazorStandalone_WithConfiguration_BindsFileLoaderPanelOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Xbim:Standalone:FileLoaderPanel:AllowIfcFiles"] = "false",
                ["Xbim:Standalone:FileLoaderPanel:AllowCustomHeaders"] = "false",
                ["Xbim:Standalone:FileLoaderPanel:AutoCloseOnLoad"] = "false"
            })
            .Build();

        // Act
        services.AddWexBlazorStandalone(configuration);
        var provider = services.BuildServiceProvider();
        var options = provider.GetService<XbimBlazorOptions>();

        // Assert
        Assert.NotNull(options);
        Assert.False(options.FileLoaderPanel.AllowIfcFiles);
        Assert.False(options.FileLoaderPanel.AllowCustomHeaders);
        Assert.False(options.FileLoaderPanel.AutoCloseOnLoad);
    }

    [Fact]
    public void AddWexBlazorStandalone_WithConfiguration_BindsDemoModels()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Xbim:Standalone:FileLoaderPanel:DemoModels:0:Name"] = "Test Model",
                ["Xbim:Standalone:FileLoaderPanel:DemoModels:0:Path"] = "models/test.wexbim",
                ["Xbim:Standalone:FileLoaderPanel:DemoModels:0:Description"] = "A test model",
                ["Xbim:Standalone:FileLoaderPanel:DemoModels:1:Name"] = "Second Model",
                ["Xbim:Standalone:FileLoaderPanel:DemoModels:1:Path"] = "models/second.wexbim"
            })
            .Build();

        // Act
        services.AddWexBlazorStandalone(configuration);
        var provider = services.BuildServiceProvider();
        var options = provider.GetService<XbimBlazorOptions>();

        // Assert
        Assert.NotNull(options);
        Assert.Equal(2, options.FileLoaderPanel.DemoModels.Count);
        Assert.Equal("Test Model", options.FileLoaderPanel.DemoModels[0].Name);
        Assert.Equal("models/test.wexbim", options.FileLoaderPanel.DemoModels[0].Path);
        Assert.Equal("A test model", options.FileLoaderPanel.DemoModels[0].Description);
        Assert.Equal("Second Model", options.FileLoaderPanel.DemoModels[1].Name);
    }

    [Fact]
    public void AddXbimBlazorServer_WithConfiguration_BindsAllOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Xbim:Standalone:Theme:InitialTheme"] = "Light",
                ["Xbim:Standalone:FileLoaderPanel:AllowIfcFiles"] = "true"
            })
            .Build();

        // Act
        services.AddXbimBlazorServer(configuration);
        var provider = services.BuildServiceProvider();
        var options = provider.GetService<XbimBlazorOptions>();

        // Assert
        Assert.NotNull(options);
        Assert.Equal(ViewerTheme.Light, options.InitialTheme);
        Assert.True(options.FileLoaderPanel.AllowIfcFiles);
    }

    [Fact]
    public void AddWexBlazorPlatformConnected_WithConfiguration_BindsServerOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Xbim:Server:BaseUrl"] = "https://api.example.com",
                ["Xbim:Server:RequireAuthentication"] = "true",
                ["Xbim:Server:TimeoutSeconds"] = "60"
            })
            .Build();

        // Act
        services.AddWexBlazorPlatformConnected(configuration);
        var provider = services.BuildServiceProvider();
        var serverOptions = provider.GetService<XbimServerOptions>();

        // Assert
        Assert.NotNull(serverOptions);
        Assert.Equal("https://api.example.com", serverOptions.BaseUrl);
        Assert.True(serverOptions.RequireAuthentication);
        Assert.Equal(60, serverOptions.TimeoutSeconds);
    }

    [Fact]
    public void AddWexBlazorPlatformConnected_WithConfiguration_RegistersServerServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Xbim:Server:BaseUrl"] = "https://api.example.com"
            })
            .Build();

        // Act
        services.AddWexBlazorPlatformConnected(configuration);

        // Assert
        Assert.Contains(services, d => d.ServiceType == typeof(IWorkspacesService));
        Assert.Contains(services, d => d.ServiceType == typeof(IProjectsService));
        Assert.Contains(services, d => d.ServiceType == typeof(IFilesService));
        Assert.Contains(services, d => d.ServiceType == typeof(IModelsService));
    }

    [Fact]
    public void AddWexBlazorPlatformConnected_WithMissingBaseUrl_ThrowsInvalidOperationException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                // Missing BaseUrl
                ["Xbim:Server:RequireAuthentication"] = "true"
            })
            .Build();

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            services.AddWexBlazorPlatformConnected(configuration));
        Assert.Contains("BaseUrl", ex.Message);
        Assert.Contains("required", ex.Message);
    }

    [Fact]
    public void AddWexBlazorPlatformConnected_WithInvalidBaseUrl_ThrowsInvalidOperationException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Xbim:Server:BaseUrl"] = "not-a-valid-url"
            })
            .Build();

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            services.AddWexBlazorPlatformConnected(configuration));
        Assert.Contains("valid HTTP or HTTPS URL", ex.Message);
    }

    [Fact]
    public void XbimServerOptions_Validate_ThrowsOnMissingBaseUrl()
    {
        // Arrange
        var options = new XbimServerOptions { BaseUrl = null };

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => options.Validate());
        Assert.Contains("BaseUrl", ex.Message);
    }

    [Fact]
    public void XbimServerOptions_Validate_ThrowsOnInvalidUrl()
    {
        // Arrange
        var options = new XbimServerOptions { BaseUrl = "ftp://invalid.com" };

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => options.Validate());
        Assert.Contains("HTTP or HTTPS", ex.Message);
    }

    [Fact]
    public void XbimServerOptions_Validate_SucceedsWithValidUrl()
    {
        // Arrange
        var options = new XbimServerOptions { BaseUrl = "https://api.example.com" };

        // Act & Assert - Should not throw
        options.Validate();
    }

    [Fact]
    public void FileLoaderPanelOptions_ToDemoModelList_ConvertsCorrectly()
    {
        // Arrange
        var options = new FileLoaderPanelOptions();
        options.AddDemoModel("Model 1", "path/to/model1.wexbim", "First model");
        options.AddDemoModel("Model 2", "https://example.com/model2.wexbim");

        // Act
        var demoModels = options.ToDemoModelList();

        // Assert
        Assert.Equal(2, demoModels.Count);
        Assert.Equal("Model 1", demoModels[0].Name);
        Assert.Equal("path/to/model1.wexbim", demoModels[0].Path);
        Assert.Equal("First model", demoModels[0].Description);
        Assert.Equal("Model 2", demoModels[1].Name);
        Assert.Null(demoModels[1].Description);
    }

    [Fact]
    public void ThemeOptions_GetViewerTheme_ReturnsCorrectTheme()
    {
        // Arrange & Act & Assert
        Assert.Equal(ViewerTheme.Light, new ThemeOptions { InitialTheme = "Light" }.GetViewerTheme());
        Assert.Equal(ViewerTheme.Light, new ThemeOptions { InitialTheme = "light" }.GetViewerTheme());
        Assert.Equal(ViewerTheme.Light, new ThemeOptions { InitialTheme = "LIGHT" }.GetViewerTheme());
        Assert.Equal(ViewerTheme.Dark, new ThemeOptions { InitialTheme = "Dark" }.GetViewerTheme());
        Assert.Equal(ViewerTheme.Dark, new ThemeOptions { InitialTheme = "invalid" }.GetViewerTheme());
        Assert.Equal(ViewerTheme.Dark, new ThemeOptions { InitialTheme = null! }.GetViewerTheme());
    }

}
