using LokovApp.DTOs;
using LokovApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LokovApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ClientResponseDto>>> GetClients(
            [FromQuery] string? search,
            [FromQuery] string? status
        )
        {
            var clients = await _clientService.GetAllClientsAsync(search, status);
            return Ok(clients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClientResponseDto>> GetClient(int id)
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
                return BadRequest(
                    new { message = "Ошибка при создании клиента", error = ex.Message }
                );
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ClientResponseDto>> UpdateClient(int id, UpdateClientDto dto)
        {
            var client = await _clientService.UpdateClientAsync(id, dto);
            if (client == null)
                return NotFound(new { message = "Клиент не найден" });

            return Ok(client);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteClient(int id)
        {
            var result = await _clientService.DeleteClientAsync(id);
            if (!result)
                return NotFound(new { message = "Клиент не найден" });

            return NoContent();
        }
    }
}
