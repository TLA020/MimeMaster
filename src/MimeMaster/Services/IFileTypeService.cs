using MimeMaster.Models;

namespace MimeMaster.Services;

/// <summary>
/// Service for detecting MIME types of files based on their signatures and file name
/// </summary>
public interface IFileTypeService
{
    /// <summary>
    /// Determines the MIME type of a file based on its signature and file name
    /// </summary>
    /// <param name="fileName">The name of the file, used to determine the file extension</param>
    /// <param name="fileData">The binary content of the file, used to examine file signatures</param>
    /// <returns>A <see cref="FileType"/> containing the MIME type and extension information</returns>
    /// <exception cref="ArgumentException">Thrown when fileName or fileData is null or empty</exception>
    /// <exception cref="InvalidOperationException">Thrown when the file type cannot be determined</exception>
    FileType GetMimeType(string fileName, byte[] fileData);
}
