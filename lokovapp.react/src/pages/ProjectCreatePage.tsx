import React, { useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { projectsApi } from '../api/projects.api';
import { brigadesApi } from '../api/brigades.api';
import type { CreateProjectDto } from '../types/project';
import type { Client } from '../types/client';
import type { Brigade } from '../api/brigades.api';
import { Button } from '../components/common/Button';
import { LoadingSpinner } from '../components/common/LoadingSpinner';
import { clientsApi } from '../api/client.api';

export const ProjectCreatePage: React.FC = () => {
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();
    const preselectedClientId = searchParams.get('clientId');

    const [formData, setFormData] = useState<CreateProjectDto>({
        clientId: preselectedClientId || '',
        name: '',
        description: '',
        type: 'MajorRepair',
        address: '',
        estimatedCost: 0,
        startDate: '',
        plannedEndDate: '',
        brigadeId: ''
    });
    const [clients, setClients] = useState<Client[]>([]);
    const [brigades, setBrigades] = useState<Brigade[]>([]);
    const [loading, setLoading] = useState(false);
    const [loadingData, setLoadingData] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [errors, setErrors] = useState<Record<string, string>>({});

    useEffect(() => {
        loadData();
    }, []);

    const loadData = async () => {
        try {
            setLoadingData(true);
            const [clientsResult, brigadesResult] = await Promise.all([
                clientsApi.getClients({ pageSize: 100 }),
                brigadesApi.getBrigades()
            ]);
            setClients(clientsResult.items);
            setBrigades(brigadesResult);
        } catch (err) {
            console.error('Ошибка загрузки данных:', err);
        } finally {
            setLoadingData(false);
        }
    };

    const validate = (): boolean => {
        const newErrors: Record<string, string> = {};
        if (!formData.clientId) newErrors.clientId = 'Выберите клиента';
        if (!formData.name.trim()) newErrors.name = 'Название обязательно';
        if (!formData.type) newErrors.type = 'Выберите тип проекта';
        if (!formData.address.trim()) newErrors.address = 'Адрес обязателен';
        if (!formData.estimatedCost || formData.estimatedCost <= 0) newErrors.estimatedCost = 'Укажите стоимость';
        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);

        if (!validate()) return;

        setLoading(true);

        try {
            const dataToSend: CreateProjectDto = {
                ...formData,
                brigadeId: formData.brigadeId || undefined
            };
            const project = await projectsApi.createProject(dataToSend);
            navigate(`/projects/${project.id}`);
        } catch (err: any) {
            console.error('Create project error:', err);
            const errorMessage = err.response?.data?.message
                || err.response?.data?.title
                || err.message
                || 'Ошибка при создании проекта';
            setError(errorMessage);
        } finally {
            setLoading(false);
        }
    };

    const updateField = (field: keyof CreateProjectDto, value: string | number) => {
        setFormData(prev => ({ ...prev, [field]: value }));
        if (errors[field]) {
            setErrors(prev => {
                const newErrors = { ...prev };
                delete newErrors[field];
                return newErrors;
            });
        }
    };

    if (loadingData) {
        return <LoadingSpinner text="Загрузка данных..." />;
    }

    return (
        <div className="max-w-2xl mx-auto space-y-6">
            <div className="flex items-center justify-between">
                <div>
                    <button onClick={() => navigate('/projects')} className="text-blue-600 hover:text-blue-700 text-sm mb-2 block">
                        ← Назад к списку проектов
                    </button>
                    <h1 className="text-2xl font-bold text-gray-900">Новый проект</h1>
                </div>
            </div>

            <form onSubmit={handleSubmit} className="bg-white rounded-lg shadow-md p-6 space-y-5 border border-gray-200">
                {/* Клиент */}
                <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Клиент *</label>
                    <select
                        value={formData.clientId}
                        onChange={(e) => updateField('clientId', e.target.value)}
                        className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 ${errors.clientId ? 'border-red-300' : 'border-gray-300'}`}
                    >
                        <option value="">Выберите клиента</option>
                        {clients.map(client => (
                            <option key={client.id} value={client.id}>
                                {client.fullName} - {client.phone}
                            </option>
                        ))}
                    </select>
                    {errors.clientId && <p className="text-red-500 text-xs mt-1">{errors.clientId}</p>}
                </div>

                {/* Тип проекта */}
                <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Тип проекта *</label>
                    <select
                        value={formData.type}
                        onChange={(e) => updateField('type', e.target.value)}
                        className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 ${errors.type ? 'border-red-300' : 'border-gray-300'}`}
                    >
                        <option value="MajorRepair">Капитальный ремонт</option>
                        <option value="PartialRepair">Частичный ремонт</option>
                        <option value="RoofWorks">Кровельные работы</option>
                        <option value="FacadeWorks">Фасадные работы</option>
                        <option value="CombinedWorks">Комплексные работы</option>
                    </select>
                    {errors.type && <p className="text-red-500 text-xs mt-1">{errors.type}</p>}
                </div>

                {/* Название */}
                <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Название *</label>
                    <input
                        type="text"
                        value={formData.name}
                        onChange={(e) => updateField('name', e.target.value)}
                        placeholder="Например: Капитальный ремонт 2-к квартиры"
                        className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 ${errors.name ? 'border-red-300' : 'border-gray-300'}`}
                    />
                    {errors.name && <p className="text-red-500 text-xs mt-1">{errors.name}</p>}
                </div>

                {/* Описание */}
                <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Описание</label>
                    <textarea
                        value={formData.description || ''}
                        onChange={(e) => updateField('description', e.target.value)}
                        rows={3}
                        placeholder="Опишите детали проекта..."
                        className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                    />
                </div>

                {/* Адрес */}
                <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Адрес объекта *</label>
                    <input
                        type="text"
                        value={formData.address}
                        onChange={(e) => updateField('address', e.target.value)}
                        placeholder="г. Москва, ул. Примерная, д. 1, кв. 1"
                        className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 ${errors.address ? 'border-red-300' : 'border-gray-300'}`}
                    />
                    {errors.address && <p className="text-red-500 text-xs mt-1">{errors.address}</p>}
                </div>

                {/* Бригада */}
                <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Бригада</label>
                    <select
                        value={formData.brigadeId || ''}
                        onChange={(e) => updateField('brigadeId', e.target.value)}
                        className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                    >
                        <option value="">Не назначать</option>
                        {brigades.map(brigade => (
                            <option key={brigade.id} value={brigade.id}>
                                {brigade.name} - {brigade.foremanName} ({brigade.workersCount} чел.)
                            </option>
                        ))}
                    </select>
                </div>

                {/* Сметная стоимость */}
                <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Сметная стоимость (₽) *</label>
                    <input
                        type="number"
                        value={formData.estimatedCost || ''}
                        onChange={(e) => updateField('estimatedCost', Number(e.target.value))}
                        placeholder="1000000"
                        min="0"
                        step="1000"
                        className={`w-full px-3 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 ${errors.estimatedCost ? 'border-red-300' : 'border-gray-300'}`}
                    />
                    {errors.estimatedCost && <p className="text-red-500 text-xs mt-1">{errors.estimatedCost}</p>}
                </div>

                {/* Даты */}
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Дата начала</label>
                        <input
                            type="date"
                            value={formData.startDate || ''}
                            onChange={(e) => updateField('startDate', e.target.value)}
                            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                        />
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Плановое окончание</label>
                        <input
                            type="date"
                            value={formData.plannedEndDate || ''}
                            onChange={(e) => updateField('plannedEndDate', e.target.value)}
                            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                        />
                    </div>
                </div>

                {/* Ошибка */}
                {error && (
                    <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg text-sm">
                        {error}
                    </div>
                )}

                {/* Кнопки */}
                <div className="flex justify-end gap-3 pt-4 border-t border-gray-200">
                    <Button variant="secondary" onClick={() => navigate('/projects')} type="button">
                        Отмена
                    </Button>
                    <Button type="submit" loading={loading}>
                        Создать проект
                    </Button>
                </div>
            </form>
        </div>
    );
};