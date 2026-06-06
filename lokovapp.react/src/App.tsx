import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { Layout } from './components/common/Layout';
import { PrivateRoute } from './components/common/PrivateRoute';
import { ClientDetailPage } from './pages/ClientDetailPage';
import { ClientsPage } from './pages/ClientPage';
import { DashboardPage } from './pages/DashBoardPage';
import { ProjectsPage } from './pages/ProjectsPage';
import { ReportsPage } from './pages/ReportPage';
import { UsersPage } from './pages/UsersPage';
import { ProjectCreatePage } from './pages/ProjectCreatePage';
import { ProjectEditPage } from './pages/ProjectEditPage';
import { ProjectDetailPage } from './pages/ProjectDetailPage';
import { ClientEditPage } from './pages/ClientEditPage';
import { ProfilePage } from './pages/ProfilePage';


const App: React.FC = () => {
  return (
    <Router>
      <AuthProvider>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route path="/" element={<PrivateRoute><Layout /></PrivateRoute>}>
            <Route index element={<Navigate to="/dashboard" replace />} />
            <Route path="dashboard" element={<DashboardPage />} />
            <Route path="clients" element={<ClientsPage />} />
            <Route path="clients/:id" element={<ClientDetailPage />} />
            <Route path="clients/:id/edit" element={<ClientEditPage />} />

            <Route path="projects" element={<ProjectsPage />} />
            <Route path="projects/:id" element={<ProjectDetailPage />} />
            <Route path="reports" element={<ReportsPage />} />
            <Route path="users" element={<UsersPage />} />
            <Route path="projects" element={<ProjectsPage />} />
            <Route path="projects/new" element={<ProjectCreatePage />} />
            <Route path="projects/:id" element={<ProjectDetailPage />} />
            <Route path="projects/:id/edit" element={<ProjectEditPage />} />

            <Route path="profile" element={<ProfilePage />} />

          </Route>
        </Routes>
      </AuthProvider>
    </Router>
  );
};

export default App;