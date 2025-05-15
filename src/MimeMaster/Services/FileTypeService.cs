using Microsoft.Extensions.Logging;
using MimeMaster.Models;

namespace MimeMaster.Services;

/// <summary>
/// Service implementation for detecting MIME types of files based on their content and file name
/// </summary>
public class FileTypeService : IFileTypeService
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileTypeService"/> class
    /// </summary>
    /// <param name="logger">The logger to use for recording operations</param>
    /// <exception cref="ArgumentNullException">Thrown when logger is null</exception>
    public FileTypeService(ILogger<FileTypeService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Determines the MIME type of a file based on its content and file name
    /// </summary>
    /// <param name="fileName">The name of the file, used to determine the file extension</param>
    /// <param name="fileData">The binary content of the file, used to examine file signatures</param>
    /// <returns>A <see cref="FileType"/> containing the MIME type and extension information</returns>
    /// <exception cref="ArgumentException">Thrown when fileName or fileData is null or empty</exception>
    /// <exception cref="InvalidOperationException">Thrown when the file type cannot be determined</exception>
    public FileType GetMimeType(string fileName, byte[] fileData)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));
        }

        if (fileData == null || fileData.Length == 0)
        {
            throw new ArgumentException("File data cannot be null or empty.", nameof(fileData));
        }

        _logger.LogDebug("Detecting MIME type for file {FileName} with size {FileSize} bytes", 
            fileName, fileData.Length);

        var signature = FileSignatures.GeFileTypeSignature(fileName, fileData) ?? 
            throw new InvalidOperationException($"Unknown file type for file {fileName}");
            
        var fileType = FileType.FromSignature(signature);
        
        _logger.LogDebug("Detected MIME type {MimeType} for file {FileName}", 
            fileType.Mime, fileName);
            
        return fileType;
    }
}
