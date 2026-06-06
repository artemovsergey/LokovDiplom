export interface DashboardData {
  summaryCards: SummaryCards;
  monthlyRevenue: MonthlyRevenue[];
  projectTypeDistribution: ProjectTypeDistribution[];
  clientSources: ClientSource[];
  brigadeLoad: BrigadeLoad[];
  recentProjects: RecentProject[];
  topClients: TopClient[];
}

export interface SummaryCards {
  totalClients: number;
  activeProjects: number;
  completedProjects: number;
  totalRevenue: number;
  monthlyRevenue: number;
  outstandingDebt: number;
  newClientsThisMonth: number;
  overdueProjects: number;
}

export interface MonthlyRevenue {
  month: string;
  revenue: number;
  expenses: number;
  profit: number;
}

export interface ProjectTypeDistribution {
  type: string;
  label: string;
  count: number;
  totalAmount: number;
}

export interface ClientSource {
  source: string;
  label: string;
  count: number;
  conversionRate: number;
}

export interface BrigadeLoad {
  brigadeId: string;
  brigadeName: string;
  currentProjects: number;
  maxCapacity: number;
  loadPercentage: number;
}

export interface RecentProject {
  id: string;
  number: string;
  name: string;
  clientName: string;
  status: string;
  budget: number;
  completionPercent: number;
}

export interface TopClient {
  id: string;
  fullName: string;
  projectsCount: number;
  totalAmount: number;
}