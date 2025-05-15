using MimeMaster.Models;

namespace MimeMaster.Services;

/// <summary>
/// Interface for validating files against configured constraints
/// </summary>
public interface IValidationService
{
    /// <summary>
    /// Validates a file against allowed MIME types and size constraints
    /// </summary>
    /// <param name="fileName">Name of the file</param>
    /// <param name="fileData">Binary content of the file</param>
    /// <param name="allowedMimeTypes">Comma-separated list of allowed MIME types</param>
    /// <param name="maxFileSize">Maximum allowed file size in bytes</param>
    /// <param name="minFileSize">Minimum allowed file size in bytes</param>
    /// <returns>ValidationResult containing validation errors if any</returns>
    ValidationResult Validate(string fileName, byte[] fileData, string allowedMimeTypes, long maxFileSize, long minFileSize = 0);
}
