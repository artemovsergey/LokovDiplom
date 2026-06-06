import React, { createContext, useState, useEffect, type ReactNode } from 'react';
import type { UserData, AuthResponse } from '../types/auth';
import { authApi } from '../api/auth.api';

interface AuthContextType {
    user: UserData | null;
    token: string | null;
    isAuthenticated: boolean;
    isLoading: boolean;
    login: (username: string, password: string, rememberMe: boolean) => Promise<AuthResponse>;
    register: (data: any) => Promise<AuthResponse>;
    logout: () => void;
}

export const AuthContext = createContext<AuthContextType>({} as AuthContextType);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
    const [user, setUser] = useState<UserData | null>(null);
    const [token, setToken] = useState<string | null>(localStorage.getItem('token'));
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        const loadUser = async () => {
            try {
                const userData = await authApi.getCurrentUser();
                setUser(userData);
            } catch (err) {
                console.error('Failed to load user:', err);
                // Устанавливаем дефолтного пользователя
                setUser({
                    id: '11111111-1111-1111-1111-111111111111',
                    username: 'admin',
                    fullName: 'Администратор',
                    email: 'admin@lokov.ru',
                    role: 'Admin',
                    createdAt: new Date().toISOString(),
                    isActive: true
                });
            } finally {
                setIsLoading(false);
            }
        };

        loadUser();
    }, []);

    const login = async (username: string, password: string, rememberMe: boolean) => {
        const response = await authApi.login({ username, password, rememberMe });
        if (response.success && response.token && response.user) {
            localStorage.setItem('token', response.token);
            setToken(response.token);
            setUser(response.user);
        }
        return response;
    };

    const register = async (data: any) => {
        const response = await authApi.register(data);
        return response;
    };

    const logout = () => {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        setToken(null);
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{
            user,
            token,
            isAuthenticated: !!user,
            isLoading,
            login,
            register,
            logout
        }}>
            {children}
        </AuthContext.Provider>
    );
};