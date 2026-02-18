using Moq;
using Xbim.WexBlazor.Services.Abstractions.Server;
using Xbim.WexBlazor.Services.Server;
using Xbim.WexServer.Client;

namespace Xbim.WexBlazor.Tests.Server;

public class ModelsServiceTests
{
    private readonly Mock<IXbimApiClient> _mockClient;
    private readonly IModelsService _service;

    public ModelsServiceTests()
    {
        _mockClient = new Mock<IXbimApiClient>();
        _service = new ModelsService(_mockClient.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallClientAndReturnResult()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = new CreateModelRequest { Name = "Test Model" };
        var expected = new ModelDto { Id = Guid.NewGuid(), ProjectId = projectId, Name = "Test Model" };
        _mockClient.Setup(c => c.CreateModelAsync(projectId, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.CreateAsync(projectId, request);

        // Assert
        Assert.Equal(expected.Id, result.Id);
        Assert.Equal(expected.Name, result.Name);
    }

    [Fact]
    public async Task CreateAsync_WithNullRequest_ShouldThrow()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateAsync(Guid.NewGuid(), null!));
    }

    [Fact]
    public async Task GetAsync_ShouldReturnModel()
    {
        // Arrange
        var modelId = Guid.NewGuid();
        var expected = new ModelDto { Id = modelId, Name = "Test" };
        _mockClient.Setup(c => c.GetModelAsync(modelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.GetAsync(modelId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.Id, result.Id);
    }

    [Fact]
    public async Task GetAsync_WhenNotFound_ShouldReturnNull()
    {
        // Arrange
        var modelId = Guid.NewGuid();
        _mockClient.Setup(c => c.GetModelAsync(modelId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new XbimApiException("Not found", 404, null, new Dictionary<string, IEnumerable<string>>(), null));

        // Act
        var result = await _service.GetAsync(modelId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ListAsync_ShouldReturnPagedList()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var expected = new ModelDtoPagedList
        {
            Items = new List<ModelDto> { new() { Id = Guid.NewGuid(), Name = "Test" } },
            Page = 1,
            PageSize = 20
        };
        _mockClient.Setup(c => c.ListModelsAsync(projectId, 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.ListAsync(projectId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items!);
    }

    [Fact]
    public async Task CreateVersionAsync_ShouldCallClientAndReturnResult()
    {
        // Arrange
        var modelId = Guid.NewGuid();
        var request = new CreateModelVersionRequest { IfcFileId = Guid.NewGuid() };
        var expected = new ModelVersionDto { Id = Guid.NewGuid(), ModelId = modelId, Status = ProcessingStatus._0 };
        _mockClient.Setup(c => c.CreateModelVersionAsync(modelId, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.CreateVersionAsync(modelId, request);

        // Assert
        Assert.Equal(expected.Id, result.Id);
        Assert.Equal(ProcessingStatus._0, result.Status);
    }

    [Fact]
    public async Task CreateVersionAsync_WithNullRequest_ShouldThrow()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateVersionAsync(Guid.NewGuid(), null!));
    }

    [Fact]
    public async Task GetVersionAsync_ShouldReturnVersion()
    {
        // Arrange
        var versionId = Guid.NewGuid();
        var expected = new ModelVersionDto { Id = versionId, VersionNumber = 1 };
        _mockClient.Setup(c => c.GetModelVersionAsync(versionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.GetVersionAsync(versionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.Id, result.Id);
    }

    [Fact]
    public async Task GetVersionAsync_WhenNotFound_ShouldReturnNull()
    {
        // Arrange
        var versionId = Guid.NewGuid();
        _mockClient.Setup(c => c.GetModelVersionAsync(versionId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new XbimApiException("Not found", 404, null, new Dictionary<string, IEnumerable<string>>(), null));

        // Act
        var result = await _service.GetVersionAsync(versionId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ListVersionsAsync_ShouldReturnPagedList()
    {
        // Arrange
        var modelId = Guid.NewGuid();
        var expected = new ModelVersionDtoPagedList
        {
            Items = new List<ModelVersionDto> { new() { Id = Guid.NewGuid(), VersionNumber = 1 } },
            Page = 1,
            PageSize = 20
        };
        _mockClient.Setup(c => c.ListModelVersionsAsync(modelId, 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.ListVersionsAsync(modelId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items!);
    }

    [Fact]
    public async Task GetVersionAsync_When401_ShouldThrowXbimServiceException()
    {
        // Arrange
        var versionId = Guid.NewGuid();
        _mockClient.Setup(c => c.GetModelVersionAsync(versionId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new XbimApiException("Unauthorized", 401, null, new Dictionary<string, IEnumerable<string>>(), null));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<XbimServiceException>(() => _service.GetVersionAsync(versionId));
        Assert.True(ex.IsUnauthorized);
    }
}
