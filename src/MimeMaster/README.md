# MimeMaster

MimeMaster is a .NET library for detecting and validating file types based on file signatures (magic numbers). It provides accurate file type detection regardless of file extensions by examining file headers and trailers.

## Usage

```csharp
// Create services with dependency injection
services.AddMimeMaster();

// Or create services manually
var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<FileTypeService>();
var fileTypeService = new FileTypeService(logger);

// Detect MIME type
string filePath = "path/to/your/file.pdf";
byte[] fileData = File.ReadAllBytes(filePath);
var fileType = fileTypeService.GetMimeType(filePath, fileData);

Console.WriteLine($"MIME type: {fileType.Mime}");
Console.WriteLine($"Extension: {fileType.Extension}");
```

See the main README.md at the root of the repository for more detailed documentation.
