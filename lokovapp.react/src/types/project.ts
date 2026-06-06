export interface Project {
  id: string;
  number: string;
  clientId: string;
  clientName: string;
  type: string;
  typeDisplay: string;
  name: string;
  description?: string;
  address: string;
  estimatedCost: number;
  actualCost?: number;
  paidAmount: number;
  debt: number;
  completionPercentage: number;
  status: string;
  startDate?: string;
  plannedEndDate?: string;
  actualEndDate?: string;
  brigadeName?: string;
  createdAt: string;
}

export interface CreateProjectDto {
  clientId: string;
  name: string;
  description?: string;
  type: string;
  address: string;
  estimatedCost: number;
  startDate?: string;
  plannedEndDate?: string;
  brigadeId?: string;
}

export interface UpdateProjectDto {
  name: string;
  description?: string;
  type: string;
  address: string;
  estimatedCost: number;
  status?: string;
  startDate?: string;
  plannedEndDate?: string;
  brigadeId?: string;
}

export interface ProjectFilter {
  search?: string;
  status?: string;
  type?: string;
  clientId?: string;
  startFrom?: string;
  startTo?: string;
  page?: number;
  pageSize?: number;
}