import React, { useEffect, useState, useCallback, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import type { Client, ClientFilter } from '../../types/client';
import type { PagedResponse } from '../../types/common';
import { Button } from '../common/Button';
import { StatusBadge } from '../common/StatusBadge';
import { SearchBar } from '../common/SearchBar';
import { Pagination } from '../common/Pagination';
import { LoadingSpinner } from '../common/LoadingSpinner';
import { ErrorMessage } from '../common/ErrorMessage';
import { EmptyState } from '../common/EmptyState';
import { ClientForm } from './ClientForm';
import { clientsApi } from '../../api/client.api';

export const ClientList: React.FC = () => {
    const navigate = useNavigate();
    const [data, setData] = useState<PagedResponse<Client> | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [filter, setFilter] = useState<ClientFilter>({ page: 1, pageSize: 12 });
    const [showForm, setShowForm] = useState(false);
    const [editingClient, setEditingClient] = useState<Client | null>(null);

    const prevFilterRef = useRef<ClientFilter | null>(null);

    const loadClients = useCallback(async () => {
        try {
            setLoading(true);
            setError(null);
            const result = await clientsApi.getClients(filter);
            setData(result);
        } catch (err: any) {
            setError(err.response?.data?.message || 'Ошибка при загрузке клиентов');
        } finally {
            setLoading(false);
        }
    }, [filter]);

    useEffect(() => {
        const currentFilter = {
            search: filter.search,
            status: filter.status,
            page: filter.page,
            pageSize: filter.pageSize
        };

        const prevFilter = prevFilterRef.current;

        if (prevFilter &&
            prevFilter.search === currentFilter.search &&
            prevFilter.status === currentFilter.status &&
            prevFilter.page === currentFilter.page &&
            prevFilter.pageSize === currentFilter.pageSize) {
            return;
        }

        prevFilterRef.current = currentFilter;
        loadClients();
    }, [filter, loadClients]);

    const handleSearch = useCallback((search: string) => {
        setFilter(prev => ({ ...prev, search, page: 1 }));
    }, []);

    const handleStatusFilter = useCallback((status: string) => {
        setFilter(prev => ({ ...prev, status: status || undefined, page: 1 }));
    }, []);

    const handlePageChange = useCallback((page: number) => {
        setFilter(prev => ({ ...prev, page }));
    }, []);

    const handleFormSuccess = useCallback(() => {
        setShowForm(false);
        setEditingClient(null);
        setFilter(prev => ({ ...prev }));
    }, []);

    const handleAddNew = useCallback(() => {
        setEditingClient(null);
        setShowForm(true);
    }, []);

    const handleCloseForm = useCallback(() => {
        setShowForm(false);
        setEditingClient(null);
    }, []);

    const handleRetry = useCallback(() => {
        setFilter(prev => ({ ...prev }));
    }, []);

    return (
        <div className="space-y-6">
            <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
                <h1 className="text-2xl font-bold text-gray-900">Клиенты</h1>
                <Button onClick={handleAddNew}>
                    <svg className="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
                    </svg>
                    Добавить клиента
                </Button>
            </div>

            <div className="space-y-4">
                <SearchBar onSearch={handleSearch} placeholder="Поиск по имени, телефону или email..." />
                <div className="flex flex-wrap gap-2">
                    <button onClick={() => handleStatusFilter('')} className={`px-3 py-1.5 rounded-full text-sm font-medium ${!filter.status ? 'bg-blue-600 text-white' : 'bg-gray-100 text-gray-700 hover:bg-gray-200'}`}>Все</button>
                    {['Potential', 'Active', 'Inactive', 'Regular', 'Archived'].map(status => (
                        <button key={status} onClick={() => handleStatusFilter(status)} className={`px-3 py-1.5 rounded-full text-sm font-medium ${filter.status === status ? 'bg-blue-600 text-white' : 'bg-gray-100 text-gray-700 hover:bg-gray-200'}`}>
                            {status === 'Potential' ? 'Потенциальные' : status === 'Active' ? 'Активные' : status === 'Inactive' ? 'Неактивные' : status === 'Regular' ? 'Постоянные' : 'Архивные'}
                        </button>
                    ))}
                </div>
            </div>

            {loading ? (
                <LoadingSpinner />
            ) : error ? (
                <ErrorMessage message={error} onRetry={handleRetry} />
            ) : data && data.items.length === 0 ? (
                <EmptyState title="Клиенты не найдены" description="Попробуйте изменить параметры поиска или добавьте нового клиента" action={<Button onClick={handleAddNew}>Добавить клиента</Button>} />
            ) : (
                <>
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                        {data?.items.map(client => (
                            <div key={client.id} className="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow cursor-pointer border border-gray-200" onClick={() => navigate(`/clients/${client.id}`)}>
                                <div className="flex justify-between items-start mb-4">
                                    <h3 className="text-lg font-semibold text-gray-900 truncate">{client.fullName}</h3>
                                    <StatusBadge status={client.status} type="client" size="sm" />
                                </div>
                                <div className="space-y-2 text-sm">
                                    <div className="flex items-center text-gray-600">
                                        <svg className="w-4 h-4 mr-2 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" /></svg>
                                        <span className="truncate">{client.phone}</span>
                                    </div>
                                    {client.email && (
                                        <div className="flex items-center text-gray-600">
                                            <svg className="w-4 h-4 mr-2 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" /></svg>
                                            <span className="truncate">{client.email}</span>
                                        </div>
                                    )}
                                    <div className="flex items-center text-gray-600">
                                        <svg className="w-4 h-4 mr-2 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" /></svg>
                                        <span className="truncate">{client.address}</span>
                                    </div>
                                </div>
                                <div className="mt-4 pt-4 border-t border-gray-200 flex justify-between items-center text-sm">
                                    <span className="text-gray-500">Проектов: <span className="font-semibold text-gray-700">{client.projectsCount}</span></span>
                                    {client.debt > 0 && <span className="text-red-600 font-semibold">Долг: {client.debt.toLocaleString()} ₽</span>}
                                </div>
                            </div>
                        ))}
                    </div>
                    {data && <Pagination currentPage={data.page} totalPages={data.totalPages} onPageChange={handlePageChange} />}
                </>
            )}

            {showForm && <ClientForm client={editingClient} onClose={handleCloseForm} onSuccess={handleFormSuccess} />}
        </div>
    );
};