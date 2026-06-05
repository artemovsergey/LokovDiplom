using LokovApp.Data;
using LokovApp.DTOs;
using LokovApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LokovApp.Services;

public interface IProjectService
{
    Task<PagedResponse<ProjectResponseDto>> GetProjectsAsync(ProjectFilterDto filter);
    Task<ProjectResponseDto?> GetProjectByIdAsync(Guid id);
    Task<ProjectResponseDto> CreateProjectAsync(CreateProjectDto dto);
    Task<ProjectResponseDto?> UpdateProjectAsync(Guid id, UpdateProjectDto dto);
    Task<bool> DeleteProjectAsync(Guid id);
    Task<bool> UpdateProjectStatusAsync(Guid id, string status);
}

public class ProjectService : IProjectService
{
    private readonly LokovAppContext _context;

    public ProjectService(LokovAppContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<ProjectResponseDto>> GetProjectsAsync(ProjectFilterDto filter)
    {
        var query = _context
            .Projects.Include(p => p.Client)
            .Include(p => p.Payments)
            .Include(p => p.Brigade)
            .Include(p => p.Stages)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(search)
                || p.Number.ToLower().Contains(search)
                || p.Client.LastName.ToLower().Contains(search)
                || p.Address.ToLower().Contains(search)
            );
        }

        if (
            !string.IsNullOrWhiteSpace(filter.Status)
            && Enum.TryParse<ProjectStatus>(filter.Status, true, out var status)
        )
        {
            query = query.Where(p => p.Status == status);
        }

        if (
            !string.IsNullOrWhiteSpace(filter.Type)
            && Enum.TryParse<ProjectType>(filter.Type, true, out var type)
        )
        {
            query = query.Where(p => p.Type == type);
        }

        if (filter.ClientId.HasValue)
            query = query.Where(p => p.ClientId == filter.ClientId.Value);

        if (filter.StartFrom.HasValue)
            query = query.Where(p => p.StartDate >= filter.StartFrom.Value);
        if (filter.StartTo.HasValue)
            query = query.Where(p => p.StartDate <= filter.StartTo.Value);

        var totalCount = await query.CountAsync();

        var projects = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var items = projects.Select(MapToResponseDto).ToList();

        return new PagedResponse<ProjectResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize,
        };
    }

    public async Task<ProjectResponseDto?> GetProjectByIdAsync(Guid id)
    {
        var project = await _context
            .Projects.Include(p => p.Client)
            .Include(p => p.Payments)
            .Include(p => p.Brigade)
            .Include(p => p.Stages)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        return project == null ? null : MapToResponseDto(project);
    }

    public async Task<ProjectResponseDto> CreateProjectAsync(CreateProjectDto dto)
    {
        var client = await _context.Clients.FindAsync(dto.ClientId);
        if (client == null)
            throw new InvalidOperationException("Клиент не найден");

        var projectNumber = await GenerateProjectNumber(dto.Type);

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Number = projectNumber,
            ClientId = dto.ClientId,
            Name = dto.Name,
            Description = dto.Description,
            Type = Enum.TryParse<ProjectType>(dto.Type, true, out var type)
                ? type
                : ProjectType.MajorRepair,
            Address = dto.Address,
            EstimatedCost = dto.EstimatedCost,
            StartDate = dto.StartDate,
            PlannedEndDate = dto.PlannedEndDate,
            BrigadeId = dto.BrigadeId,
            Status = ProjectStatus.New,
            CreatedAt = DateTime.UtcNow,
        };

        _context.Projects.Add(project);

        // Обновляем статус клиента на Active
        if (client.Status == ClientStatus.Potential || client.Status == ClientStatus.Inactive)
            client.Status = ClientStatus.Active;

        await _context.SaveChangesAsync();
        return await GetProjectByIdAsync(project.Id) ?? MapToResponseDto(project);
    }

    public async Task<ProjectResponseDto?> UpdateProjectAsync(Guid id, UpdateProjectDto dto)
    {
        var project = await _context
            .Projects.Include(p => p.Client)
            .Include(p => p.Payments)
            .Include(p => p.Brigade)
            .Include(p => p.Stages)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (project == null)
            return null;

        project.Name = dto.Name;
        project.Description = dto.Description;
        project.Type = Enum.TryParse<ProjectType>(dto.Type, true, out var type)
            ? type
            : project.Type;
        project.Address = dto.Address;
        project.EstimatedCost = dto.EstimatedCost;
        project.StartDate = dto.StartDate;
        project.PlannedEndDate = dto.PlannedEndDate;
        project.BrigadeId = dto.BrigadeId;
        project.UpdatedAt = DateTime.UtcNow;

        if (
            !string.IsNullOrWhiteSpace(dto.Status)
            && Enum.TryParse<ProjectStatus>(dto.Status, true, out var status)
        )
        {
            project.Status = status;
            if (status == ProjectStatus.Completed)
                project.ActualEndDate = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return MapToResponseDto(project);
    }

    public async Task<bool> DeleteProjectAsync(Guid id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
            return false;

        project.IsDeleted = true;
        project.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateProjectStatusAsync(Guid id, string status)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null || !Enum.TryParse<ProjectStatus>(status, true, out var newStatus))
            return false;

        project.Status = newStatus;
        project.UpdatedAt = DateTime.UtcNow;

        if (newStatus == ProjectStatus.Completed)
            project.ActualEndDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<string> GenerateProjectNumber(string type)
    {
        var prefix = type switch
        {
            "MajorRepair" => "KR",
            "PartialRepair" => "PR",
            "RoofWorks" => "ROOF",
            "FacadeWorks" => "FAC",
            "CombinedWorks" => "CW",
            _ => "PRJ",
        };

        var currentYear = DateTime.UtcNow.Year;
        var count = await _context
            .Projects.IgnoreQueryFilters()
            .CountAsync(p => p.Number.StartsWith($"{prefix}-{currentYear}"));

        return $"{prefix}-{currentYear}-{count + 1:D3}";
    }

    private static ProjectResponseDto MapToResponseDto(Project project)
    {
        var paidAmount = project.Payments?.Sum(p => p.Amount) ?? 0;
        var totalStages = project.Stages?.Count ?? 0;
        var completedStages = project.Stages?.Count(s => s.Status == StageStatus.Completed) ?? 0;
        var completionPercentage =
            totalStages > 0 ? (int)((double)completedStages / totalStages * 100) : 0;

        var typeDisplay = project.Type switch
        {
            ProjectType.MajorRepair => "Капитальный ремонт",
            ProjectType.PartialRepair => "Частичный ремонт",
            ProjectType.RoofWorks => "Кровельные работы",
            ProjectType.FacadeWorks => "Фасадные работы",
            ProjectType.CombinedWorks => "Комплексные работы",
            _ => project.Type.ToString(),
        };

        return new ProjectResponseDto
        {
            Id = project.Id,
            Number = project.Number,
            ClientId = project.ClientId,
            ClientName = project.Client?.FullName ?? "",
            Type = project.Type.ToString(),
            TypeDisplay = typeDisplay,
            Name = project.Name,
            Description = project.Description,
            Address = project.Address,
            EstimatedCost = project.EstimatedCost,
            ActualCost = project.ActualCost,
            PaidAmount = paidAmount,
            CompletionPercentage = completionPercentage,
            Status = project.Status.ToString(),
            StartDate = project.StartDate,
            PlannedEndDate = project.PlannedEndDate,
            ActualEndDate = project.ActualEndDate,
            BrigadeName = project.Brigade?.Name,
            CreatedAt = project.CreatedAt,
        };
    }
}
