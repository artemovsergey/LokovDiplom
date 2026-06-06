import type { DashboardData } from '../types/report';
import api from './api';

export const reportsApi = {
  getDashboard: async (): Promise<DashboardData> => {
    const response = await api.get<DashboardData>('/Reports/dashboard');
    return response.data;
  },

  exportClientsPdf: async (): Promise<Blob> => {
    const response = await api.get('/Reports/clients/export/pdf', { responseType: 'blob' });
    return response.data;
  },

  exportClientsExcel: async (): Promise<Blob> => {
    const response = await api.get('/Reports/clients/export/excel', { responseType: 'blob' });
    return response.data;
  },

  exportProjectsPdf: async (): Promise<Blob> => {
    const response = await api.get('/Reports/projects/export/pdf', { responseType: 'blob' });
    return response.data;
  },

  exportProjectsExcel: async (): Promise<Blob> => {
    const response = await api.get('/Reports/projects/export/excel', { responseType: 'blob' });
    return response.data;
  },

  exportFinancialPdf: async (): Promise<Blob> => {
    const response = await api.get('/Reports/financial/export/pdf', { responseType: 'blob' });
    return response.data;
  },

  exportFinancialExcel: async (): Promise<Blob> => {
    const response = await api.get('/Reports/financial/export/excel', { responseType: 'blob' });
    return response.data;
  }
};