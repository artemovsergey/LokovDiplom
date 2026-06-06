import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import type { UpdateClientDto } from '../types/client';
import { Button } from '../components/common/Button';
import { LoadingSpinner } from '../components/common/LoadingSpinner';
import { ErrorMessage } from '../components/common/ErrorMessage';
import { clientsApi } from '../api/client.api';

export const ClientEditPage: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [formData, setFormData] = useState<UpdateClientDto>({
        firstName: '',
        lastName: '',
        patronymic: '',
        phone: '',
        additionalPhone: '',
        email: '',
        address: '',
        source: 'DirectContact',
        status: 'Potential',
        category: 'Individual'
    });
    const [errors, setErrors] = useState<Record<string, string>>({});

    useEffect(() => {
        if (id) loadClient(id);
    }, [id]);

    const loadClient = async (clientId: string) => {
        try {
            setLoading(true);
            const client = await clientsApi.getClient(clientId);
            setFormData({
                firstName: client.firstName,
                lastName: client.lastName,
                patronymic: client.patronymic || '',
                phone: client.phone,
                additionalPhone: client.additionalPhone || '',
                email: client.email || '',
                address: client.address,
                source: client.source,
                status: client.status,
                category: client.category
            });
        } catch (err) {
            setError('Клиент не найден');
        } finally {
            setLoading(false);
        }
    };

    const validate = (): boolean => {
        const newErrors: Record<string, string> = {};
        if (!formData.firstName.trim()) newErrors.firstName = 'Имя обязательно';
        if (!formData.lastName.trim()) newErrors.lastName = 'Фамилия обязательна';
        if (!formData.phone.trim()) newErrors.phone = 'Телефон обязателен';
        if (!formData.address.trim()) newErrors.address = 'Адрес обязателен';
        if (formData.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
            newErrors.email = 'Неверный формат email';
        }
        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!validate() || !id) return;

        setSaving(true);
        setError(null);

        try {
            await clientsApi.updateClient(id, formData);
            navigate(`/clients/${id}`);
        } catch (err: any) {
            setError(err.response?.data?.message || 'Ошибка при сохранении');
        } finally {
            setSaving(false);
        }
    };

    const updateField = (field: keyof UpdateClientDto, value: string) => {
        setFormData(prev => ({ ...prev, [field]: value }));
        if (errors[field]) {
            setErrors(prev => {
                const newErrors = { ...prev };
                delete newErrors[field];
                return newErrors;
            });
        }
    };

    if (loading) return <LoadingSpinner text="Загрузка клиента..." />;
    if (error && !formData.firstName) return <ErrorMessage message={error} />;

    return (
        <div className="max-w-2xl mx-auto space-y-6">
            <div>
                <button onClick={() => navigate(`/clients/${id}`)} className="text-blue-600 hover:text-blue-700 text-sm mb-2 block">
                    ← Назад к клиенту
                </button>
                <h1 className="text-2xl font-bold text-gray-900">Редактирование клиента</h1>
            </div>

            <form onSubmit={handleSubmit} className="bg-white rounded-lg shadow-md p-6 space-y-5 border border-gray-200">
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Фамилия *</label>
                        <input
                            type="text"
                            value={formData.lastName}
                            onChange={(e) => updateField('lastName', e.target.value)}
                            className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 ${errors.lastName ? 'border-red-300' : 'border-gray-300'}`}
                        />
                        {errors.lastName && <p className="text-red-500 text-xs mt-1">{errors.lastName}</p>}
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Имя *</label>
                        <input
                            type="text"
                            value={formData.firstName}
                            onChange={(e) => updateField('firstName', e.target.value)}
                            className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 ${errors.firstName ? 'border-red-300' : 'border-gray-300'}`}
                        />
                        {errors.firstName && <p className="text-red-500 text-xs mt-1">{errors.firstName}</p>}
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Отчество</label>
                        <input
                            type="text"
                            value={formData.patronymic || ''}
                            onChange={(e) => updateField('patronymic', e.target.value)}
                            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                        />
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Телефон *</label>
                        <input
                            type="tel"
                            value={formData.phone}
                            onChange={(e) => updateField('phone', e.target.value)}
                            className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 ${errors.phone ? 'border-red-300' : 'border-gray-300'}`}
                        />
                        {errors.phone && <p className="text-red-500 text-xs mt-1">{errors.phone}</p>}
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Доп. телефон</label>
                        <input
                            type="tel"
                            value={formData.additionalPhone || ''}
                            onChange={(e) => updateField('additionalPhone', e.target.value)}
                            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                        />
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Email</label>
                        <input
                            type="email"
                            value={formData.email || ''}
                            onChange={(e) => updateField('email', e.target.value)}
                            className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 ${errors.email ? 'border-red-300' : 'border-gray-300'}`}
                        />
                        {errors.email && <p className="text-red-500 text-xs mt-1">{errors.email}</p>}
                    </div>
                </div>

                <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Адрес *</label>
                    <input
                        type="text"
                        value={formData.address}
                        onChange={(e) => updateField('address', e.target.value)}
                        className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 ${errors.address ? 'border-red-300' : 'border-gray-300'}`}
                    />
                    {errors.address && <p className="text-red-500 text-xs mt-1">{errors.address}</p>}
                </div>

                <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Статус</label>
                        <select
                            value={formData.status}
                            onChange={(e) => updateField('status', e.target.value)}
                            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                        >
                            <option value="Potential">Потенциальный</option>
                            <option value="Active">Активный</option>
                            <option value="Inactive">Неактивный</option>
                            <option value="Regular">Постоянный</option>
                            <option value="Archived">Архивный</option>
                        </select>
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Источник</label>
                        <select
                            value={formData.source}
                            onChange={(e) => updateField('source', e.target.value)}
                            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                        >
                            <option value="Recommendation">Рекомендация</option>
                            <option value="Internet">Интернет</option>
                            <option value="SocialMedia">Соцсети</option>
                            <option value="Advertisement">Реклама</option>
                            <option value="DirectContact">Прямое обращение</option>
                            <option value="Other">Другое</option>
                        </select>
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Категория</label>
                        <select
                            value={formData.category}
                            onChange={(e) => updateField('category', e.target.value)}
                            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                        >
                            <option value="Individual">Физ. лицо</option>
                            <option value="LegalEntity">Юр. лицо</option>
                            <option value="Entrepreneur">ИП</option>
                        </select>
                    </div>
                </div>

                {error && (
                    <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg text-sm">
                        {error}
                    </div>
                )}

                <div className="flex justify-end gap-3 pt-4 border-t border-gray-200">
                    <Button variant="secondary" onClick={() => navigate(`/clients/${id}`)} type="button">
                        Отмена
                    </Button>
                    <Button type="submit" loading={saving}>
                        Сохранить
                    </Button>
                </div>
            </form>
        </div>
    );
};