import React from 'react';

interface StatusBadgeProps {
    status: string;
    type: 'client' | 'project' | 'task';
    size?: 'sm' | 'md';
}

const clientStatuses: Record<string, { label: string; color: string }> = {
    Potential: { label: 'Потенциальный', color: 'bg-yellow-100 text-yellow-800 border-yellow-200' },
    Active: { label: 'Активный', color: 'bg-green-100 text-green-800 border-green-200' },
    Inactive: { label: 'Неактивный', color: 'bg-gray-100 text-gray-800 border-gray-200' },
    Regular: { label: 'Постоянный', color: 'bg-blue-100 text-blue-800 border-blue-200' },
    Archived: { label: 'Архивный', color: 'bg-red-100 text-red-800 border-red-200' }
};

const projectStatuses: Record<string, { label: string; color: string }> = {
    New: { label: 'Новый', color: 'bg-blue-100 text-blue-800 border-blue-200' },
    Inspection: { label: 'Осмотр', color: 'bg-purple-100 text-purple-800 border-purple-200' },
    Estimate: { label: 'Смета', color: 'bg-indigo-100 text-indigo-800 border-indigo-200' },
    Approval: { label: 'Согласование', color: 'bg-yellow-100 text-yellow-800 border-yellow-200' },
    Contract: { label: 'Договор', color: 'bg-orange-100 text-orange-800 border-orange-200' },
    MaterialPurchase: { label: 'Закупка', color: 'bg-cyan-100 text-cyan-800 border-cyan-200' },
    InProgress: { label: 'В работе', color: 'bg-green-100 text-green-800 border-green-200' },
    QualityControl: { label: 'Контроль', color: 'bg-teal-100 text-teal-800 border-teal-200' },
    Completed: { label: 'Завершен', color: 'bg-emerald-100 text-emerald-800 border-emerald-200' },
    Warranty: { label: 'Гарантия', color: 'bg-lime-100 text-lime-800 border-lime-200' },
    Cancelled: { label: 'Отменен', color: 'bg-red-100 text-red-800 border-red-200' }
};

export const StatusBadge: React.FC<StatusBadgeProps> = ({ status, type, size = 'md' }) => {
    const statuses = type === 'client' ? clientStatuses : projectStatuses;
    const config = statuses[status] || { label: status, color: 'bg-gray-100 text-gray-800 border-gray-200' };
    const sizeClass = size === 'sm' ? 'px-2 py-0.5 text-xs' : 'px-2.5 py-1 text-sm';

    return (
        <span className={`inline-flex items-center rounded-full border font-medium ${config.color} ${sizeClass}`}>
            {config.label}
        </span>
    );
};