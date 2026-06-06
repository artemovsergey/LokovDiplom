import api from './api';
import type { UserListItem, UserData, CreateUserRequest, UpdateUserRequest, ResetPasswordRequest } from '../types/user';

export const usersApi = {
  getUsers: async (): Promise<UserListItem[]> => {
    const response = await api.get<UserListItem[]>('/Users');
    return response.data;
  },

  getUser: async (id: string): Promise<UserData> => {
    const response = await api.get<UserData>(`/Users/${id}`);
    return response.data;
  },

  createUser: async (data: CreateUserRequest): Promise<UserData> => {
    const response = await api.post<UserData>('/Users', data);
    return response.data;
  },

  updateUser: async (id: string, data: UpdateUserRequest): Promise<UserData> => {
    const response = await api.put<UserData>(`/Users/${id}`, data);
    return response.data;
  },

  deleteUser: async (id: string): Promise<void> => {
      const response = await api.delete(`/Users/${id}`);
      return response.data;
  },

  toggleUserStatus: async (id: string): Promise<void> => {
    await api.patch(`/Users/${id}/toggle-status`);
  },

  resetPassword: async (id: string, data: ResetPasswordRequest): Promise<void> => {
    await api.post(`/Users/${id}/reset-password`, data);
  },

  checkUsername: async (username: string): Promise<{ exists: boolean }> => {
    const response = await api.get<{ exists: boolean }>('/Users/check-username', { params: { username } });
    return response.data;
  },

  checkEmail: async (email: string): Promise<{ exists: boolean }> => {
    const response = await api.get<{ exists: boolean }>('/Users/check-email', { params: { email } });
    return response.data;
  }
};