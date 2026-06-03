import React, { useState } from 'react';
import { useClients } from '../hooks/useClients';
import ClientCard from './ClientCard';
import SearchBar from './SearchBar';
import ClientForm from './ClientForm';
import { ClientStatus } from '../types/client';

const statusLabels: Record<string, string> = {
    [ClientStatus.Potential]: 'Потенциальный',
    [ClientStatus.Active]: 'Активный',
    [ClientStatus.Inactive]: 'Неактивный',
    [ClientStatus.Completed]: 'Завершенный'
};

const statusColors: Record<string, string> = {
    [ClientStatus.Potential]: 'bg-yellow-100 text-yellow-800',
    [ClientStatus.Active]: 'bg-green-100 text-green-800',
    [ClientStatus.Inactive]: 'bg-gray-100 text-gray-800',
    [ClientStatus.Completed]: 'bg-blue-100 text-blue-800'
};

const ClientList: React.FC = () => {
    const { clients, loading, error, fetchClients } = useClients();
    const [showForm, setShowForm] = useState(false);
    const [selectedStatus, setSelectedStatus] = useState<string>('');

    const handleSearch = (query: string) => {
        fetchClients(query, selectedStatus);
    };

    const handleStatusFilter = (status: string) => {
        setSelectedStatus(status);
        fetchClients(undefined, status);
    };

    if (loading) {
        return (
            <div className="flex justify-center items-center h-64">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
            </div>
        );
    }

    return (
        <div className="container mx-auto px-4 py-8">
            <div className="flex justify-between items-center mb-6">
                <h1 className="text-3xl font-bold text-gray-900">Клиенты</h1>
                <button
                    onClick={() => setShowForm(true)}
                    className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"
                >
                    Добавить клиента
                </button>
            </div>

            <div className="mb-6">
                <SearchBar onSearch={handleSearch} placeholder="Поиск по имени или телефону..." />
            </div>

            <div className="flex gap-2 mb-6">
                <button
                    onClick={() => handleStatusFilter('')}
                    className={`px-3 py-1 rounded-full text-sm ${!selectedStatus ? 'bg-blue-600 text-white' : 'bg-gray-200 text-gray-700'}`}
                >
                    Все
                </button>
                {Object.entries(statusLabels).map(([status, label]) => (
                    <button
                        key={status}
                        onClick={() => handleStatusFilter(status)}
                        className={`px-3 py-1 rounded-full text-sm ${selectedStatus === status ? 'bg-blue-600 text-white' : 'bg-gray-200 text-gray-700'}`}
                    >
                        {label}
                    </button>
                ))}
            </div>

            {error && (
                <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
                    {error}
                </div>
            )}

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {clients.map(client => (
                    <ClientCard key={client.id} client={client} />
                ))}
            </div>

            {clients.length === 0 && !loading && (
                <div className="text-center py-12">
                    <p className="text-gray-500 text-lg">Клиенты не найдены</p>
                </div>
            )}

            {showForm && (
                <ClientForm
                    onClose={() => setShowForm(false)}
                    onSuccess={() => {
                        setShowForm(false);
                        fetchClients();
                    }}
                />
            )}
        </div>
    );
};

export default ClientList;