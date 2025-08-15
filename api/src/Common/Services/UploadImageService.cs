using Api.Common.Constants;

namespace Api.Common.Services;

public interface IImageUploadService
{
    Task<string> UploadImageAsync(Folders.Uploads folderName, IFormFile file, CancellationToken cancellationToken);
    bool DeleteImage(Folders.Uploads folderName, string fileName);
}

public class ImageUploadService(IWebHostEnvironment environment) : IImageUploadService
{
    public async Task<string> UploadImageAsync(Folders.Uploads folderName, IFormFile file, CancellationToken cancellationToken)
    {
        long currentUnixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        string extension = Path.GetExtension(file.FileName);
        string fileName = $"{currentUnixTimestamp}{extension}";
        string uploadPath = Path.Join(environment.WebRootPath, folderName.ToString(), fileName);
 
        using FileStream fileStream = File.OpenWrite(uploadPath);
        await file.CopyToAsync(fileStream, cancellationToken);
        
        return fileName;
    }

    public bool DeleteImage(Folders.Uploads folderName, string fileName)
    {
        string uploadPath = Path.Join(environment.WebRootPath, folderName.ToString(), fileName);
        if (!File.Exists(uploadPath))
        {
            return false;
        }

        File.Delete(uploadPath);
        return true;
    }
}
