using LokovApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LokovApp.Controllers;

[ApiController]
[Route("api/v1/[controller]")] // Требуется авторизация для всех методов
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Получение данных для главного дашборда
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<ActionResult> GetDashboard()
    {
        var data = await _reportService.GetDashboardDataAsync();
        return Ok(data);
    }

    /// <summary>
    /// Отчет по клиентам (JSON)
    /// </summary>
    [HttpGet("clients")]
    public async Task<ActionResult> GetClientsReport(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to
    )
    {
        var report = await _reportService.GetClientsReportAsync(from, to);
        return Ok(report);
    }

    /// <summary>
    /// Экспорт отчета по клиентам в PDF
    /// </summary>
    [HttpGet("clients/export/pdf")]
    public async Task<IActionResult> ExportClientsPdf(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to
    )
    {
        var pdf = await _reportService.ExportClientsReportPdfAsync(from, to);
        return File(pdf, "application/pdf", $"clients_report_{DateTime.Now:yyyyMMdd}.pdf");
    }

    /// <summary>
    /// Экспорт отчета по клиентам в Excel
    /// </summary>
    [HttpGet("clients/export/excel")]
    public async Task<IActionResult> ExportClientsExcel(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to
    )
    {
        var excel = await _reportService.ExportClientsReportExcelAsync(from, to);
        return File(
            excel,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"clients_report_{DateTime.Now:yyyyMMdd}.xlsx"
        );
    }

    /// <summary>
    /// Отчет по проектам (JSON)
    /// </summary>
    [HttpGet("projects")]
    public async Task<ActionResult> GetProjectsReport(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to
    )
    {
        var report = await _reportService.GetProjectsReportAsync(from, to);
        return Ok(report);
    }

    /// <summary>
    /// Экспорт отчета по проектам в PDF
    /// </summary>
    [HttpGet("projects/export/pdf")]
    public async Task<IActionResult> ExportProjectsPdf(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to
    )
    {
        var pdf = await _reportService.ExportProjectsReportPdfAsync(from, to);
        return File(pdf, "application/pdf", $"projects_report_{DateTime.Now:yyyyMMdd}.pdf");
    }

    /// <summary>
    /// Экспорт отчета по проектам в Excel
    /// </summary>
    [HttpGet("projects/export/excel")]
    public async Task<IActionResult> ExportProjectsExcel(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to
    )
    {
        var excel = await _reportService.ExportProjectsReportExcelAsync(from, to);
        return File(
            excel,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"projects_report_{DateTime.Now:yyyyMMdd}.xlsx"
        );
    }

    /// <summary>
    /// Финансовый отчет (JSON)
    /// </summary>
    [HttpGet("financial")]
    public async Task<ActionResult> GetFinancialReport(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to
    )
    {
        var report = await _reportService.GetFinancialReportAsync(from, to);
        return Ok(report);
    }

    /// <summary>
    /// Экспорт финансового отчета в PDF
    /// </summary>
    [HttpGet("financial/export/pdf")]
    public async Task<IActionResult> ExportFinancialPdf(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to
    )
    {
        var pdf = await _reportService.ExportFinancialReportPdfAsync(from, to);
        return File(pdf, "application/pdf", $"financial_report_{DateTime.Now:yyyyMMdd}.pdf");
    }

    /// <summary>
    /// Экспорт финансового отчета в Excel
    /// </summary>
    [HttpGet("financial/export/excel")]
    public async Task<IActionResult> ExportFinancialExcel(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to
    )
    {
        var excel = await _reportService.ExportFinancialReportExcelAsync(from, to);
        return File(
            excel,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"financial_report_{DateTime.Now:yyyyMMdd}.xlsx"
        );
    }
}
