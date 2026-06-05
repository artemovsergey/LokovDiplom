using LokovApp.DTOs;
using LokovApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LokovApp.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<ProjectResponseDto>>> GetProjects(
        [FromQuery] ProjectFilterDto filter
    )
    {
        var projects = await _projectService.GetProjectsAsync(filter);
        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectResponseDto>> GetProject(Guid id)
    {
        var project = await _projectService.GetProjectByIdAsync(id);
        if (project == null)
            return NotFound(new { message = "Проект не найден" });

        return Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectResponseDto>> CreateProject(CreateProjectDto dto)
    {
        try
        {
            var project = await _projectService.CreateProjectAsync(dto);
            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Ошибка при создании проекта", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProjectResponseDto>> UpdateProject(Guid id, UpdateProjectDto dto)
    {
        var project = await _projectService.UpdateProjectAsync(id, dto);
        if (project == null)
            return NotFound(new { message = "Проект не найден" });

        return Ok(project);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProject(Guid id)
    {
        var result = await _projectService.DeleteProjectAsync(id);
        if (!result)
            return NotFound(new { message = "Проект не найден" });

        return NoContent();
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult> UpdateProjectStatus(Guid id, [FromBody] UpdateStatusDto dto)
    {
        var result = await _projectService.UpdateProjectStatusAsync(id, dto.Status);
        if (!result)
            return BadRequest(new { message = "Не удалось обновить статус проекта" });

        return Ok(new { message = "Статус проекта обновлен" });
    }
}

public class UpdateStatusDto
{
    public string Status { get; set; } = string.Empty;
}
