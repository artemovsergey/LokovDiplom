import React, { useEffect, useState, useCallback } from 'react';
import { usersApi } from '../api/users.api';
import type { UserListItem, CreateUserRequest, UpdateUserRequest } from '../types/user';
import { Button } from '../components/common/Button';
import { LoadingSpinner } from '../components/common/LoadingSpinner';
import { ErrorMessage } from '../components/common/ErrorMessage';
import { EmptyState } from '../components/common/EmptyState';
import { Modal } from '../components/common/Modal';
import { useAuth } from '../hooks/useAuth';

export const UsersPage: React.FC = () => {
    const { user: currentUser } = useAuth();
    const [users, setUsers] = useState<UserListItem[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [showForm, setShowForm] = useState(false);
    const [editingUser, setEditingUser] = useState<UserListItem | null>(null);
    const [saving, setSaving] = useState(false);
    const [formError, setFormError] = useState<string | null>(null);

    // Форма создания/редактирования
    const [formData, setFormData] = useState<CreateUserRequest>({
        username: '',
        password: '',
        fullName: '',
        email: '',
        role: 'Manager'
    });
    const [formErrors, setFormErrors] = useState<Record<string, string>>({});

    const loadUsers = useCallback(async () => {
        try {
            setLoading(true);
            setError(null);
            const data = await usersApi.getUsers();
            setUsers(data);
        } catch (err) {
            setError('Ошибка при загрузке пользователей');
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        loadUsers();
    }, [loadUsers]);

    const handleAddNew = () => {
        setEditingUser(null);
        setFormData({
            username: '',
            password: '',
            fullName: '',
            email: '',
            role: 'Manager'
        });
        setFormErrors({});
        setFormError(null);
        setShowForm(true);
    };

    const handleEdit = (user: UserListItem) => {
        setEditingUser(user);
        setFormData({
            username: user.username,
            password: '',
            fullName: user.fullName,
            email: user.email,
            role: user.role
        });
        setFormErrors({});
        setFormError(null);
        setShowForm(true);
    };

    const handleCloseForm = () => {
        setShowForm(false);
        setEditingUser(null);
    };

    const validateForm = (): boolean => {
        const errors: Record<string, string> = {};

        if (!editingUser) {
            if (!formData.username.trim()) errors.username = 'Логин обязателен';
            if (!formData.password || formData.password.length < 6) errors.password = 'Пароль минимум 6 символов';
        }
        if (!formData.fullName.trim()) errors.fullName = 'Имя обязательно';
        if (!formData.email.trim()) errors.email = 'Email обязателен';
        else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) errors.email = 'Неверный формат email';

        setFormErrors(errors);
        return Object.keys(errors).length === 0;
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!validateForm()) return;

        setSaving(true);
        setFormError(null);

        try {
            if (editingUser) {
                const updateData: UpdateUserRequest = {
                    fullName: formData.fullName,
                    email: formData.email,
                    role: formData.role,
                    isActive: editingUser.isActive
                };
                await usersApi.updateUser(editingUser.id, updateData);
            } else {
                await usersApi.createUser(formData);
            }
            handleCloseForm();
            loadUsers();
        } catch (err: any) {
            setFormError(err.response?.data?.message || 'Ошибка при сохранении');
        } finally {
            setSaving(false);
        }
    };

    const handleToggleStatus = async (user: UserListItem) => {
        if (user.id === currentUser?.id) {
            alert('Нельзя заблокировать самого себя');
            return;
        }

        if (!window.confirm(`Вы уверены, что хотите ${user.isActive ? 'заблокировать' : 'разблокировать'} пользователя?`)) {
            return;
        }

        try {
            await usersApi.toggleUserStatus(user.id);
            loadUsers();
        } catch (err) {
            alert('Ошибка при изменении статуса');
        }
    };

    const handleResetPassword = async (user: UserListItem) => {
        const newPassword = prompt('Введите новый пароль (минимум 6 символов):');
        if (!newPassword || newPassword.length < 6) {
            alert('Пароль должен содержать минимум 6 символов');
            return;
        }

        const confirmPassword = prompt('Подтвердите новый пароль:');
        if (newPassword !== confirmPassword) {
            alert('Пароли не совпадают');
            return;
        }

        try {
            await usersApi.resetPassword(user.id, {
                newPassword,
                confirmNewPassword: confirmPassword
            });
            alert('Пароль успешно сброшен');
        } catch (err) {
            alert('Ошибка при сбросе пароля');
        }
    };

    const handleDelete = async (user: UserListItem) => {
        if (user.id === currentUser?.id) {
            alert('Нельзя удалить самого себя');
            return;
        }

        if (!window.confirm(`Вы уверены, что хотите удалить пользователя "${user.fullName}"? Это действие нельзя отменить.`)) {
            return;
        }

        try {
            await usersApi.deleteUser(user.id);
            // Немедленно удаляем из локального состояния
            setUsers(prev => prev.filter(u => u.id !== user.id));
            alert('Пользователь удален');
        } catch (err: any) {
            const errorMessage = err.response?.data?.message || 'Ошибка при удалении пользователя';
            alert(errorMessage);
        }
    };

    const roleLabels: Record<string, string> = {
        Admin: 'Администратор',
        Manager: 'Менеджер',
        Foreman: 'Прораб',
        Accountant: 'Бухгалтер',
        Brigadier: 'Бригадир'
    };

    const roleColors: Record<string, string> = {
        Admin: 'bg-red-100 text-red-800',
        Manager: 'bg-blue-100 text-blue-800',
        Foreman: 'bg-green-100 text-green-800',
        Accountant: 'bg-purple-100 text-purple-800',
        Brigadier: 'bg-yellow-100 text-yellow-800'
    };

    if (loading) return <LoadingSpinner text="Загрузка пользователей..." />;
    if (error) return <ErrorMessage message={error} onRetry={loadUsers} />;

    return (
        <div className="space-y-6">
            <div className="flex justify-between items-center">
                <h1 className="text-2xl font-bold text-gray-900">Пользователи</h1>
                <Button onClick={handleAddNew}>
                    <svg className="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
                    </svg>
                    Добавить пользователя
                </Button>
            </div>

            {users.length === 0 ? (
                <EmptyState
                    title="Нет пользователей"
                    description="Добавьте первого пользователя"
                    action={<Button onClick={handleAddNew}>Добавить пользователя</Button>}
                />
            ) : (
                <div className="bg-white rounded-lg shadow border border-gray-200 overflow-hidden">
                    <div className="overflow-x-auto">
                        <table className="min-w-full divide-y divide-gray-200">
                            <thead className="bg-gray-50">
                                <tr>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Пользователь</th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Роль</th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Статус</th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Последний вход</th>
                                    <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Действия</th>
                                </tr>
                            </thead>
                            <tbody className="bg-white divide-y divide-gray-200">
                                {users.map((user) => (
                                    <tr key={user.id} className="hover:bg-gray-50">
                                        <td className="px-6 py-4 whitespace-nowrap">
                                            <div className="flex items-center">
                                                <div className="w-8 h-8 bg-blue-100 text-blue-700 rounded-full flex items-center justify-center font-semibold text-sm mr-3">
                                                    {user.fullName.charAt(0)}
                                                </div>
                                                <div>
                                                    <div className="font-medium text-gray-900">{user.fullName}</div>
                                                    <div className="text-sm text-gray-500">{user.email}</div>
                                                </div>
                                            </div>
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap">
                                            <span className={`px-2 py-1 text-xs rounded-full ${roleColors[user.role] || 'bg-gray-100 text-gray-800'}`}>
                                                {roleLabels[user.role] || user.role}
                                            </span>
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap">
                                            <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${user.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                                                }`}>
                                                {user.isActive ? 'Активен' : 'Заблокирован'}
                                            </span>
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                            {user.lastLogin ? new Date(user.lastLogin).toLocaleString('ru-RU') : 'Никогда'}
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap text-right text-sm">
                                            <div className="flex items-center justify-end space-x-2">
                                                <button
                                                    onClick={() => handleEdit(user)}
                                                    className="text-blue-600 hover:text-blue-900 font-medium"
                                                >
                                                    ✏️
                                                </button>
                                                {user.id !== currentUser?.id && (
                                                    <>
                                                        <button
                                                            onClick={() => handleToggleStatus(user)}
                                                            className={`font-medium ${user.isActive ? 'text-yellow-600 hover:text-yellow-900' : 'text-green-600 hover:text-green-900'}`}
                                                            title={user.isActive ? 'Заблокировать' : 'Разблокировать'}
                                                        >
                                                            {user.isActive ? '🔒' : '🔓'}
                                                        </button>
                                                        <button
                                                            onClick={() => handleResetPassword(user)}
                                                            className="text-purple-600 hover:text-purple-900 font-medium"
                                                            title="Сбросить пароль"
                                                        >
                                                            🔑
                                                        </button>
                                                        <button
                                                            onClick={() => handleDelete(user)}
                                                            className="text-red-600 hover:text-red-900 font-medium"
                                                            title="Удалить"
                                                        >
                                                            🗑️
                                                        </button>
                                                    </>
                                                )}
                                                {user.id === currentUser?.id && (
                                                    <span className="text-gray-400 text-xs">Это вы</span>
                                                )}
                                            </div>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                </div>
            )}

            {/* Модальное окно формы */}
            {showForm && (
                <Modal
                    isOpen={showForm}
                    onClose={handleCloseForm}
                    title={editingUser ? 'Редактирование пользователя' : 'Новый пользователь'}
                    size="md"
                    footer={
                        <>
                            <Button variant="secondary" onClick={handleCloseForm}>Отмена</Button>
                            <Button onClick={handleSubmit} loading={saving}>
                                {editingUser ? 'Сохранить' : 'Создать'}
                            </Button>
                        </>
                    }
                >
                    <form onSubmit={handleSubmit} className="space-y-4">
                        {/* Логин (только при создании) */}
                        {!editingUser && (
                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-1">Логин *</label>
                                <input
                                    type="text"
                                    value={formData.username}
                                    onChange={(e) => setFormData(prev => ({ ...prev, username: e.target.value }))}
                                    className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 ${formErrors.username ? 'border-red-300' : 'border-gray-300'}`}
                                />
                                {formErrors.username && <p className="text-red-500 text-xs mt-1">{formErrors.username}</p>}
                            </div>
                        )}

                        {/* Пароль (только при создании) */}
                        {!editingUser && (
                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-1">Пароль *</label>
                                <input
                                    type="password"
                                    value={formData.password}
                                    onChange={(e) => setFormData(prev => ({ ...prev, password: e.target.value }))}
                                    className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 ${formErrors.password ? 'border-red-300' : 'border-gray-300'}`}
                                    placeholder="Минимум 6 символов"
                                />
                                {formErrors.password && <p className="text-red-500 text-xs mt-1">{formErrors.password}</p>}
                            </div>
                        )}

                        {/* Полное имя */}
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Полное имя *</label>
                            <input
                                type="text"
                                value={formData.fullName}
                                onChange={(e) => setFormData(prev => ({ ...prev, fullName: e.target.value }))}
                                className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 ${formErrors.fullName ? 'border-red-300' : 'border-gray-300'}`}
                            />
                            {formErrors.fullName && <p className="text-red-500 text-xs mt-1">{formErrors.fullName}</p>}
                        </div>

                        {/* Email */}
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Email *</label>
                            <input
                                type="email"
                                value={formData.email}
                                onChange={(e) => setFormData(prev => ({ ...prev, email: e.target.value }))}
                                className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 ${formErrors.email ? 'border-red-300' : 'border-gray-300'}`}
                            />
                            {formErrors.email && <p className="text-red-500 text-xs mt-1">{formErrors.email}</p>}
                        </div>

                        {/* Роль */}
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Роль *</label>
                            <select
                                value={formData.role}
                                onChange={(e) => setFormData(prev => ({ ...prev, role: e.target.value }))}
                                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                            >
                                <option value="Admin">Администратор</option>
                                <option value="Manager">Менеджер</option>
                                <option value="Foreman">Прораб</option>
                                <option value="Accountant">Бухгалтер</option>
                                <option value="Brigadier">Бригадир</option>
                            </select>
                        </div>

                        {formError && (
                            <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg text-sm">
                                {formError}
                            </div>
                        )}
                    </form>
                </Modal>
            )}
        </div>
    );
};