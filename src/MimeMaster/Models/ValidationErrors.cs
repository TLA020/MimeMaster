namespace MimeMaster.Models;

/// <summary>
/// Specifies the types of validation errors that can occur when validating files
/// </summary>
[Flags]
public enum ValidationErrors
{
    /// <summary>
    /// No validation errors
    /// </summary>
    None = 0,
    
    /// <summary>
    /// The file type is not in the list of allowed types
    /// </summary>
    FileTypeNotAllowed = 1 << 0,
    
    /// <summary>
    /// The file size exceeds the maximum allowed size
    /// </summary>
    FileTooLarge = 1 << 1,
    
    /// <summary>
    /// The file size is below the minimum required size
    /// </summary>
    FileTooSmall = 1 << 2,
    
    /// <summary>
    /// The file type could not be determined or is not recognized
    /// </summary>
    FileTypeUnknown = 1 << 3
}
