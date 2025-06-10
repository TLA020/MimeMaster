# 🧙‍♂️ MimeMaster

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![Build Status](https://img.shields.io/github/actions/workflow/status/TLA020/MimeMaster/build-and-test.yml?branch=master&label=build&logo=github&style=flat-square)](https://github.com/TLA020/MimeMaster/actions)
[![Tests](https://img.shields.io/github/actions/workflow/status/TLA020/MimeMaster/build-and-test.yml?branch=master&label=tests&logo=github&style=flat-square)](https://github.com/TLA020/MimeMaster/actions)
[![NuGet](https://img.shields.io/nuget/v/MimeMaster.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/MimeMaster)
[![Downloads](https://img.shields.io/nuget/dt/MimeMaster.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/MimeMaster)
[![Coverage Status](https://img.shields.io/codecov/c/github/TLA020/MimeMaster/master?logo=codecov&style=flat-square)](https://codecov.io/gh/TLA020/MimeMaster)

MimeMaster is a fast, accurate .NET library for detecting and validating file types based on file signatures (magic numbers). It provides reliable file type detection regardless of file extensions by examining file headers and trailers.

## ✨ Features

- 🔍 **Accurate MIME type detection** based on file signatures
- ✅ **File validation** against allowed MIME types and size constraints
- ⚡ **Async/await support** with cancellation tokens for high-performance applications
- 🌊 **Streaming validation** for large files without loading them entirely into memory
- 📦 **Support for common file formats**: PDF, Office documents, images, and more
- 🧩 **Extensible design** for adding custom file types
- 🔄 **Integration with .NET dependency injection**

## 📦 Installation

### Package Manager Console

```
Install-Package MimeMaster
```

### .NET CLI

```
dotnet add package MimeMaster
```

## 🚀 Quick Start

### Basic Usage

```csharp
// Create services manually
var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<FileTypeService>();
var fileTypeService = new FileTypeService(logger);

// Detect MIME type
string filePath = "path/to/your/file.pdf";
byte[] fileData = File.ReadAllBytes(filePath);
var fileType = fileTypeService.GetMimeType(filePath, fileData);

Console.WriteLine($"MIME type: {fileType.Mime}");
Console.WriteLine($"Extension: {fileType.Extension}");
```

### With Dependency Injection

```csharp
// In Startup.ConfigureServices or Program.cs
services.AddMimeMaster();

// In your controller or service
public class MyService
{
    private readonly IFileTypeService _fileTypeService;
    private readonly IValidationService _validationService;
    
    public MyService(IFileTypeService fileTypeService, IValidationService validationService)
    {
        _fileTypeService = fileTypeService;
        _validationService = validationService;
    }
    
    public void ProcessFile(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        
        // Detect MIME type
        var fileType = fileTypeService.GetMimeType(filePath, fileData);
        
        // Validate file
        string allowedMimeTypes = "application/pdf,image/jpeg,image/png";
        long maxFileSize = 5 * 1024 * 1024; // 5MB
        
        var validationResult = _validationService.Validate(
            filePath, 
            fileData, 
            allowedMimeTypes, 
            maxFileSize);
            
        if (validationResult.HasFailed)
        {
            foreach (var invalidFile in validationResult.InvalidFiles)
            {
                Console.WriteLine($"Validation failed: {invalidFile.Errors}");
            }
        }
    }
}
```

### Async/Streaming Examples

```csharp
// Async file type detection with byte array
var fileData = await File.ReadAllBytesAsync("path/to/file.pdf");
var fileType = await _fileTypeService.GetMimeTypeAsync("file.pdf", fileData);

// Async file type detection from stream
using var fileStream = File.OpenRead("path/to/file.pdf");
var fileType = await _fileTypeService.GetMimeTypeFromStreamAsync("file.pdf", fileStream);

// Async validation with byte array
var validationResult = await _validationService.ValidateAsync(
    "file.pdf", 
    fileData, 
    "application/pdf,image/jpeg", 
    maxFileSize: 10 * 1024 * 1024, // 10MB
    cancellationToken: cancellationToken);

// Async validation from stream
using var fileStream = File.OpenRead("path/to/file.pdf");
var validationResult = await _validationService.ValidateAsync(
    "file.pdf", 
    fileStream, 
    "application/pdf,image/jpeg", 
    maxFileSize: 10 * 1024 * 1024);

// Streaming validation for large files (optimized for memory efficiency)
using var fileStream = File.OpenRead("path/to/large-file.pdf");
var validationResult = await _validationService.ValidateStreamAsync(
    "large-file.pdf", 
    fileStream, 
    "application/pdf", 
    maxFileSize: 100 * 1024 * 1024, // 100MB
    cancellationToken: cancellationToken);
```

### Benefits of Streaming Validation

- **Memory Efficient**: `ValidateStreamAsync` only reads file headers/trailers, not the entire file
- **Early Exit**: For seekable streams, size validation happens before MIME type detection
- **Large File Support**: Handle files of any size without memory constraints
- **Cancellation Support**: All async methods support `CancellationToken` for responsive applications

## 📋 Supported File Types

MimeMaster currently supports the following file types:

- 📄 PDF (`.pdf`)
- 📝 Word Documents (`.doc`, `.docx`)
- 📊 Excel Spreadsheets (`.xls`, `.xlsx`)
- 📊 PowerPoint Presentations (`.ppt`, `.pptx`)
- 🖼️ Images (`.jpg`, `.jpeg`, `.png`, `.bmp`, `.gif`)
- 🎬 Video (`.mp4`, `.flv`)
- 📄 Text (`.txt`, `.rtf`)

## 🔍 File Signatures

File signatures, also known as "magic numbers" or "magic bytes," are specific patterns at the beginning (headers) or end (trailers) of a file that identify its type regardless of file extension. MimeMaster uses these signatures to accurately identify file types.

The implementation is inspired by file signature tables and format specifications.

### Common File Signatures

```
Format     Hex Signature                ASCII        Trailer                        Description
-------    -------------                -----        -------                        -----------
DOC/XLS    D0 CF 11 E0 A1 B1 1A E1     ÐÏ.à¡±.á     -                              MS Office OLE Compound File
PDF        25 50 44 46 2D               %PDF-        0A 25 25 45 4F 46 (.%%EOF)     PDF Document
                                                     0A 25 25 45 4F 46 0A (.%%EOF.)
                                                     0D 0A 25 25 45 4F 46 0D 0A     
                                                     0D 25 25 45 4F 46 0D (.%%EOF.)
PNG        89 50 4E 47 0D 0A 1A 0A      ‰PNG....     49 45 4E 44 AE 42 60 82        PNG Image
                                                     (IEND®B`‚...)
DOCX/PPTX  50 4B 03 04 14 00 06 00      PK......     50 4B 05 06 (PK..) + 18 bytes  Office Open XML (OOXML) Document
XLSX                                                 at the end of the file
JPG        FF D8                         ÿØ          FF D9                          JPEG Image
ZIP        50 4B 03 04                   PK..        -                              ZIP Archive
PST        21 42 44 4E                   !BDN        -                              MS Outlook Personal Folder
```

### MS Office Subheaders

Office binary formats (DOC, XLS, PPT) have specific subheaders at byte offset 512:

```
Format   Offset        Hex Signature                Description
------   ------        -------------                -----------
DOC      512           EC A5 C1 00                  Word document subheader
XLS      512           FD FF FF FF nn 00            Excel spreadsheet subheader
         or 512        FD FF FF FF nn 02            
         or 512        09 08 10 00 00 06 05 00      
PPT      512           A0 46 1D F0                  PowerPoint presentation subheader
         or 512        00 6E 1E F0
         or 512        0F 00 E8 03
         or 512        FD FF FF FF nn nn 00 00
```

### Microsoft Office OOXML Formats

For Microsoft Office Open XML formats (DOCX, PPTX, XLSX), there is a specific detection pattern:

- Header signature: `50 4B 03 04 14 00 06 00` ("PK......")
- Trailer signature: Look for `50 4B 05 06` ("PK..") followed by 18 additional bytes at the end of the file

Unlike the older Office formats (DOC, PPT, XLS), there is no subheader for OOXML files. OOXML files are essentially ZIP files, so you can rename them with a `.ZIP` extension and then extract them to examine their contents. Inside, you'll find the `[Content_Types].xml` file that describes the content types, along with specific XML files:

- DOCX: Look for `word/document.xml`
- XLSX: Look for `xl/workbook.xml`
- PPTX: Look for `ppt/presentation.xml`

When examining the `[Content_Types].xml` file, look for the `<Override PartName=` tag, where you will find `word`, `ppt`, or `xl`, respectively.

The library includes signatures for various file formats and can be extended to support additional formats as needed.

## 📚 API Reference

### IFileTypeService

```csharp
public interface IFileTypeService
{
    // Synchronous methods
    FileType GetMimeType(string fileName, byte[] fileData);
    
    // Asynchronous methods
    Task<FileType> GetMimeTypeAsync(string fileName, byte[] fileData, CancellationToken cancellationToken = default);
    Task<FileType> GetMimeTypeFromStreamAsync(string fileName, Stream stream, CancellationToken cancellationToken = default);
}
```

### IValidationService

```csharp
public interface IValidationService
{
    // Synchronous methods
    ValidationResult Validate(
        string fileName, 
        byte[] fileData, 
        string allowedMimeTypes, 
        long maxFileSize, 
        long minFileSize = 0);
        
    // Asynchronous methods
    Task<ValidationResult> ValidateAsync(
        string fileName, 
        byte[] fileData, 
        string allowedMimeTypes, 
        long maxFileSize, 
        long minFileSize = 0, 
        CancellationToken cancellationToken = default);
        
    Task<ValidationResult> ValidateAsync(
        string fileName, 
        Stream stream, 
        string allowedMimeTypes, 
        long maxFileSize, 
        long minFileSize = 0, 
        CancellationToken cancellationToken = default);
        
    Task<ValidationResult> ValidateStreamAsync(
        string fileName, 
        Stream stream, 
        string allowedMimeTypes, 
        long maxFileSize, 
        long minFileSize = 0, 
        CancellationToken cancellationToken = default);
}
```

### ValidationErrors

```csharp
[Flags]
public enum ValidationErrors
{
    None = 0,
    FileTypeNotAllowed = 1 << 0,
    FileTooLarge = 1 << 1,
    FileTooSmall = 1 << 2,
    FileTypeUnknown = 1 << 3
}
```

## 📊 Performance

MimeMaster is designed for high-performance applications:

- ⚡ **Fast detection**: Optimized signature matching algorithms
- 🧠 **Low memory overhead**: Minimal allocations during detection
- 🌊 **Streaming support**: Process large files without loading them entirely into memory
- 🔄 **Thread-safe**: Safe for concurrent use
- ⚡ **Async/await**: Non-blocking operations with cancellation support
- 🚀 **Early exit optimization**: Size validation happens before expensive MIME type detection

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Commit your changes: `git commit -m 'Add amazing feature'`
4. Push to the branch: `git push origin feature/amazing-feature`
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- File signatures information sourced from various format specifications and references
- Inspired by other open-source MIME type detection libraries

---

<p align="center">Made with ❤️ for the .NET community</p>
# MimeMaster
