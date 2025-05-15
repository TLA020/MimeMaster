using MimeMaster.Models;

namespace MimeMaster.Tests;

public class FileSignaturesAdditionalTests
{
    [Fact]
    public void GeFileTypeSignature_NullParameters_ReturnsNull()
    {
        // Act
        var result = FileSignatures.GeFileTypeSignature(null, null);
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public void GeFileTypeSignature_EmptyParameters_ReturnsNull()
    {
        // Act
        var result = FileSignatures.GeFileTypeSignature("", Array.Empty<byte>());
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public void GeFileTypeSignature_FileWithInvalidHeader_ReturnsNull()
    {
        // Arrange
        var fileName = "test.pdf";
        var fileData = new byte[] { 0x01, 0x02, 0x03, 0x04 }; // Not a PDF header
        
        // Act
        var result = FileSignatures.GeFileTypeSignature(fileName, fileData);
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public void GeFileTypeSignature_FileWithInvalidExtension_ReturnsNull()
    {
        // Arrange - PDF header with wrong extension
        var fileName = "test.txt";
        var fileData = new byte[1024];
        fileData[0] = 0x25; // %
        fileData[1] = 0x50; // P
        fileData[2] = 0x44; // D
        fileData[3] = 0x46; // F
        
        // Act
        var result = FileSignatures.GeFileTypeSignature(fileName, fileData);
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public void GeFileTypeSignature_TxtFile_ReturnsNull()
    {
        // Arrange
        var fileName = "test.txt";
        var fileData = new byte[1024]; // Empty file
        
        // Act
        var result = FileSignatures.GeFileTypeSignature(fileName, fileData);
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public void GeFileTypeSignature_UnknownExtension_ReturnsNull()
    {
        // Arrange - PDF header with unknown extension
        var fileName = "test.xyz";
        var fileData = new byte[1024];
        fileData[0] = 0x25; // %
        fileData[1] = 0x50; // P
        fileData[2] = 0x44; // D
        fileData[3] = 0x46; // F
        
        // Act
        var result = FileSignatures.GeFileTypeSignature(fileName, fileData);
        
        // Assert
        Assert.Null(result);
    }
    
    [Theory]
    [InlineData(".pdf")]
    public void GeFileTypeSignature_KnownTypes_DetectsCorrectly(string extension)
    {
        // Arrange
        var fileName = $"test{extension}";
        var fileData = new byte[1024];
        
        // Set appropriate header based on extension
        if (extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            // PDF header
            fileData[0] = 0x25; // %
            fileData[1] = 0x50; // P
            fileData[2] = 0x44; // D
            fileData[3] = 0x46; // F
            
            // Add PDF EOF trailer
            var trailer = new byte[] { 0x0A, 0x25, 0x25, 0x45, 0x4F, 0x46 }; // .%%EOF
            Array.Copy(trailer, 0, fileData, fileData.Length - trailer.Length, trailer.Length);
        }
        
        // Act
        var result = FileSignatures.GeFileTypeSignature(fileName, fileData);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(extension.ToUpperInvariant(), result.Extension);
    }
}