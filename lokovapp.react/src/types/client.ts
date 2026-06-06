export interface Client {
  id: string;
  firstName: string;
  lastName: string;
  patronymic?: string;
  fullName: string;
  phone: string;
  additionalPhone?: string;
  email?: string;
  address: string;
  source: string;
  status: string;
  category: string;
  projectsCount: number;
  totalPayments: number;
  debt: number;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateClientDto {
  firstName: string;
  lastName: string;
  patronymic?: string;
  phone: string;
  additionalPhone?: string;
  email?: string;
  address: string;
  source?: string;
  category?: string;
}

export interface UpdateClientDto {
  firstName: string;
  lastName: string;
  patronymic?: string;
  phone: string;
  additionalPhone?: string;
  email?: string;
  address: string;
  source?: string;
  status?: string;
  category?: string;
}

export interface ClientFilter {
  search?: string;
  status?: string;
  source?: string;
  category?: string;
  createdFrom?: string;
  createdTo?: string;
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortOrder?: string;
}