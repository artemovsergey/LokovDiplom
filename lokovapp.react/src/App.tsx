import React from 'react';
import ClientList from './components/ClientList';

const App: React.FC = () => {
  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm">
        <div className="container mx-auto px-4 py-4">
          <h1 className="text-2xl font-bold text-gray-900">
            ИП Локов А.М. - Учет клиентов
          </h1>
        </div>
      </nav>
      <ClientList />
    </div>
  );
};

export default App;