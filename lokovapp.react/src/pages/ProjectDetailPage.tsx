import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { LoadingSpinner } from '../components/common/LoadingSpinner';
import { ErrorMessage } from '../components/common/ErrorMessage';
import { StatusBadge } from '../components/common/StatusBadge';
import type { Project } from '../types/project';
import { projectsApi } from '../api/projects.api';
import { PhotoGallery } from '../components/photos/PhotoGallery';
import { Button } from '../components/common/Button';

export const ProjectDetailPage: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [project, setProject] = useState<Project | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [activeTab, setActiveTab] = useState<'info' | 'photos' | 'payments'>('info');

    useEffect(() => {
        if (id) loadProject(id);
    }, [id]);

    const loadProject = async (projectId: string) => {
        try {
            setLoading(true);
            const data = await projectsApi.getProject(projectId);
            setProject(data);
        } catch (err) {
            setError('Проект не найден');
        } finally {
            setLoading(false);
        }
    };

    if (loading) return <LoadingSpinner text="Загрузка проекта..." />;
    if (error) return <ErrorMessage message={error} onRetry={() => id && loadProject(id)} />;
    if (!project) return null;

    const typeLabels: Record<string, string> = {
        MajorRepair: 'Капитальный ремонт',
        PartialRepair: 'Частичный ремонт',
        RoofWorks: 'Кровельные работы',
        FacadeWorks: 'Фасадные работы',
        CombinedWorks: 'Комплексные работы'
    };

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <div>
                    <button onClick={() => navigate('/projects')} className="text-blue-600 hover:text-blue-700 text-sm mb-2 block">
                        ← Назад к списку
                    </button>
                    <h1 className="text-2xl font-bold text-gray-900">
                        {project.number} - {project.name}
                    </h1>
                    <p className="text-gray-500 mt-1">Клиент: {project.clientName}</p>
                </div>
                <div className="flex items-center gap-3">
                    <StatusBadge status={project.status} type="project" />
                    <Button variant="secondary" size="sm" onClick={() => navigate(`/projects/${project.id}/edit`)}>
                        ✏️ Редактировать
                    </Button>
                </div>
            </div>

            {/* Табы */}
            <div className="border-b border-gray-200">
                <nav className="flex space-x-8">
                    {['info', 'photos', 'payments'].map((tab) => (
                        <button
                            key={tab}
                            onClick={() => setActiveTab(tab as any)}
                            className={`py-4 px-1 border-b-2 font-medium text-sm transition-colors ${activeTab === tab
                                ? 'border-blue-600 text-blue-600'
                                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                                }`}
                        >
                            {tab === 'info' ? 'Информация' : tab === 'photos' ? 'Фотографии' : 'Платежи'}
                        </button>
                    ))}
                </nav>
            </div>

            {/* Контент табов */}
            {activeTab === 'info' && (
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                    <div className="bg-white rounded-lg shadow p-6 border border-gray-200">
                        <h3 className="text-lg font-semibold mb-4">Основная информация</h3>
                        <div className="space-y-3">
                            <div className="flex justify-between">
                                <span className="text-gray-500">Тип</span>
                                <span>{typeLabels[project.type] || project.typeDisplay}</span>
                            </div>
                            <div className="flex justify-between">
                                <span className="text-gray-500">Адрес</span>
                                <span className="text-right max-w-[300px]">{project.address}</span>
                            </div>
                            <div className="flex justify-between">
                                <span className="text-gray-500">Бригада</span>
                                <span>{project.brigadeName || 'Не назначена'}</span>
                            </div>
                            <div className="flex justify-between">
                                <span className="text-gray-500">Начало</span>
                                <span>{project.startDate ? new Date(project.startDate).toLocaleDateString('ru-RU') : '-'}</span>
                            </div>
                            <div className="flex justify-between">
                                <span className="text-gray-500">Плановое окончание</span>
                                <span>{project.plannedEndDate ? new Date(project.plannedEndDate).toLocaleDateString('ru-RU') : '-'}</span>
                            </div>
                        </div>
                    </div>

                    <div className="bg-white rounded-lg shadow p-6 border border-gray-200">
                        <h3 className="text-lg font-semibold mb-4">Финансы</h3>
                        <div className="space-y-3">
                            <div className="flex justify-between">
                                <span className="text-gray-500">Сметная стоимость</span>
                                <span className="font-semibold">{project.estimatedCost.toLocaleString()} ₽</span>
                            </div>
                            <div className="flex justify-between">
                                <span className="text-gray-500">Оплачено</span>
                                <span className="font-semibold text-green-600">{project.paidAmount.toLocaleString()} ₽</span>
                            </div>
                            <div className="flex justify-between">
                                <span className="text-gray-500">Задолженность</span>
                                <span className={`font-semibold ${project.debt > 0 ? 'text-red-600' : 'text-green-600'}`}>
                                    {project.debt.toLocaleString()} ₽
                                </span>
                            </div>
                            <div className="flex justify-between">
                                <span className="text-gray-500">Выполнение</span>
                                <div className="flex items-center">
                                    <div className="w-32 bg-gray-200 rounded-full h-2 mr-2">
                                        <div className="bg-blue-600 h-2 rounded-full" style={{ width: `${project.completionPercentage}%` }} />
                                    </div>
                                    <span className="font-semibold">{project.completionPercentage}%</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            )}

            {activeTab === 'photos' && (
                <PhotoGallery projectId={project.id} />
            )}

            {activeTab === 'payments' && (
                <div className="bg-white rounded-lg shadow p-6 border border-gray-200">
                    <p className="text-gray-500 text-center py-8">История платежей будет доступна позже</p>
                </div>
            )}
        </div>
    );
};