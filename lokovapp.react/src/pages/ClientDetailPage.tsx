import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { projectsApi } from '../api/projects.api';
import type { Client } from '../types/client';
import type { Project } from '../types/project';
import { LoadingSpinner } from '../components/common/LoadingSpinner';
import { ErrorMessage } from '../components/common/ErrorMessage';
import { StatusBadge } from '../components/common/StatusBadge';
import { Button } from '../components/common/Button';
import { clientsApi } from '../api/client.api';

export const ClientDetailPage: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [client, setClient] = useState<Client | null>(null);
    const [projects, setProjects] = useState<Project[]>([]);
    const [loading, setLoading] = useState(true);
    const [loadingProjects, setLoadingProjects] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (id) {
            loadClient(id);
            loadProjects(id);
        }
    }, [id]);

    const loadClient = async (clientId: string) => {
        try {
            setLoading(true);
            setError(null);
            const data = await clientsApi.getClient(clientId);
            setClient(data);
        } catch (err) {
            setError('Клиент не найден');
        } finally {
            setLoading(false);
        }
    };

    const loadProjects = async (clientId: string) => {
        try {
            setLoadingProjects(true);
            const result = await projectsApi.getClientProjects(clientId, { pageSize: 50 });
            setProjects(result.items);
        } catch (err) {
            console.error('Ошибка при загрузке проектов клиента:', err);
        } finally {
            setLoadingProjects(false);
        }
    };

    const handleDelete = async () => {
        if (!id) return;
        if (window.confirm('Вы уверены, что хотите удалить клиента?')) {
            try {
                await clientsApi.deleteClient(id);
                navigate('/clients');
            } catch (err) {
                alert('Ошибка при удалении клиента');
            }
        }
    };

    const handleArchive = async () => {
        if (!id) return;
        if (window.confirm('Вы уверены, что хотите архивировать клиента?')) {
            try {
                await clientsApi.archiveClient(id);
                loadClient(id);
            } catch (err) {
                alert('Ошибка при архивации клиента');
            }
        }
    };

    if (loading) return <LoadingSpinner text="Загрузка клиента..." />;
    if (error) return <ErrorMessage message={error} onRetry={() => id && loadClient(id)} />;
    if (!client) return null;

    const sourceLabels: Record<string, string> = {
        Recommendation: 'Рекомендация',
        Internet: 'Интернет',
        SocialMedia: 'Социальные сети',
        Advertisement: 'Реклама',
        DirectContact: 'Прямое обращение',
        Other: 'Другое'
    };

    const categoryLabels: Record<string, string> = {
        Individual: 'Физическое лицо',
        LegalEntity: 'Юридическое лицо',
        Entrepreneur: 'ИП'
    };

    const typeLabels: Record<string, { label: string; icon: string }> = {
        MajorRepair: { label: 'Капремонт', icon: '🏠' },
        PartialRepair: { label: 'Частичный', icon: '🔧' },
        RoofWorks: { label: 'Кровля', icon: '🏗️' },
        FacadeWorks: { label: 'Фасад', icon: '🧱' },
        CombinedWorks: { label: 'Комплекс', icon: '🏢' }
    };

    return (
        <div className="space-y-6">
            {/* Заголовок */}
            <div className="flex items-center justify-between">
                <div>
                    <button onClick={() => navigate('/clients')} className="text-blue-600 hover:text-blue-700 text-sm mb-2 block">
                        ← Назад к списку
                    </button>
                    <h1 className="text-2xl font-bold text-gray-900">{client.fullName}</h1>
                </div>
                <div className="flex gap-2">
                    <Button variant="secondary" onClick={() => navigate(`/clients/${client.id}/edit`)}>
                        ✏️ Редактировать
                    </Button>
                    <Button variant="primary" onClick={() => navigate(`/projects/new?clientId=${client.id}`)}>
                        + Создать проект
                    </Button>
                </div>
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                {/* Информация о клиенте */}
                <div className="lg:col-span-1">
                    <div className="bg-white rounded-lg shadow p-6 border border-gray-200 space-y-4">
                        <div className="flex justify-between items-center">
                            <StatusBadge status={client.status} type="client" />
                            <span className="text-sm text-gray-500">{categoryLabels[client.category] || client.category}</span>
                        </div>

                        <div className="space-y-3 pt-2">
                            <div>
                                <p className="text-xs text-gray-500">Телефон</p>
                                <p className="font-medium">{client.phone}</p>
                            </div>
                            {client.additionalPhone && (
                                <div>
                                    <p className="text-xs text-gray-500">Доп. телефон</p>
                                    <p className="font-medium">{client.additionalPhone}</p>
                                </div>
                            )}
                            {client.email && (
                                <div>
                                    <p className="text-xs text-gray-500">Email</p>
                                    <p className="font-medium">{client.email}</p>
                                </div>
                            )}
                            <div>
                                <p className="text-xs text-gray-500">Адрес</p>
                                <p className="font-medium">{client.address}</p>
                            </div>
                            <div>
                                <p className="text-xs text-gray-500">Источник</p>
                                <p className="font-medium">{sourceLabels[client.source] || client.source}</p>
                            </div>
                            <div>
                                <p className="text-xs text-gray-500">Дата создания</p>
                                <p className="font-medium">{new Date(client.createdAt).toLocaleDateString('ru-RU')}</p>
                            </div>
                        </div>

                        <hr className="border-gray-200" />

                        <div className="space-y-3">
                            <div className="flex justify-between">
                                <span className="text-gray-500">Проектов</span>
                                <span className="font-semibold text-blue-600">{client.projectsCount}</span>
                            </div>
                            <div className="flex justify-between">
                                <span className="text-gray-500">Общие платежи</span>
                                <span className="font-semibold text-green-600">{client.totalPayments.toLocaleString()} ₽</span>
                            </div>
                            {client.debt > 0 && (
                                <div className="flex justify-between">
                                    <span className="text-gray-500">Задолженность</span>
                                    <span className="font-semibold text-red-600">{client.debt.toLocaleString()} ₽</span>
                                </div>
                            )}
                        </div>

                        <hr className="border-gray-200" />

                        <div className="flex flex-col gap-2">
                            <Button variant="ghost" size="sm" onClick={handleArchive}>
                                📦 Архивировать
                            </Button>
                            <Button variant="danger" size="sm" onClick={handleDelete}>
                                🗑️ Удалить
                            </Button>
                        </div>
                    </div>
                </div>

                {/* Проекты клиента */}
                <div className="lg:col-span-2">
                    <div className="bg-white rounded-lg shadow p-6 border border-gray-200">
                        <div className="flex items-center justify-between mb-4">
                            <h3 className="text-lg font-semibold text-gray-900">Проекты клиента</h3>
                            <Button size="sm" onClick={() => navigate(`/projects/new?clientId=${client.id}`)}>
                                + Новый проект
                            </Button>
                        </div>

                        {loadingProjects ? (
                            <LoadingSpinner text="Загрузка проектов..." />
                        ) : projects.length === 0 ? (
                            <div className="text-center py-8">
                                <p className="text-gray-500">У клиента пока нет проектов</p>
                                <Button
                                    variant="primary"
                                    size="sm"
                                    className="mt-4"
                                    onClick={() => navigate(`/projects/new?clientId=${client.id}`)}
                                >
                                    Создать первый проект
                                </Button>
                            </div>
                        ) : (
                            <div className="space-y-4">
                                {projects.map(project => (
                                    <div
                                        key={project.id}
                                        className="border border-gray-200 rounded-lg p-4 hover:shadow-md transition-shadow cursor-pointer"
                                        onClick={() => navigate(`/projects/${project.id}`)}
                                    >
                                        <div className="flex items-start justify-between">
                                            <div className="flex items-center space-x-3">
                                                <span className="text-2xl">
                                                    {typeLabels[project.type]?.icon || '📋'}
                                                </span>
                                                <div>
                                                    <p className="text-xs text-gray-500">{project.number}</p>
                                                    <h4 className="font-semibold text-gray-900">{project.name}</h4>
                                                    <p className="text-sm text-gray-500">{project.address}</p>
                                                </div>
                                            </div>
                                            <StatusBadge status={project.status} type="project" size="sm" />
                                        </div>

                                        <div className="mt-3 flex items-center justify-between">
                                            <div className="flex items-center space-x-4">
                                                <div>
                                                    <p className="text-xs text-gray-500">Бюджет</p>
                                                    <p className="font-semibold">{project.estimatedCost.toLocaleString()} ₽</p>
                                                </div>
                                                {project.debt > 0 && (
                                                    <div>
                                                        <p className="text-xs text-gray-500">Долг</p>
                                                        <p className="font-semibold text-red-600">{project.debt.toLocaleString()} ₽</p>
                                                    </div>
                                                )}
                                            </div>
                                            <div className="flex items-center space-x-2">
                                                <div className="w-24 bg-gray-200 rounded-full h-2">
                                                    <div
                                                        className="bg-blue-600 h-2 rounded-full"
                                                        style={{ width: `${project.completionPercentage}%` }}
                                                    />
                                                </div>
                                                <span className="text-sm font-medium">{project.completionPercentage}%</span>
                                            </div>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
};