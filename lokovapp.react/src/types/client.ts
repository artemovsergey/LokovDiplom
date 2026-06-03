export interface Client {
  id: number;
  firstName: string;
  lastName: string;
  patronymic?: string;
  phone: string;
  email?: string;
  address?: string;
  status: ClientStatus;
  projectsCount: number;
  createdAt: string;
  updatedAt?: string;
  fullName: string;
}

export enum ClientStatus {
  Potential = 'Potential',
  Active = 'Active',
  Inactive = 'Inactive',
  Completed = 'Completed'
}

export type ClientStatus2 = 'Potential' | 'Active' | 'Inactive' | 'Completed';

export interface CreateClientDto {
  firstName: string;
  lastName: string;
  patronymic?: string;
  phone: string;
  email?: string;
  address?: string;
}

export interface UpdateClientDto {
  firstName: string;
  lastName: string;
  patronymic?: string;
  phone: string;
  email?: string;
  address?: string;
  status: ClientStatus;
}