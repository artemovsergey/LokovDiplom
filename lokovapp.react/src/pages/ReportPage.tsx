import React, { useState } from 'react';
import { Button } from '../components/common/Button';
import { reportsApi } from '../api/reports.api';

export const ReportsPage: React.FC = () => {
    const [loadingPdf, setLoadingPdf] = useState<string | null>(null);
    const [loadingExcel, setLoadingExcel] = useState<string | null>(null);

    const downloadFile = (blob: Blob, fileName: string) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = fileName;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
    };

    const handleExportPdf = async (type: string) => {
        setLoadingPdf(type);
        try {
            let blob: Blob;
            const today = new Date().toISOString().split('T')[0];

            switch (type) {
                case 'clients':
                    blob = await reportsApi.exportClientsPdf();
                    downloadFile(blob, `clients_report_${today}.pdf`);
                    break;
                case 'projects':
                    blob = await reportsApi.exportProjectsPdf();
                    downloadFile(blob, `projects_report_${today}.pdf`);
                    break;
                case 'financial':
                    blob = await reportsApi.exportFinancialPdf();
                    downloadFile(blob, `financial_report_${today}.pdf`);
                    break;
            }
        } catch (err) {
            alert('Ошибка при экспорте PDF');
        } finally {
            setLoadingPdf(null);
        }
    };

    const handleExportExcel = async (type: string) => {
        setLoadingExcel(type);
        try {
            let blob: Blob;
            const today = new Date().toISOString().split('T')[0];

            switch (type) {
                case 'clients':
                    blob = await reportsApi.exportClientsExcel();
                    downloadFile(blob, `clients_report_${today}.xlsx`);
                    break;
                case 'projects':
                    blob = await reportsApi.exportProjectsExcel();
                    downloadFile(blob, `projects_report_${today}.xlsx`);
                    break;
                case 'financial':
                    blob = await reportsApi.exportFinancialExcel();
                    downloadFile(blob, `financial_report_${today}.xlsx`);
                    break;
            }
        } catch (err) {
            alert('Ошибка при экспорте Excel');
        } finally {
            setLoadingExcel(null);
        }
    };

    const reports = [
        { id: 'clients', name: 'Отчет по клиентам', description: 'Список клиентов с контактами, статусами и статистикой', icon: '👥' },
        { id: 'projects', name: 'Отчет по проектам', description: 'Список проектов с финансовыми показателями и статусами', icon: '🏗️' },
        { id: 'financial', name: 'Финансовый отчет', description: 'Доходы, расходы, прибыль по проектам и месяцам', icon: '💰' },
    ];

    return (
        <div className="space-y-6">
            <h1 className="text-2xl font-bold text-gray-900">Отчеты</h1>

            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                {reports.map((report) => (
                    <div key={report.id} className="bg-white rounded-lg shadow p-6 border border-gray-200">
                        <span className="text-4xl">{report.icon}</span>
                        <h3 className="text-lg font-semibold text-gray-900 mt-4">{report.name}</h3>
                        <p className="text-sm text-gray-600 mt-2">{report.description}</p>
                        <div className="mt-6 space-y-2">
                            <Button
                                variant="primary"
                                size="sm"
                                className="w-full"
                                loading={loadingPdf === report.id}
                                onClick={() => handleExportPdf(report.id)}
                            >
                                📄 Экспорт PDF
                            </Button>
                            <Button
                                variant="secondary"
                                size="sm"
                                className="w-full"
                                loading={loadingExcel === report.id}
                                onClick={() => handleExportExcel(report.id)}
                            >
                                📊 Экспорт Excel
                            </Button>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
};