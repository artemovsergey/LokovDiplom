import api from './api';
import type { AuthRequest, AuthResponse, RegisterRequest, UserData } from '../types/auth';

export const authApi = {
  login: async (data: AuthRequest): Promise<AuthResponse> => {
    const response = await api.post<AuthResponse>('/Auth/login', data);
    return response.data;
  },

  register: async (data: RegisterRequest): Promise<AuthResponse> => {
    const response = await api.post<AuthResponse>('/Auth/register', data);
    return response.data;
  },

  logout: async (): Promise<void> => {
    await api.post('/Auth/logout');
  },

  getCurrentUser: async (): Promise<UserData> => {
    const response = await api.get<UserData>('/Auth/me');
    return response.data;
  },

  validateToken: async (): Promise<void> => {
    await api.get('/Auth/validate-token');
  },

  updateProfile: async (data: { fullName: string; email: string; phone?: string }): Promise<void> => {
    await api.put('/Auth/profile', data);
  },

  changePassword: async (data: { currentPassword: string; newPassword: string; confirmNewPassword: string }): Promise<void> => {
    await api.post('/Auth/change-password', data);
  }
};