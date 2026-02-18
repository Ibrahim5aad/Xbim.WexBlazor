using Xbim.WexBlazor.Services.Server;
using Xbim.WexServer.Client;

namespace Xbim.WexBlazor.Tests.Server;

public class XbimServiceExceptionTests
{
    [Theory]
    [InlineData(401, true, false, false, false, false, false)]
    [InlineData(403, false, true, false, false, false, false)]
    [InlineData(404, false, false, true, false, false, false)]
    [InlineData(409, false, false, false, true, false, false)]
    [InlineData(400, false, false, false, false, true, false)]
    [InlineData(500, false, false, false, false, false, true)]
    [InlineData(502, false, false, false, false, false, true)]
    [InlineData(503, false, false, false, false, false, true)]
    public void StatusCodeProperties_ShouldReturnCorrectValues(
        int statusCode,
        bool isUnauthorized,
        bool isForbidden,
        bool isNotFound,
        bool isConflict,
        bool isBadRequest,
        bool isServerError)
    {
        // Arrange
        var ex = new XbimServiceException("Test", statusCode);

        // Assert
        Assert.Equal(isUnauthorized, ex.IsUnauthorized);
        Assert.Equal(isForbidden, ex.IsForbidden);
        Assert.Equal(isNotFound, ex.IsNotFound);
        Assert.Equal(isConflict, ex.IsConflict);
        Assert.Equal(isBadRequest, ex.IsBadRequest);
        Assert.Equal(isServerError, ex.IsServerError);
    }

    [Fact]
    public void FromApiException_ShouldCreateCorrectException()
    {
        // Arrange
        var apiException = new XbimApiException(
            "API error",
            403,
            "Response body",
            new Dictionary<string, IEnumerable<string>>(),
            null);

        // Act
        var serviceException = XbimServiceException.FromApiException(apiException);

        // Assert
        Assert.Equal(403, serviceException.StatusCode);
        Assert.Equal("Response body", serviceException.Response);
        Assert.True(serviceException.IsForbidden);
        Assert.Same(apiException, serviceException.InnerException);
    }

    [Theory]
    [InlineData(401, "Authentication required")]
    [InlineData(403, "Access denied")]
    [InlineData(404, "not found")]
    [InlineData(409, "conflict")]
    [InlineData(400, "Invalid request")]
    [InlineData(500, "server error")]
    public void FromApiException_ShouldCreateUserFriendlyMessage(int statusCode, string expectedMessagePart)
    {
        // Arrange
        var apiException = new XbimApiException(
            "API error",
            statusCode,
            null,
            new Dictionary<string, IEnumerable<string>>(),
            null);

        // Act
        var serviceException = XbimServiceException.FromApiException(apiException);

        // Assert
        Assert.Contains(expectedMessagePart, serviceException.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_ShouldSetAllProperties()
    {
        // Arrange
        var innerException = new Exception("Inner");

        // Act
        var ex = new XbimServiceException("Test message", 404, "Response", innerException);

        // Assert
        Assert.Equal("Test message", ex.Message);
        Assert.Equal(404, ex.StatusCode);
        Assert.Equal("Response", ex.Response);
        Assert.Same(innerException, ex.InnerException);
    }
}
