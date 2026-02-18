using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xbim.WexBlazor.Components;

namespace Xbim.WexBlazor.Tests.Components;

/// <summary>
/// Tests for <see cref="FileLoaderPanel"/> component hosting mode restrictions.
/// </summary>
public class FileLoaderPanelTests : TestContext
{
    #region Hosting Mode Restriction Tests

    [Fact]
    public void FileLoaderPanel_InStandaloneMode_ShouldRenderSuccessfully()
    {
        // Arrange
        Services.AddXbimBlazorStandalone();

        // bUnit provides FakeNavigationManager automatically

        // Act & Assert - Should not throw
        var cut = RenderComponent<FileLoaderPanel>(parameters => parameters
            .Add(p => p.IsVisible, true));

        // Verify component rendered
        Assert.NotNull(cut);
        Assert.NotNull(cut.Find(".file-loader-panel"));
    }

    [Fact]
    public void FileLoaderPanel_InServerMode_ShouldRenderSuccessfully()
    {
        // Arrange - AddXbimBlazorServer is for IFC processing, still standalone mode
        Services.AddXbimBlazorServer();

        // Act & Assert - Should not throw (Server mode is still standalone)
        var cut = RenderComponent<FileLoaderPanel>(parameters => parameters
            .Add(p => p.IsVisible, true));

        // Verify component rendered
        Assert.NotNull(cut);
    }

    [Fact]
    public void FileLoaderPanel_InPlatformConnectedMode_ShouldThrowStandaloneOnlyComponentException()
    {
        // Arrange
        Services.AddXbimBlazorPlatformConnected("https://localhost:5000");

        // Act & Assert
        var exception = Assert.Throws<StandaloneOnlyComponentException>(() =>
        {
            RenderComponent<FileLoaderPanel>();
        });

        // Verify exception details
        Assert.Equal("FileLoaderPanel", exception.ComponentName);
        Assert.Contains("Standalone-only", exception.Message);
        Assert.Contains("Workspace", exception.Message);
        Assert.Contains("Project", exception.Message);
        Assert.Contains("Model", exception.Message);
        Assert.Contains("Version", exception.Message);
    }

    [Fact]
    public void FileLoaderPanel_InPlatformConnectedMode_ExceptionShouldSuggestNavigation()
    {
        // Arrange
        Services.AddXbimBlazorPlatformConnected("https://localhost:5000");

        // Act
        var exception = Assert.Throws<StandaloneOnlyComponentException>(() =>
        {
            RenderComponent<FileLoaderPanel>();
        });

        // Assert - Exception message should suggest using navigation instead
        Assert.Contains("navigation", exception.Message);
        Assert.Contains("AddXbimBlazorStandalone", exception.Message);
    }

    #endregion

    #region Backward Compatibility Tests

    [Fact]
    public void FileLoaderPanel_WithAddXbimBlazor_ShouldWork()
    {
        // Arrange - Backward compatible method
        Services.AddXbimBlazor();

        // Act & Assert - Should not throw
        var cut = RenderComponent<FileLoaderPanel>(parameters => parameters
            .Add(p => p.IsVisible, true));

        Assert.NotNull(cut);
    }

    #endregion

    #region Error Message Clarity Tests

    [Fact]
    public void StandaloneOnlyComponentException_MessageFormat_ShouldBeUserFriendly()
    {
        // Arrange
        Services.AddXbimBlazorPlatformConnected("https://localhost:5000");

        // Act
        var exception = Assert.Throws<StandaloneOnlyComponentException>(() =>
        {
            RenderComponent<FileLoaderPanel>();
        });

        // Assert - Message should be actionable and clear
        // The message should explain:
        // 1. What component failed
        // 2. Why it failed (standalone-only)
        // 3. What to do instead (use navigation)
        // 4. How to fix if standalone is needed (use different registration)
        Assert.Contains("FileLoaderPanel", exception.Message);
        Assert.Contains("Standalone-only", exception.Message);
        Assert.Contains("->", exception.Message); // Arrow notation for navigation path
    }

    #endregion

    #region Hosting Mode Verification Tests

    [Fact]
    public void FileLoaderPanel_StandaloneMode_HostingModeProviderIsStandalone()
    {
        // Arrange
        Services.AddXbimBlazorStandalone();
        var provider = Services.BuildServiceProvider();

        // Act
        var hostingModeProvider = provider.GetRequiredService<IXbimHostingModeProvider>();

        // Assert
        Assert.True(hostingModeProvider.IsStandalone);
        Assert.False(hostingModeProvider.IsPlatformConnected);
    }

    [Fact]
    public void FileLoaderPanel_PlatformConnectedMode_HostingModeProviderIsPlatformConnected()
    {
        // Arrange
        Services.AddXbimBlazorPlatformConnected("https://localhost:5000");
        var provider = Services.BuildServiceProvider();

        // Act
        var hostingModeProvider = provider.GetRequiredService<IXbimHostingModeProvider>();

        // Assert
        Assert.False(hostingModeProvider.IsStandalone);
        Assert.True(hostingModeProvider.IsPlatformConnected);
    }

    #endregion
}
