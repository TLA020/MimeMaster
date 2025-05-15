namespace MimeMaster.Models;

/// <summary>
/// Represents the result of a file validation operation
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Gets a value indicating whether the validation has failed
    /// </summary>
    public bool HasFailed => InvalidFiles.Count > 0;

    /// <summary>
    /// Gets a list of invalid files with their respective validation errors
    /// </summary>
    public List<InvalidFile> InvalidFiles { get; } = new();

    /// <summary>
    /// Adds an invalid file to the result
    /// </summary>
    /// <param name="fileName">Name of the invalid file</param>
    /// <param name="errors">Validation errors for the file</param>
    public void AddInvalidFile(string fileName, ValidationErrors errors)
    {
        InvalidFiles.Add(new InvalidFile(fileName, errors));
    }
}

/// <summary>
/// Represents a file that failed validation
/// </summary>
/// <param name="FileName">Name of the invalid file</param>
/// <param name="Errors">Validation errors for the file</param>
public record InvalidFile(string FileName, ValidationErrors Errors);
