using System.Text.Json;
using FileUploadManager.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FileUploadManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController: ControllerBase
{
    private readonly string _uploadPath;
    private const string  TotalChunksKey = "TotalChunks";
    public FileUploadController(IWebHostEnvironment env)
    {
        _uploadPath = Path.Combine(env.ContentRootPath, "wwwroot", "uploads");
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }


    }

    [HttpPost("upload")]
    [RequestSizeLimit(100_000_000)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult> UploadFile(IFormFile file)
    {
        if (file.Length == 0)
        {
            return BadRequest(new ApiResponse(false, "No file uploaded."));
        }
        
        var fileName = file.FileName;
        var newFileName = $"{Guid.NewGuid()}_{fileName}";
        
        await  using var stream = new FileStream(Path.Combine(_uploadPath, newFileName), FileMode.Create);

        await file.CopyToAsync(stream);

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var fileUrl = $"{baseUrl}/uploads/{newFileName}";

        return Ok(new ApiResponse(true, "File uploaded successfully", new
        {
            fileName = newFileName,
            sizeInMB = Math.Round(file.Length / (1024.0 * 1024.0), 2),
            fileUrl,
            uploadedAt = DateTime.UtcNow
        }));


    }

    [HttpPost("upload/initiate")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
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
            request.FileName,
            request.TotalChunks,
            createdAt = DateTime.Now

        });
        System.IO.File.WriteAllText( metadataPath,metadata);

        return Ok(new ApiResponse(true, "Upload session initialized successfully.", new
        {
            uploadId
        }));

    }

    [HttpPost("upload/chunk")]
    [RequestSizeLimit(100_000_000)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ChunkUpload([FromQuery] int chunkIndex, [FromQuery] string uploadId, IFormFile  chunk )
    {
        if (string.IsNullOrWhiteSpace(uploadId)) return BadRequest(new ApiResponse(false, "Missing uploadId"));
        if (chunk.Length == 0) return BadRequest(new ApiResponse(false, "Chunk file is empty."));
        
        var chunkDir = Path.Combine(_uploadPath, "temp", uploadId);
        if (!Directory.Exists(chunkDir))
            return NotFound("Upload session not found. Initialize first.");

        var chunkFileName = $"chunk_{chunkIndex}";
        
         await using var stream  = new FileStream(Path.Combine(chunkDir, chunkFileName),  FileMode.Create);
         await chunk.CopyToAsync(stream);

         var metadata = GetFileMetadata(chunkDir, uploadId);
         if (metadata == null) return BadRequest();
         var totalChunks = metadata[TotalChunksKey];
         var uploadedChunks = Directory.GetFiles(chunkDir, "chunk_*").Length;
        
         return Ok(new ApiResponse(true, "Chunk uploaded successfully.", new
         {
             chunkIndex,
             uploadedChunks,
             totalChunks,
             isComplete = uploadedChunks == totalChunks
         }));    
    }

    [HttpPost("upload/complete")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CompleteMergeUpload([FromQuery] string uploadId)
    {
        if (string.IsNullOrWhiteSpace(uploadId)) return BadRequest("Missing uploadId");
        var chunkUploadPath = Path.Combine(_uploadPath, "temp" , uploadId);
        if (!Directory.Exists(chunkUploadPath))
            return BadRequest(new ApiResponse(false, "Upload session not found. Initialize first."));

        var metadata = GetFileMetadata(chunkUploadPath, uploadId);
        if (metadata == null) return BadRequest(new ApiResponse(false, "Upload metadata is required for merging chunks."));

        var fileName = metadata["FileName"];
        var totalChunks = metadata[TotalChunksKey];

        // Sort all chunks in correct order
        var uploadedChunks = Directory.GetFiles(chunkUploadPath, "chunk_*")
            .OrderBy(f => int.Parse(Path.GetFileName(f).Split('_')[1]))
            .ToList();
        
        if (uploadedChunks.Count != totalChunks) return BadRequest($"Upload not complete. Total required chunks {totalChunks} but got {uploadedChunks.Count}");

        var finalFilePath = Path.Combine(_uploadPath, fileName);
        
        await using var fileStream = new FileStream(finalFilePath, FileMode.Create);
        foreach (var chunkFilePath in uploadedChunks)
        {
            await using var chunk = new FileStream(chunkFilePath, FileMode.Open);
            await chunk.CopyToAsync(fileStream);
        }
        
        Directory.Delete(chunkUploadPath, true);
        
        var fileInfo = new FileInfo(finalFilePath);
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var fileUrl = $"{baseUrl}/uploads/{fileName}";

        return Ok(new ApiResponse(true, "File merged successfully.", new
        {
            fileName,
            sizeInMB = Math.Round(fileInfo.Length / (1024.0 * 1024.0), 2),
            fileUrl,
            uploadedAt = DateTime.UtcNow
        }));    
    }
    

    private static Dictionary<string, dynamic>?  GetFileMetadata(string chunkDir, string uploadId)
    {
        var result = new Dictionary<string, dynamic>();
        var metadataPath = Path.Combine(chunkDir, "metadata.json");
        if (!System.IO.File.Exists(metadataPath)) return null;
        var metadata = JsonDocument.Parse(System.IO.File.ReadAllText(metadataPath));
        var  chunks =  metadata.RootElement.TryGetProperty(TotalChunksKey, out var totalChunks)
            ? totalChunks.GetInt32()
            : 0;
        var name = metadata.RootElement.TryGetProperty("FileName", out var fileName)
            ? fileName.ToString()
            : $"merged{uploadId}";
        result.Add("FileName", name);
        result.Add(TotalChunksKey, chunks);
        return result ;
    }
    

}