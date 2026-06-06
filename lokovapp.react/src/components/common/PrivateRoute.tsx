import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { LoadingSpinner } from './LoadingSpinner';
import { useAuth } from '../../hooks/useAuth';

export const PrivateRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const { isAuthenticated, isLoading } = useAuth();
    const location = useLocation();

    if (isLoading) {
        return <LoadingSpinner size="lg" text="Загрузка..." />;
    }

    if (!isAuthenticated) {
        return <Navigate to="/login" state={{ from: location }} replace />;
    }

    return <>{children}</>;
};