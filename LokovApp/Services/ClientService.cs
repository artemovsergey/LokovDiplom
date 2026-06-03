using LokovApp.Data;
using LokovApp.DTOs;
using LokovApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LokovApp.Services
{
    public interface IClientService
    {
        Task<List<ClientResponseDto>> GetAllClientsAsync(string? search, string? status);
        Task<ClientResponseDto?> GetClientByIdAsync(int id);
        Task<ClientResponseDto> CreateClientAsync(CreateClientDto dto);
        Task<ClientResponseDto?> UpdateClientAsync(int id, UpdateClientDto dto);
        Task<bool> DeleteClientAsync(int id);
    }

    public class ClientService : IClientService
    {
        private readonly LokovAppContext _context;

        public ClientService(LokovAppContext context)
        {
            _context = context;
        }

        public async Task<List<ClientResponseDto>> GetAllClientsAsync(
            string? search,
            string? status
        )
        {
            var query = _context.Clients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(c =>
                    c.FirstName.ToLower().Contains(search)
                    || c.LastName.ToLower().Contains(search)
                    || c.Phone.Contains(search)
                );
            }

            if (
                !string.IsNullOrWhiteSpace(status)
                && Enum.TryParse<ClientStatus>(status, true, out var clientStatus)
            )
            {
                query = query.Where(c => c.Status == clientStatus);
            }

            return await query
                .Select(c => new ClientResponseDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Patronymic = c.Patronymic,
                    Phone = c.Phone,
                    Email = c.Email,
                    Address = c.Address,
                    Status = c.Status.ToString(),
                    ProjectsCount = c.Projects.Count,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                })
                .ToListAsync();
        }

        public async Task<ClientResponseDto?> GetClientByIdAsync(int id)
        {
            return await _context
                .Clients.Where(c => c.Id == id)
                .Select(c => new ClientResponseDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Patronymic = c.Patronymic,
                    Phone = c.Phone,
                    Email = c.Email,
                    Address = c.Address,
                    Status = c.Status.ToString(),
                    ProjectsCount = c.Projects.Count,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ClientResponseDto> CreateClientAsync(CreateClientDto dto)
        {
            var client = new Client
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Patronymic = dto.Patronymic,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return new ClientResponseDto
            {
                Id = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Patronymic = client.Patronymic,
                Phone = client.Phone,
                Email = client.Email,
                Address = client.Address,
                Status = client.Status.ToString(),
                ProjectsCount = 0,
                CreatedAt = client.CreatedAt,
            };
        }

        public async Task<ClientResponseDto?> UpdateClientAsync(int id, UpdateClientDto dto)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                return null;

            client.FirstName = dto.FirstName;
            client.LastName = dto.LastName;
            client.Patronymic = dto.Patronymic;
            client.Phone = dto.Phone;
            client.Email = dto.Email;
            client.Address = dto.Address;
            client.UpdatedAt = DateTime.UtcNow;

            if (Enum.TryParse<ClientStatus>(dto.Status, true, out var status))
            {
                client.Status = status;
            }

            await _context.SaveChangesAsync();

            return await GetClientByIdAsync(id);
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                return false;

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
