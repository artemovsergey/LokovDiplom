import React from 'react';
import { ClientStatus, type Client } from '../types/client';

interface ClientCardProps {
    client: Client;
    onClick?: () => void;
}

const statusLabels: Record<string, string> = {
    [ClientStatus.Potential]: 'Потенциальный',
    [ClientStatus.Active]: 'Активный',
    [ClientStatus.Inactive]: 'Неактивный',
    [ClientStatus.Completed]: 'Завершенный'
};

const statusColors: Record<string, string> = {
    [ClientStatus.Potential]: 'bg-yellow-100 text-yellow-800 border-yellow-200',
    [ClientStatus.Active]: 'bg-green-100 text-green-800 border-green-200',
    [ClientStatus.Inactive]: 'bg-gray-100 text-gray-800 border-gray-200',
    [ClientStatus.Completed]: 'bg-blue-100 text-blue-800 border-blue-200'
};

const ClientCard: React.FC<ClientCardProps> = ({ client, onClick }) => {
    return (
        <div
            onClick={onClick}
            className="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow cursor-pointer border border-gray-200"
        >
            <div className="flex justify-between items-start mb-4">
                <h3 className="text-xl font-semibold text-gray-900">
                    {client.lastName} {client.firstName}
                </h3>
                <span className={`px-2 py-1 rounded-full text-xs border ${statusColors[client.status]}`}>
                    {statusLabels[client.status]}
                </span>
            </div>

            {client.patronymic && (
                <p className="text-gray-600 mb-2">{client.patronymic}</p>
            )}

            <div className="space-y-2">
                <div className="flex items-center text-gray-600">
                    <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
                    </svg>
                    <span className="text-sm">{client.phone}</span>
                </div>

                {client.email && (
                    <div className="flex items-center text-gray-600">
                        <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
                        </svg>
                        <span className="text-sm">{client.email}</span>
                    </div>
                )}

                {client.address && (
                    <div className="flex items-center text-gray-600">
                        <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                        </svg>
                        <span className="text-sm">{client.address}</span>
                    </div>
                )}
            </div>

            <div className="mt-4 pt-4 border-t border-gray-200">
                <div className="flex justify-between text-sm text-gray-500">
                    <span>Проектов: {client.projectsCount}</span>
                    <span>{new Date(client.createdAt).toLocaleDateString('ru-RU')}</span>
                </div>
            </div>
        </div>
    );
};

export default ClientCard;