using Moq;
using Xbim.WexBlazor.Services.Abstractions.Server;
using Xbim.WexBlazor.Services.Server;
using Xbim.WexServer.Client;

namespace Xbim.WexBlazor.Tests.Server;

public class ProjectsServiceTests
{
    private readonly Mock<IXbimApiClient> _mockClient;
    private readonly IProjectsService _service;

    public ProjectsServiceTests()
    {
        _mockClient = new Mock<IXbimApiClient>();
        _service = new ProjectsService(_mockClient.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCallClientAndReturnResult()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var request = new CreateProjectRequest { Name = "Test Project" };
        var expected = new ProjectDto { Id = Guid.NewGuid(), WorkspaceId = workspaceId, Name = "Test Project" };
        _mockClient.Setup(c => c.CreateProjectAsync(workspaceId, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.CreateAsync(workspaceId, request);

        // Assert
        Assert.Equal(expected.Id, result.Id);
        Assert.Equal(expected.WorkspaceId, result.WorkspaceId);
    }

    [Fact]
    public async Task CreateAsync_WithNullRequest_ShouldThrow()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateAsync(Guid.NewGuid(), null!));
    }

    [Fact]
    public async Task GetAsync_ShouldReturnProject()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var expected = new ProjectDto { Id = projectId, Name = "Test" };
        _mockClient.Setup(c => c.GetProjectAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.GetAsync(projectId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.Id, result.Id);
    }

    [Fact]
    public async Task GetAsync_WhenNotFound_ShouldReturnNull()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        _mockClient.Setup(c => c.GetProjectAsync(projectId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new XbimApiException("Not found", 404, null, new Dictionary<string, IEnumerable<string>>(), null));

        // Act
        var result = await _service.GetAsync(projectId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ListAsync_ShouldReturnPagedList()
    {
        // Arrange
        var workspaceId = Guid.NewGuid();
        var expected = new ProjectDtoPagedList
        {
            Items = new List<ProjectDto> { new() { Id = Guid.NewGuid(), Name = "Test" } },
            Page = 1,
            PageSize = 20,
            TotalCount = 1
        };
        _mockClient.Setup(c => c.ListProjectsAsync(workspaceId, 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.ListAsync(workspaceId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items!);
    }

    [Fact]
    public async Task UpdateAsync_ShouldCallClientAndReturnResult()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = new UpdateProjectRequest { Name = "Updated" };
        var expected = new ProjectDto { Id = projectId, Name = "Updated" };
        _mockClient.Setup(c => c.UpdateProjectAsync(projectId, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.UpdateAsync(projectId, request);

        // Assert
        Assert.Equal(expected.Name, result.Name);
    }

    [Fact]
    public async Task UpdateAsync_When403_ShouldThrowXbimServiceException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var request = new UpdateProjectRequest { Name = "Updated" };
        _mockClient.Setup(c => c.UpdateProjectAsync(projectId, request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new XbimApiException("Forbidden", 403, null, new Dictionary<string, IEnumerable<string>>(), null));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<XbimServiceException>(() => _service.UpdateAsync(projectId, request));
        Assert.True(ex.IsForbidden);
    }
}
