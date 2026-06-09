import axios from 'axios';

// const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5001/api/v1';
const API_URL = import.meta.env.VITE_API_URL;

const api = axios.create({
  baseURL: API_URL,
  headers: { 
    'Content-Type': 'application/json'
  }
});

// Логирование запросов
api.interceptors.request.use((config) => {
  console.log('Request:', config.method?.toUpperCase(), config.url, config.data instanceof FormData ? 'FormData' : config.data);
  return config;
});

// Обработка ошибок (без редиректа при 401)
api.interceptors.response.use(
  (response) => {
    console.log('Response:', response.status);
    return response;
  },
  (error) => {
    console.error('API Error:', error.response?.status, error.response?.data);
    // Не делаем редирект при 401
    return Promise.reject(error);
  }
);

export default api;