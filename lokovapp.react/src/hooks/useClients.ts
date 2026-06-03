import { useState, useEffect, useCallback } from 'react';
import type { Client } from '../types/client';
import { clientService } from '../services/api';

export const useClients = () => {
  const [clients, setClients] = useState<Client[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchClients = useCallback(async (search?: string, status?: string) => {
    try {
      setLoading(true);
      setError(null);
      const data = await clientService.getClients(search, status);
      setClients(data);
    } catch (err) {
      setError('Ошибка при загрузке клиентов');
      console.error('Error fetching clients:', err);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchClients();
  }, [fetchClients]);

  return { clients, loading, error, fetchClients };
};