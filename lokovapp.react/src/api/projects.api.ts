import api from './api';
import type { Project, CreateProjectDto, UpdateProjectDto, ProjectFilter } from '../types/project';
import type { PagedResponse } from '../types/common';

export const projectsApi = {
  getProjects: async (filter?: ProjectFilter): Promise<PagedResponse<Project>> => {
    const params = new URLSearchParams();
    if (filter?.search) params.append('Search', filter.search);
    if (filter?.status) params.append('Status', filter.status);
    if (filter?.type) params.append('Type', filter.type);
    if (filter?.clientId) params.append('ClientId', filter.clientId);
    if (filter?.startFrom) params.append('StartFrom', filter.startFrom);
    if (filter?.startTo) params.append('StartTo', filter.startTo);
    if (filter?.page) params.append('Page', filter.page.toString());
    if (filter?.pageSize) params.append('PageSize', filter.pageSize.toString());

    const response = await api.get<PagedResponse<Project>>('/Projects', { params });
    return response.data;
  },

  getProject: async (id: string): Promise<Project> => {
    const response = await api.get<Project>(`/Projects/${id}`);
    return response.data;
  },

  createProject: async (project: CreateProjectDto): Promise<Project> => {
    // Очищаем данные перед отправкой
    const dataToSend: Record<string, any> = {
      clientId: project.clientId,
      name: project.name.trim(),
      type: project.type,
      address: project.address.trim(),
      estimatedCost: project.estimatedCost || 0,
    };

    // Добавляем опциональные поля только если они заполнены
    if (project.description?.trim()) {
      dataToSend.description = project.description.trim();
    }
    if (project.startDate) {
      dataToSend.startDate = new Date(project.startDate).toISOString();
    }
    if (project.plannedEndDate) {
      dataToSend.plannedEndDate = new Date(project.plannedEndDate).toISOString();
    }
    if (project.brigadeId) {
      dataToSend.brigadeId = project.brigadeId;
    }

    console.log('Sending create project request:', dataToSend);
    
    const response = await api.post<Project>('/Projects', dataToSend);
    return response.data;
  },

  updateProject: async (id: string, project: UpdateProjectDto): Promise<Project> => {
    const dataToSend: Record<string, any> = {
      name: project.name.trim(),
      type: project.type,
      address: project.address.trim(),
      estimatedCost: project.estimatedCost || 0,
    };

    if (project.description?.trim()) {
      dataToSend.description = project.description.trim();
    }
    if (project.status) {
      dataToSend.status = project.status;
    }
    if (project.startDate) {
      dataToSend.startDate = new Date(project.startDate).toISOString();
    }
    if (project.plannedEndDate) {
      dataToSend.plannedEndDate = new Date(project.plannedEndDate).toISOString();
    }
    if (project.brigadeId) {
      dataToSend.brigadeId = project.brigadeId;
    }

    const response = await api.put<Project>(`/Projects/${id}`, dataToSend);
    return response.data;
  },

  deleteProject: async (id: string): Promise<void> => {
    await api.delete(`/Projects/${id}`);
  },

  updateProjectStatus: async (id: string, status: string): Promise<void> => {
    await api.patch(`/Projects/${id}/status`, { status });
  },

  getClientProjects: async (clientId: string, filter?: ProjectFilter): Promise<PagedResponse<Project>> => {
    const params = new URLSearchParams();
    if (filter?.search) params.append('Search', filter.search);
    if (filter?.status) params.append('Status', filter.status);
    if (filter?.type) params.append('Type', filter.type);
    if (filter?.page) params.append('Page', filter.page.toString());
    if (filter?.pageSize) params.append('PageSize', filter.pageSize.toString());

    const response = await api.get<PagedResponse<Project>>(`/Clients/${clientId}/projects`, { params });
    return response.data;
  }
};