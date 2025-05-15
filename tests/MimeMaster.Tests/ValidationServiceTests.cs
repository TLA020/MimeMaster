using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using MimeMaster.Models;
using MimeMaster.Services;
using Moq;

namespace MimeMaster.Tests;

public class ValidationServiceTests
{
    [Theory]
    [InlineData("hello-world.doc", ValidationErrors.FileTypeNotAllowed | ValidationErrors.FileTooLarge, new[] { "text/plain", "application/pdf" })]
    [InlineData("hello-world.doc", ValidationErrors.None, new[] { "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" })]
    [InlineData("hello-world.docx", ValidationErrors.FileTypeNotAllowed | ValidationErrors.FileTooLarge, new[] { "text/plain", "application/pdf" })]
    [InlineData("hello-world.docx", ValidationErrors.None, new[] { "application/vnd.openxmlformats-officedocument.wordprocessingml.document" })]
    [InlineData("hello-world.jpg", ValidationErrors.None, new[] { "image/jpeg" })]
    [InlineData("hello-world.jpg", ValidationErrors.FileTypeNotAllowed | ValidationErrors.FileTooLarge, new[] { "text/plain", "application/pdf" })]
    [InlineData("hello-world.pdf", ValidationErrors.None, new[] { "application/pdf" })]
    [InlineData("hello-world.pdf", ValidationErrors.FileTypeNotAllowed | ValidationErrors.FileTooLarge, new[] { "text/plain", "image/jpeg" })]
    [InlineData("hello-world.png", ValidationErrors.None, new[] { "image/png" })]
    [InlineData("hello-world.png", ValidationErrors.FileTypeNotAllowed | ValidationErrors.FileTooLarge, new[] { "text/plain", "application/pdf" })]
    public void Validate_WithDifferentFiles_ReturnsExpectedResult(string fileName,
        ValidationErrors expected,
        string[] allowedMimeTypes)
    {
        const int maxFileSize = 1024 * 1024; // 1MB

        // Load the test file
        var filePath = Path.Combine("Resources", fileName);
        var fileData = File.ReadAllBytes(filePath);

        // Set up file size for testing FileTooLarge validation
        var fileDataToTest = fileData;
        if (expected.HasFlag(ValidationErrors.FileTooLarge))
        {
            // Create a larger file data array for testing
            var largerSize = maxFileSize + 1024;
            fileDataToTest = new byte[largerSize];
            Array.Copy(fileData, fileDataToTest, Math.Min(fileData.Length, largerSize));
        }

        // Set up mocks
        var fileTypeServiceMock = new Mock<IFileTypeService>();
        var loggerMock = new Mock<ILogger<ValidationService>>();

        // Configure the mock based on the file extension
        switch (Path.GetExtension(fileName).ToLowerInvariant())
        {
            case ".doc":
                fileTypeServiceMock.Setup(x => x.GetMimeType(It.IsAny<string>(), It.IsAny<byte[]>()))
                    .Returns(FileType.Doc);
                break;
            case ".docx":
                fileTypeServiceMock.Setup(x => x.GetMimeType(It.IsAny<string>(), It.IsAny<byte[]>()))
                    .Returns(FileType.Docx);
                break;
            case ".jpg":
                fileTypeServiceMock.Setup(x => x.GetMimeType(It.IsAny<string>(), It.IsAny<byte[]>()))
                    .Returns(FileType.Jpeg);
                break;
            case ".pdf":
                fileTypeServiceMock.Setup(x => x.GetMimeType(It.IsAny<string>(), It.IsAny<byte[]>()))
                    .Returns(FileType.Pdf);
                break;
            case ".png":
                fileTypeServiceMock.Setup(x => x.GetMimeType(It.IsAny<string>(), It.IsAny<byte[]>()))
                    .Returns(FileType.Png);
                break;
            default:
                fileTypeServiceMock.Setup(x => x.GetMimeType(It.IsAny<string>(), It.IsAny<byte[]>()))
                    .Throws(new InvalidOperationException("Unknown file type"));
                break;
        }

        // Create service and validate
        var validationService = new ValidationService(fileTypeServiceMock.Object, loggerMock.Object);
        var result = validationService.Validate(
            fileName,
            fileDataToTest,
            string.Join(",", allowedMimeTypes),
            maxFileSize);

        // Check expectations
        if (expected == ValidationErrors.None)
        {
            Assert.False(result.HasFailed, $"{fileName} should be valid");
            Assert.Empty(result.InvalidFiles);
        }
        else
        {
            Assert.True(result.HasFailed, $"{fileName} should be invalid");
            Assert.Single(result.InvalidFiles);

            var invalidFile = result.InvalidFiles.First();
            Assert.Equal(fileName, invalidFile.FileName);

            if (expected.HasFlag(ValidationErrors.FileTypeUnknown))
            {
                Assert.True(invalidFile.Errors.HasFlag(ValidationErrors.FileTypeUnknown),
                    $"File {fileName} should have FileTypeUnknown error");
            }
            else
            {
                // Check FileTooLarge flag
                var expectedHasFileTooLarge = expected.HasFlag(ValidationErrors.FileTooLarge);
                var actualHasFileTooLarge = invalidFile.Errors.HasFlag(ValidationErrors.FileTooLarge);
                Assert.True(expectedHasFileTooLarge == actualHasFileTooLarge,
                    $"File {fileName} FileTooLarge validation mismatch - Expected: {expectedHasFileTooLarge}, Actual: {actualHasFileTooLarge}");

                // Check FileTooSmall flag
                var expectedHasFileTooSmall = expected.HasFlag(ValidationErrors.FileTooSmall);
                var actualHasFileTooSmall = invalidFile.Errors.HasFlag(ValidationErrors.FileTooSmall);
                Assert.True(expectedHasFileTooSmall == actualHasFileTooSmall,
                    $"File {fileName} FileTooSmall validation mismatch - Expected: {expectedHasFileTooSmall}, Actual: {actualHasFileTooSmall}");

                // Check FileTypeNotAllowed flag
                var expectedHasFileTypeNotAllowed = expected.HasFlag(ValidationErrors.FileTypeNotAllowed);
                var actualHasFileTypeNotAllowed = invalidFile.Errors.HasFlag(ValidationErrors.FileTypeNotAllowed);
                Assert.True(expectedHasFileTypeNotAllowed == actualHasFileTypeNotAllowed,
                    $"File {fileName} FileTypeNotAllowed validation mismatch - Expected: {expectedHasFileTypeNotAllowed}, Actual: {actualHasFileTypeNotAllowed}");
            }
        }
    }

    [Fact]
    public void Validate_UnknownFileType_ReturnsFileTypeUnknown()
    {
        // Create test data
        const string fileName = "unknown.xyz";
        var fileData = new byte[100]; // Dummy data

        // Set up mocks
        var fileTypeServiceMock = new Mock<IFileTypeService>();
        var loggerMock = new Mock<ILogger<ValidationService>>();

        fileTypeServiceMock.Setup(x => x.GetMimeType(It.IsAny<string>(), It.IsAny<byte[]>()))
            .Throws(new InvalidOperationException("Unknown file type"));

        // Create service and validate
        var validationService = new ValidationService(fileTypeServiceMock.Object, loggerMock.Object);
        var result = validationService.Validate(
            fileName,
            fileData,
            "application/pdf,image/jpeg",
            1024 * 1024);

        // Check expectations
        Assert.True(result.HasFailed);
        Assert.Single(result.InvalidFiles);

        var invalidFile = result.InvalidFiles.First();
        Assert.Equal(fileName, invalidFile.FileName);
        Assert.True(invalidFile.Errors.HasFlag(ValidationErrors.FileTypeUnknown));
    }
}
