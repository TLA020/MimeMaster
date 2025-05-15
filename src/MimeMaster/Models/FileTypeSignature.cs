namespace MimeMaster.Models;

/// <summary>
/// Represents a file type signature used for identifying file types
/// </summary>
public class FileTypeSignature
{
    /// <summary>
    /// Gets or sets the file extension associated with this signature (e.g., ".PDF")
    /// </summary>
    public required string Extension { get; init; }
    
    /// <summary>
    /// Gets or sets the MIME type associated with this signature (e.g., "application/pdf")
    /// </summary>
    public required string MimeType { get; init; }
    
    /// <summary>
    /// Gets or sets the binary signatures found at the beginning of the file (file headers)
    /// </summary>
    public required Signature[] Headers { get; init; }
    
    /// <summary>
    /// Gets or sets the binary signatures found at the end of the file (file trailers), if any
    /// </summary>
    public Signature[]? Trailers { get; init; }
    
    /// <summary>
    /// Gets or sets a string to search for within the file content to confirm the file type,
    /// particularly useful for archive-based formats like Office Open XML
    /// </summary>
    public string? ContentSniffTarget { get; init; }
}
