import axios from 'axios';
import type { Client, CreateClientDto, UpdateClientDto } from '../types/client';

const API_URL = import.meta.env.VITE_API_URL;

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

export const clientService = {
  getClients: async (search?: string, status?: string): Promise<Client[]> => {
    const params = new URLSearchParams();
    if (search) params.append('search', search);
    if (status) params.append('status', status);
    
    const response = await api.get<Client[]>('/Clients', { params });
    return response.data;
  },

  getClient: async (id: number): Promise<Client> => {
    const response = await api.get<Client>(`/Clients/${id}`);
    return response.data;
  },

  createClient: async (client: CreateClientDto): Promise<Client> => {
    const response = await api.post<Client>('/Clients', client);
    return response.data;
  },

  updateClient: async (id: number, client: UpdateClientDto): Promise<Client> => {
    const response = await api.put<Client>(`/Clients/${id}`, client);
    return response.data;
  },

  deleteClient: async (id: number): Promise<void> => {
    await api.delete(`/clients/${id}`);
  }
};