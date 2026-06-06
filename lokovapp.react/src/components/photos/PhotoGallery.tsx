import React, { useEffect, useState, useCallback } from 'react';
import type { Photo, PhotoFilter } from '../../types/photo';
import type { PagedResponse } from '../../types/common';
import { Button } from '../common/Button';
import { LoadingSpinner } from '../common/LoadingSpinner';
import { EmptyState } from '../common/EmptyState';
import { ErrorMessage } from '../common/ErrorMessage';
import { Pagination } from '../common/Pagination';
import { PhotoUpload } from './PhotoUpload';
import { photosApi } from '../../api/photos.api';
import { PhotoCard } from './PhotoCard';

interface PhotoGalleryProps {
    projectId: string;
}

export const PhotoGallery: React.FC<PhotoGalleryProps> = ({ projectId }) => {
    const [data, setData] = useState<PagedResponse<Photo> | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [filter, setFilter] = useState<PhotoFilter>({ page: 1, pageSize: 12, sortBy: 'date', sortOrder: 'desc' });
    const [showUpload, setShowUpload] = useState(false);
    const [selectedPhotos, setSelectedPhotos] = useState<Set<string>>(new Set());
    const [viewingPhoto, setViewingPhoto] = useState<Photo | null>(null);

    const loadPhotos = useCallback(async () => {
        try {
            setLoading(true);
            setError(null);
            const result = await photosApi.getProjectPhotos(projectId, filter);
            setData(result);
        } catch (err: any) {
            setError(err.response?.data?.message || 'Ошибка при загрузке фотографий');
        } finally {
            setLoading(false);
        }
    }, [projectId, filter]);

    useEffect(() => {
        loadPhotos();
    }, [loadPhotos]);

    const handlePageChange = (page: number) => {
        setFilter(prev => ({ ...prev, page }));
    };

    const handleCategoryFilter = (category: string) => {
        setFilter(prev => ({ ...prev, category: category || undefined, page: 1 }));
    };

    const handleUploadSuccess = () => {
        setShowUpload(false);
        loadPhotos();
    };

    const handleSelectPhoto = (photoId: string) => {
        setSelectedPhotos(prev => {
            const newSet = new Set(prev);
            if (newSet.has(photoId)) {
                newSet.delete(photoId);
            } else {
                newSet.add(photoId);
            }
            return newSet;
        });
    };

    const handleDeleteSelected = async () => {
        if (selectedPhotos.size === 0) return;
        if (!window.confirm(`Удалить ${selectedPhotos.size} фотографий?`)) return;

        try {
            await photosApi.deleteMultiplePhotos(projectId, Array.from(selectedPhotos));
            setSelectedPhotos(new Set());
            loadPhotos();
        } catch (err) {
            alert('Ошибка при удалении фотографий');
        }
    };

    const handleDeletePhoto = async (photoId: string) => {
        if (!window.confirm('Удалить фотографию?')) return;
        try {
            await photosApi.deletePhoto(projectId, photoId);
            loadPhotos();
        } catch (err) {
            alert('Ошибка при удалении фотографии');
        }
    };

    const categories = [
        { value: '', label: 'Все' },
        { value: 'Before', label: 'До работ' },
        { value: 'WorkProgress', label: 'Процесс' },
        { value: 'After', label: 'После работ' },
        { value: 'Defect', label: 'Дефекты' },
        { value: 'Materials', label: 'Материалы' },
    ];

    return (
        <div className="space-y-4">
            {/* Верхняя панель */}
            <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
                <div className="flex flex-wrap gap-2">
                    {categories.map((cat) => (
                        <button
                            key={cat.value}
                            onClick={() => handleCategoryFilter(cat.value)}
                            className={`px-3 py-1.5 rounded-full text-sm font-medium transition-colors ${(filter.category || '') === cat.value
                                ? 'bg-blue-600 text-white'
                                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                                }`}
                        >
                            {cat.label}
                        </button>
                    ))}
                </div>
                <div className="flex gap-2">
                    {selectedPhotos.size > 0 && (
                        <Button variant="danger" size="sm" onClick={handleDeleteSelected}>
                            Удалить ({selectedPhotos.size})
                        </Button>
                    )}
                    <Button size="sm" onClick={() => setShowUpload(true)}>
                        <svg className="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
                        </svg>
                        Загрузить фото
                    </Button>
                </div>
            </div>

            {/* Контент */}
            {loading ? (
                <LoadingSpinner text="Загрузка фотографий..." />
            ) : error ? (
                <ErrorMessage message={error} onRetry={loadPhotos} />
            ) : data && data.items.length === 0 ? (
                <EmptyState
                    title="Нет фотографий"
                    description="Загрузите фотографии объекта до, во время или после работ"
                    icon={
                        <svg className="mx-auto h-16 w-16 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                        </svg>
                    }
                    action={<Button onClick={() => setShowUpload(true)}>Загрузить фото</Button>}
                />
            ) : (
                <>
                    <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
                        {data?.items.map((photo) => (
                            <PhotoCard
                                key={photo.id}
                                photo={photo}
                                projectId={projectId}
                                isSelected={selectedPhotos.has(photo.id)}
                                onSelect={handleSelectPhoto}
                                onDelete={handleDeletePhoto}
                                onView={setViewingPhoto}
                            />
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

            {/* Модальное окно загрузки */}
            {showUpload && (
                <PhotoUpload
                    projectId={projectId}
                    onClose={() => setShowUpload(false)}
                    onSuccess={handleUploadSuccess}
                />
            )}

            {/* Модальное окно просмотра */}
            {viewingPhoto && (
                <div
                    className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-90 p-4"
                    onClick={() => setViewingPhoto(null)}
                >
                    <div className="relative max-w-5xl max-h-[90vh]">
                        <button
                            onClick={() => setViewingPhoto(null)}
                            className="absolute -top-10 right-0 text-white hover:text-gray-300"
                        >
                            <svg className="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                            </svg>
                        </button>
                        <img
                            src={photosApi.getPhotoUrl(projectId, viewingPhoto.id)}
                            alt={viewingPhoto.description || viewingPhoto.originalFileName}
                            className="max-w-full max-h-[85vh] object-contain rounded-lg"
                        />
                        <div className="absolute bottom-0 left-0 right-0 bg-gradient-to-t from-black to-transparent p-4 rounded-b-lg">
                            <p className="text-white font-medium">{viewingPhoto.originalFileName}</p>
                            {viewingPhoto.description && (
                                <p className="text-gray-300 text-sm">{viewingPhoto.description}</p>
                            )}
                            <p className="text-gray-400 text-xs mt-1">
                                {new Date(viewingPhoto.createdAt).toLocaleString('ru-RU')} • {viewingPhoto.uploadedByName}
                            </p>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};