export interface AuthRequest {
  username: string;
  password: string;
  rememberMe: boolean;
}

export interface AuthResponse {
  success: boolean;
  message: string;
  user?: UserData;
  token?: string;
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

export interface RegisterRequest {
  username: string;
  password: string;
  confirmPassword: string;
  fullName: string;
  email: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}

export interface UpdateProfileRequest {
  fullName: string;
  email: string;
  phone?: string;
}