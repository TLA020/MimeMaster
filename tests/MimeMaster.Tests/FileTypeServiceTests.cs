using Microsoft.Extensions.Logging;
using MimeMaster.Services;
using Moq;

namespace MimeMaster.Tests;

public class FileTypeServiceTests
{
    [Fact]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new FileTypeService(null!));
        Assert.Equal("logger", exception.ParamName);
    }

    [Fact]
    public void GetMimeType_NullFileName_ThrowsArgumentException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<FileTypeService>>();
        var service = new FileTypeService(loggerMock.Object);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            service.GetMimeType(null!, [0x25, 0x50, 0x44, 0x46]));

        Assert.Equal("fileName", exception.ParamName);
        Assert.Contains("File name cannot be null or empty", exception.Message);
    }

    [Fact]
    public void GetMimeType_EmptyFileName_ThrowsArgumentException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<FileTypeService>>();
        var service = new FileTypeService(loggerMock.Object);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            service.GetMimeType("", [0x25, 0x50, 0x44, 0x46]));
        Assert.Equal("fileName", exception.ParamName);
        Assert.Contains("File name cannot be null or empty", exception.Message);
    }

    [Fact]
    public void GetMimeType_NullFileData_ThrowsArgumentException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<FileTypeService>>();
        var service = new FileTypeService(loggerMock.Object);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            service.GetMimeType("test.pdf", null!));
        Assert.Equal("fileData", exception.ParamName);
        Assert.Contains("File data cannot be null or empty", exception.Message);
    }

    [Fact]
    public void GetMimeType_EmptyFileData_ThrowsArgumentException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<FileTypeService>>();
        var service = new FileTypeService(loggerMock.Object);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            service.GetMimeType("test.pdf", []));
        Assert.Equal("fileData", exception.ParamName);
        Assert.Contains("File data cannot be null or empty", exception.Message);
    }

    [Fact]
    public void GetMimeType_UnknownFileType_ThrowsInvalidOperationException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<FileTypeService>>();
        var service = new FileTypeService(loggerMock.Object);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            service.GetMimeType("unknown.xyz", [0x00, 0x01, 0x02, 0x03]));
        Assert.Contains("Unknown file type", exception.Message);
    }

    [Theory]
    [InlineData("test.pdf", new byte[] { 0x25, 0x50, 0x44, 0x46 }, "application/pdf", ".pdf")]
    public void GetMimeType_ValidFileData_ReturnsCorrectFileType(string fileName, byte[] headerBytes, string expectedMime, string expectedExtension)
    {
        // Arrange
    var loggerMock = new Mock<ILogger<FileTypeService>>();
    var service = new FileTypeService(loggerMock.Object);

    // Create test file data with valid header
    var fileData = new byte[1024];
    Array.Copy(headerBytes, fileData, headerBytes.Length);

    // Add PDF trailer for PDF test
    if (fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
    {
        // Add PDF EOF trailer
        var trailer = new byte[] { 0x0A, 0x25, 0x25, 0x45, 0x4F, 0x46 }; // .%%EOF
        Array.Copy(trailer, 0, fileData, fileData.Length - trailer.Length, trailer.Length);
    }

        // Act
        var result = service.GetMimeType(fileName, fileData);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedMime, result.Mime);
        Assert.Equal(expectedExtension, result.Extension);
    }
}
