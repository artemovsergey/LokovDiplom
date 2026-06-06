using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace LokovApp.Services;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile file, string subfolder);
    Task<string> CreateThumbnailAsync(string filePath, string fileName);
    Task<byte[]> GetFileAsync(string filePath);
    Task<bool> DeleteFileAsync(string filePath);
    string GetUploadsPath();
    string GetRelativePath(string absolutePath);
    string GetAbsolutePath(string relativePath);
}

public class FileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public FileStorageService(IWebHostEnvironment environment, IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
    }

    public string GetUploadsPath()
    {
        var uploadsPath = _configuration["FileStorage:UploadsPath"] ?? "uploads";
        var fullPath = Path.Combine(_environment.ContentRootPath, uploadsPath);

        if (!Directory.Exists(fullPath))
            Directory.CreateDirectory(fullPath);

        return fullPath;
    }

    public async Task<string> SaveFileAsync(IFormFile file, string subfolder)
    {
        var uploadsPath = GetUploadsPath();
        var folderPath = Path.Combine(uploadsPath, subfolder);

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        // Генерируем уникальное имя файла
        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        var uniqueFileName = $"{Guid.NewGuid():N}{fileExtension}";
        var fullPath = Path.Combine(folderPath, uniqueFileName);

        // Сохраняем файл
        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return GetRelativePath(fullPath);
    }

    public async Task<string> CreateThumbnailAsync(string relativePath, string originalFileName)
    {
        var absolutePath = GetAbsolutePath(relativePath);

        if (!File.Exists(absolutePath))
            throw new FileNotFoundException("Исходный файл не найден");

        var uploadsPath = GetUploadsPath();
        var thumbnailsFolder = Path.Combine(uploadsPath, "thumbnails");

        if (!Directory.Exists(thumbnailsFolder))
            Directory.CreateDirectory(thumbnailsFolder);

        var fileExtension = Path.GetExtension(originalFileName).ToLower();
        var thumbnailFileName = $"thumb_{Guid.NewGuid():N}{fileExtension}";
        var thumbnailPath = Path.Combine(thumbnailsFolder, thumbnailFileName);

        try
        {
            using var image = await Image.LoadAsync(absolutePath);

            // Максимальный размер превью
            var maxWidth = 400;
            var maxHeight = 400;

            image.Mutate(x =>
                x.Resize(
                    new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(maxWidth, maxHeight),
                    }
                )
            );

            await image.SaveAsync(thumbnailPath);

            return GetRelativePath(thumbnailPath);
        }
        catch
        {
            // Если не удалось создать превью (например, не изображение)
            return relativePath;
        }
    }

    public async Task<byte[]> GetFileAsync(string relativePath)
    {
        var absolutePath = GetAbsolutePath(relativePath);

        if (!File.Exists(absolutePath))
            throw new FileNotFoundException("Файл не найден");

        return await File.ReadAllBytesAsync(absolutePath);
    }

    public async Task<bool> DeleteFileAsync(string relativePath)
    {
        var absolutePath = GetAbsolutePath(relativePath);

        if (!File.Exists(absolutePath))
            return false;

        await Task.Run(() => File.Delete(absolutePath));
        return true;
    }

    public string GetRelativePath(string absolutePath)
    {
        var uploadsPath = GetUploadsPath();
        return Path.GetRelativePath(uploadsPath, absolutePath).Replace("\\", "/");
    }

    public string GetAbsolutePath(string relativePath)
    {
        var uploadsPath = GetUploadsPath();
        return Path.Combine(uploadsPath, relativePath);
    }
}
