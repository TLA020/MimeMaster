using System.Text;
using MimeMaster.Models;


// https://sceweb.sce.uhcl.edu/abeysekera/itec3831/labs/FILE%20SIGNATURES%20TABLE.pdf
// https://en.wikipedia.org/wiki/List_of_file_signatures

namespace MimeMaster.Models;

/// <summary>
/// Provides functionality to identify file types based on their binary signatures
/// </summary>
public static class FileSignatures
{
    private static readonly List<FileTypeSignature> _fileSignatures =
    [
        new FileTypeSignature
        {
            Extension = ".PNG",
            MimeType = "image/png",
            Headers =
            [
                new Signature([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A])
            ],
            Trailers =
            [
                new Signature([0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82])
            ]
        },

        new FileTypeSignature
        {
            Extension = ".JPG",
            MimeType = "image/jpeg",
            Headers =
            [
                new Signature([0xFF, 0xD8, 0xFF, 0xE0]),
                new Signature([0xFF, 0xD8, 0xFF, 0xE1]),
                new Signature([0xFF, 0xD8, 0xFF, 0xDB]),
                new Signature([0xFF, 0xD8, 0xFF, 0xEE]),
                new Signature([0xFF, 0xD8, 0xFF, 0xE2]),
                new Signature([0xFF, 0xD8, 0xFF, 0xE3])
            ],
            Trailers =
            [
                new Signature([0xFF, 0xD9])
            ]
        },

        new FileTypeSignature
        {
            Extension = ".JPEG",
            MimeType = "image/jpeg",
            Headers =
            [
                new Signature([0xFF, 0xD8, 0xFF, 0xE0]),
                new Signature([0xFF, 0xD8, 0xFF, 0xE2]),
                new Signature([0xFF, 0xD8, 0xFF, 0xE3])
            ],
            Trailers =
            [
                new Signature([0xFF, 0xD9])
            ]
        },


        new FileTypeSignature
        {
            Extension = ".DOC",
            MimeType = "application/msword",
            Headers = [new Signature([0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1])]
        },


        new FileTypeSignature
        {
            Extension = ".PDF",
            MimeType = "application/pdf",
            Headers =
            [
                new Signature([0x25, 0x50, 0x44, 0x46])
            ],
            Trailers =
            [
                new Signature([0x0A, 0x25, 0x25, 0x45, 0x4F, 0x46]),
                new Signature([0x0A, 0x25, 0x25, 0x45, 0x4F, 0x46, 0x0A]),
                new Signature([0x0D, 0x0A, 0x25, 0x25, 0x45, 0x4F, 0x46, 0x0D, 0x0A]),
                new Signature([0x0D, 0x25, 0x25, 0x45, 0x4F, 0x46, 0x0D])
            ]
        },


        new FileTypeSignature
        {
            Extension = ".XLS",
            MimeType = "application/vnd.ms-excel",
            Headers =
            [
                new Signature([0x09, 0x08, 0x10, 0x00, 0x00, 0x06, 0x05, 0x00], 512),
                new Signature([0xFD, 0xFF, 0xFF, 0xFF], 512)
            ]
        },


        new FileTypeSignature
        {
            Extension = ".XLSX",
            MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            Headers =
            [
                new Signature([0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00])
            ],
            Trailers =
            [
                new Signature([0x50, 0x4B, 0x05, 0x06]),
                new Signature([0x50, 0x4B, 0x07, 0x08])
            ],
            ContentSniffTarget = "xl/workbook.xml"
        },


        new FileTypeSignature
        {
            Extension = ".BMP",
            MimeType = "image/bmp",
            Headers =
            [
                new Signature([0x42, 0x4D])
            ]
        },

        new FileTypeSignature
        {
            Extension = ".MP4",
            MimeType = "video/mp4",
            Headers =
            [
                new Signature([0x00, 0x00, 0x00, 0x20, 0x66, 0x74, 0x79, 0x70, 0x69, 0x73, 0x6F, 0x6D])
            ]
        },

        new FileTypeSignature
        {
            Extension = ".DOCX",
            MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            Headers =
            [
                new Signature([0xEC, 0xA5, 0xC1, 0x00], 512)
            ],
            Trailers =
            [
                new Signature([0x50, 0x4B, 0x05, 0x06, 0x18],
                    22),
                new Signature([0x50, 0x4B, 0x07, 0x08, 0x18],
                    22)
            ],
            ContentSniffTarget = "word/document.xml"
        },

        new FileTypeSignature
        {
            Extension = ".PPT",
            MimeType = "application/mspowerpoint",
            Headers =
            [
                new Signature([0xA0, 0x46, 0x1D, 0xF0], 512),
                new Signature([0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1])
            ],
            ContentSniffTarget = "ppt/presentation.xml"
        },

        new FileTypeSignature
        {
            Extension = ".PPTX",
            MimeType = "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            Headers =
            [
                new Signature([0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00])
            ],
            Trailers =
            [
                new Signature([0x50, 0x4B, 0x05, 0x06, 0x18]),
                new Signature([0x50, 0x4B, 0x07, 0x08, 0x18])
            ]
        }
    ];

    /// <summary>
    /// Gets all file signatures supported by the library
    /// </summary>
    /// <returns>A list of all file type signatures</returns>
    public static List<FileTypeSignature> GetFileSignatures()
    {
        return _fileSignatures;
    }

    /// <summary>
    /// Identifies the file type based on its name and content by examining file signatures
    /// </summary>
    /// <param name="fileName">The name of the file, used to determine the expected file extension</param>
    /// <param name="data">The binary content of the file to examine</param>
    /// <returns>A <see cref="FileTypeSignature"/> if the file type is recognized; otherwise, null</returns>
    public static FileTypeSignature? GeFileTypeSignature(string fileName, byte[] data)
    {
        var extension = Path.GetExtension(fileName)?.ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(extension))
        {
            return null;
        }

        var fileTypeSignature = _fileSignatures.FirstOrDefault(f => f.Extension.ToUpperInvariant() == extension);

        if (fileTypeSignature == null)
        {
            return null;
        }

        foreach (var header in fileTypeSignature.Headers)
        {
            if (data.Length < header.Offset + header.Value.Length)
            {
                // data too short to contain header
                continue;
            }

            if (data.Skip(header.Offset).Take(header.Value.Length).SequenceEqual(header.Value))
            {
                // Check for trailer if present
                if (fileTypeSignature.Trailers == null)
                {
                    return fileTypeSignature;
                }

                foreach (var trailer in fileTypeSignature.Trailers)
                {
                    var trailerOffset = data.Length - trailer.Value.Length - trailer.Offset;
                    if (trailerOffset >= 0 && data.Skip(trailerOffset).Take(trailer.Value.Length).SequenceEqual(trailer.Value))
                    {
                        if (string.IsNullOrWhiteSpace(fileTypeSignature.ContentSniffTarget))
                        {
                            return fileTypeSignature;
                        }

                        var content = Encoding.UTF8.GetString(data);
                        return content.Contains(fileTypeSignature.ContentSniffTarget) ? fileTypeSignature : null;
                    }
                }
            }

            // Check for content target if present
            if (!string.IsNullOrEmpty(fileTypeSignature.ContentSniffTarget))
            {
                var content = Encoding.UTF8.GetString(data);
                if (content.Contains(fileTypeSignature.ContentSniffTarget))
                {
                    return fileTypeSignature;
                }
            }
        }

        return null;
    }
}
