using Microsoft.Extensions.DependencyInjection;
using Xbim.WexBlazor.Services.Abstractions.Server;
using Xbim.WexBlazor.Services.Server;
using Xbim.WexBlazor.Services.Server.Guards;
using Xbim.Api.Client;

namespace Xbim.WexBlazor.Tests.Server;

/// <summary>
/// Tests for server-only service guard implementations and <see cref="ServerServiceNotConfiguredException"/>.
/// </summary>
public class NotConfiguredServicesTests
{
    #region ServerServiceNotConfiguredException Tests

    [Fact]
    public void ServerServiceNotConfiguredException_ShouldContainServiceName()
    {
        // Arrange & Act
        var exception = new ServerServiceNotConfiguredException(typeof(IWorkspacesService));

        // Assert
        Assert.Equal("IWorkspacesService", exception.ServiceName);
    }

    [Fact]
    public void ServerServiceNotConfiguredException_ShouldContainActionableMessage()
    {
        // Arrange & Act
        var exception = new ServerServiceNotConfiguredException(typeof(IWorkspacesService));

        // Assert
        Assert.Contains("IWorkspacesService", exception.Message);
        Assert.Contains("AddXbimBlazorPlatformConnected", exception.Message);
        Assert.Contains("standalone mode", exception.Message);
    }

    [Fact]
    public void ServerServiceNotConfiguredException_WithStringName_ShouldWork()
    {
        // Arrange & Act
        var exception = new ServerServiceNotConfiguredException("CustomService");

        // Assert
        Assert.Equal("CustomService", exception.ServiceName);
        Assert.Contains("CustomService", exception.Message);
    }

    [Fact]
    public void ServerServiceNotConfiguredException_WithCustomMessage_ShouldUseCustomMessage()
    {
        // Arrange & Act
        var exception = new ServerServiceNotConfiguredException("CustomService", "Custom error message");

        // Assert
        Assert.Equal("CustomService", exception.ServiceName);
        Assert.Equal("Custom error message", exception.Message);
    }

    [Fact]
    public void ServerServiceNotConfiguredException_ShouldBeInvalidOperationException()
    {
        // Arrange & Act
        var exception = new ServerServiceNotConfiguredException(typeof(IWorkspacesService));

        // Assert
        Assert.IsAssignableFrom<InvalidOperationException>(exception);
    }

    #endregion

    #region Guard Registration in Standalone Mode Tests

    [Fact]
    public void AddXbimBlazorStandalone_ShouldRegisterGuardServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddXbimBlazorStandalone();

