namespace MimeMaster.Models;

/// <summary>
/// Represents a binary signature pattern used for file type identification
/// </summary>
/// <param name="value">The byte sequence that makes up the signature</param>
/// <param name="offset">The offset (in bytes) from the beginning/end of the file where this signature should be found</param>
public class Signature(byte[] value, int offset = 0)
{
    /// <summary>
    /// Gets or sets the byte sequence that makes up the signature
    /// </summary>
    public byte[] Value { get; set; } = value;
    
    /// <summary>
    /// Gets or sets the offset (in bytes) from the beginning/end of the file where this signature should be found
    /// </summary>
    public int Offset { get; set; } = offset;
}