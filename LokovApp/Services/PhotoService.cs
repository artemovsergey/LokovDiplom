using LokovApp.Data;
using LokovApp.Dtos;
using LokovApp.DTOs;
using LokovApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LokovApp.Services;

public interface IPhotoService
{
    Task<List<PhotoResponseDto>> UploadPhotosAsync(
        Guid projectId,
        UploadMultiplePhotosDto dto,
        Guid userId
    );
    Task<PhotoResponseDto> UploadPhotoAsync(Guid projectId, UploadPhotoDto dto, Guid userId);
    Task<PagedResponse<PhotoResponseDto>> GetProjectPhotosAsync(
        Guid projectId,
        PhotoFilterDto filter
    );
    Task<PhotoResponseDto?> GetPhotoByIdAsync(Guid photoId);
    Task<PhotoResponseDto?> UpdatePhotoAsync(Guid photoId, UpdatePhotoDto dto);
    Task<bool> DeletePhotoAsync(Guid photoId);
    Task<(byte[] FileData, string ContentType, string FileName)> DownloadPhotoAsync(Guid photoId);
    Task<(byte[] FileData, string ContentType)> GetThumbnailAsync(Guid photoId);
    Task<bool> DeleteMultiplePhotosAsync(List<Guid> photoIds);
    Task<bool> UpdatePhotosOrderAsync(List<Guid> photoIds);
}

public class PhotoService : IPhotoService
{
    private readonly LokovAppContext _context;
    private readonly IFileStorageService _fileStorage;

