import React, { useEffect, useState, useCallback, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { projectsApi } from '../../api/projects.api';
import type { Project, ProjectFilter } from '../../types/project';
import type { PagedResponse } from '../../types/common';
import { Button } from '../common/Button';
import { StatusBadge } from '../common/StatusBadge';
import { SearchBar } from '../common/SearchBar';
import { Pagination } from '../common/Pagination';
import { LoadingSpinner } from '../common/LoadingSpinner';
import { ErrorMessage } from '../common/ErrorMessage';
import { EmptyState } from '../common/EmptyState';

const typeLabels: Record<string, string> = {
  MajorRepair: 'Капремонт',
  PartialRepair: 'Частичный',
  RoofWorks: 'Кровля',
  FacadeWorks: 'Фасад',
  CombinedWorks: 'Комплекс'
};

const typeIcons: Record<string, string> = {
  MajorRepair: '🏠',
  PartialRepair: '🔧',
  RoofWorks: '🏗️',
  FacadeWorks: '🧱',
  CombinedWorks: '🏢'
};

export const ProjectList: React.FC = () => {
  const navigate = useNavigate();
  const [data, setData] = useState<PagedResponse<Project> | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filter, setFilter] = useState<ProjectFilter>({ page: 1, pageSize: 12 });

  const prevFilterRef = useRef<ProjectFilter | null>(null);

  const loadProjects = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const result = await projectsApi.getProjects(filter);
      setData(result);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Ошибка при загрузке проектов');
    } finally {
      setLoading(false);
    }
  }, [filter]);

  useEffect(() => {
    const currentFilter = {
      search: filter.search,
      status: filter.status,
      type: filter.type,
      page: filter.page,
      pageSize: filter.pageSize
    };

    const prevFilter = prevFilterRef.current;

    if (prevFilter &&
      prevFilter.search === currentFilter.search &&
      prevFilter.status === currentFilter.status &&
      prevFilter.type === currentFilter.type &&
      prevFilter.page === currentFilter.page &&
      prevFilter.pageSize === currentFilter.pageSize) {
      return;
    }

    prevFilterRef.current = currentFilter;
    loadProjects();
  }, [filter, loadProjects]);

  const handleSearch = useCallback((search: string) => {
    setFilter(prev => ({ ...prev, search, page: 1 }));
  }, []);

  const handleStatusFilter = useCallback((status: string) => {
    setFilter(prev => ({ ...prev, status: status || undefined, page: 1 }));
  }, []);

  const handleTypeFilter = useCallback((type: string) => {
    setFilter(prev => ({ ...prev, type: type || undefined, page: 1 }));
  }, []);

  const handlePageChange = useCallback((page: number) => {
    setFilter(prev => ({ ...prev, page }));
  }, []);

  const handleRetry = useCallback(() => {
    setFilter(prev => ({ ...prev }));
  }, []);

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <h1 className="text-2xl font-bold text-gray-900">Проекты</h1>
        <Button onClick={() => navigate('/projects/new')}>
          <svg className="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
          </svg>
          Новый проект
        </Button>
      </div>

      <div className="space-y-4">
        <SearchBar onSearch={handleSearch} placeholder="Поиск по номеру, названию или адресу..." />

        <div className="flex flex-wrap gap-2">
          <span className="text-sm text-gray-500 self-center mr-2">Статус:</span>
          {['', 'New', 'InProgress', 'Completed', 'Cancelled'].map(status => (
            <button
              key={status}
              onClick={() => handleStatusFilter(status)}
              className={`px-3 py-1.5 rounded-full text-sm font-medium ${(filter.status || '') === status ? 'bg-blue-600 text-white' : 'bg-gray-100 text-gray-700 hover:bg-gray-200'}`}
            >
              {status === '' ? 'Все' :
                status === 'New' ? 'Новые' :
                  status === 'InProgress' ? 'В работе' :
                    status === 'Completed' ? 'Завершенные' : 'Отмененные'}
            </button>
          ))}
        </div>

        <div className="flex flex-wrap gap-2">
          <span className="text-sm text-gray-500 self-center mr-2">Тип:</span>
          {['', 'MajorRepair', 'PartialRepair', 'RoofWorks', 'FacadeWorks', 'CombinedWorks'].map(type => (
            <button
              key={type}
              onClick={() => handleTypeFilter(type)}
              className={`px-3 py-1.5 rounded-full text-sm font-medium ${(filter.type || '') === type ? 'bg-blue-600 text-white' : 'bg-gray-100 text-gray-700 hover:bg-gray-200'}`}
            >
              {type === '' ? 'Все' : typeLabels[type] || type}
            </button>
          ))}
        </div>
      </div>

      {loading ? (
        <LoadingSpinner />
      ) : error ? (
        <ErrorMessage message={error} onRetry={handleRetry} />
      ) : data && data.items.length === 0 ? (
        <EmptyState
          title="Проекты не найдены"
          description="Попробуйте изменить параметры поиска или создайте новый проект"
          action={<Button onClick={() => navigate('/projects/new')}>Создать проект</Button>}
        />
      ) : (
        <>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {data?.items.map(project => (
              <div
                key={project.id}
                className="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition-shadow cursor-pointer border border-gray-200"
                onClick={() => navigate(`/projects/${project.id}`)}
              >
                <div className="flex justify-between items-start mb-3">
                  <div className="flex items-center">
                    <span className="text-2xl mr-3">{typeIcons[project.type] || '📋'}</span>
                    <div>
                      <p className="text-sm text-gray-500">{project.number}</p>
                      <h3 className="font-semibold text-gray-900 truncate max-w-[200px]">{project.name}</h3>
                    </div>
                  </div>
                  <StatusBadge status={project.status} type="project" size="sm" />
                </div>

                <div className="space-y-2 text-sm">
                  <p className="text-gray-600 truncate">{project.clientName}</p>
                  <p className="text-gray-500 truncate">{project.address}</p>
                </div>

                <div className="mt-4 pt-4 border-t border-gray-200">
                  <div className="flex justify-between items-center mb-2">
                    <span className="text-sm text-gray-500">Выполнение</span>
                    <span className="text-sm font-semibold">{project.completionPercentage}%</span>
                  </div>
                  <div className="w-full bg-gray-200 rounded-full h-2">
                    <div
                      className="bg-blue-600 h-2 rounded-full"
                      style={{ width: `${project.completionPercentage}%` }}
                    />
                  </div>
                </div>

                <div className="mt-4 flex justify-between items-center">
                  <div>
                    <p className="text-xs text-gray-500">Бюджет</p>
                    <p className="font-semibold text-gray-900">{project.estimatedCost.toLocaleString()} ₽</p>
                  </div>
                  {project.debt > 0 && (
                    <div className="text-right">
                      <p className="text-xs text-gray-500">Долг</p>
                      <p className="font-semibold text-red-600">{project.debt.toLocaleString()} ₽</p>
                    </div>
                  )}
                </div>
              </div>
            ))}
          </div>

          {data && (
            <Pagination
              currentPage={data.page}
              totalPages={data.totalPages}
              totalItems={data.totalCount}
              onPageChange={handlePageChange}
            />
          )}
        </>
      )}
    </div>
  );
};