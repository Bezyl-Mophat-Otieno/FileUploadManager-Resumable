using Microsoft.AspNetCore.Mvc;

namespace FileUploadManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController(IWebHostEnvironment env): ControllerBase
{

    [HttpPost]
    [RequestSizeLimit(100_000_000)]
    public async Task<ActionResult> UploadFile(IFormFile file)
    {
        if (file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }
        
        var uploadPath = Path.Combine(env.ContentRootPath,"wwwroot", "uploads");
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        var fileName = file.FileName;
        var newFileName = $"{Guid.NewGuid()}_{fileName}";
        
        await  using var stream = new FileStream(Path.Combine(uploadPath, newFileName), FileMode.Create);

        await file.CopyToAsync(stream);

        var baseUrl = $"{Request.Scheme}//{Request.Host}";
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

}