
using MimeMaster.Models;

namespace MimeMaster.Tests;

public class FileSignaturesTests
{
    [Theory]
    [InlineData("hello-world.doc", ".DOC")]
    [InlineData("hello-world.docx", ".DOCX")]
    [InlineData("hello-world.jpg", ".JPG")]
    [InlineData("hello-world.pdf", ".PDF")]
    [InlineData("hello-world.png", ".PNG")]
    [InlineData("hello-world.ppt", ".PPT")]
    [InlineData("hello-world.xls", ".XLS")]
    [InlineData("hello-world.xlsx", ".XLSX")]
    public void GetFileType_ValidFile_ReturnsCorrectFileType(string fileName, string expectedExtension)
    {
        var filePath = Path.Combine("Resources", fileName);
        var fileData = File.ReadAllBytes(filePath);

        var fileType = FileSignatures.GeFileTypeSignature(filePath, fileData) ?? throw new Exception("File type not found");

        Assert.Equal(expectedExtension, fileType.Extension);
    }
}
