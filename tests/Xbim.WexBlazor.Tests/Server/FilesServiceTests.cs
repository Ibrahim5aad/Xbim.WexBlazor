using Moq;
using Xbim.WexBlazor.Services.Abstractions.Server;
using Xbim.WexBlazor.Services.Server;
using Xbim.WexServer.Client;

namespace Xbim.WexBlazor.Tests.Server;

public class FilesServiceTests
{
    private readonly Mock<IXbimApiClient> _mockClient;
    private readonly IFilesService _service;

    public FilesServiceTests()
    {
        _mockClient = new Mock<IXbimApiClient>();
        _service = new FilesService(_mockClient.Object);
    }

    [Fact]
    public async Task ReserveUploadAsync_ShouldCallClientAndReturnResult()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = new ReserveUploadRequest { FileName = "test.ifc", ContentType = "application/octet-stream" };
        var sessionId = Guid.NewGuid();
        var expected = new ReserveUploadResponse
        {
            Session = new UploadSessionDto { Id = sessionId, FileName = "test.ifc" }
        };
        _mockClient.Setup(c => c.ReserveUploadAsync(projectId, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.ReserveUploadAsync(projectId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(sessionId, result.Session?.Id);
    }

    [Fact]
    public async Task ReserveUploadAsync_WithNullRequest_ShouldThrow()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.ReserveUploadAsync(Guid.NewGuid(), null!));
    }

    [Fact]
    public async Task GetUploadSessionAsync_ShouldReturnSession()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var expected = new UploadSessionDto { Id = sessionId, FileName = "test.ifc" };
        _mockClient.Setup(c => c.GetUploadSessionAsync(projectId, sessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.GetUploadSessionAsync(projectId, sessionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(sessionId, result.Id);
    }

    [Fact]
    public async Task UploadContentAsync_ShouldCallClientAndReturnResult()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var file = new FileParameter(new MemoryStream(new byte[] { 0x01, 0x02 }), "test.ifc", "application/octet-stream");
        var expected = new UploadContentResponse { BytesUploaded = 2 };
        _mockClient.Setup(c => c.UploadContentAsync(projectId, sessionId, file, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.UploadContentAsync(projectId, sessionId, file);

        // Assert
        Assert.Equal(2, result.BytesUploaded);
    }

    [Fact]
    public async Task UploadContentAsync_WithNullFile_ShouldThrow()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.UploadContentAsync(Guid.NewGuid(), Guid.NewGuid(), null!));
    }

    [Fact]
    public async Task CommitUploadAsync_ShouldCallClientAndReturnResult()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var fileId = Guid.NewGuid();
        var expected = new CommitUploadResponse { File = new FileDto { Id = fileId } };
        _mockClient.Setup(c => c.CommitUploadAsync(projectId, sessionId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.CommitUploadAsync(projectId, sessionId);

        // Assert
        Assert.NotNull(result.File);
        Assert.Equal(fileId, result.File.Id);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnFile()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var expected = new FileDto { Id = fileId, Name = "test.ifc" };
        _mockClient.Setup(c => c.GetFileAsync(fileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.GetAsync(fileId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(fileId, result.Id);
    }

    [Fact]
    public async Task GetAsync_WhenNotFound_ShouldReturnNull()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        _mockClient.Setup(c => c.GetFileAsync(fileId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new XbimApiException("Not found", 404, null, new Dictionary<string, IEnumerable<string>>(), null));

        // Act
        var result = await _service.GetAsync(fileId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ListAsync_ShouldReturnPagedList()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var expected = new FileDtoPagedList
        {
            Items = new List<FileDto> { new() { Id = Guid.NewGuid(), Name = "test.ifc" } },
            Page = 1,
            PageSize = 20
        };
        _mockClient.Setup(c => c.ListFilesAsync(projectId, null, null, 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.ListAsync(projectId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items!);
    }

    [Fact]
    public async Task ListAsync_WithFilters_ShouldPassCorrectParameters()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var expected = new FileDtoPagedList { Items = new List<FileDto>() };
        _mockClient.Setup(c => c.ListFilesAsync(projectId, FileKind._0, FileCategory._0, 2, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        await _service.ListAsync(projectId, FileKind._0, FileCategory._0, 2, 10);

        // Assert
        _mockClient.Verify(c => c.ListFilesAsync(projectId, FileKind._0, FileCategory._0, 2, 10, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallClientAndReturnResult()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var expected = new FileDto { Id = fileId };
        _mockClient.Setup(c => c.DeleteFileAsync(fileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.DeleteAsync(fileId);

        // Assert
        Assert.Equal(fileId, result.Id);
    }

    [Fact]
    public async Task DeleteAsync_When403_ShouldThrowXbimServiceException()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        _mockClient.Setup(c => c.DeleteFileAsync(fileId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new XbimApiException("Forbidden", 403, null, new Dictionary<string, IEnumerable<string>>(), null));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<XbimServiceException>(() => _service.DeleteAsync(fileId));
        Assert.True(ex.IsForbidden);
    }
}
