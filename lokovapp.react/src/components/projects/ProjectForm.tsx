import React, { useState, useEffect } from 'react';
import type { Project, CreateProjectDto, UpdateProjectDto } from '../../types/project';
import { projectsApi } from '../../api/projects.api';
import { Modal } from '../common/Modal';
import { Button } from '../common/Button';

interface ProjectFormProps {
  project: Project | null;
  clientId?: string;
  onClose: () => void;
  onSuccess: () => void;
}

export const ProjectForm: React.FC<ProjectFormProps> = ({ project, clientId, onClose, onSuccess }) => {
  const isEditing = !!project;
  const [formData, setFormData] = useState<CreateProjectDto>({
    clientId: clientId || '',
    name: '',
    description: '',
    type: 'MajorRepair',
    address: '',
    estimatedCost: 0,
    startDate: '',
    plannedEndDate: '',
    brigadeId: ''
  });
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (project) {
      setFormData({
        clientId: project.clientId,
        name: project.name,
        description: project.description || '',
        type: project.type,
        address: project.address,
        estimatedCost: project.estimatedCost,
        startDate: project.startDate?.split('T')[0] || '',
        plannedEndDate: project.plannedEndDate?.split('T')[0] || '',
        brigadeId: ''
      });
    }
  }, [project]);

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {};
    if (!formData.clientId) newErrors.clientId = 'Выберите клиента';
    if (!formData.name.trim()) newErrors.name = 'Название обязательно';
    if (!formData.address.trim()) newErrors.address = 'Адрес обязателен';
    if (formData.estimatedCost <= 0) newErrors.estimatedCost = 'Укажите стоимость';
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;

    setLoading(true);
    try {
      if (isEditing && project) {
        await projectsApi.updateProject(project.id, formData as UpdateProjectDto);
      } else {
        await projectsApi.createProject(formData);
      }
      onSuccess();
    } catch (err: any) {
      setErrors({ submit: err.response?.data?.message || 'Ошибка при сохранении' });
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal
      isOpen={true}
      onClose={onClose}
      title={isEditing ? 'Редактирование проекта' : 'Новый проект'}
      size="lg"
      footer={
        <>
          <Button variant="secondary" onClick={onClose}>Отмена</Button>
          <Button onClick={handleSubmit} loading={loading}>
            {isEditing ? 'Сохранить' : 'Создать'}
          </Button>
        </>
      }
    >
      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Тип проекта *</label>
          <select
            value={formData.type}
            onChange={(e) => setFormData({ ...formData, type: e.target.value })}
            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
          >
            <option value="MajorRepair">Капитальный ремонт</option>
            <option value="PartialRepair">Частичный ремонт</option>
            <option value="RoofWorks">Кровельные работы</option>
            <option value="FacadeWorks">Фасадные работы</option>
            <option value="CombinedWorks">Комплексные работы</option>
          </select>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Название *</label>
          <input
            type="text"
            value={formData.name}
            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
          />
          {errors.name && <p className="text-red-500 text-xs mt-1">{errors.name}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Описание</label>
          <textarea
            value={formData.description}
            onChange={(e) => setFormData({ ...formData, description: e.target.value })}
            rows={3}
            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
          />
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Адрес *</label>
          <input
            type="text"
            value={formData.address}
            onChange={(e) => setFormData({ ...formData, address: e.target.value })}
            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
          />
          {errors.address && <p className="text-red-500 text-xs mt-1">{errors.address}</p>}
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Сметная стоимость *</label>
            <input
              type="number"
              value={formData.estimatedCost}
              onChange={(e) => setFormData({ ...formData, estimatedCost: Number(e.target.value) })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
            />
            {errors.estimatedCost && <p className="text-red-500 text-xs mt-1">{errors.estimatedCost}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Дата начала</label>
            <input
              type="date"
              value={formData.startDate}
              onChange={(e) => setFormData({ ...formData, startDate: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Плановое окончание</label>
            <input
              type="date"
              value={formData.plannedEndDate}
              onChange={(e) => setFormData({ ...formData, plannedEndDate: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
            />
          </div>
        </div>

        {errors.submit && (
          <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg">
            {errors.submit}
          </div>
        )}
      </form>
    </Modal>
  );
};