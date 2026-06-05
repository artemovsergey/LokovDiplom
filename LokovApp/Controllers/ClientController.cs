using LokovApp.DTOs;
using LokovApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LokovApp.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;
    private readonly IProjectService _projectService;

    public ClientsController(IClientService clientService, IProjectService projectService)
    {
        _clientService = clientService;
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<ClientResponseDto>>> GetClients(
        [FromQuery] ClientFilterDto filter
    )
    {
        var clients = await _clientService.GetClientsAsync(filter);
        return Ok(clients);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClientResponseDto>> GetClient(Guid id)
    {
        var client = await _clientService.GetClientByIdAsync(id);
        if (client == null)
            return NotFound(new { message = "Клиент не найден" });

        return Ok(client);
    }

    [HttpPost]
    public async Task<ActionResult<ClientResponseDto>> CreateClient(CreateClientDto dto)
    {
        try
        {
            var client = await _clientService.CreateClientAsync(dto);
            return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Ошибка при создании клиента", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ClientResponseDto>> UpdateClient(Guid id, UpdateClientDto dto)
    {
        var client = await _clientService.UpdateClientAsync(id, dto);
        if (client == null)
            return NotFound(new { message = "Клиент не найден" });

        return Ok(client);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteClient(Guid id)
    {
        var result = await _clientService.DeleteClientAsync(id);
        if (!result)
            return NotFound(new { message = "Клиент не найден" });

        return NoContent();
    }

    [HttpPatch("{id}/archive")]
    public async Task<ActionResult> ArchiveClient(Guid id)
    {
        var result = await _clientService.ArchiveClientAsync(id);
        if (!result)
            return NotFound(new { message = "Клиент не найден" });

        return Ok(new { message = "Клиент архивирован" });
    }

    [HttpPatch("{id}/restore")]
    public async Task<ActionResult> RestoreClient(Guid id)
    {
        var result = await _clientService.RestoreClientAsync(id);
        if (!result)
            return NotFound(new { message = "Клиент не найден" });

        return Ok(new { message = "Клиент восстановлен" });
    }

    [HttpGet("{id}/projects")]
    public async Task<ActionResult<PagedResponse<ProjectResponseDto>>> GetClientProjects(
        Guid id,
        [FromQuery] ProjectFilterDto filter
    )
    {
        filter.ClientId = id;
        var projects = await _projectService.GetProjectsAsync(filter);
        return Ok(projects);
    }
}
