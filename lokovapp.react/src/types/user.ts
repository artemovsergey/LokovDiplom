export interface UserListItem {
  id: string;
  username: string;
  fullName: string;
  email: string;
  role: string;
  isActive: boolean;
  lastLogin?: string;
  createdAt: string;
  projectsCount: number;
}

export interface UserData {
  id: string;
  username: string;
  fullName: string;
  email: string;
  role: string;
  createdAt: string;
  lastLogin?: string;
  isActive: boolean;
}

export interface CreateUserRequest {
  username: string;
  password: string;
  fullName: string;
  email: string;
  role: string;
}

export interface UpdateUserRequest {
  fullName: string;
  email: string;
  role?: string;
  isActive?: boolean;
}

export interface ResetPasswordRequest {
  newPassword: string;
  confirmNewPassword: string;
}