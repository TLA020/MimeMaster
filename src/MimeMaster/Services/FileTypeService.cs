using Microsoft.Extensions.Logging;
using MimeMaster.Models;
using System.Text;

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

    /// <summary>
    /// Asynchronously determines the MIME type of a file based on its content and file name
    /// </summary>
    /// <param name="fileName">The name of the file, used to determine the file extension</param>
    /// <param name="fileData">The binary content of the file, used to examine file signatures</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>A <see cref="FileType"/> containing the MIME type and extension information</returns>
    /// <exception cref="ArgumentException">Thrown when fileName or fileData is null or empty</exception>
    /// <exception cref="InvalidOperationException">Thrown when the file type cannot be determined</exception>
    public Task<FileType> GetMimeTypeAsync(string fileName, byte[] fileData, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(GetMimeType(fileName, fileData));
    }

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
    public async Task<FileType> GetMimeTypeAsync(string fileName, Stream stream, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));
        }

        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        cancellationToken.ThrowIfCancellationRequested();

        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);
        var fileData = memoryStream.ToArray();

        return GetMimeType(fileName, fileData);
    }

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
    public async Task<FileType> GetMimeTypeFromStreamAsync(string fileName, Stream stream, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));
        }

        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        cancellationToken.ThrowIfCancellationRequested();

        var extension = Path.GetExtension(fileName)?.ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(extension))
        {
            throw new InvalidOperationException($"Cannot determine file type without extension for file {fileName}");
        }

        _logger.LogDebug("Detecting MIME type for file {FileName} from stream", fileName);

        var signature = await GetFileTypeSignatureFromStreamAsync(fileName, stream, cancellationToken);
        if (signature == null)
        {
            throw new InvalidOperationException($"Unknown file type for file {fileName}");
        }

        var fileType = FileType.FromSignature(signature);

        _logger.LogDebug("Detected MIME type {MimeType} for file {FileName}",
            fileType.Mime, fileName);

        return fileType;
    }

    private async Task<FileTypeSignature?> GetFileTypeSignatureFromStreamAsync(string fileName, Stream stream, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(fileName)?.ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(extension))
        {
            return null;
        }

        var fileSignatures = FileSignatures.GetFileSignatures();
        var fileTypeSignature = fileSignatures.FirstOrDefault(f => f.Extension.ToUpperInvariant() == extension);

        if (fileTypeSignature == null)
        {
            return null;
        }

        const int bufferSize = 1024;
        var headerBuffer = new byte[bufferSize];
        var originalPosition = stream.Position;

        try
        {
            stream.Position = 0;
            var headerBytesRead = await stream.ReadAsync(headerBuffer, 0, bufferSize, cancellationToken);

            foreach (var header in fileTypeSignature.Headers)
            {
                if (headerBytesRead < header.Offset + header.Value.Length)
                {
                    continue;
                }

                var headerMatch = headerBuffer.Skip(header.Offset).Take(header.Value.Length).SequenceEqual(header.Value);
                if (!headerMatch)
                {
                    continue;
                }

                if (fileTypeSignature.Trailers == null || fileTypeSignature.Trailers.Length == 0)
                {
                    return fileTypeSignature;
                }

                if (stream.CanSeek)
                {
                    var trailerBuffer = new byte[bufferSize];
                    var streamLength = stream.Length;
                    var trailerPosition = Math.Max(0, streamLength - bufferSize);
                    stream.Position = trailerPosition;
                    var trailerBytesRead = await stream.ReadAsync(trailerBuffer, 0, bufferSize, cancellationToken);

                    foreach (var trailer in fileTypeSignature.Trailers)
                    {
                        var trailerOffset = trailerBytesRead - trailer.Value.Length - trailer.Offset;
                        if (trailerOffset >= 0 && trailerOffset < trailerBytesRead)
                        {
                            var trailerMatch = trailerBuffer.Skip(trailerOffset).Take(trailer.Value.Length).SequenceEqual(trailer.Value);
                            if (trailerMatch)
                            {
                                if (string.IsNullOrWhiteSpace(fileTypeSignature.ContentSniffTarget))
                                {
                                    return fileTypeSignature;
                                }

                                return await CheckContentSniffTargetAsync(stream, fileTypeSignature, cancellationToken) ? fileTypeSignature : null;
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(fileTypeSignature.ContentSniffTarget))
                {
                    return await CheckContentSniffTargetAsync(stream, fileTypeSignature, cancellationToken) ? fileTypeSignature : null;
                }
            }
        }
        finally
        {
            if (stream.CanSeek)
            {
                stream.Position = originalPosition;
            }
        }

        return null;
    }

    private async Task<bool> CheckContentSniffTargetAsync(Stream stream, FileTypeSignature fileTypeSignature, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(fileTypeSignature.ContentSniffTarget))
        {
            return true;
        }

        var originalPosition = stream.Position;
        try
        {
            stream.Position = 0;
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            var content = await reader.ReadToEndAsync(cancellationToken);
            return content.Contains(fileTypeSignature.ContentSniffTarget);
        }
        finally
        {
            if (stream.CanSeek)
            {
                stream.Position = originalPosition;
            }
        }
    }
}
