import React, { useState } from 'react';
import { Outlet, NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';

const navigation = [
    { name: 'Дашборд', href: '/dashboard', icon: '📊' },
    { name: 'Клиенты', href: '/clients', icon: '👥' },
    { name: 'Проекты', href: '/projects', icon: '🏗️' },
    { name: 'Отчеты', href: '/reports', icon: '📄' },
];

const adminNavigation = [
    { name: 'Пользователи', href: '/users', icon: '⚙️' },
];

export const Layout: React.FC = () => {
    const { user, logout } = useAuth();
    const navigate = useNavigate();
    const [sidebarOpen, setSidebarOpen] = useState(false);
    const [userMenuOpen, setUserMenuOpen] = useState(false);

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    const isAdmin = user?.role === 'Admin';

    // Объединяем навигацию
    const allNavigation = [...navigation, ...(isAdmin ? adminNavigation : [])];

    const linkClasses = (isActive: boolean) =>
        `flex items-center px-4 py-3 rounded-lg text-sm font-medium transition-colors ${isActive ? 'bg-blue-50 text-blue-700' : 'text-gray-700 hover:bg-gray-100'
        }`;

    return (
        <div className="min-h-screen bg-gray-50">
            {/* Мобильная боковая панель */}
            {sidebarOpen && (
                <div className="fixed inset-0 z-40 lg:hidden">
                    <div className="fixed inset-0 bg-black bg-opacity-50" onClick={() => setSidebarOpen(false)} />
                    <div className="fixed inset-y-0 left-0 w-64 bg-white shadow-xl">
                        <div className="p-6 border-b border-gray-200">
                            <h2 className="text-xl font-bold text-gray-900">Локов CRM</h2>
                        </div>
                        <nav className="mt-4 px-4 space-y-2">
                            {allNavigation.map((item) => (
                                <NavLink
                                    key={item.href}
                                    to={item.href}
                                    onClick={() => setSidebarOpen(false)}
                                    className={({ isActive }) => linkClasses(isActive)}
                                >
                                    <span className="mr-3">{item.icon}</span>
                                    {item.name}
                                </NavLink>
                            ))}
                        </nav>
                    </div>
                </div>
            )}

            {/* Боковая панель (десктоп) */}
            <div className="hidden lg:fixed lg:inset-y-0 lg:flex lg:w-64 lg:flex-col">
                <div className="flex flex-col flex-grow bg-white border-r border-gray-200">
                    <div className="p-6 border-b border-gray-200">
                        <h1 className="text-xl font-bold text-gray-900">🏗️ Локов CRM</h1>
                        <p className="text-xs text-gray-500 mt-1">ИП Локов А.М.</p>
                    </div>
                    <nav className="flex-1 px-4 mt-4 space-y-1 overflow-y-auto">
                        {navigation.map((item) => (
                            <NavLink
                                key={item.href}
                                to={item.href}
                                className={({ isActive }) => linkClasses(isActive)}
                            >
                                <span className="mr-3 text-lg">{item.icon}</span>
                                {item.name}
                            </NavLink>
                        ))}

                        {/* Раздел администрирования */}
                        {isAdmin && (
                            <>
                                <div className="pt-4 pb-2">
                                    <p className="px-4 text-xs font-semibold text-gray-400 uppercase tracking-wider">
                                        Администрирование
                                    </p>
                                </div>
                                {adminNavigation.map((item) => (
                                    <NavLink
                                        key={item.href}
                                        to={item.href}
                                        className={({ isActive }) => linkClasses(isActive)}
                                    >
                                        <span className="mr-3 text-lg">{item.icon}</span>
                                        {item.name}
                                    </NavLink>
                                ))}
                            </>
                        )}
                    </nav>
                    <div className="p-4 border-t border-gray-200">
                        <div className="text-xs text-gray-500">© 2026 ИП Локов А.М.</div>
                    </div>
                </div>
            </div>

            {/* Основной контент */}
            <div className="lg:pl-64">
                {/* Верхняя панель */}
                <div className="sticky top-0 z-30 bg-white border-b border-gray-200 shadow-sm">
                    <div className="flex items-center justify-between h-16 px-4 sm:px-6">
                        <button
                            onClick={() => setSidebarOpen(true)}
                            className="lg:hidden text-gray-500 hover:text-gray-700"
                        >
                            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
                            </svg>
                        </button>
                        <div className="flex-1" />

                        {/* Меню пользователя */}
                        <div className="relative">
                            <button
                                onClick={() => setUserMenuOpen(!userMenuOpen)}
                                className="flex items-center space-x-3 px-3 py-2 rounded-lg hover:bg-gray-100 transition-colors"
                            >
                                <div className="w-8 h-8 bg-blue-600 rounded-full flex items-center justify-center text-white font-semibold text-sm">
                                    {user?.fullName?.charAt(0) || 'U'}
                                </div>
                                <div className="hidden sm:block text-left">
                                    <p className="text-sm font-medium text-gray-700">{user?.fullName || 'Пользователь'}</p>
                                    <p className="text-xs text-gray-500">{user?.role === 'Admin' ? 'Администратор' : 'Менеджер'}</p>
                                </div>
                                <svg className="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                                </svg>
                            </button>

                            {userMenuOpen && (
                                <>
                                    <div className="fixed inset-0 z-40" onClick={() => setUserMenuOpen(false)} />
                                    <div className="absolute right-0 mt-2 w-48 bg-white rounded-lg shadow-lg border border-gray-200 py-1 z-50">
                                        <button
                                            onClick={() => { navigate('/profile'); setUserMenuOpen(false); }}
                                            className="w-full text-left px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                                        >
                                            👤 Профиль
                                        </button>
                                        {isAdmin && (
                                            <button
                                                onClick={() => { navigate('/users'); setUserMenuOpen(false); }}
                                                className="w-full text-left px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                                            >
                                                ⚙️ Пользователи
                                            </button>
                                        )}
                                        <hr className="my-1 border-gray-200" />
                                        <button
                                            onClick={handleLogout}
                                            className="w-full text-left px-4 py-2 text-sm text-red-600 hover:bg-red-50"
                                        >
                                            🚪 Выйти
                                        </button>
                                    </div>
                                </>
                            )}
                        </div>
                    </div>
                </div>

                {/* Контент страницы */}
                <main className="p-4 sm:p-6 lg:p-8">
                    <Outlet />
                </main>
            </div>
        </div>
    );
};