using MimeMaster.Models;

namespace MimeMaster.Tests;

public class ModelsTests
{
    [Fact]
    public void FileType_Constructor_SetsProperties()
    {
        var fileTypeSignature = new FileTypeSignature
        {
            Extension = ".PDF",
            MimeType = "application/pdf",
            Headers = [new Signature([0x25, 0x50, 0x44, 0x46])]
        };

        var fileType = FileType.FromSignature(fileTypeSignature);

        // Assert
        Assert.Equal("application/pdf", fileType.Mime);
        Assert.Equal(".pdf", fileType.Extension);
    }

    [Fact]
    public void FileType_FromSignature_CreatesCorrectFileType()
    {
        // Arrange
        var fileTypeSignature = new FileTypeSignature
        {
            Extension = ".PDF",
            MimeType = "application/pdf",
            Headers = [new Signature([0x25, 0x50, 0x44, 0x46])]
        };

        // Act
        var fileType = FileType.FromSignature(fileTypeSignature);

        // Assert
        Assert.Equal(fileTypeSignature.MimeType, fileType.Mime);
        Assert.Equal(".pdf", fileType.Extension);
    }

    [Fact]
    public void InvalidFile_Constructor_SetsProperties()
    {
        // Arrange
        var fileName = "test.pdf";
        var errors = ValidationErrors.FileTooLarge | ValidationErrors.FileTypeNotAllowed;

        // Act
        var invalidFile = new InvalidFile(fileName, errors);

        // Assert
        Assert.Equal(fileName, invalidFile.FileName);
        Assert.Equal(errors, invalidFile.Errors);
    }

    [Theory]
    [InlineData(ValidationErrors.None, false)]
    [InlineData(ValidationErrors.FileTooLarge, true)]
    [InlineData(ValidationErrors.FileTooSmall, true)]
    [InlineData(ValidationErrors.FileTypeNotAllowed, true)]
    [InlineData(ValidationErrors.FileTypeUnknown, true)]
    [InlineData(ValidationErrors.FileTooLarge | ValidationErrors.FileTypeNotAllowed, true)]
    public void ValidationErrors_HasFlag_ReturnsExpectedResult(ValidationErrors errors, bool expectedHasError)
    {
        // Assert
        Assert.Equal(expectedHasError, errors != ValidationErrors.None);
    }
}
