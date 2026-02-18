using Moq;
using Xbim.WexBlazor.Services.Abstractions.Server;
using Xbim.WexBlazor.Services.Server;
using Xbim.Api.Client;

namespace Xbim.WexBlazor.Tests.Server;

public class ProcessingServiceTests
{
    private readonly Mock<IXbimApiClient> _mockClient;
    private readonly ProcessingService _service;

    public ProcessingServiceTests()
    {
        _mockClient = new Mock<IXbimApiClient>();
        _service = new ProcessingService(_mockClient.Object);
    }

    [Fact]
    public async Task GetStatusAsync_ShouldReturnModelVersion()
    {
        // Arrange
        var versionId = Guid.NewGuid();
        var expected = new ModelVersionDto { Id = versionId, Status = ProcessingStatus._1 };
        _mockClient.Setup(c => c.GetModelVersionAsync(versionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.GetStatusAsync(versionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ProcessingStatus._1, result.Status);
    }

    [Fact]
    public async Task GetStatusAsync_WhenNotFound_ShouldReturnNull()
    {
        // Arrange
        var versionId = Guid.NewGuid();
        _mockClient.Setup(c => c.GetModelVersionAsync(versionId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new XbimApiException("Not found", 404, null, new Dictionary<string, IEnumerable<string>>(), null));

        // Act
        var result = await _service.GetStatusAsync(versionId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetStatusAsync_When401_ShouldThrowXbimServiceException()
    {
        // Arrange
        var versionId = Guid.NewGuid();
        _mockClient.Setup(c => c.GetModelVersionAsync(versionId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new XbimApiException("Unauthorized", 401, null, new Dictionary<string, IEnumerable<string>>(), null));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<XbimServiceException>(() => _service.GetStatusAsync(versionId));
        Assert.True(ex.IsUnauthorized);
    }

    [Fact]
    public void StartWatching_ShouldAddToWatchedVersions()
    {
        // Arrange
        var versionId = Guid.NewGuid();
        _mockClient.Setup(c => c.GetModelVersionAsync(versionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ModelVersionDto { Id = versionId, Status = ProcessingStatus._0 });

        // Act
        _service.StartWatching(versionId);

        // Assert
        Assert.Contains(versionId, _service.WatchedVersions);
    }

    [Fact]
    public void StartWatching_WithInvalidInterval_ShouldThrow()
    {
        // Arrange
        var versionId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _service.StartWatching(versionId, 50));
    }

    [Fact]
    public void StartWatching_SameVersionTwice_ShouldNotDuplicate()
    {
        // Arrange
        var versionId = Guid.NewGuid();
        _mockClient.Setup(c => c.GetModelVersionAsync(versionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ModelVersionDto { Id = versionId, Status = ProcessingStatus._0 });

        // Act
        _service.StartWatching(versionId);
        _service.StartWatching(versionId);

        // Assert
        Assert.Single(_service.WatchedVersions);
    }

    [Fact]
    public void StopWatching_ShouldRemoveFromWatchedVersions()
    {
        // Arrange
        var versionId = Guid.NewGuid();
        _mockClient.Setup(c => c.GetModelVersionAsync(versionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ModelVersionDto { Id = versionId, Status = ProcessingStatus._0 });
        _service.StartWatching(versionId);

        // Act
        _service.StopWatching(versionId);

        // Assert
        Assert.DoesNotContain(versionId, _service.WatchedVersions);
    }

    [Fact]
    public void StopWatchingAll_ShouldClearAllWatchers()
    {
        // Arrange
        var versionId1 = Guid.NewGuid();
        var versionId2 = Guid.NewGuid();
        _mockClient.Setup(c => c.GetModelVersionAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => new ModelVersionDto { Id = id, Status = ProcessingStatus._0 });
        _service.StartWatching(versionId1);
        _service.StartWatching(versionId2);

        // Act
        _service.StopWatchingAll();

        // Assert
        Assert.Empty(_service.WatchedVersions);
    }

    [Fact]
    public async Task WhenStatusChanges_ShouldRaiseEvent()
    {
        // Arrange
        var versionId = Guid.NewGuid();
        var callCount = 0;
        ModelVersionStatusChangedEventArgs? receivedArgs = null;

        _mockClient.SetupSequence(c => c.GetModelVersionAsync(versionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ModelVersionDto { Id = versionId, Status = ProcessingStatus._0 })
            .ReturnsAsync(new ModelVersionDto { Id = versionId, Status = ProcessingStatus._1 })
            .ReturnsAsync(new ModelVersionDto { Id = versionId, Status = ProcessingStatus._2 });

        _service.OnStatusChanged += args =>
        {
            callCount++;
            receivedArgs = args;
        };

        // Act
        _service.StartWatching(versionId, 100);
        await Task.Delay(350); // Allow time for polling
        _service.StopWatching(versionId);

        // Assert - Should have at least one status change event
        Assert.True(callCount > 0);
        Assert.NotNull(receivedArgs);
        Assert.Equal(versionId, receivedArgs.VersionId);
    }

    [Fact]
    public async Task WhenProcessingComplete_ShouldStopWatching()
    {
        // Arrange
        var versionId = Guid.NewGuid();

        _mockClient.SetupSequence(c => c.GetModelVersionAsync(versionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ModelVersionDto { Id = versionId, Status = ProcessingStatus._1 })
            .ReturnsAsync(new ModelVersionDto { Id = versionId, Status = ProcessingStatus._2 }); // Ready

        // Act
        _service.StartWatching(versionId, 100);
        await Task.Delay(300); // Allow time for polling and auto-stop

        // Assert
        Assert.Empty(_service.WatchedVersions);
    }

    [Fact]
    public async Task WhenProcessingFails_ShouldStopWatching()
    {
        // Arrange
        var versionId = Guid.NewGuid();

        _mockClient.SetupSequence(c => c.GetModelVersionAsync(versionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ModelVersionDto { Id = versionId, Status = ProcessingStatus._1 })
            .ReturnsAsync(new ModelVersionDto { Id = versionId, Status = ProcessingStatus._3, ErrorMessage = "Failed" }); // Failed

        // Act
        _service.StartWatching(versionId, 100);
        await Task.Delay(300); // Allow time for polling and auto-stop

        // Assert
        Assert.Empty(_service.WatchedVersions);
    }

    [Fact]
    public void Dispose_ShouldStopAllWatchers()
    {
        // Arrange
        var versionId = Guid.NewGuid();
        _mockClient.Setup(c => c.GetModelVersionAsync(versionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ModelVersionDto { Id = versionId, Status = ProcessingStatus._0 });
        _service.StartWatching(versionId);

        // Act
        _service.Dispose();

        // Assert
        Assert.Empty(_service.WatchedVersions);
    }

    [Fact]
    public void StartWatching_AfterDispose_ShouldThrow()
    {
        // Arrange
        _service.Dispose();

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => _service.StartWatching(Guid.NewGuid()));
    }
}
