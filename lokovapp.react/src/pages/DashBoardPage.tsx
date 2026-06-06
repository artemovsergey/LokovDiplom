import React, { useEffect, useState } from 'react';
import { LoadingSpinner } from '../components/common/LoadingSpinner';
import { ErrorMessage } from '../components/common/ErrorMessage';
import {
  BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer,
  PieChart, Pie, Cell
} from 'recharts';
import type { DashboardData } from '../types/report';
import { reportsApi } from '../api/reports.api';

const COLORS = ['#3B82F6', '#10B981', '#F59E0B', '#EF4444', '#8B5CF6', '#EC4899'];

export const DashboardPage: React.FC = () => {
  const [data, setData] = useState<DashboardData | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadDashboard();
  }, []);

  const loadDashboard = async () => {
    try {
      setLoading(true);
      const result = await reportsApi.getDashboard();
      setData(result);
    } catch (err) {
      setError('Ошибка при загрузке дашборда');
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <LoadingSpinner text="Загрузка дашборда..." />;
  if (error) return <ErrorMessage message={error} onRetry={loadDashboard} />;
  if (!data) return null;

  const cards = [
    { label: 'Всего клиентов', value: data.summaryCards.totalClients, icon: '👥', color: 'bg-blue-500' },
    { label: 'Активных проектов', value: data.summaryCards.activeProjects, icon: '🏗️', color: 'bg-green-500' },
    { label: 'Выручка за месяц', value: `${data.summaryCards.monthlyRevenue.toLocaleString()} ₽`, icon: '💰', color: 'bg-purple-500' },
    { label: 'Задолженность', value: `${data.summaryCards.outstandingDebt.toLocaleString()} ₽`, icon: '⚠️', color: 'bg-red-500' },
    { label: 'Новых клиентов', value: data.summaryCards.newClientsThisMonth, icon: '🆕', color: 'bg-indigo-500' },
    { label: 'Просрочено проектов', value: data.summaryCards.overdueProjects, icon: '⏰', color: 'bg-orange-500' },
  ];

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">Дашборд</h1>

      {/* Карточки */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6 gap-4">
        {cards.map((card) => (
          <div key={card.label} className="bg-white rounded-lg shadow p-4 border border-gray-200">
            <div className="flex items-center justify-between">
              <span className="text-2xl">{card.icon}</span>
            </div>
            <p className="text-2xl font-bold text-gray-900 mt-2">{card.value}</p>
            <p className="text-sm text-gray-600">{card.label}</p>
          </div>
        ))}
      </div>

      {/* Графики */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Доходы по месяцам */}
        <div className="bg-white rounded-lg shadow p-6 border border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">Доходы и расходы по месяцам</h3>
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={data.monthlyRevenue}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="month" fontSize={12} />
              <YAxis fontSize={12} />
              <Tooltip />
              <Legend />
              <Bar dataKey="revenue" name="Доходы" fill="#10B981" />
              <Bar dataKey="expenses" name="Расходы" fill="#EF4444" />
            </BarChart>
          </ResponsiveContainer>
        </div>

        {/* Распределение проектов */}
        <div className="bg-white rounded-lg shadow p-6 border border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">Типы проектов</h3>
          <ResponsiveContainer width="100%" height={300}>
            <PieChart>
              <Pie data={data.projectTypeDistribution} dataKey="count" nameKey="label" cx="50%" cy="50%" outerRadius={100} label>
                {data.projectTypeDistribution.map((_, index) => (
                  <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                ))}
              </Pie>
              <Tooltip />
              <Legend />
            </PieChart>
          </ResponsiveContainer>
        </div>

        {/* Топ клиентов */}
        <div className="bg-white rounded-lg shadow p-6 border border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">Топ-5 клиентов</h3>
          <div className="space-y-3">
            {data.topClients.map((client, index) => (
              <div key={client.id} className="flex items-center justify-between">
                <div className="flex items-center">
                  <span className="w-8 h-8 bg-blue-100 text-blue-700 rounded-full flex items-center justify-center font-semibold text-sm mr-3">
                    {index + 1}
                  </span>
                  <div>
                    <p className="font-medium text-gray-900">{client.fullName}</p>
                    <p className="text-sm text-gray-500">{client.projectsCount} проектов</p>
                  </div>
                </div>
                <span className="font-semibold text-gray-900">{client.totalAmount.toLocaleString()} ₽</span>
              </div>
            ))}
          </div>
        </div>

        {/* Загрузка бригад */}
        <div className="bg-white rounded-lg shadow p-6 border border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">Загрузка бригад</h3>
          <div className="space-y-4">
            {data.brigadeLoad.map((brigade) => (
              <div key={brigade.brigadeId}>
                <div className="flex justify-between text-sm mb-1">
                  <span className="text-gray-700">{brigade.brigadeName}</span>
                  <span className="text-gray-500">{brigade.currentProjects}/{brigade.maxCapacity}</span>
                </div>
                <div className="w-full bg-gray-200 rounded-full h-2.5">
                  <div
                    className={`h-2.5 rounded-full ${brigade.loadPercentage > 80 ? 'bg-red-500' : brigade.loadPercentage > 50 ? 'bg-yellow-500' : 'bg-green-500'}`}
                    style={{ width: `${Math.min(brigade.loadPercentage, 100)}%` }}
                  />
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};