using System.Text.Json;
using FileUploadManager.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FileUploadManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController: ControllerBase
{
    private readonly string _uploadPath;
    public FileUploadController(IWebHostEnvironment env)
    {
        _uploadPath = Path.Combine(env.ContentRootPath, "wwwroot", "uploads");
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }


    }

    [HttpPost("default")]
    [RequestSizeLimit(100_000_000)]
    public async Task<ActionResult> UploadFile(IFormFile file)
    {
        if (file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }
        
        var fileName = file.FileName;
        var newFileName = $"{Guid.NewGuid()}_{fileName}";
        
        await  using var stream = new FileStream(Path.Combine(_uploadPath, newFileName), FileMode.Create);

        await file.CopyToAsync(stream);

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var fileUrl = $"{baseUrl}/uploads/{newFileName}";

        return Ok(new
        {
            fileName = file.FileName,
            savedAs = newFileName,
            contentType = file.ContentType,
            sizeInKBB = Math.Round((file.Length / 1024.0), 2),
            uploadTime = DateTime.UtcNow,
            publicUrl = fileUrl
            
        });


    }

    [HttpPost("chunk-upload/init")]
    public IActionResult InitializeChunkUpload([FromBody] InitiUploadRequest request)
    {
        var uploadId = Guid.NewGuid().ToString();
        var chunkUploadPath = Path.Combine(_uploadPath, "temp" ,uploadId);
        if (!Directory.Exists(chunkUploadPath))
        {
            Directory.CreateDirectory(chunkUploadPath);
        }

        if (string.IsNullOrWhiteSpace(request.FileName))
            return Ok(new
            {
                uploadId,
                message = "Upload session initialized successfully."
            });
        var metadataPath = Path.Combine(chunkUploadPath, "metadata.json");
        var metadata = JsonSerializer.Serialize(new
        {
            fileName = request.FileName,
            chunkSize = request.TotalChunks,
            createdAt = DateTime.Now,

        });
        System.IO.File.WriteAllText( metadataPath,metadata);

        return Ok(new
        {
            uploadId,
            message = "Upload session initialized successfully."
        });

    }

}