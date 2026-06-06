import api from './api';

export interface Brigade {
  id: string;
  name: string;
  foremanName: string;
  phone?: string;
  workersCount: number;
  specialization: string;
  isActive: boolean;
}

export const brigadesApi = {
  getBrigades: async (): Promise<Brigade[]> => {
    const response = await api.get<Brigade[]>('/Brigades');
    return response.data;
  }
};