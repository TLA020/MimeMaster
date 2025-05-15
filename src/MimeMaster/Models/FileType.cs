namespace MimeMaster.Models;

/// <summary>
/// Represents a file type with its MIME type and extension
/// </summary>
public class FileType
{
    /// <summary>PDF file type</summary>
    public static readonly FileType Pdf = new ("application/pdf", ".pdf");
    
    /// <summary>Microsoft Word document file type</summary>
    public static readonly FileType Doc = new ("application/msword", ".doc");
    
    /// <summary>Microsoft Word Open XML document file type</summary>
    public static readonly FileType Docx = new ("application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx");
    
    /// <summary>Rich Text Format file type</summary>
    public static readonly FileType Rtf = new ("application/rtf", ".rtf");
    
    /// <summary>Microsoft Excel spreadsheet file type</summary>
    public static readonly FileType Xls = new ("application/vnd.ms-excel", ".xls");
    
    /// <summary>Microsoft Excel Open XML spreadsheet file type</summary>
    public static readonly FileType Xlsx = new ("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ".xlsx");
    
    /// <summary>JPEG image file type</summary>
    public static readonly FileType Jpeg = new ("image/jpeg", ".jpg");
    
    /// <summary>PNG image file type</summary>
    public static readonly FileType Png = new ("image/png", ".png");
    
    /// <summary>GIF image file type</summary>
    public static readonly FileType Gif = new ("image/gif", ".gif");
    
    /// <summary>MP4 video file type</summary>
    public static readonly FileType Mp4 = new ("video/mp4", ".mp4");
    
    /// <summary>MP4 video file type (ES variant)</summary>
    public static readonly FileType Mp4V = new ("video/mp4v-es", ".mp4v");
    
    /// <summary>Flash video file type</summary>
    public static readonly FileType Flv = new ("video/x-flv", ".flv");
    
    /// <summary>BMP image file type</summary>
    public static readonly FileType Bmp = new ("image/bmp", ".bmp");

    /// <summary>
    /// Gets the MIME type of the file
    /// </summary>
    public string Mime { get; }
    
    /// <summary>
    /// Gets the file extension
    /// </summary>
    public string Extension { get; }

    private FileType(string mime, string extension)
    {
        Mime = mime;
        Extension = extension;
    }

    /// <summary>
    /// Creates a FileType from a file type signature
    /// </summary>
    /// <param name="signature">The file type signature to create from</param>
    /// <returns>A new FileType instance</returns>
    public static FileType FromSignature(FileTypeSignature signature)
    {
        return new FileType(signature.MimeType, signature.Extension.ToLower());
    }
}
