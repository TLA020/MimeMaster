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

    /// <summary>
    /// Asynchronously determines the MIME type of a file based on its signature and file name
    /// </summary>
    /// <param name="fileName">The name of the file, used to determine the file extension</param>
    /// <param name="fileData">The binary content of the file, used to examine file signatures</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>A <see cref="FileType"/> containing the MIME type and extension information</returns>
    /// <exception cref="ArgumentException">Thrown when fileName or fileData is null or empty</exception>
    /// <exception cref="InvalidOperationException">Thrown when the file type cannot be determined</exception>
    Task<FileType> GetMimeTypeAsync(string fileName, byte[] fileData, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously determines the MIME type of a file from a stream
    /// </summary>
    /// <param name="fileName">The name of the file, used to determine the file extension</param>
    /// <param name="stream">The stream containing the file data</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>A <see cref="FileType"/> containing the MIME type and extension information</returns>
    /// <exception cref="ArgumentException">Thrown when fileName is null or empty</exception>
    /// <exception cref="ArgumentNullException">Thrown when stream is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the file type cannot be determined</exception>
    Task<FileType> GetMimeTypeAsync(string fileName, Stream stream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines the MIME type of a file by reading only the necessary bytes for signature detection
    /// </summary>
    /// <param name="fileName">The name of the file, used to determine the file extension</param>
    /// <param name="stream">The stream containing the file data</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>A <see cref="FileType"/> containing the MIME type and extension information</returns>
    /// <exception cref="ArgumentException">Thrown when fileName is null or empty</exception>
    /// <exception cref="ArgumentNullException">Thrown when stream is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when the file type cannot be determined</exception>
    Task<FileType> GetMimeTypeFromStreamAsync(string fileName, Stream stream, CancellationToken cancellationToken = default);
}
