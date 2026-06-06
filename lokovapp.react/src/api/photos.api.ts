import api from './api';
import type { Photo, PhotoFilter } from '../types/photo';
import type { PagedResponse } from '../types/common';

export const photosApi = {
  getProjectPhotos: async (projectId: string, filter?: PhotoFilter): Promise<PagedResponse<Photo>> => {
    const params = new URLSearchParams();
    if (filter?.category) params.append('Category', filter.category);
    if (filter?.stageId) params.append('StageId', filter.stageId);
    if (filter?.uploadedFrom) params.append('UploadedFrom', filter.uploadedFrom);
    if (filter?.uploadedTo) params.append('UploadedTo', filter.uploadedTo);
    if (filter?.page) params.append('Page', filter.page.toString());
    if (filter?.pageSize) params.append('PageSize', filter.pageSize.toString());
    if (filter?.sortBy) params.append('SortBy', filter.sortBy);
    if (filter?.sortOrder) params.append('SortOrder', filter.sortOrder);

    const response = await api.get<PagedResponse<Photo>>(`/projects/${projectId}/Photos`, { params });
    return response.data;
  },

  getPhoto: async (projectId: string, photoId: string): Promise<Photo> => {
    const response = await api.get<Photo>(`/projects/${projectId}/Photos/${photoId}`);
    return response.data;
  },

  uploadPhoto: async (
    projectId: string,
    file: File,
    data?: {
      category?: string;
      description?: string;
      stageId?: string;
      sortOrder?: number;
    }
  ): Promise<Photo> => {
    const formData = new FormData();
    formData.append('File', file);
    if (data?.category) formData.append('Category', data.category);
    if (data?.description) formData.append('Description', data.description);
    if (data?.stageId) formData.append('StageId', data.stageId);
    if (data?.sortOrder !== undefined) formData.append('SortOrder', data.sortOrder.toString());

    const response = await api.post<Photo>(`/projects/${projectId}/Photos/upload`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return response.data;
  },

  uploadMultiplePhotos: async (
    projectId: string,
    files: File[],
    data?: {
      category?: string;
      description?: string;
      stageId?: string;
    }
  ): Promise<Photo[]> => {
    const formData = new FormData();
    files.forEach((file) => formData.append('Files', file));
    if (data?.category) formData.append('Category', data.category);
    if (data?.description) formData.append('Description', data.description);
    if (data?.stageId) formData.append('StageId', data.stageId);

    const response = await api.post<Photo[]>(`/projects/${projectId}/Photos/upload-multiple`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return response.data;
  },

  updatePhoto: async (
    projectId: string,
    photoId: string,
    data: {
      description?: string;
      category?: string;
      sortOrder?: number;
      stageId?: string;
    }
  ): Promise<Photo> => {
    const response = await api.put<Photo>(`/projects/${projectId}/Photos/${photoId}`, data);
    return response.data;
  },

  deletePhoto: async (projectId: string, photoId: string): Promise<void> => {
    await api.delete(`/projects/${projectId}/Photos/${photoId}`);
  },

  deleteMultiplePhotos: async (projectId: string, photoIds: string[]): Promise<void> => {
    await api.delete(`/projects/${projectId}/Photos/batch`, { data: photoIds });
  },

  updatePhotoOrder: async (projectId: string, photoIds: string[]): Promise<void> => {
    await api.put(`/projects/${projectId}/Photos/order`, photoIds);
  },

  getPhotoUrl: (projectId: string, photoId: string): string => {
    const baseUrl = api.defaults.baseURL || '';
    return `${baseUrl}/projects/${projectId}/Photos/${photoId}/file`;
  },

  getThumbnailUrl: (projectId: string, photoId: string): string => {
    const baseUrl = api.defaults.baseURL || '';
    return `${baseUrl}/projects/${projectId}/Photos/${photoId}/thumbnail`;
  }
};