import type { Client, ClientFilter, CreateClientDto, UpdateClientDto } from '../types/client';
import type { PagedResponse } from '../types/common';
import api from './api';

export const clientsApi = {
  getClients: async (filter?: ClientFilter): Promise<PagedResponse<Client>> => {
    const params = new URLSearchParams();
    if (filter?.search) params.append('Search', filter.search);
    if (filter?.status) params.append('Status', filter.status);
    if (filter?.source) params.append('Source', filter.source);
    if (filter?.category) params.append('Category', filter.category);
    if (filter?.createdFrom) params.append('CreatedFrom', filter.createdFrom);
    if (filter?.createdTo) params.append('CreatedTo', filter.createdTo);
    if (filter?.page) params.append('Page', filter.page.toString());
    if (filter?.pageSize) params.append('PageSize', filter.pageSize.toString());
    if (filter?.sortBy) params.append('SortBy', filter.sortBy);
    if (filter?.sortOrder) params.append('SortOrder', filter.sortOrder);

    const response = await api.get<PagedResponse<Client>>('/Clients', { params });
    return response.data;
  },

  getClient: async (id: string): Promise<Client> => {
    const response = await api.get<Client>(`/Clients/${id}`);
    return response.data;
  },

  createClient: async (client: CreateClientDto): Promise<Client> => {
    const response = await api.post<Client>('/Clients', client);
    return response.data;
  },

  updateClient: async (id: string, client: UpdateClientDto): Promise<Client> => {
    const response = await api.put<Client>(`/Clients/${id}`, client);
    return response.data;
  },

  deleteClient: async (id: string): Promise<void> => {
    await api.delete(`/Clients/${id}`);
  },

  archiveClient: async (id: string): Promise<void> => {
    await api.patch(`/Clients/${id}/archive`);
  },

  restoreClient: async (id: string): Promise<void> => {
    await api.patch(`/Clients/${id}/restore`);
  }
};