    // Разрешенные типы файлов
    private static readonly string[] AllowedExtensions =
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".gif",
        ".bmp",
        ".webp",
    };
    private static readonly string[] AllowedMimeTypes =
    {
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/bmp",
        "image/webp",
    };
    private const long MaxFileSize = 20 * 1024 * 1024; // 20 MB

    public PhotoService(LokovAppContext context, IFileStorageService fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<PhotoResponseDto> UploadPhotoAsync(
        Guid projectId,
        UploadPhotoDto dto,
        Guid userId
    )
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null)
            throw new InvalidOperationException("Проект не найден");

        ValidateFile(dto.File);

        var subfolder = $"projects/{projectId}";
        var filePath = await _fileStorage.SaveFileAsync(dto.File, subfolder);

        string thumbnailPath;
        try
        {
            thumbnailPath = await _fileStorage.CreateThumbnailAsync(filePath, dto.File.FileName);
        }
        catch
        {
            thumbnailPath = filePath;
        }

        var category = Enum.TryParse<PhotoCategory>(dto.Category, true, out var parsedCategory)
            ? parsedCategory
            : PhotoCategory.WorkProgress;

        if (dto.StageId.HasValue)
        {
            var stage = await _context.ProjectStages.FirstOrDefaultAsync(s =>
                s.Id == dto.StageId.Value && s.ProjectId == projectId
            );
            if (stage == null)
                dto.StageId = null;
        }

        var photo = new ProjectPhoto
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            OriginalFileName = dto.File.FileName,
            StoredFileName = Path.GetFileName(filePath),
            FilePath = filePath,
            ContentType = dto.File.ContentType,
            FileSize = dto.File.Length,
            Category = category,
            SortOrder = dto.SortOrder ?? 0,
            Description = dto.Description,
            StageId = dto.StageId,
            ThumbnailFileName = thumbnailPath != filePath ? Path.GetFileName(thumbnailPath) : null,
            ThumbnailPath = thumbnailPath != filePath ? thumbnailPath : null,
            UploadedById = null, // Не привязываем к пользователю
            CreatedAt = DateTime.UtcNow,
        };

        _context.ProjectPhotos.Add(photo);
        await _context.SaveChangesAsync();

        return MapToResponseDto(photo);
    }

    public async Task<List<PhotoResponseDto>> UploadPhotosAsync(
        Guid projectId,
        UploadMultiplePhotosDto dto,
        Guid userId
    )
    {
        var results = new List<PhotoResponseDto>();

        foreach (var file in dto.Files)
        {
            try
            {
                var uploadDto = new UploadPhotoDto
                {
                    File = file,
                    Category = dto.Category,
                    Description = dto.Description,
                    StageId = dto.StageId,
                };

                var photo = await UploadPhotoAsync(projectId, uploadDto, userId);
                results.Add(photo);
            }
            catch (Exception ex)
            {
                // Логируем ошибку для отдельного файла, но продолжаем загрузку остальных
                System.Diagnostics.Debug.WriteLine(
                    $"Error uploading file {file.FileName}: {ex.Message}"
                );
            }
        }

        return results;
    }

    public async Task<PagedResponse<PhotoResponseDto>> GetProjectPhotosAsync(
        Guid projectId,
        PhotoFilterDto filter
    )
    {
        var query = _context
            .ProjectPhotos.Include(p => p.UploadedBy)
            .Include(p => p.Stage)
            .Where(p => p.ProjectId == projectId);

        // Фильтр по категории
        if (
            !string.IsNullOrWhiteSpace(filter.Category)
            && Enum.TryParse<PhotoCategory>(filter.Category, true, out var category)
        )
        {
            query = query.Where(p => p.Category == category);
        }

        // Фильтр по этапу
        if (filter.StageId.HasValue)
        {
            query = query.Where(p => p.StageId == filter.StageId.Value);
        }

        // Фильтр по дате загрузки
        if (filter.UploadedFrom.HasValue)
            query = query.Where(p => p.CreatedAt >= filter.UploadedFrom.Value);
        if (filter.UploadedTo.HasValue)
            query = query.Where(p => p.CreatedAt <= filter.UploadedTo.Value);

        // Сортировка
        query = filter.SortBy?.ToLower() switch
        {
            "date" => filter.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(p => p.CreatedAt)
                : query.OrderBy(p => p.CreatedAt),
            "name" => filter.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(p => p.OriginalFileName)
                : query.OrderBy(p => p.OriginalFileName),
            "size" => filter.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(p => p.FileSize)
                : query.OrderBy(p => p.FileSize),
            _ => query.OrderBy(p => p.SortOrder).ThenByDescending(p => p.CreatedAt),
        };

        var totalCount = await query.CountAsync();

        var photos = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var items = photos.Select(MapToResponseDto).ToList();

        return new PagedResponse<PhotoResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize,
        };
    }

    public async Task<PhotoResponseDto?> GetPhotoByIdAsync(Guid photoId)
    {
        var photo = await _context
            .ProjectPhotos.Include(p => p.Project)
            .Include(p => p.UploadedBy)
            .Include(p => p.Stage)
            .FirstOrDefaultAsync(p => p.Id == photoId);

        return photo == null ? null : MapToResponseDto(photo);
    }

    public async Task<PhotoResponseDto?> UpdatePhotoAsync(Guid photoId, UpdatePhotoDto dto)
    {
        var photo = await _context
            .ProjectPhotos.Include(p => p.Project)
            .Include(p => p.UploadedBy)
            .Include(p => p.Stage)
            .FirstOrDefaultAsync(p => p.Id == photoId);

        if (photo == null)
            return null;

        if (dto.Description != null)
            photo.Description = dto.Description;

        if (
            !string.IsNullOrWhiteSpace(dto.Category)
            && Enum.TryParse<PhotoCategory>(dto.Category, true, out var category)
        )
        {
            photo.Category = category;
        }

        if (dto.SortOrder.HasValue)
            photo.SortOrder = dto.SortOrder.Value;

        if (dto.StageId.HasValue)
        {
            var stage = await _context.ProjectStages.FirstOrDefaultAsync(s =>
                s.Id == dto.StageId.Value && s.ProjectId == photo.ProjectId
            );
            if (stage != null)
                photo.StageId = dto.StageId;
        }

        await _context.SaveChangesAsync();
        return MapToResponseDto(photo);
    }

    public async Task<bool> DeletePhotoAsync(Guid photoId)
    {
        var photo = await _context.ProjectPhotos.FindAsync(photoId);
        if (photo == null)
            return false;

        // Удаляем файлы
        await _fileStorage.DeleteFileAsync(photo.FilePath);
        if (photo.ThumbnailPath != null)
            await _fileStorage.DeleteFileAsync(photo.ThumbnailPath);

        // Удаляем запись из БД
        _context.ProjectPhotos.Remove(photo);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteMultiplePhotosAsync(List<Guid> photoIds)
    {
        var photos = await _context.ProjectPhotos.Where(p => photoIds.Contains(p.Id)).ToListAsync();

        foreach (var photo in photos)
        {
            await _fileStorage.DeleteFileAsync(photo.FilePath);
            if (photo.ThumbnailPath != null)
                await _fileStorage.DeleteFileAsync(photo.ThumbnailPath);
        }

        _context.ProjectPhotos.RemoveRange(photos);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdatePhotosOrderAsync(List<Guid> photoIds)
    {
        var photos = await _context.ProjectPhotos.Where(p => photoIds.Contains(p.Id)).ToListAsync();

        for (int i = 0; i < photoIds.Count; i++)
        {
            var photo = photos.FirstOrDefault(p => p.Id == photoIds[i]);
            if (photo != null)
                photo.SortOrder = i;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<(byte[] FileData, string ContentType, string FileName)> DownloadPhotoAsync(
        Guid photoId
    )
    {
        var photo = await _context.ProjectPhotos.FindAsync(photoId);
        if (photo == null)
            throw new FileNotFoundException("Фотография не найдена");

        var fileData = await _fileStorage.GetFileAsync(photo.FilePath);
        return (fileData, photo.ContentType, photo.OriginalFileName);
    }

    public async Task<(byte[] FileData, string ContentType)> GetThumbnailAsync(Guid photoId)
    {
        var photo = await _context.ProjectPhotos.FindAsync(photoId);
        if (photo == null)
            throw new FileNotFoundException("Фотография не найдена");

        var thumbnailPath = photo.ThumbnailPath ?? photo.FilePath;
        var fileData = await _fileStorage.GetFileAsync(thumbnailPath);

        return (fileData, "image/jpeg");
    }

    private void ValidateFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Файл не выбран или пустой");

        if (file.Length > MaxFileSize)
            throw new ArgumentException($"Размер файла превышает {MaxFileSize / 1024 / 1024} MB");

        var extension = Path.GetExtension(file.FileName).ToLower();
        if (!AllowedExtensions.Contains(extension))
            throw new ArgumentException(
                $"Недопустимое расширение файла. Разрешены: {string.Join(", ", AllowedExtensions)}"
            );

        if (!AllowedMimeTypes.Contains(file.ContentType))
            throw new ArgumentException(
                $"Недопустимый тип файла. Разрешены: {string.Join(", ", AllowedMimeTypes)}"
            );
    }

    private static PhotoResponseDto MapToResponseDto(ProjectPhoto photo)
    {
        var categoryDisplay = photo.Category switch
        {
            PhotoCategory.Before => "До начала работ",
            PhotoCategory.WorkProgress => "Процесс работы",
            PhotoCategory.After => "После завершения",
            PhotoCategory.Defect => "Дефекты",
            PhotoCategory.Materials => "Материалы",
            PhotoCategory.Documentation => "Документация",
            PhotoCategory.Other => "Другое",
            _ => photo.Category.ToString(),
        };

        return new PhotoResponseDto
        {
            Id = photo.Id,
            ProjectId = photo.ProjectId,
            ProjectNumber = photo.Project?.Number ?? "",
            OriginalFileName = photo.OriginalFileName,
            ContentType = photo.ContentType,
            FileSize = photo.FileSize,
            Category = photo.Category.ToString(),
            CategoryDisplay = categoryDisplay,
            SortOrder = photo.SortOrder,
            Description = photo.Description,
            StageId = photo.StageId,
            StageName = photo.Stage?.Name,
            Url = $"/api/v1/projects/{photo.ProjectId}/photos/{photo.Id}/file",
            ThumbnailUrl =
                photo.ThumbnailPath != null
                    ? $"/api/v1/projects/{photo.ProjectId}/photos/{photo.Id}/thumbnail"
                    : $"/api/v1/projects/{photo.ProjectId}/photos/{photo.Id}/file",
            CreatedAt = photo.CreatedAt,
            UploadedByName = photo.UploadedBy?.FullName ?? "Система", // Значение по умолчанию
            Latitude = photo.Latitude,
            Longitude = photo.Longitude,
            TakenAt = photo.TakenAt,
        };
    }
}
