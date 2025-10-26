namespace FileUploadManager.Dtos;

public record ApiResponse(bool Success, string Message, object? Data = null);

