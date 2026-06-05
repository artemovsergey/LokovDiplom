using ClosedXML.Excel;
using LokovApp.Data;
using LokovApp.Dtos;
using LokovApp.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LokovApp.Services;

public interface IReportService
{
    Task<DashboardDataDto> GetDashboardDataAsync();

    Task<ClientsReportDto> GetClientsReportAsync(DateTime? from, DateTime? to);
    Task<byte[]> ExportClientsReportPdfAsync(DateTime? from, DateTime? to);
    Task<byte[]> ExportClientsReportExcelAsync(DateTime? from, DateTime? to);

    Task<ProjectsReportDto> GetProjectsReportAsync(DateTime? from, DateTime? to);
    Task<byte[]> ExportProjectsReportPdfAsync(DateTime? from, DateTime? to);
    Task<byte[]> ExportProjectsReportExcelAsync(DateTime? from, DateTime? to);

    Task<FinancialReportDto> GetFinancialReportAsync(DateTime? from, DateTime? to);
    Task<byte[]> ExportFinancialReportPdfAsync(DateTime? from, DateTime? to);
    Task<byte[]> ExportFinancialReportExcelAsync(DateTime? from, DateTime? to);
}

public class ReportService : IReportService
{
    private readonly LokovAppContext _context;

    public ReportService(LokovAppContext context)
    {
        _context = context;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    #region Dashboard

    public async Task<DashboardDataDto> GetDashboardDataAsync()
    {
        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var clients = await _context.Clients.Include(c => c.Projects).ToListAsync();
        var projects = await _context
            .Projects.Include(p => p.Payments)
            .Include(p => p.Expenses)
            .Include(p => p.Brigade)
            .Include(p => p.Client)
            .ToListAsync();

        return new DashboardDataDto
        {
            SummaryCards = new SummaryCardsDto
            {
                TotalClients = clients.Count,
                ActiveProjects = projects.Count(p => p.Status == ProjectStatus.InProgress),
                CompletedProjects = projects.Count(p => p.Status == ProjectStatus.Completed),
                TotalRevenue = projects.Sum(p => p.Payments.Sum(pay => pay.Amount)),
                MonthlyRevenue = projects
                    .Where(p => p.Payments.Any(pay => pay.PaymentDate >= monthStart))
                    .Sum(p =>
                        p.Payments.Where(pay => pay.PaymentDate >= monthStart)
                            .Sum(pay => pay.Amount)
                    ),
                OutstandingDebt = projects.Sum(p =>
                    p.EstimatedCost - p.Payments.Sum(pay => pay.Amount)
                ),
                NewClientsThisMonth = clients.Count(c => c.CreatedAt >= monthStart),
                OverdueProjects = projects.Count(p =>
                    p.PlannedEndDate < now && p.Status == ProjectStatus.InProgress
                ),
            },
            MonthlyRevenue = await GetMonthlyRevenueAsync(),
            ProjectTypeDistribution = GetProjectTypeDistribution(projects),
            ClientSources = GetClientSources(clients),
            BrigadeLoad = GetBrigadeLoad(projects),
            RecentProjects = projects
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .Select(p => new RecentProjectDto
                {
                    Id = p.Id,
                    Number = p.Number,
                    Name = p.Name,
                    ClientName = p.Client?.FullName ?? "",
                    Status = p.Status.ToString(),
                    Budget = p.EstimatedCost,
                    CompletionPercent = CalculateCompletionPercent(p),
                })
                .ToList(),
            TopClients = clients
                .OrderByDescending(c => c.Projects.Sum(p => p.EstimatedCost))
                .Take(5)
                .Select(c => new TopClientDto
                {
                    Id = c.Id,
                    FullName = c.FullName,
                    ProjectsCount = c.Projects.Count,
                    TotalAmount = c.Projects.Sum(p => p.EstimatedCost),
                })
                .ToList(),
        };
    }

    #endregion

    #region Clients Report

    public async Task<ClientsReportDto> GetClientsReportAsync(DateTime? from, DateTime? to)
    {
        var query = _context
            .Clients.Include(c => c.Projects)
            .ThenInclude(p => p.Payments)
            .AsQueryable();

        if (from.HasValue)
            query = query.Where(c => c.CreatedAt >= from.Value);
        if (to.HasValue)
            query = query.Where(c => c.CreatedAt <= to.Value);

        var clients = await query.ToListAsync();

        return new ClientsReportDto
        {
            GeneratedAt = DateTime.UtcNow,
            Period =
                $"{(from?.ToString("dd.MM.yyyy") ?? "начало")} - {(to?.ToString("dd.MM.yyyy") ?? "сегодня")}",
            TotalCount = clients.Count,
            Items = clients
                .Select(c => new ClientReportItemDto
                {
                    FullName = c.FullName,
                    Phone = c.Phone,
                    Email = c.Email ?? "",
                    Address = c.Address,
                    Status = c.Status.ToString(),
                    Source = c.Source.ToString(),
                    ProjectsCount = c.Projects.Count,
                    TotalPayments = c.Projects.Sum(p => p.Payments.Sum(pay => pay.Amount)),
                    CreatedAt = c.CreatedAt,
                })
                .ToList(),
            ByStatus = clients
                .GroupBy(c => c.Status.ToString())
                .ToDictionary(g => g.Key, g => g.Count()),
            BySource = clients
                .GroupBy(c => c.Source.ToString())
                .ToDictionary(g => g.Key, g => g.Count()),
        };
    }

    public async Task<byte[]> ExportClientsReportPdfAsync(DateTime? from, DateTime? to)
    {
        var report = await GetClientsReportAsync(from, to);
        return GenerateClientsPdf(report);
    }

    public async Task<byte[]> ExportClientsReportExcelAsync(DateTime? from, DateTime? to)
    {
        var report = await GetClientsReportAsync(from, to);
        return GenerateClientsExcel(report);
    }

    #endregion

    #region Projects Report

    public async Task<ProjectsReportDto> GetProjectsReportAsync(DateTime? from, DateTime? to)
    {
        var query = _context.Projects.Include(p => p.Client).Include(p => p.Payments).AsQueryable();

        if (from.HasValue)
            query = query.Where(p => p.StartDate >= from.Value || p.CreatedAt >= from.Value);
        if (to.HasValue)
            query = query.Where(p => p.StartDate <= to.Value || p.CreatedAt <= to.Value);

        var projects = await query.ToListAsync();

        return new ProjectsReportDto
        {
            GeneratedAt = DateTime.UtcNow,
            Period =
                $"{(from?.ToString("dd.MM.yyyy") ?? "начало")} - {(to?.ToString("dd.MM.yyyy") ?? "сегодня")}",
            TotalCount = projects.Count,
            TotalEstimatedCost = projects.Sum(p => p.EstimatedCost),
            TotalActualCost = projects.Sum(p => p.ActualCost ?? 0),
            Items = projects
                .Select(p => new ProjectReportItemDto
                {
                    Number = p.Number,
                    Name = p.Name,
                    ClientName = p.Client?.FullName ?? "",
                    Type = GetTypeDisplay(p.Type),
                    Status = p.Status.ToString(),
                    EstimatedCost = p.EstimatedCost,
                    ActualCost = p.ActualCost ?? 0,
                    PaidAmount = p.Payments.Sum(pay => pay.Amount),
                    Debt = p.EstimatedCost - p.Payments.Sum(pay => pay.Amount),
                    StartDate = p.StartDate,
                    EndDate = p.ActualEndDate,
                })
                .ToList(),
            ByStatus = projects
                .GroupBy(p => p.Status.ToString())
                .ToDictionary(g => g.Key, g => g.Count()),
            ByType = projects
                .GroupBy(p => p.Type.ToString())
                .ToDictionary(g => g.Key, g => g.Count()),
        };
    }

    public async Task<byte[]> ExportProjectsReportPdfAsync(DateTime? from, DateTime? to)
    {
        var report = await GetProjectsReportAsync(from, to);
        return GenerateProjectsPdf(report);
    }

    public async Task<byte[]> ExportProjectsReportExcelAsync(DateTime? from, DateTime? to)
    {
        var report = await GetProjectsReportAsync(from, to);
        return GenerateProjectsExcel(report);
    }

    #endregion

    #region Financial Report

    public async Task<FinancialReportDto> GetFinancialReportAsync(DateTime? from, DateTime? to)
    {
        var projectsQuery = _context
            .Projects.Include(p => p.Client)
            .Include(p => p.Payments)
            .Include(p => p.Expenses)
            .AsQueryable();

        if (from.HasValue)
            projectsQuery = projectsQuery.Where(p =>
                p.StartDate >= from.Value || p.CreatedAt >= from.Value
            );
        if (to.HasValue)
            projectsQuery = projectsQuery.Where(p =>
                p.StartDate <= to.Value || p.CreatedAt <= to.Value
            );

        var projects = await projectsQuery.ToListAsync();

        var monthlyBreakdown = await GetMonthlyFinancialBreakdown(projects, from, to);

        return new FinancialReportDto
        {
            GeneratedAt = DateTime.UtcNow,
            Period =
                $"{(from?.ToString("dd.MM.yyyy") ?? "начало")} - {(to?.ToString("dd.MM.yyyy") ?? "сегодня")}",
            Summary = new FinancialSummaryDto
            {
                TotalProjects = projects.Count,
                TotalEstimatedCost = projects.Sum(p => p.EstimatedCost),
                TotalActualCost = projects.Sum(p => p.ActualCost ?? 0),
                TotalPayments = projects.Sum(p => p.Payments.Sum(pay => pay.Amount)),
                TotalExpenses = projects.Sum(p => p.Expenses.Sum(e => e.Amount)),
                TotalDebt = projects.Sum(p => p.EstimatedCost - p.Payments.Sum(pay => pay.Amount)),
                TotalProfit = projects.Sum(p =>
                    p.Payments.Sum(pay => pay.Amount) - (p.ActualCost ?? p.EstimatedCost)
                ),
                AverageProfitMargin = projects.Any()
                    ? projects.Average(p => CalculateProfitMargin(p))
                    : 0,
            },
            Projects = projects
                .Select(p => new FinancialProjectItemDto
                {
                    ProjectNumber = p.Number,
                    ProjectName = p.Name,
                    ClientName = p.Client?.FullName ?? "",
                    EstimatedCost = p.EstimatedCost,
                    ActualCost = p.ActualCost ?? 0,
                    Payments = p.Payments.Sum(pay => pay.Amount),
                    Expenses = p.Expenses.Sum(e => e.Amount),
                    Debt = p.EstimatedCost - p.Payments.Sum(pay => pay.Amount),
                    Profit = p.Payments.Sum(pay => pay.Amount) - (p.ActualCost ?? p.EstimatedCost),
                    ProfitMargin = CalculateProfitMargin(p),
                })
                .ToList(),
            MonthlyBreakdown = monthlyBreakdown,
        };
    }

    public async Task<byte[]> ExportFinancialReportPdfAsync(DateTime? from, DateTime? to)
    {
        var report = await GetFinancialReportAsync(from, to);
        return GenerateFinancialPdf(report);
    }

    public async Task<byte[]> ExportFinancialReportExcelAsync(DateTime? from, DateTime? to)
    {
        var report = await GetFinancialReportAsync(from, to);
        return GenerateFinancialExcel(report);
    }

    #endregion

    #region Private Helpers

    private async Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync()
    {
        var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
        var payments = await _context
            .Payments.Where(p => p.PaymentDate >= sixMonthsAgo)
            .ToListAsync();

        var expenses = await _context
            .ProjectExpenses.Where(e => e.Date >= sixMonthsAgo)
            .ToListAsync();

        var months = Enumerable
            .Range(0, 6)
            .Select(i => sixMonthsAgo.AddMonths(i))
            .Select(d => new
            {
                Month = d.ToString("yyyy-MM"),
                Label = d.ToString("MMMM yyyy"),
                Start = new DateTime(d.Year, d.Month, 1),
                End = new DateTime(d.Year, d.Month, 1).AddMonths(1).AddDays(-1),
            })
            .ToList();

        return months
            .Select(m => new MonthlyRevenueDto
            {
                Month = m.Label,
                Revenue = payments
                    .Where(p => p.PaymentDate >= m.Start && p.PaymentDate <= m.End)
                    .Sum(p => p.Amount),
                Expenses = expenses
                    .Where(e => e.Date >= m.Start && e.Date <= m.End)
                    .Sum(e => e.Amount),
                Profit =
                    payments
                        .Where(p => p.PaymentDate >= m.Start && p.PaymentDate <= m.End)
                        .Sum(p => p.Amount)
                    - expenses.Where(e => e.Date >= m.Start && e.Date <= m.End).Sum(e => e.Amount),
            })
            .ToList();
    }

    private async Task<List<MonthlyFinancialDto>> GetMonthlyFinancialBreakdown(
        List<Project> projects,
        DateTime? from,
        DateTime? to
    )
    {
        var startDate = from ?? projects.Min(p => p.CreatedAt);
        var endDate = to ?? DateTime.UtcNow;

        var months = new List<DateTime>();
        var current = new DateTime(startDate.Year, startDate.Month, 1);
        while (current <= endDate)
        {
            months.Add(current);
            current = current.AddMonths(1);
        }

        return months
            .Select(m => new MonthlyFinancialDto
            {
                Month = m.ToString("MMMM yyyy"),
                Revenue = projects.Sum(p =>
                    p.Payments.Where(pay =>
                            pay.PaymentDate.Year == m.Year && pay.PaymentDate.Month == m.Month
                        )
                        .Sum(pay => pay.Amount)
                ),
                Expenses = projects.Sum(p =>
                    p.Expenses.Where(e => e.Date.Year == m.Year && e.Date.Month == m.Month)
                        .Sum(e => e.Amount)
                ),
                Profit =
                    projects.Sum(p =>
                        p.Payments.Where(pay =>
                                pay.PaymentDate.Year == m.Year && pay.PaymentDate.Month == m.Month
                            )
                            .Sum(pay => pay.Amount)
                    )
                    - projects.Sum(p =>
                        p.Expenses.Where(e => e.Date.Year == m.Year && e.Date.Month == m.Month)
                            .Sum(e => e.Amount)
                    ),
            })
            .ToList();
    }

    private static List<ProjectTypeDistributionDto> GetProjectTypeDistribution(
        List<Project> projects
    )
    {
        return projects
            .GroupBy(p => p.Type)
            .Select(g => new ProjectTypeDistributionDto
            {
                Type = g.Key.ToString(),
                Label = GetTypeDisplay(g.Key),
                Count = g.Count(),
                TotalAmount = g.Sum(p => p.EstimatedCost),
            })
            .ToList();
    }

    private static List<ClientSourceDto> GetClientSources(List<Client> clients)
    {
        var total = clients.Count;
        return clients
            .GroupBy(c => c.Source)
            .Select(g => new ClientSourceDto
            {
                Source = g.Key.ToString(),
                Label = GetSourceDisplay(g.Key),
                Count = g.Count(),
                ConversionRate = total > 0 ? (decimal)g.Count() / total * 100 : 0,
            })
            .ToList();
    }

    private static List<BrigadeLoadDto> GetBrigadeLoad(List<Project> projects)
    {
        var brigades = projects
            .Where(p => p.Brigade != null)
            .GroupBy(p => p.Brigade!)
            .Select(g => new BrigadeLoadDto
            {
                BrigadeId = g.Key.Id,
                BrigadeName = g.Key.Name,
                CurrentProjects = g.Count(p => p.Status == ProjectStatus.InProgress),
                MaxCapacity = g.Key.WorkersCount * 2,
                LoadPercentage =
                    g.Key.WorkersCount > 0
                        ? (int)(
                            (double)g.Count(p => p.Status == ProjectStatus.InProgress)
                            / (g.Key.WorkersCount * 2)
                            * 100
                        )
                        : 0,
            })
            .ToList();

        return brigades;
    }

    private static int CalculateCompletionPercent(Project project)
    {
        var totalStages = project.Stages?.Count ?? 0;
        var completedStages = project.Stages?.Count(s => s.Status == StageStatus.Completed) ?? 0;
        return totalStages > 0 ? (int)((double)completedStages / totalStages * 100) : 0;
    }

    private static decimal CalculateProfitMargin(Project project)
    {
        var revenue = project.Payments.Sum(p => p.Amount);
        var cost = project.ActualCost ?? project.EstimatedCost;
        return cost > 0 ? (revenue - cost) / cost * 100 : 0;
    }

    private static string GetTypeDisplay(ProjectType type) =>
        type switch
        {
            ProjectType.MajorRepair => "Капитальный ремонт",
            ProjectType.PartialRepair => "Частичный ремонт",
            ProjectType.RoofWorks => "Кровельные работы",
            ProjectType.FacadeWorks => "Фасадные работы",
            ProjectType.CombinedWorks => "Комплексные работы",
            _ => type.ToString(),
        };

    private static string GetSourceDisplay(ClientSource source) =>
        source switch
        {
            ClientSource.Recommendation => "Рекомендация",
            ClientSource.Internet => "Интернет",
            ClientSource.SocialMedia => "Социальные сети",
            ClientSource.Advertisement => "Реклама",
            ClientSource.DirectContact => "Прямое обращение",
            ClientSource.Other => "Другое",
            _ => source.ToString(),
        };

    #endregion

    #region PDF Generation

    private byte[] GenerateClientsPdf(ClientsReportDto report)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(ComposeHeader);
                page.Content().Element(c => ComposeClientsTable(c, report));
                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    private byte[] GenerateProjectsPdf(ProjectsReportDto report)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Element(ComposeHeader);
                page.Content().Element(c => ComposeProjectsTable(c, report));
                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    private byte[] GenerateFinancialPdf(FinancialReportDto report)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Element(ComposeHeader);
                page.Content().Element(c => ComposeFinancialContent(c, report));
                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    // Общий заголовок для всех отчетов
    private void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Text("ИП Локов А.М.").FontSize(14).Bold();
            column.Item().Text("CRM Система учета клиентов и проектов").FontSize(10).Italic();
            column.Item().PaddingBottom(5);
            column.Item().LineHorizontal(1);
            column.Item().PaddingBottom(10);
        });
    }

    // Общий футер для всех отчетов
    private void ComposeFooter(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().PaddingTop(5);
            column.Item().LineHorizontal(1);
            column
                .Item()
                .Text(text =>
                {
                    text.Span("Сформировано: ");
                    text.Span(DateTime.Now.ToString("dd.MM.yyyy HH:mm")).Bold();
                    text.AlignRight();
                });
        });
    }

    // Компоновка контента для отчета по клиентам
    private void ComposeClientsTable(IContainer container, ClientsReportDto report)
    {
        container.Column(column =>
        {
            // Заголовок отчета
            column.Item().Text("Отчет по клиентам").FontSize(16).Bold();
            column.Item().Text($"Период: {report.Period}").FontSize(10);
            column.Item().Text($"Всего клиентов: {report.TotalCount}").FontSize(10);
            column.Item().PaddingBottom(10);

            // Таблица с данными
            column
                .Item()
                .Table(table =>
                {
                    // Определение колонок
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3); // ФИО
                        columns.RelativeColumn(2); // Телефон
                        columns.RelativeColumn(2); // Email
                        columns.RelativeColumn(1.5f); // Статус
                        columns.RelativeColumn(1); // Проекты
                        columns.RelativeColumn(1.5f); // Платежи
                    });

                    // Заголовок таблицы
                    table.Header(header =>
                    {
                        header
                            .Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(3)
                            .Text("ФИО")
                            .Bold()
                            .FontSize(9);
                        header
                            .Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(3)
                            .Text("Телефон")
                            .Bold()
                            .FontSize(9);
                        header
                            .Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(3)
                            .Text("Email")
                            .Bold()
                            .FontSize(9);
                        header
                            .Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(3)
                            .Text("Статус")
                            .Bold()
                            .FontSize(9);
                        header
                            .Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(3)
                            .Text("Проекты")
                            .Bold()
                            .FontSize(9);
                        header
                            .Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(3)
                            .Text("Платежи, ₽")
                            .Bold()
                            .FontSize(9);
                    });

                    // Данные таблицы
                    foreach (var item in report.Items)
                    {
                        table
                            .Cell()
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(3)
                            .Text(item.FullName)
                            .FontSize(9);
                        table
                            .Cell()
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(3)
                            .Text(item.Phone)
                            .FontSize(9);
                        table
                            .Cell()
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(3)
                            .Text(item.Email)
                            .FontSize(8);
                        table
                            .Cell()
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(3)
                            .Text(item.Status)
                            .FontSize(9);
                        table
                            .Cell()
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(3)
                            .Text(item.ProjectsCount.ToString())
                            .FontSize(9);
                        table
                            .Cell()
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(3)
                            .Text(item.TotalPayments.ToString("N0"))
                            .FontSize(9);
                    }
                });

            // Статистика по статусам
            if (report.ByStatus.Any())
            {
                column.Item().PaddingTop(15);
                column.Item().Text("Распределение по статусам").FontSize(12).Bold();
                column.Item().PaddingBottom(5);

                foreach (var status in report.ByStatus)
                {
                    column.Item().Text($"• {status.Key}: {status.Value} клиентов").FontSize(10);
                }
            }
        });
    }

    // Компоновка контента для отчета по проектам
    private void ComposeProjectsTable(IContainer container, ProjectsReportDto report)
    {
        container.Column(column =>
        {
            // Заголовок отчета
            column.Item().Text("Отчет по проектам").FontSize(16).Bold();
            column.Item().Text($"Период: {report.Period}").FontSize(10);
            column
                .Item()
                .Row(row =>
                {
                    row.RelativeItem().Text($"Всего проектов: {report.TotalCount}").FontSize(10);
                    row.RelativeItem()
                        .Text($"Общая сметная стоимость: {report.TotalEstimatedCost:N2} ₽")
                        .FontSize(10);
                    row.RelativeItem()
                        .Text($"Фактические затраты: {report.TotalActualCost:N2} ₽")
                        .FontSize(10);
                });
            column.Item().PaddingBottom(10);

            // Таблица проектов
            column
                .Item()
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(90); // Номер
                        columns.RelativeColumn(3); // Название
                        columns.RelativeColumn(2); // Клиент
                        columns.RelativeColumn(1.5f); // Тип
                        columns.RelativeColumn(1.5f); // Статус
                        columns.ConstantColumn(90); // Смета
                        columns.ConstantColumn(90); // Оплачено
                        columns.ConstantColumn(90); // Долг
                    });

                    // Заголовок
                    table.Header(header =>
                    {
                        header
                            .Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(3)
                            .Text("Номер")
                            .Bold()
                            .FontSize(8);
                        header
                            .Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(3)
                            .Text("Название")
                            .Bold()
                            .FontSize(8);
                        header
                            .Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(3)
                            .Text("Клиент")
                            .Bold()
                            .FontSize(8);
                        header
                            .Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(3)
                            .Text("Тип")
                            .Bold()
                            .FontSize(8);
                        header
                            .Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(3)
                            .Text("Статус")
                            .Bold()
                            .FontSize(8);
                        header
                            .Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(3)
                            .Text("Смета, ₽")
                            .Bold()
                            .FontSize(8);
                        header
                            .Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(3)
                            .Text("Оплачено, ₽")
                            .Bold()
                            .FontSize(8);
                        header
                            .Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(3)
                            .Text("Долг, ₽")
                            .Bold()
                            .FontSize(8);
                    });

                    // Данные
                    foreach (var item in report.Items)
                    {
                        table
                            .Cell()
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(3)
                            .Text(item.Number)
                            .FontSize(8);
                        table
                            .Cell()
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(3)
                            .Text(item.Name)
                            .FontSize(8);
                        table
                            .Cell()
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(3)
                            .Text(item.ClientName)
                            .FontSize(8);
                        table
                            .Cell()
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(3)
                            .Text(item.Type)
                            .FontSize(8);
                        table
                            .Cell()
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(3)
                            .Text(item.Status)
                            .FontSize(8);
                        table
                            .Cell()
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(3)
                            .Text(item.EstimatedCost.ToString("N0"))
                            .FontSize(8);
                        table
                            .Cell()
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(3)
                            .Text(item.PaidAmount.ToString("N0"))
                            .FontSize(8);
                        table
                            .Cell()
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(3)
                            .Text(item.Debt.ToString("N0"))
                            .FontSize(8);
                    }
                });

            // Статистика по типам
            if (report.ByType.Any())
            {
                column.Item().PaddingTop(15);
                column.Item().Text("Распределение по типам проектов").FontSize(12).Bold();
                column.Item().PaddingBottom(5);

                foreach (var type in report.ByType)
                {
                    column.Item().Text($"• {type.Key}: {type.Value} проектов").FontSize(10);
                }
            }
        });
    }

    // Компоновка контента для финансового отчета
    private void ComposeFinancialContent(IContainer container, FinancialReportDto report)
    {
        container.Column(column =>
        {
            // Заголовок
            column.Item().Text("Финансовый отчет").FontSize(16).Bold();
            column.Item().Text($"Период: {report.Period}").FontSize(10);
            column.Item().PaddingBottom(10);

            // Сводные показатели
            column.Item().Text("Ключевые показатели").FontSize(14).Bold();
            column.Item().PaddingBottom(5);

            column
                .Item()
                .Row(row =>
                {
                    row.RelativeItem()
                        .Column(col =>
                        {
                            col.Item().Text("Доходы").FontSize(9).FontColor(Colors.Grey.Darken1);
                            col.Item()
                                .Text($"{report.Summary.TotalPayments:N2} ₽")
                                .FontSize(14)
                                .Bold();
                        });
                    row.RelativeItem()
                        .Column(col =>
                        {
                            col.Item().Text("Расходы").FontSize(9).FontColor(Colors.Grey.Darken1);
                            col.Item()
                                .Text($"{report.Summary.TotalExpenses:N2} ₽")
                                .FontSize(14)
                                .Bold();
                        });
                    row.RelativeItem()
                        .Column(col =>
                        {
                            col.Item().Text("Прибыль").FontSize(9).FontColor(Colors.Grey.Darken1);
                            col.Item()
                                .Text($"{report.Summary.TotalProfit:N2} ₽")
                                .FontSize(14)
                                .Bold();
                        });
                    row.RelativeItem()
                        .Column(col =>
                        {
                            col.Item()
                                .Text("Маржинальность")
                                .FontSize(9)
                                .FontColor(Colors.Grey.Darken1);
                            col.Item()
                                .Text($"{report.Summary.AverageProfitMargin:F1}%")
                                .FontSize(14)
                                .Bold();
                        });
                });

            column.Item().PaddingBottom(5);
            column
                .Item()
                .Row(row =>
                {
                    row.RelativeItem()
                        .Column(col =>
                        {
                            col.Item()
                                .Text("Дебиторская задолженность")
                                .FontSize(9)
                                .FontColor(Colors.Grey.Darken1);
                            col.Item()
                                .Text($"{report.Summary.TotalDebt:N2} ₽")
                                .FontSize(14)
                                .Bold()
                                .FontColor(Colors.Red.Darken2);
                        });
                    row.RelativeItem()
                        .Column(col =>
                        {
                            col.Item()
                                .Text("Всего проектов")
                                .FontSize(9)
                                .FontColor(Colors.Grey.Darken1);
                            col.Item()
                                .Text(report.Summary.TotalProjects.ToString())
                                .FontSize(14)
                                .Bold();
                        });
                });

            column.Item().PaddingBottom(15);

            // Таблица по проектам
            column.Item().Text("Детализация по проектам").FontSize(14).Bold();
            column.Item().PaddingBottom(5);

            column
                .Item()
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(80); // Номер
                        columns.RelativeColumn(3); // Проект
                        columns.RelativeColumn(2); // Клиент
                        columns.ConstantColumn(90); // Смета
                        columns.ConstantColumn(90); // Оплаты
                        columns.ConstantColumn(90); // Расходы
                        columns.ConstantColumn(90); // Прибыль
                    });

                    // Заголовок
                    table.Header(header =>
                    {
                        header
                            .Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(3)
                            .Text("Номер")
                            .Bold()
                            .FontSize(8);
                        header
                            .Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(3)
                            .Text("Проект")
                            .Bold()
                            .FontSize(8);
                        header
                            .Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(3)
                            .Text("Клиент")
                            .Bold()
                            .FontSize(8);
                        header
                            .Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(3)
                            .Text("Смета, ₽")
                            .Bold()
                            .FontSize(8);
                        header
                            .Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(3)
                            .Text("Оплаты, ₽")
                            .Bold()
                            .FontSize(8);
                        header
                            .Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(3)
                            .Text("Расходы, ₽")
                            .Bold()
                            .FontSize(8);
                        header
                            .Cell()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(3)
                            .Text("Прибыль, ₽")
                            .Bold()
                            .FontSize(8);
                    });

                    // Данные
                    foreach (var item in report.Projects)
                    {
                        table
                            .Cell()
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(3)
                            .Text(item.ProjectNumber)
                            .FontSize(8);
                        table
                            .Cell()
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(3)
                            .Text(item.ProjectName)
                            .FontSize(8);
                        table
                            .Cell()
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(3)
                            .Text(item.ClientName)
                            .FontSize(8);
                        table
                            .Cell()
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(3)
                            .Text(item.EstimatedCost.ToString("N0"))
                            .FontSize(8);
                        table
                            .Cell()
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(3)
                            .Text(item.Payments.ToString("N0"))
                            .FontSize(8);
                        table
                            .Cell()
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(3)
                            .Text(item.Expenses.ToString("N0"))
                            .FontSize(8);

                        var profitColor =
                            item.Profit >= 0 ? Colors.Green.Darken2 : Colors.Red.Darken2;
                        table
                            .Cell()
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(3)
                            .Text(item.Profit.ToString("N0"))
                            .FontSize(8)
                            .FontColor(profitColor);
                    }
                });
        });
    }

    #endregion

    #region Excel Generation

    private byte[] GenerateClientsExcel(ClientsReportDto report)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Клиенты");

        // Заголовки
        worksheet.Cell("A1").Value = "ФИО";
        worksheet.Cell("B1").Value = "Телефон";
        worksheet.Cell("C1").Value = "Email";
        worksheet.Cell("D1").Value = "Адрес";
        worksheet.Cell("E1").Value = "Статус";
        worksheet.Cell("F1").Value = "Источник";
        worksheet.Cell("G1").Value = "Проектов";
        worksheet.Cell("H1").Value = "Платежи";
        worksheet.Cell("I1").Value = "Дата создания";

        var headerRange = worksheet.Range("A1:I1");
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        // Данные
        for (int i = 0; i < report.Items.Count; i++)
        {
            var row = i + 2;
            var item = report.Items[i];
            worksheet.Cell(row, 1).Value = item.FullName;
            worksheet.Cell(row, 2).Value = item.Phone;
            worksheet.Cell(row, 3).Value = item.Email;
            worksheet.Cell(row, 4).Value = item.Address;
            worksheet.Cell(row, 5).Value = item.Status;
            worksheet.Cell(row, 6).Value = item.Source;
            worksheet.Cell(row, 7).Value = item.ProjectsCount;
            worksheet.Cell(row, 8).Value = item.TotalPayments;
            worksheet.Cell(row, 9).Value = item.CreatedAt.ToString("dd.MM.yyyy");
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private byte[] GenerateProjectsExcel(ProjectsReportDto report)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Проекты");

        worksheet.Cell("A1").Value = "Номер";
        worksheet.Cell("B1").Value = "Название";
        worksheet.Cell("C1").Value = "Клиент";
        worksheet.Cell("D1").Value = "Тип";
        worksheet.Cell("E1").Value = "Статус";
        worksheet.Cell("F1").Value = "Смета";
        worksheet.Cell("G1").Value = "Факт. затраты";
        worksheet.Cell("H1").Value = "Оплачено";
        worksheet.Cell("I1").Value = "Долг";
        worksheet.Cell("J1").Value = "Начало";
        worksheet.Cell("K1").Value = "Окончание";

        var headerRange = worksheet.Range("A1:K1");
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        for (int i = 0; i < report.Items.Count; i++)
        {
            var row = i + 2;
            var item = report.Items[i];
            worksheet.Cell(row, 1).Value = item.Number;
            worksheet.Cell(row, 2).Value = item.Name;
            worksheet.Cell(row, 3).Value = item.ClientName;
            worksheet.Cell(row, 4).Value = item.Type;
            worksheet.Cell(row, 5).Value = item.Status;
            worksheet.Cell(row, 6).Value = item.EstimatedCost;
            worksheet.Cell(row, 7).Value = item.ActualCost;
            worksheet.Cell(row, 8).Value = item.PaidAmount;
            worksheet.Cell(row, 9).Value = item.Debt;
            worksheet.Cell(row, 10).Value = item.StartDate?.ToString("dd.MM.yyyy");
            worksheet.Cell(row, 11).Value = item.EndDate?.ToString("dd.MM.yyyy");
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private byte[] GenerateFinancialExcel(FinancialReportDto report)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Финансы");

        // Сводка
        worksheet.Cell("A1").Value = "Финансовая сводка";
        worksheet.Cell("A1").Style.Font.Bold = true;
        worksheet.Cell("A1").Style.Font.FontSize = 14;

        worksheet.Cell("A3").Value = "Всего проектов:";
        worksheet.Cell("B3").Value = report.Summary.TotalProjects;
        worksheet.Cell("A4").Value = "Выручка:";
        worksheet.Cell("B4").Value = report.Summary.TotalPayments;
        worksheet.Cell("A5").Value = "Расходы:";
        worksheet.Cell("B5").Value = report.Summary.TotalExpenses;
        worksheet.Cell("A6").Value = "Прибыль:";
        worksheet.Cell("B6").Value = report.Summary.TotalProfit;
        worksheet.Cell("A7").Value = "Долг:";
        worksheet.Cell("B7").Value = report.Summary.TotalDebt;
        worksheet.Cell("A8").Value = "Маржинальность:";
        worksheet.Cell("B8").Value = report.Summary.AverageProfitMargin / 100;
        worksheet.Cell("B8").Style.NumberFormat.Format = "0.00%";

        // Детализация по проектам
        worksheet.Cell("A10").Value = "Детализация по проектам";
        worksheet.Cell("A10").Style.Font.Bold = true;

        worksheet.Cell("A11").Value = "Номер";
        worksheet.Cell("B11").Value = "Проект";
        worksheet.Cell("C11").Value = "Клиент";
        worksheet.Cell("D11").Value = "Смета";
        worksheet.Cell("E11").Value = "Затраты";
        worksheet.Cell("F11").Value = "Оплаты";
        worksheet.Cell("G11").Value = "Прибыль";
        worksheet.Cell("H11").Value = "Маржа";

        var headerRange = worksheet.Range("A11:H11");
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        for (int i = 0; i < report.Projects.Count; i++)
        {
            var row = i + 12;
            var item = report.Projects[i];
            worksheet.Cell(row, 1).Value = item.ProjectNumber;
            worksheet.Cell(row, 2).Value = item.ProjectName;
            worksheet.Cell(row, 3).Value = item.ClientName;
            worksheet.Cell(row, 4).Value = item.EstimatedCost;
            worksheet.Cell(row, 5).Value = item.ActualCost;
            worksheet.Cell(row, 6).Value = item.Payments;
            worksheet.Cell(row, 7).Value = item.Profit;
            worksheet.Cell(row, 8).Value = item.ProfitMargin / 100;
            worksheet.Cell(row, 8).Style.NumberFormat.Format = "0.00%";
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    #endregion
}
