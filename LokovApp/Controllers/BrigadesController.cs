namespace LokovApp.Controllers;

using LokovApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/v1/[controller]")]
public class BrigadesController : ControllerBase
{
    private readonly LokovAppContext _context;

    public BrigadesController(LokovAppContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<BrigadeResponse>>> GetBrigades()
    {
        var brigades = await _context
            .Brigades.Where(b => b.IsActive)
            .OrderBy(b => b.Name)
            .Select(b => new BrigadeResponse
            {
                Id = b.Id,
                Name = b.Name,
                ForemanName = b.ForemanName,
                Phone = b.Phone,
                WorkersCount = b.WorkersCount,
                Specialization = b.Specialization.ToString(),
                IsActive = b.IsActive,
            })
            .ToListAsync();

        return Ok(brigades);
    }
}

public class BrigadeResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ForemanName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public int WorkersCount { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
