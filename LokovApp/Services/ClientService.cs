using LokovApp.Data;
using LokovApp.DTOs;
using LokovApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LokovApp.Services;

public interface IClientService
{
    Task<PagedResponse<ClientResponseDto>> GetClientsAsync(ClientFilterDto filter);
    Task<ClientResponseDto?> GetClientByIdAsync(Guid id);
    Task<ClientResponseDto> CreateClientAsync(CreateClientDto dto);
    Task<ClientResponseDto?> UpdateClientAsync(Guid id, UpdateClientDto dto);
    Task<bool> DeleteClientAsync(Guid id);
    Task<bool> ArchiveClientAsync(Guid id);
    Task<bool> RestoreClientAsync(Guid id);
}

public class ClientService : IClientService
{
    private readonly LokovAppContext _context;

    public ClientService(LokovAppContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<ClientResponseDto>> GetClientsAsync(ClientFilterDto filter)
    {
        var query = _context
            .Clients.Include(c => c.Projects)
            .ThenInclude(p => p.Payments)
            .AsQueryable();

        // Поиск
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.ToLower();
            query = query.Where(c =>
                c.FirstName.ToLower().Contains(search)
                || c.LastName.ToLower().Contains(search)
                || (c.Patronymic != null && c.Patronymic.ToLower().Contains(search))
                || c.Phone.Contains(search)
                || (c.Email != null && c.Email.ToLower().Contains(search))
                || c.Address.ToLower().Contains(search)
            );
        }

        // Фильтр по статусу
        if (
            !string.IsNullOrWhiteSpace(filter.Status)
            && Enum.TryParse<ClientStatus>(filter.Status, true, out var status)
        )
        {
            query = query.Where(c => c.Status == status);
        }

        // Фильтр по источнику
        if (
            !string.IsNullOrWhiteSpace(filter.Source)
            && Enum.TryParse<ClientSource>(filter.Source, true, out var source)
        )
        {
            query = query.Where(c => c.Source == source);
        }

        // Фильтр по категории
        if (
            !string.IsNullOrWhiteSpace(filter.Category)
            && Enum.TryParse<ClientCategory>(filter.Category, true, out var category)
        )
        {
            query = query.Where(c => c.Category == category);
        }

        // Фильтр по дате
        if (filter.CreatedFrom.HasValue)
            query = query.Where(c => c.CreatedAt >= filter.CreatedFrom.Value);
        if (filter.CreatedTo.HasValue)
            query = query.Where(c => c.CreatedAt <= filter.CreatedTo.Value);

        // Сортировка
        query = filter.SortBy?.ToLower() switch
        {
            "name" => filter.SortOrder == "desc"
                ? query.OrderByDescending(c => c.LastName).ThenByDescending(c => c.FirstName)
                : query.OrderBy(c => c.LastName).ThenBy(c => c.FirstName),
            "createdat" => filter.SortOrder == "desc"
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt),
            "status" => filter.SortOrder == "desc"
                ? query.OrderByDescending(c => c.Status)
                : query.OrderBy(c => c.Status),
            _ => query.OrderByDescending(c => c.CreatedAt),
        };

        var totalCount = await query.CountAsync();

        var clients = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var items = clients
            .Select(c => new ClientResponseDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Patronymic = c.Patronymic,
                FullName = c.FullName,
                Phone = c.Phone,
                AdditionalPhone = c.AdditionalPhone,
                Email = c.Email,
                Address = c.Address,
                Source = c.Source.ToString(),
                Status = c.Status.ToString(),
                Category = c.Category.ToString(),
                ProjectsCount = c.Projects.Count,
                TotalPayments = c.Projects.Sum(p => p.Payments.Sum(pay => pay.Amount)),
                Debt = c.Projects.Sum(p => p.EstimatedCost - p.Payments.Sum(pay => pay.Amount)),
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
            })
            .ToList();

        return new PagedResponse<ClientResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize,
        };
    }

    public async Task<ClientResponseDto?> GetClientByIdAsync(Guid id)
    {
        var client = await _context
            .Clients.Include(c => c.Projects)
            .ThenInclude(p => p.Payments)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (client == null)
            return null;

        return MapToResponseDto(client);
    }

    public async Task<ClientResponseDto> CreateClientAsync(CreateClientDto dto)
    {
        var client = new Client
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Patronymic = dto.Patronymic,
            Phone = dto.Phone,
            AdditionalPhone = dto.AdditionalPhone,
            Email = dto.Email,
            Address = dto.Address,
            Source = Enum.TryParse<ClientSource>(dto.Source, true, out var source)
                ? source
                : ClientSource.Other,
            Category = Enum.TryParse<ClientCategory>(dto.Category, true, out var category)
                ? category
                : ClientCategory.Individual,
            CreatedAt = DateTime.UtcNow,
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        return MapToResponseDto(client);
    }

    public async Task<ClientResponseDto?> UpdateClientAsync(Guid id, UpdateClientDto dto)
    {
        var client = await _context
            .Clients.Include(c => c.Projects)
            .ThenInclude(p => p.Payments)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (client == null)
            return null;

        client.FirstName = dto.FirstName;
        client.LastName = dto.LastName;
        client.Patronymic = dto.Patronymic;
        client.Phone = dto.Phone;
        client.AdditionalPhone = dto.AdditionalPhone;
        client.Email = dto.Email;
        client.Address = dto.Address;
        client.UpdatedAt = DateTime.UtcNow;

        if (Enum.TryParse<ClientSource>(dto.Source, true, out var source))
            client.Source = source;
        if (Enum.TryParse<ClientStatus>(dto.Status, true, out var status))
            client.Status = status;
        if (Enum.TryParse<ClientCategory>(dto.Category, true, out var category))
            client.Category = category;

        await _context.SaveChangesAsync();
        return MapToResponseDto(client);
    }

    public async Task<bool> DeleteClientAsync(Guid id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null)
            return false;

        client.IsDeleted = true;
        client.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ArchiveClientAsync(Guid id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null)
            return false;

        client.Status = ClientStatus.Archived;
        client.ArchivedAt = DateTime.UtcNow;
        client.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RestoreClientAsync(Guid id)
    {
        var client = await _context
            .Clients.IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted);

        if (client == null)
            return false;

        client.IsDeleted = false;
        client.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    private static ClientResponseDto MapToResponseDto(Client client)
    {
        return new ClientResponseDto
        {
            Id = client.Id,
            FirstName = client.FirstName,
            LastName = client.LastName,
            Patronymic = client.Patronymic,
            FullName = client.FullName,
            Phone = client.Phone,
            AdditionalPhone = client.AdditionalPhone,
            Email = client.Email,
            Address = client.Address,
            Source = client.Source.ToString(),
            Status = client.Status.ToString(),
            Category = client.Category.ToString(),
            ProjectsCount = client.Projects?.Count ?? 0,
            TotalPayments = client.Projects?.Sum(p => p.Payments?.Sum(pay => pay.Amount) ?? 0) ?? 0,
            Debt =
                client.Projects?.Sum(p =>
                    p.EstimatedCost - (p.Payments?.Sum(pay => pay.Amount) ?? 0)
                ) ?? 0,
            CreatedAt = client.CreatedAt,
            UpdatedAt = client.UpdatedAt,
        };
    }
}
