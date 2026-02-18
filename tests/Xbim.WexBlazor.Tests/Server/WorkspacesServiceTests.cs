using Moq;
using Xbim.WexBlazor.Services.Abstractions.Server;
using Xbim.WexBlazor.Services.Server;
using Xbim.Api.Client;

namespace Xbim.WexBlazor.Tests.Server;

public class WorkspacesServiceTests
{
    private readonly Mock<IXbimApiClient> _mockClient;
    private readonly IWorkspacesService _service;

    public WorkspacesServiceTests()
    {
        _mockClient = new Mock<IXbimApiClient>();
        _service = new WorkspacesService(_mockClient.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallClientAndReturnResult()
    {
        // Arrange
        var request = new CreateWorkspaceRequest { Name = "Test Workspace" };
        var expected = new WorkspaceDto { Id = Guid.NewGuid(), Name = "Test Workspace" };
        _mockClient.Setup(c => c.CreateWorkspaceAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        Assert.Equal(expected.Id, result.Id);
        Assert.Equal(expected.Name, result.Name);
        _mockClient.Verify(c => c.CreateWorkspaceAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNullRequest_ShouldThrow()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateAsync(null!));
    }

    [Fact]
    public async Task CreateAsync_WhenClientThrows401_ShouldThrowXbimServiceException()
    {
        // Arrange
        var request = new CreateWorkspaceRequest { Name = "Test" };
        _mockClient.Setup(c => c.CreateWorkspaceAsync(request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new XbimApiException("Unauthorized", 401, null, new Dictionary<string, IEnumerable<string>>(), null));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<XbimServiceException>(() => _service.CreateAsync(request));
        Assert.True(ex.IsUnauthorized);
        Assert.Equal(401, ex.StatusCode);
    }

    [Fact]
    public async Task CreateAsync_WhenClientThrows403_ShouldThrowXbimServiceException()
    {
        // Arrange
        var request = new CreateWorkspaceRequest { Name = "Test" };
        _mockClient.Setup(c => c.CreateWorkspaceAsync(request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new XbimApiException("Forbidden", 403, null, new Dictionary<string, IEnumerable<string>>(), null));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<XbimServiceException>(() => _service.CreateAsync(request));
        Assert.True(ex.IsForbidden);
        Assert.Equal(403, ex.StatusCode);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnWorkspace()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var expected = new WorkspaceDto { Id = workspaceId, Name = "Test" };
        _mockClient.Setup(c => c.GetWorkspaceAsync(workspaceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.GetAsync(workspaceId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.Id, result.Id);
    }

    [Fact]
    public async Task GetAsync_WhenNotFound_ShouldReturnNull()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        _mockClient.Setup(c => c.GetWorkspaceAsync(workspaceId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new XbimApiException("Not found", 404, null, new Dictionary<string, IEnumerable<string>>(), null));

        // Act
        var result = await _service.GetAsync(workspaceId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ListAsync_ShouldReturnPagedList()
    {
        // Arrange
        var expected = new WorkspaceDtoPagedList
        {
            Items = new List<WorkspaceDto> { new() { Id = Guid.NewGuid(), Name = "Test" } },
            Page = 1,
            PageSize = 20,
            TotalCount = 1
        };
        _mockClient.Setup(c => c.ListWorkspacesAsync(1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.ListAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items!);
    }

    [Fact]
    public async Task ListAsync_WithPaging_ShouldPassCorrectParameters()
    {
        // Arrange
        var expected = new WorkspaceDtoPagedList { Items = new List<WorkspaceDto>(), Page = 2, PageSize = 10 };
        _mockClient.Setup(c => c.ListWorkspacesAsync(2, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.ListAsync(page: 2, pageSize: 10);

        // Assert
        _mockClient.Verify(c => c.ListWorkspacesAsync(2, 10, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldCallClientAndReturnResult()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var request = new UpdateWorkspaceRequest { Name = "Updated" };
        var expected = new WorkspaceDto { Id = workspaceId, Name = "Updated" };
        _mockClient.Setup(c => c.UpdateWorkspaceAsync(workspaceId, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.UpdateAsync(workspaceId, request);

        // Assert
        Assert.Equal(expected.Name, result.Name);
        _mockClient.Verify(c => c.UpdateWorkspaceAsync(workspaceId, request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNullRequest_ShouldThrow()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateAsync(Guid.NewGuid(), null!));
    }
}
