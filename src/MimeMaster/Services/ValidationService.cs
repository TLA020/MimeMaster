using Microsoft.Extensions.Logging;
using MimeMaster.Models;

namespace MimeMaster.Services;

/// <summary>
/// Service for validating files against configured constraints
/// </summary>
public class ValidationService : IValidationService
{
    private readonly IFileTypeService _fileTypeService;
    private readonly ILogger<ValidationService> _logger;

    /// <summary>
    /// Initializes a new instance of the ValidationService class
    /// </summary>
    /// <param name="fileTypeService">The file type service to use for MIME type detection</param>
    /// <param name="logger">The logger to use for recording validation operations</param>
    /// <exception cref="ArgumentNullException">Thrown when fileTypeService or logger is null</exception>
    public ValidationService(IFileTypeService fileTypeService, ILogger<ValidationService> logger)
    {
        _fileTypeService = fileTypeService ?? throw new ArgumentNullException(nameof(fileTypeService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Validates a file against allowed MIME types and size constraints
    /// </summary>
    /// <param name="fileName">Name of the file</param>
    /// <param name="fileData">Binary content of the file</param>
    /// <param name="allowedMimeTypes">Comma-separated list of allowed MIME types</param>
    /// <param name="maxFileSize">Maximum allowed file size in bytes</param>
    /// <param name="minFileSize">Minimum allowed file size in bytes</param>
    /// <returns>ValidationResult containing validation errors if any</returns>
    public ValidationResult Validate(string fileName, byte[] fileData, string allowedMimeTypes, long maxFileSize, long minFileSize = 0)
    {
        var result = new ValidationResult();
        ValidationErrors errors = ValidationErrors.None;

        _logger.LogDebug("Validating file {FileName} with size {FileSize} bytes", fileName, fileData.Length);

        // Check file size constraints
        if (fileData.Length > maxFileSize)
        {
            _logger.LogWarning("File {FileName} is too large. Size: {FileSize}, Max allowed: {MaxSize}", 
                fileName, fileData.Length, maxFileSize);
            errors |= ValidationErrors.FileTooLarge;
        }
        
        if (fileData.Length < minFileSize)
        {
            _logger.LogWarning("File {FileName} is too small. Size: {FileSize}, Min allowed: {MinSize}", 
                fileName, fileData.Length, minFileSize);
            errors |= ValidationErrors.FileTooSmall;
        }

        try
        {
            // Detect file type
            var fileType = _fileTypeService.GetMimeType(fileName, fileData);
            
            // Check if file type is allowed
            var allowed = IsFileTypeAllowed(fileType.Mime, allowedMimeTypes);
            if (!allowed)
            {
                _logger.LogWarning("File type {MimeType} is not allowed for file {FileName}", 
                    fileType.Mime, fileName);
                errors |= ValidationErrors.FileTypeNotAllowed;
            }
        }
        catch (InvalidOperationException)
        {
            _logger.LogWarning("Unknown file type for file {FileName}", fileName);
            errors |= ValidationErrors.FileTypeUnknown;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating file {FileName}", fileName);
            errors |= ValidationErrors.FileTypeUnknown;
        }

        if (errors != ValidationErrors.None)
        {
            result.AddInvalidFile(fileName, errors);
        }

        return result;
    }

    private bool IsFileTypeAllowed(string mimeType, string allowedMimeTypes)
    {
        if (string.IsNullOrWhiteSpace(allowedMimeTypes))
        {
            return true; // If no allowed MIME types are specified, all types are allowed
        }

        var allowedTypes = allowedMimeTypes.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return allowedTypes.Contains(mimeType, StringComparer.OrdinalIgnoreCase);
    }
}
