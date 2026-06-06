import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { authApi } from '../api/auth.api';
import { Button } from '../components/common/Button';
import { LoadingSpinner } from '../components/common/LoadingSpinner';

export const ProfilePage: React.FC = () => {
    const navigate = useNavigate();
    const { user, logout } = useAuth();
    const [loading, setLoading] = useState(false);
    const [message, setMessage] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);

    // Форма изменения пароля
    const [passwordForm, setPasswordForm] = useState({
        currentPassword: '',
        newPassword: '',
        confirmNewPassword: ''
    });

    // Форма редактирования профиля
    const [profileForm, setProfileForm] = useState({
        fullName: user?.fullName || '',
        email: user?.email || '',
        phone: ''
    });

    const [editMode, setEditMode] = useState(false);

    useEffect(() => {
        if (user) {
            setProfileForm({
                fullName: user.fullName,
                email: user.email,
                phone: ''
            });
        }
    }, [user]);

    const handleUpdateProfile = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        setError(null);
        setMessage(null);

        try {
            await authApi.updateProfile({
                fullName: profileForm.fullName,
                email: profileForm.email,
                phone: profileForm.phone || undefined
            });
            setMessage('Профиль обновлен');
            setEditMode(false);
        } catch (err: any) {
            setError(err.response?.data?.message || 'Ошибка при обновлении профиля');
        } finally {
            setLoading(false);
        }
    };

    const handleChangePassword = async (e: React.FormEvent) => {
        e.preventDefault();
        
        if (passwordForm.newPassword !== passwordForm.confirmNewPassword) {
            setError('Новые пароли не совпадают');
            return;
        }

        if (passwordForm.newPassword.length < 6) {
            setError('Пароль должен содержать минимум 6 символов');
            return;
        }

        setLoading(true);
        setError(null);
        setMessage(null);

        try {
            await authApi.changePassword({
                currentPassword: passwordForm.currentPassword,
                newPassword: passwordForm.newPassword,
                confirmNewPassword: passwordForm.confirmNewPassword
            });
            setMessage('Пароль успешно изменен');
            setPasswordForm({
                currentPassword: '',
                newPassword: '',
                confirmNewPassword: ''
            });
        } catch (err: any) {
            setError(err.response?.data?.message || 'Ошибка при смене пароля');
        } finally {
            setLoading(false);
        }
    };

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    const roleLabels: Record<string, string> = {
        Admin: 'Администратор',
        Manager: 'Менеджер',
        Foreman: 'Прораб',
        Accountant: 'Бухгалтер',
        Brigadier: 'Бригадир'
    };

    if (!user) {
        return <LoadingSpinner text="Загрузка профиля..." />;
    }

    return (
        <div className="max-w-2xl mx-auto space-y-6">
            <div>
                <button onClick={() => navigate('/dashboard')} className="text-blue-600 hover:text-blue-700 text-sm mb-2 block">
                    ← Назад
                </button>
                <h1 className="text-2xl font-bold text-gray-900">Профиль пользователя</h1>
            </div>

            {/* Уведомления */}
            {message && (
                <div className="bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded-lg text-sm">
                    {message}
                </div>
            )}
            {error && (
                <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg text-sm">
                    {error}
                </div>
            )}

            {/* Информация о пользователе */}
            <div className="bg-white rounded-lg shadow-md p-6 border border-gray-200">
                <div className="flex items-center space-x-4 mb-6">
                    <div className="w-16 h-16 bg-blue-600 rounded-full flex items-center justify-center text-white text-2xl font-bold">
                        {user.fullName.charAt(0)}
                    </div>
                    <div>
                        <h2 className="text-xl font-semibold text-gray-900">{user.fullName}</h2>
                        <p className="text-gray-500">{roleLabels[user.role] || user.role}</p>
                    </div>
                </div>

                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 mb-6">
                    <div>
                        <p className="text-xs text-gray-500">Логин</p>
                        <p className="font-medium">{user.username}</p>
                    </div>
                    <div>
                        <p className="text-xs text-gray-500">Email</p>
                        <p className="font-medium">{user.email}</p>
                    </div>
                    <div>
                        <p className="text-xs text-gray-500">Роль</p>
                        <p className="font-medium">{roleLabels[user.role] || user.role}</p>
                    </div>
                    <div>
                        <p className="text-xs text-gray-500">Дата регистрации</p>
                        <p className="font-medium">{new Date(user.createdAt).toLocaleDateString('ru-RU')}</p>
                    </div>
                    {user.lastLogin && (
                        <div>
                            <p className="text-xs text-gray-500">Последний вход</p>
                            <p className="font-medium">{new Date(user.lastLogin).toLocaleString('ru-RU')}</p>
                        </div>
                    )}
                </div>

                <Button variant="secondary" onClick={() => setEditMode(!editMode)}>
                    {editMode ? 'Отменить' : '✏️ Редактировать профиль'}
                </Button>
            </div>

            {/* Форма редактирования профиля */}
            {editMode && (
                <div className="bg-white rounded-lg shadow-md p-6 border border-gray-200">
                    <h3 className="text-lg font-semibold text-gray-900 mb-4">Редактирование профиля</h3>
                    <form onSubmit={handleUpdateProfile} className="space-y-4">
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Полное имя</label>
                            <input
                                type="text"
                                value={profileForm.fullName}
                                onChange={(e) => setProfileForm(prev => ({ ...prev, fullName: e.target.value }))}
                                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                                required
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Email</label>
                            <input
                                type="email"
                                value={profileForm.email}
                                onChange={(e) => setProfileForm(prev => ({ ...prev, email: e.target.value }))}
                                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                                required
                            />
                        </div>
                        <div className="flex justify-end">
                            <Button type="submit" loading={loading}>
                                Сохранить
                            </Button>
                        </div>
                    </form>
                </div>
            )}

            {/* Форма смены пароля */}
            <div className="bg-white rounded-lg shadow-md p-6 border border-gray-200">
                <h3 className="text-lg font-semibold text-gray-900 mb-4">Сменить пароль</h3>
                <form onSubmit={handleChangePassword} className="space-y-4">
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Текущий пароль</label>
                        <input
                            type="password"
                            value={passwordForm.currentPassword}
                            onChange={(e) => setPasswordForm(prev => ({ ...prev, currentPassword: e.target.value }))}
                            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                            required
                        />
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Новый пароль</label>
                        <input
                            type="password"
                            value={passwordForm.newPassword}
                            onChange={(e) => setPasswordForm(prev => ({ ...prev, newPassword: e.target.value }))}
                            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                            required
                            minLength={6}
                        />
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Подтвердите новый пароль</label>
                        <input
                            type="password"
                            value={passwordForm.confirmNewPassword}
                            onChange={(e) => setPasswordForm(prev => ({ ...prev, confirmNewPassword: e.target.value }))}
                            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                            required
                        />
                    </div>
                    <div className="flex justify-end">
                        <Button type="submit" loading={loading}>
                            Сменить пароль
                        </Button>
                    </div>
                </form>
            </div>

            {/* Кнопка выхода */}
            <div className="bg-white rounded-lg shadow-md p-6 border border-gray-200">
                <Button variant="danger" onClick={handleLogout}>
                    🚪 Выйти из системы
                </Button>
            </div>
        </div>
    );
};