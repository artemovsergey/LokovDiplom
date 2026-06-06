using LokovApp.Dtos;
using LokovApp.DTOs;
using LokovApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LokovApp.Controllers;

[ApiController]
[Route("api/v1/projects/{projectId}/[controller]")]
// НЕ наследуемся от BaseApiController, чтобы не использовать GetCurrentUserId()
public class PhotosController : ControllerBase
{
    private readonly IPhotoService _photoService;

    // ID администратора по умолчанию
    private static readonly Guid DefaultUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public PhotosController(IPhotoService photoService)
    {
        _photoService = photoService;
    }

    [HttpPost("upload")]
    [RequestSizeLimit(21_000_000)]
    public async Task<ActionResult<PhotoResponseDto>> UploadPhoto(
        Guid projectId,
        [FromForm] UploadPhotoDto dto
    )
    {
        try
        {
            var photo = await _photoService.UploadPhotoAsync(projectId, dto, DefaultUserId);
            return CreatedAtAction(nameof(GetPhoto), new { projectId, photoId = photo.Id }, photo);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new { success = false, message = $"Ошибка при загрузке фото: {ex.Message}" }
            );
        }
    }

    [HttpPost("upload-multiple")]
    [RequestSizeLimit(105_000_000)]
    public async Task<ActionResult<List<PhotoResponseDto>>> UploadMultiplePhotos(
        Guid projectId,
        [FromForm] UploadMultiplePhotosDto dto
    )
    {
        try
        {
            var photos = await _photoService.UploadPhotosAsync(projectId, dto, DefaultUserId);
            return Ok(photos);
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new { success = false, message = $"Ошибка при загрузке фото: {ex.Message}" }
            );
        }
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<PhotoResponseDto>>> GetPhotos(
        Guid projectId,
        [FromQuery] PhotoFilterDto filter
    )
    {
        try
        {
            var photos = await _photoService.GetProjectPhotosAsync(projectId, filter);
            return Ok(photos);
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new { success = false, message = $"Ошибка при получении фото: {ex.Message}" }
            );
        }
    }

    [HttpGet("{photoId}")]
    public async Task<ActionResult<PhotoResponseDto>> GetPhoto(Guid projectId, Guid photoId)
    {
        var photo = await _photoService.GetPhotoByIdAsync(photoId);
        if (photo == null)
            return NotFound(new { success = false, message = "Фотография не найдена" });
        return Ok(photo);
    }

    [HttpGet("{photoId}/file")]
    public async Task<IActionResult> DownloadPhoto(Guid projectId, Guid photoId)
    {
        try
        {
            var (fileData, contentType, fileName) = await _photoService.DownloadPhotoAsync(photoId);
            return File(fileData, contentType, fileName);
        }
        catch (FileNotFoundException)
        {
            return NotFound(new { success = false, message = "Фотография не найдена" });
        }
    }

    [HttpGet("{photoId}/thumbnail")]
    public async Task<IActionResult> GetThumbnail(Guid projectId, Guid photoId)
    {
        try
        {
            var (fileData, contentType) = await _photoService.GetThumbnailAsync(photoId);
            return File(fileData, contentType);
        }
        catch (FileNotFoundException)
        {
            return NotFound(new { success = false, message = "Фотография не найдена" });
        }
    }

    [HttpPut("{photoId}")]
    public async Task<ActionResult<PhotoResponseDto>> UpdatePhoto(
        Guid projectId,
        Guid photoId,
        [FromBody] UpdatePhotoDto dto
    )
    {
        var photo = await _photoService.UpdatePhotoAsync(photoId, dto);
        if (photo == null)
            return NotFound(new { success = false, message = "Фотография не найдена" });
        return Ok(photo);
    }

    [HttpDelete("{photoId}")]
    public async Task<ActionResult> DeletePhoto(Guid projectId, Guid photoId)
    {
        var result = await _photoService.DeletePhotoAsync(photoId);
        if (!result)
            return NotFound(new { success = false, message = "Фотография не найдена" });
        return Ok(new { success = true, message = "Фотография удалена" });
    }

    [HttpDelete("batch")]
    public async Task<ActionResult> DeleteMultiplePhotos(
        Guid projectId,
        [FromBody] List<Guid> photoIds
    )
    {
        await _photoService.DeleteMultiplePhotosAsync(photoIds);
        return Ok(new { success = true, message = $"Удалено фотографий: {photoIds.Count}" });
    }

    [HttpPut("order")]
    public async Task<ActionResult> UpdatePhotosOrder(
        Guid projectId,
        [FromBody] List<Guid> photoIds
    )
    {
        await _photoService.UpdatePhotosOrderAsync(photoIds);
        return Ok(new { success = true, message = "Порядок фотографий обновлен" });
    }
}