        // Assert - Guard services should be registered
        Assert.Contains(services, d => d.ServiceType == typeof(IWorkspacesService));
        Assert.Contains(services, d => d.ServiceType == typeof(IProjectsService));
        Assert.Contains(services, d => d.ServiceType == typeof(IFilesService));
        Assert.Contains(services, d => d.ServiceType == typeof(IModelsService));
        Assert.Contains(services, d => d.ServiceType == typeof(IUsageService));
        Assert.Contains(services, d => d.ServiceType == typeof(IProcessingService));
    }

    [Fact]
    public void AddXbimBlazorStandalone_GuardsShouldBeResolvable()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddXbimBlazorStandalone();
        var provider = services.BuildServiceProvider();

        // Act & Assert - Guards should be resolvable (no DI resolution failures)
        Assert.NotNull(provider.GetRequiredService<IWorkspacesService>());
        Assert.NotNull(provider.GetRequiredService<IProjectsService>());
        Assert.NotNull(provider.GetRequiredService<IFilesService>());
        Assert.NotNull(provider.GetRequiredService<IModelsService>());
        Assert.NotNull(provider.GetRequiredService<IUsageService>());
        Assert.NotNull(provider.GetRequiredService<IProcessingService>());
    }

    [Fact]
    public void AddXbimBlazorStandalone_GuardsShouldBeCorrectTypes()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddXbimBlazorStandalone();
        var provider = services.BuildServiceProvider();

        // Act
        var workspacesService = provider.GetRequiredService<IWorkspacesService>();
        var projectsService = provider.GetRequiredService<IProjectsService>();
        var filesService = provider.GetRequiredService<IFilesService>();
        var modelsService = provider.GetRequiredService<IModelsService>();
        var usageService = provider.GetRequiredService<IUsageService>();
        var processingService = provider.GetRequiredService<IProcessingService>();

        // Assert - Should be guard implementations
        Assert.IsType<NotConfiguredWorkspacesService>(workspacesService);
        Assert.IsType<NotConfiguredProjectsService>(projectsService);
        Assert.IsType<NotConfiguredFilesService>(filesService);
        Assert.IsType<NotConfiguredModelsService>(modelsService);
        Assert.IsType<NotConfiguredUsageService>(usageService);
        Assert.IsType<NotConfiguredProcessingService>(processingService);
    }

    #endregion

    #region Guard Services Throw Tests

    [Fact]
    public async Task NotConfiguredWorkspacesService_AllMethods_ShouldThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddXbimBlazorStandalone();
        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IWorkspacesService>();

        // Act & Assert
        var ex1 = await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.CreateAsync(new CreateWorkspaceRequest()));
        Assert.Contains("IWorkspacesService", ex1.Message);

        var ex2 = await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.GetAsync(Guid.NewGuid()));
        Assert.Contains("AddXbimBlazorPlatformConnected", ex2.Message);

        await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.ListAsync());

        await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.UpdateAsync(Guid.NewGuid(), new UpdateWorkspaceRequest()));
    }

    [Fact]
    public async Task NotConfiguredProjectsService_AllMethods_ShouldThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddXbimBlazorStandalone();
        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IProjectsService>();

        // Act & Assert
        await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.CreateAsync(Guid.NewGuid(), new CreateProjectRequest()));

        await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.GetAsync(Guid.NewGuid()));

        await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.ListAsync(Guid.NewGuid()));

        await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.UpdateAsync(Guid.NewGuid(), new UpdateProjectRequest()));
    }

    [Fact]
    public async Task NotConfiguredFilesService_AllMethods_ShouldThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddXbimBlazorStandalone();
        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IFilesService>();

        // Act & Assert
        await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.ReserveUploadAsync(Guid.NewGuid(), new ReserveUploadRequest()));

        await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.GetUploadSessionAsync(Guid.NewGuid(), Guid.NewGuid()));

        await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.UploadContentAsync(Guid.NewGuid(), Guid.NewGuid(), new FileParameter(Stream.Null, "test")));

        await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.CommitUploadAsync(Guid.NewGuid(), Guid.NewGuid()));

        await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.GetAsync(Guid.NewGuid()));

        await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.ListAsync(Guid.NewGuid()));

        await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.DownloadAsync(Guid.NewGuid()));

        await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.DeleteAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task NotConfiguredModelsService_AllMethods_ShouldThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddXbimBlazorStandalone();
        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IModelsService>();

        // Act & Assert
        await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.CreateAsync(Guid.NewGuid(), new CreateModelRequest()));

        await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.GetAsync(Guid.NewGuid()));

        await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.ListAsync(Guid.NewGuid()));

        await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.CreateVersionAsync(Guid.NewGuid(), new CreateModelVersionRequest()));

        await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.GetVersionAsync(Guid.NewGuid()));

        await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.ListVersionsAsync(Guid.NewGuid()));

        await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.GetWexBimAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task NotConfiguredUsageService_AllMethods_ShouldThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddXbimBlazorStandalone();
        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IUsageService>();

        // Act & Assert
        await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.GetWorkspaceUsageAsync(Guid.NewGuid()));

        await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.GetProjectUsageAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task NotConfiguredProcessingService_AllMethods_ShouldThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddXbimBlazorStandalone();
        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IProcessingService>();

        // Act & Assert
        await Assert.ThrowsAsync<ServerServiceNotConfiguredException>(
            () => service.GetStatusAsync(Guid.NewGuid()));

        Assert.Throws<ServerServiceNotConfiguredException>(
            () => service.StartWatching(Guid.NewGuid()));

        Assert.Throws<ServerServiceNotConfiguredException>(
            () => service.StopWatching(Guid.NewGuid()));

        Assert.Throws<ServerServiceNotConfiguredException>(
            () => service.StopWatchingAll());

        Assert.Throws<ServerServiceNotConfiguredException>(
            () => _ = service.WatchedVersions);

        // Event add/remove should also throw
        Assert.Throws<ServerServiceNotConfiguredException>(
            () => service.OnStatusChanged += (args) => { });

        Assert.Throws<ServerServiceNotConfiguredException>(
            () => service.OnStatusChanged -= (args) => { });
    }

    #endregion

    #region Server-Connected Overrides Guards Tests

    [Fact]
    public void AddXbimBlazorPlatformConnected_ShouldOverrideGuards()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act - Calling ServerConnected should replace guards with real implementations
        services.AddXbimBlazorPlatformConnected("https://localhost:5000");

        // Assert - Service descriptors should point to real implementations, not guards
        var workspacesDescriptor = services.First(d => d.ServiceType == typeof(IWorkspacesService));
        var projectsDescriptor = services.First(d => d.ServiceType == typeof(IProjectsService));
        var filesDescriptor = services.First(d => d.ServiceType == typeof(IFilesService));
        var modelsDescriptor = services.First(d => d.ServiceType == typeof(IModelsService));
        var usageDescriptor = services.First(d => d.ServiceType == typeof(IUsageService));
        var processingDescriptor = services.First(d => d.ServiceType == typeof(IProcessingService));

        Assert.Equal(typeof(WorkspacesService), workspacesDescriptor.ImplementationType);
        Assert.Equal(typeof(ProjectsService), projectsDescriptor.ImplementationType);
        Assert.Equal(typeof(FilesService), filesDescriptor.ImplementationType);
        Assert.Equal(typeof(ModelsService), modelsDescriptor.ImplementationType);
        Assert.Equal(typeof(UsageService), usageDescriptor.ImplementationType);
        Assert.Equal(typeof(ProcessingService), processingDescriptor.ImplementationType);
    }

    [Fact]
    public void AddXbimBlazorPlatformConnected_AfterStandalone_ShouldStillOverrideGuards()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act - First register standalone (registers guards), then server-connected (should override)
        services.AddXbimBlazorStandalone();
        services.AddXbimBlazorPlatformConnected("https://localhost:5000");

        // Assert - Service descriptor should point to real implementation
        var workspacesDescriptor = services.First(d => d.ServiceType == typeof(IWorkspacesService));
        Assert.Equal(typeof(WorkspacesService), workspacesDescriptor.ImplementationType);
    }

    [Fact]
    public void AddXbimBlazorPlatformConnected_OverridesGuardsNotDuplicates()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddXbimBlazorPlatformConnected("https://localhost:5000");

        // Assert - Should be exactly one registration per service type (not guards + real)
        Assert.Single(services, d => d.ServiceType == typeof(IWorkspacesService));
        Assert.Single(services, d => d.ServiceType == typeof(IProjectsService));
        Assert.Single(services, d => d.ServiceType == typeof(IFilesService));
        Assert.Single(services, d => d.ServiceType == typeof(IModelsService));
        Assert.Single(services, d => d.ServiceType == typeof(IUsageService));
        Assert.Single(services, d => d.ServiceType == typeof(IProcessingService));
    }

    #endregion

    #region StandaloneOnlyComponentException Tests

    [Fact]
    public void StandaloneOnlyComponentException_ShouldContainComponentName()
    {
        // Arrange & Act
        var exception = new StandaloneOnlyComponentException("FileLoaderPanel");

        // Assert
        Assert.Equal("FileLoaderPanel", exception.ComponentName);
    }

    [Fact]
    public void StandaloneOnlyComponentException_ShouldContainActionableMessage()
    {
        // Arrange & Act
        var exception = new StandaloneOnlyComponentException("FileLoaderPanel");

        // Assert
        Assert.Contains("FileLoaderPanel", exception.Message);
        Assert.Contains("Standalone-only", exception.Message);
        Assert.Contains("Workspace", exception.Message);
        Assert.Contains("Project", exception.Message);
        Assert.Contains("Model", exception.Message);
        Assert.Contains("Version", exception.Message);
        Assert.Contains("navigation", exception.Message);
    }

    [Fact]
    public void StandaloneOnlyComponentException_ShouldSuggestCorrectRegistration()
    {
        // Arrange & Act
        var exception = new StandaloneOnlyComponentException("FileLoaderPanel");

        // Assert
        Assert.Contains("AddXbimBlazorStandalone", exception.Message);
        Assert.Contains("AddXbimBlazorPlatformConnected", exception.Message);
    }

    [Fact]
    public void StandaloneOnlyComponentException_WithCustomMessage_ShouldUseCustomMessage()
    {
        // Arrange & Act
        var exception = new StandaloneOnlyComponentException("TestComponent", "Custom error message");

        // Assert
        Assert.Equal("TestComponent", exception.ComponentName);
        Assert.Equal("Custom error message", exception.Message);
    }

    [Fact]
    public void StandaloneOnlyComponentException_ShouldBeInvalidOperationException()
    {
        // Arrange & Act
        var exception = new StandaloneOnlyComponentException("FileLoaderPanel");

        // Assert
        Assert.IsAssignableFrom<InvalidOperationException>(exception);
    }

    #endregion

    #region Standalone Viewer Components Do Not Require Server Tests

    [Fact]
    public void AddXbimBlazorStandalone_StandaloneServicesWorkWithoutServer()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddXbimBlazorStandalone();
        var provider = services.BuildServiceProvider();

        // Act & Assert - Standalone services should work normally
        var themeService = provider.GetRequiredService<Xbim.WexBlazor.Services.ThemeService>();
        Assert.NotNull(themeService);

        var propertyService = provider.GetRequiredService<Xbim.WexBlazor.Services.Abstractions.IPropertyService>();
        Assert.NotNull(propertyService);

        // Standalone services should be usable (not guards that throw)
        // ThemeService methods work
        themeService.SetTheme(Xbim.WexBlazor.Models.ViewerTheme.Dark);
        Assert.Equal(Xbim.WexBlazor.Models.ViewerTheme.Dark, themeService.CurrentTheme);

        // PropertyService should be empty but functional
        var sources = propertyService.Sources;
        Assert.NotNull(sources);
        Assert.Empty(sources);
    }

    #endregion

    #region AddXbimBlazorServer Tests (IFC Processing Mode)

    [Fact]
    public void AddXbimBlazorServer_ShouldAlsoRegisterGuards()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddXbimBlazorServer();
        var provider = services.BuildServiceProvider();

        // Assert - Server services should still be guards (IFC mode doesn't include API connectivity)
        var workspacesService = provider.GetRequiredService<IWorkspacesService>();
        Assert.IsType<NotConfiguredWorkspacesService>(workspacesService);
    }

    #endregion
}
