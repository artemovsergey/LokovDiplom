import React, { useState, useEffect } from 'react';
import type { Client, CreateClientDto, UpdateClientDto } from '../../types/client';

import { Modal } from '../common/Modal';
import { Button } from '../common/Button';
import { clientsApi } from '../../api/client.api';

interface ClientFormProps {
  client: Client | null;
  onClose: () => void;
  onSuccess: () => void;
}

export const ClientForm: React.FC<ClientFormProps> = ({ client, onClose, onSuccess }) => {
  const isEditing = !!client;
  const [formData, setFormData] = useState<CreateClientDto>({
    firstName: '',
    lastName: '',
    patronymic: '',
    phone: '',
    additionalPhone: '',
    email: '',
    address: '',
    source: 'DirectContact',
    category: 'Individual'
  });
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (client) {
      setFormData({
        firstName: client.firstName,
        lastName: client.lastName,
        patronymic: client.patronymic || '',
        phone: client.phone,
        additionalPhone: client.additionalPhone || '',
        email: client.email || '',
        address: client.address,
        source: client.source,
        category: client.category
      });
    }
  }, [client]);

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {};
    if (!formData.firstName.trim()) newErrors.firstName = 'Имя обязательно';
    if (!formData.lastName.trim()) newErrors.lastName = 'Фамилия обязательна';
    if (!formData.phone.trim()) newErrors.phone = 'Телефон обязателен';
    else if (!/^\+?[\d\s\-()]{10,}$/.test(formData.phone)) newErrors.phone = 'Неверный формат';
    if (!formData.address.trim()) newErrors.address = 'Адрес обязателен';
    if (formData.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) newErrors.email = 'Неверный email';
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;

    setLoading(true);
    try {
      if (isEditing && client) {
        await clientsApi.updateClient(client.id, formData as UpdateClientDto);
      } else {
        await clientsApi.createClient(formData);
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
      title={isEditing ? 'Редактирование клиента' : 'Новый клиент'}
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
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Фамилия *</label>
            <input
              type="text"
              value={formData.lastName}
              onChange={e => setFormData({ ...formData, lastName: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            />
            {errors.lastName && <p className="text-red-500 text-xs mt-1">{errors.lastName}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Имя *</label>
            <input
              type="text"
              value={formData.firstName}
              onChange={e => setFormData({ ...formData, firstName: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            />
            {errors.firstName && <p className="text-red-500 text-xs mt-1">{errors.firstName}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Отчество</label>
            <input
              type="text"
              value={formData.patronymic}
              onChange={e => setFormData({ ...formData, patronymic: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Телефон *</label>
            <input
              type="tel"
              value={formData.phone}
              onChange={e => setFormData({ ...formData, phone: e.target.value })}
              placeholder="+7 (999) 123-45-67"
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            />
            {errors.phone && <p className="text-red-500 text-xs mt-1">{errors.phone}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Email</label>
            <input
              type="email"
              value={formData.email}
              onChange={e => setFormData({ ...formData, email: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            />
            {errors.email && <p className="text-red-500 text-xs mt-1">{errors.email}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Категория</label>
            <select
              value={formData.category}
              onChange={e => setFormData({ ...formData, category: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            >
              <option value="Individual">Физическое лицо</option>
              <option value="LegalEntity">Юридическое лицо</option>
              <option value="Entrepreneur">ИП</option>
            </select>
          </div>
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Адрес *</label>
          <input
            type="text"
            value={formData.address}
            onChange={e => setFormData({ ...formData, address: e.target.value })}
            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          />
          {errors.address && <p className="text-red-500 text-xs mt-1">{errors.address}</p>}
        </div>
        {errors.submit && (
          <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg">{errors.submit}</div>
        )}
      </form>
    </Modal>
  );
};