namespace LokovApp.Dtos;

#region Dashboard DTOs

public class DashboardDataDto
{
    public SummaryCardsDto SummaryCards { get; set; } = new();
    public List<MonthlyRevenueDto> MonthlyRevenue { get; set; } = new();
    public List<ProjectTypeDistributionDto> ProjectTypeDistribution { get; set; } = new();
    public List<ClientSourceDto> ClientSources { get; set; } = new();
    public List<BrigadeLoadDto> BrigadeLoad { get; set; } = new();
    public List<RecentProjectDto> RecentProjects { get; set; } = new();
    public List<TopClientDto> TopClients { get; set; } = new();
}

public class SummaryCardsDto
{
    public int TotalClients { get; set; }
    public int ActiveProjects { get; set; }
    public int CompletedProjects { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal OutstandingDebt { get; set; }
    public int NewClientsThisMonth { get; set; }
    public int OverdueProjects { get; set; }
}

public class MonthlyRevenueDto
{
    public string Month { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public decimal Expenses { get; set; }
    public decimal Profit { get; set; }
}

public class ProjectTypeDistributionDto
{
    public string Type { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalAmount { get; set; }
}

public class ClientSourceDto
{
    public string Source { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal ConversionRate { get; set; }
}

public class BrigadeLoadDto
{
    public Guid BrigadeId { get; set; }
    public string BrigadeName { get; set; } = string.Empty;
    public int CurrentProjects { get; set; }
    public int MaxCapacity { get; set; }
    public int LoadPercentage { get; set; }
}

public class RecentProjectDto
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public int CompletionPercent { get; set; }
}

public class TopClientDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int ProjectsCount { get; set; }
    public decimal TotalAmount { get; set; }
}

#endregion

#region Report DTOs

public class ClientsReportDto
{
    public DateTime GeneratedAt { get; set; }
    public string Period { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public List<ClientReportItemDto> Items { get; set; } = new();
    public Dictionary<string, int> ByStatus { get; set; } = new();
    public Dictionary<string, int> BySource { get; set; } = new();
}

public class ClientReportItemDto
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public int ProjectsCount { get; set; }
    public decimal TotalPayments { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ProjectsReportDto
{
    public DateTime GeneratedAt { get; set; }
    public string Period { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public decimal TotalEstimatedCost { get; set; }
    public decimal TotalActualCost { get; set; }
    public List<ProjectReportItemDto> Items { get; set; } = new();
    public Dictionary<string, int> ByStatus { get; set; } = new();
    public Dictionary<string, int> ByType { get; set; } = new();
}

public class ProjectReportItemDto
{
    public string Number { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal EstimatedCost { get; set; }
    public decimal ActualCost { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal Debt { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class FinancialReportDto
{
    public DateTime GeneratedAt { get; set; }
    public string Period { get; set; } = string.Empty;
    public FinancialSummaryDto Summary { get; set; } = new();
    public List<FinancialProjectItemDto> Projects { get; set; } = new();
    public List<MonthlyFinancialDto> MonthlyBreakdown { get; set; } = new();
}

public class FinancialSummaryDto
{
    public int TotalProjects { get; set; }
    public decimal TotalEstimatedCost { get; set; }
    public decimal TotalActualCost { get; set; }
    public decimal TotalPayments { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal TotalDebt { get; set; }
    public decimal TotalProfit { get; set; }
    public decimal AverageProfitMargin { get; set; }
}

public class FinancialProjectItemDto
{
    public string ProjectNumber { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public decimal EstimatedCost { get; set; }
    public decimal ActualCost { get; set; }
    public decimal Payments { get; set; }
    public decimal Expenses { get; set; }
    public decimal Debt { get; set; }
    public decimal Profit { get; set; }
    public decimal ProfitMargin { get; set; }
}

public class MonthlyFinancialDto
{
    public string Month { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public decimal Expenses { get; set; }
    public decimal Profit { get; set; }
}

#endregion
