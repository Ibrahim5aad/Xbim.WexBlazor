using Microsoft.Extensions.DependencyInjection;
using Xbim.WexBlazor.Models;
using Xbim.WexBlazor.Services;
using Xbim.WexBlazor.Services.Abstractions;
using Xbim.WexBlazor.Services.Server.Guards;

namespace Xbim.WexBlazor.Tests;

/// <summary>
/// Tests for <see cref="ServiceCollectionExtensions"/> standalone and backward-compatible registration methods.
/// </summary>
public class StandaloneServiceCollectionExtensionsTests
{
    #region AddWexBlazorStandalone Tests

    [Fact]
    public void AddWexBlazorStandalone_ShouldRegisterCoreServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddWexBlazorStandalone();

        // Assert - Core services should be registered
        Assert.Contains(services, d => d.ServiceType == typeof(ThemeService));
        Assert.Contains(services, d => d.ServiceType == typeof(IPropertyService));
        Assert.Contains(services, d => d.ServiceType == typeof(PropertyService));
        Assert.Contains(services, d => d.ServiceType == typeof(IfcHierarchyService));
    }

    [Fact]
    public void AddWexBlazorStandalone_ServicesShouldBeSingletons()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddWexBlazorStandalone();

        // Assert - Services should be registered as singletons
        var themeDescriptor = services.First(d => d.ServiceType == typeof(ThemeService));
        Assert.Equal(ServiceLifetime.Singleton, themeDescriptor.Lifetime);

        var propertyDescriptor = services.First(d => d.ServiceType == typeof(PropertyService));
        Assert.Equal(ServiceLifetime.Singleton, propertyDescriptor.Lifetime);
    }

    [Fact]
    public void AddWexBlazorStandalone_WithOptions_ShouldConfigureTheme()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddWexBlazorStandalone(options =>
        {
            options.InitialTheme = ViewerTheme.Dark;
            options.LightAccentColor = "#custom-light";
            options.DarkAccentColor = "#custom-dark";
        });
        var provider = services.BuildServiceProvider();
        var themeService = provider.GetRequiredService<ThemeService>();

        // Assert
        Assert.Equal(ViewerTheme.Dark, themeService.CurrentTheme);
    }

    [Fact]
    public void AddWexBlazorStandalone_WithOptions_ShouldStoreOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddWexBlazorStandalone(options =>
        {
            options.InitialTheme = ViewerTheme.Light;
        });
        var provider = services.BuildServiceProvider();
        var storedOptions = provider.GetService<XbimBlazorOptions>();

        // Assert
        Assert.NotNull(storedOptions);
        Assert.Equal(ViewerTheme.Light, storedOptions.InitialTheme);
    }

    [Fact]
    public void AddWexBlazorStandalone_ServicesCanBeResolved()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddWexBlazorStandalone();
        var provider = services.BuildServiceProvider();

        // Act & Assert - Services should be resolvable
        Assert.NotNull(provider.GetRequiredService<ThemeService>());
        Assert.NotNull(provider.GetRequiredService<IPropertyService>());
        Assert.NotNull(provider.GetRequiredService<PropertyService>());
        Assert.NotNull(provider.GetRequiredService<IfcHierarchyService>());
    }

    [Fact]
    public void AddWexBlazorStandalone_InterfaceAndConcreteResolveSameInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddWexBlazorStandalone();
        var provider = services.BuildServiceProvider();

        // Act
        var propertyInterface = provider.GetRequiredService<IPropertyService>();
        var propertyConcrete = provider.GetRequiredService<PropertyService>();

        // Assert - Same instance should be returned
        Assert.Same(propertyInterface, propertyConcrete);
    }

    [Fact]
    public void AddWexBlazorStandalone_RegistersGuardServicesInsteadOfRealServerServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddWexBlazorStandalone();
        var provider = services.BuildServiceProvider();

        // Assert - Server service interfaces ARE registered, but with guard implementations
        // that throw ServerServiceNotConfiguredException when used
        Assert.Contains(services, d => d.ServiceType == typeof(Services.Abstractions.Server.IWorkspacesService));
        Assert.Contains(services, d => d.ServiceType == typeof(Services.Abstractions.Server.IProjectsService));
        Assert.Contains(services, d => d.ServiceType == typeof(Services.Abstractions.Server.IFilesService));
        Assert.Contains(services, d => d.ServiceType == typeof(Services.Abstractions.Server.IModelsService));

        // Verify they are guard implementations (not real services)
        Assert.IsType<Services.Server.Guards.NotConfiguredWorkspacesService>(
            provider.GetRequiredService<Services.Abstractions.Server.IWorkspacesService>());
        Assert.IsType<Services.Server.Guards.NotConfiguredProjectsService>(
            provider.GetRequiredService<Services.Abstractions.Server.IProjectsService>());
    }

    #endregion

    #region AddXbimBlazor Backward Compatibility Tests

    [Fact]
    public void AddXbimBlazor_ShouldBeAliasForStandalone()
    {
        // Arrange
        var standaloneServices = new ServiceCollection();
        var backwardCompatServices = new ServiceCollection();

        // Act
        standaloneServices.AddWexBlazorStandalone();
        backwardCompatServices.AddXbimBlazor();

        // Assert - Both should register the same service types
        var standaloneTypes = standaloneServices.Select(d => d.ServiceType).OrderBy(t => t.FullName).ToList();
        var backwardTypes = backwardCompatServices.Select(d => d.ServiceType).OrderBy(t => t.FullName).ToList();

        Assert.Equal(standaloneTypes.Count, backwardTypes.Count);
        for (int i = 0; i < standaloneTypes.Count; i++)
        {
            Assert.Equal(standaloneTypes[i], backwardTypes[i]);
        }
    }

    [Fact]
    public void AddXbimBlazor_WithOptions_ShouldWorkLikeStandalone()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddXbimBlazor(options =>
        {
            options.InitialTheme = ViewerTheme.Dark;
        });
        var provider = services.BuildServiceProvider();
        var options = provider.GetService<XbimBlazorOptions>();

        // Assert
        Assert.NotNull(options);
        Assert.Equal(ViewerTheme.Dark, options.InitialTheme);
    }

    [Fact]
    public void AddXbimBlazor_ServicesCanBeResolved()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddXbimBlazor();
        var provider = services.BuildServiceProvider();

        // Act & Assert - Services should be resolvable (backward compatibility)
        Assert.NotNull(provider.GetRequiredService<ThemeService>());
        Assert.NotNull(provider.GetRequiredService<IPropertyService>());
    }

    #endregion

    #region AddXbimBlazorServer Tests

    [Fact]
    public void AddXbimBlazorServer_ShouldIncludeStandaloneServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddXbimBlazorServer();

        // Assert - Standalone services should be included
        Assert.Contains(services, d => d.ServiceType == typeof(ThemeService));
        Assert.Contains(services, d => d.ServiceType == typeof(IPropertyService));
    }

    [Fact]
    public void AddXbimBlazorServer_ShouldRegisterIfcModelService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddXbimBlazorServer();

        // Assert - IFC model service should be registered
        Assert.Contains(services, d => d.ServiceType == typeof(IIfcModelService));
        Assert.Contains(services, d => d.ServiceType == typeof(IfcModelService));
    }

    [Fact]
    public void AddXbimBlazorServer_WithOptions_ShouldConfigureTheme()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddXbimBlazorServer(options =>
        {
            options.InitialTheme = ViewerTheme.Light;
        });
        var provider = services.BuildServiceProvider();
        var themeService = provider.GetRequiredService<ThemeService>();

        // Assert
        Assert.Equal(ViewerTheme.Light, themeService.CurrentTheme);
    }

    [Fact]
    public void AddXbimBlazorServer_RegistersGuardServicesNotRealServerConnectedServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddXbimBlazorServer();
        var provider = services.BuildServiceProvider();

        // Assert - Server service interfaces ARE registered with guard implementations
        // (AddXbimBlazorServer is for IFC processing only, not API connectivity)
        Assert.Contains(services, d => d.ServiceType == typeof(Services.Abstractions.Server.IWorkspacesService));
        Assert.Contains(services, d => d.ServiceType == typeof(Services.Abstractions.Server.IProjectsService));

        // Verify they are guard implementations (not real services)
        Assert.IsType<Services.Server.Guards.NotConfiguredWorkspacesService>(
            provider.GetRequiredService<Services.Abstractions.Server.IWorkspacesService>());
        Assert.IsType<Services.Server.Guards.NotConfiguredProjectsService>(
            provider.GetRequiredService<Services.Abstractions.Server.IProjectsService>());
    }

    #endregion

    #region Hosting Mode Provider Tests

    [Fact]
    public void AddWexBlazorStandalone_ShouldRegisterStandaloneHostingMode()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddWexBlazorStandalone();
        var provider = services.BuildServiceProvider();

        // Assert
        var hostingModeProvider = provider.GetRequiredService<IXbimHostingModeProvider>();
        Assert.Equal(XbimHostingMode.Standalone, hostingModeProvider.HostingMode);
        Assert.True(hostingModeProvider.IsStandalone);
        Assert.False(hostingModeProvider.IsPlatformConnected);
    }

    [Fact]
    public void AddXbimBlazorServer_ShouldRegisterStandaloneHostingMode()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddXbimBlazorServer();
        var provider = services.BuildServiceProvider();

        // Assert - Server mode (IFC processing) is still standalone (not connected to Xbim.WexServer API)
        var hostingModeProvider = provider.GetRequiredService<IXbimHostingModeProvider>();
        Assert.Equal(XbimHostingMode.Standalone, hostingModeProvider.HostingMode);
        Assert.True(hostingModeProvider.IsStandalone);
        Assert.False(hostingModeProvider.IsPlatformConnected);
    }

    [Fact]
    public void AddWexBlazorPlatformConnected_ShouldRegisterPlatformConnectedHostingMode()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddWexBlazorPlatformConnected("https://localhost:5000");
        var provider = services.BuildServiceProvider();

        // Assert
        var hostingModeProvider = provider.GetRequiredService<IXbimHostingModeProvider>();
        Assert.Equal(XbimHostingMode.PlatformConnected, hostingModeProvider.HostingMode);
        Assert.False(hostingModeProvider.IsStandalone);
        Assert.True(hostingModeProvider.IsPlatformConnected);
    }

    [Fact]
    public void AddWexBlazorPlatformConnected_AfterStandalone_ShouldOverrideToConnectedMode()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act - First standalone (registers Standalone mode), then ServerConnected (should override)
        services.AddWexBlazorStandalone();
        services.AddWexBlazorPlatformConnected("https://localhost:5000");
        var provider = services.BuildServiceProvider();

        // Assert
        var hostingModeProvider = provider.GetRequiredService<IXbimHostingModeProvider>();
        Assert.Equal(XbimHostingMode.PlatformConnected, hostingModeProvider.HostingMode);
        Assert.True(hostingModeProvider.IsPlatformConnected);
    }

    [Fact]
    public void HostingModeProvider_ShouldBeSingleton()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddWexBlazorStandalone();
        var provider = services.BuildServiceProvider();

        // Act
        var instance1 = provider.GetRequiredService<IXbimHostingModeProvider>();
        var instance2 = provider.GetRequiredService<IXbimHostingModeProvider>();

        // Assert
        Assert.Same(instance1, instance2);
    }

    #endregion

    #region Double Registration Idempotency Tests

    [Fact]
    public void AddWexBlazorStandalone_CalledTwice_ShouldNotDuplicateServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddWexBlazorStandalone();
        services.AddWexBlazorStandalone();

        // Assert - Services should not be duplicated (TryAdd semantics)
        var themeCount = services.Count(d => d.ServiceType == typeof(ThemeService));
        var propertyServiceCount = services.Count(d => d.ServiceType == typeof(PropertyService));

        Assert.Equal(1, themeCount);
        Assert.Equal(1, propertyServiceCount);
    }

    [Fact]
    public void AddXbimBlazor_ThenAddWexBlazorStandalone_ShouldNotDuplicate()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddXbimBlazor();
        services.AddWexBlazorStandalone();

        // Assert - Services should not be duplicated
        var themeCount = services.Count(d => d.ServiceType == typeof(ThemeService));
        Assert.Equal(1, themeCount);
    }

    #endregion
}
