import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { projectsApi } from '../api/projects.api';
import { brigadesApi } from '../api/brigades.api';
import type { UpdateProjectDto } from '../types/project';
import type { Brigade } from '../api/brigades.api';
import { Button } from '../components/common/Button';
import { LoadingSpinner } from '../components/common/LoadingSpinner';
import { ErrorMessage } from '../components/common/ErrorMessage';

export const ProjectEditPage: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [brigades, setBrigades] = useState<Brigade[]>([]);
    const [formData, setFormData] = useState<UpdateProjectDto>({
        name: '',
        description: '',
        type: 'MajorRepair',
        address: '',
        estimatedCost: 0,
        status: '',
        startDate: '',
        plannedEndDate: '',
        brigadeId: ''
    });
    const [errors, setErrors] = useState<Record<string, string>>({});
    const [originalBrigadeId, setOriginalBrigadeId] = useState<string>('');

    console.log(originalBrigadeId);
    useEffect(() => {
        if (id) {
            loadProject(id);
            loadBrigades();
        }
    }, [id]);

    const loadProject = async (projectId: string) => {
        try {
            setLoading(true);
            const project = await projectsApi.getProject(projectId);
            setFormData({
                name: project.name,
                description: project.description || '',
                type: project.type,
                address: project.address,
                estimatedCost: project.estimatedCost,
                status: project.status,
                startDate: project.startDate?.split('T')[0] || '',
                plannedEndDate: project.plannedEndDate?.split('T')[0] || '',
                brigadeId: ''
            });
            setOriginalBrigadeId(project.brigadeName || '');
        } catch (err) {
            setError('Проект не найден');
        } finally {
            setLoading(false);
        }
    };

    const loadBrigades = async () => {
        try {
            const result = await brigadesApi.getBrigades();
            setBrigades(result);
        } catch (err) {
            console.error('Ошибка загрузки бригад:', err);
        }
    };

    const validate = (): boolean => {
        const newErrors: Record<string, string> = {};
        if (!formData.name.trim()) newErrors.name = 'Название обязательно';
        if (!formData.address.trim()) newErrors.address = 'Адрес обязателен';
        if (formData.estimatedCost <= 0) newErrors.estimatedCost = 'Укажите стоимость';
        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!validate() || !id) return;

        setSaving(true);
        setError(null);

        try {
            const dataToSend: UpdateProjectDto = {
                ...formData,
                brigadeId: formData.brigadeId || undefined
            };
            await projectsApi.updateProject(id, dataToSend);
            navigate(`/projects/${id}`);
        } catch (err: any) {
            setError(err.response?.data?.message || 'Ошибка при сохранении');
        } finally {
            setSaving(false);
        }
    };

    const updateField = (field: keyof UpdateProjectDto, value: string | number) => {
        setFormData(prev => ({ ...prev, [field]: value }));
        if (errors[field]) {
            setErrors(prev => {
                const newErrors = { ...prev };
                delete newErrors[field];
                return newErrors;
            });
        }
    };

    if (loading) return <LoadingSpinner text="Загрузка проекта..." />;
    if (error && !formData.name) return <ErrorMessage message={error} />;

    return (
        <div className="max-w-2xl mx-auto space-y-6">
            <div>
                <button onClick={() => navigate(`/projects/${id}`)} className="text-blue-600 hover:text-blue-700 text-sm mb-2 block">
                    ← Назад к проекту
                </button>
                <h1 className="text-2xl font-bold text-gray-900">Редактирование проекта</h1>
            </div>

            <form onSubmit={handleSubmit} className="bg-white rounded-lg shadow-md p-6 space-y-5 border border-gray-200">
                {/* Тип проекта */}
                <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Тип проекта</label>
                    <select
                        value={formData.type}
                        onChange={(e) => updateField('type', e.target.value)}
                        className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                    >
                        <option value="MajorRepair">Капитальный ремонт</option>
                        <option value="PartialRepair">Частичный ремонт</option>
                        <option value="RoofWorks">Кровельные работы</option>
                        <option value="FacadeWorks">Фасадные работы</option>
                        <option value="CombinedWorks">Комплексные работы</option>
                    </select>
                </div>

                {/* Статус */}
                <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Статус</label>
                    <select
                        value={formData.status}
                        onChange={(e) => updateField('status', e.target.value)}
                        className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                    >
                        <option value="New">Новый</option>
                        <option value="Inspection">Осмотр</option>
                        <option value="Estimate">Расчет сметы</option>
                        <option value="Approval">Согласование</option>
                        <option value="Contract">Договор подписан</option>
                        <option value="MaterialPurchase">Закупка материалов</option>
                        <option value="InProgress">В работе</option>
                        <option value="QualityControl">Контроль качества</option>
                        <option value="Completed">Завершен</option>
                        <option value="Warranty">Гарантийное обслуживание</option>
                        <option value="Cancelled">Отменен</option>
                    </select>
                </div>

                {/* Название */}
                <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Название *</label>
                    <input
                        type="text"
                        value={formData.name}
                        onChange={(e) => updateField('name', e.target.value)}
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
                        className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                    />
                </div>

                {/* Адрес */}
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

                {/* Бригада */}
                <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">Бригада</label>
                    <select
                        value={formData.brigadeId || ''}
                        onChange={(e) => updateField('brigadeId', e.target.value)}
                        className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                    >
                        <option value="">Не назначена</option>
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

                {error && (
                    <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg text-sm">
                        {error}
                    </div>
                )}

                <div className="flex justify-end gap-3 pt-4 border-t border-gray-200">
                    <Button variant="secondary" onClick={() => navigate(`/projects/${id}`)} type="button">
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