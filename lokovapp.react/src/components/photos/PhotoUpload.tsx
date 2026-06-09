import React, { useState, useCallback } from 'react';
import { useDropzone } from 'react-dropzone';
import { Modal } from '../common/Modal';
import { Button } from '../common/Button';
import { photosApi } from '../../api/photos.api';

interface PhotoUploadProps {
    projectId: string;
    onClose: () => void;
    onSuccess: () => void;
}

export const PhotoUpload: React.FC<PhotoUploadProps> = ({ projectId, onClose, onSuccess }) => {
    const [files, setFiles] = useState<File[]>([]);
    const [category, setCategory] = useState('WorkProgress');
    const [description, setDescription] = useState('');
    const [uploading, setUploading] = useState(false);
    const [progress, setProgress] = useState(0);

    const onDrop = useCallback((acceptedFiles: File[]) => {
        setFiles(prev => [...prev, ...acceptedFiles]);
    }, []);

    const { getRootProps, getInputProps, isDragActive } = useDropzone({
        onDrop,
        accept: {
            'image/jpeg': ['.jpg', '.jpeg'],
            'image/png': ['.png'],
            'image/gif': ['.gif'],
            'image/bmp': ['.bmp'],
            'image/webp': ['.webp']
        },
        maxSize: 20 * 1024 * 1024 // 20 MB
    });

    const removeFile = (index: number) => {
        setFiles(prev => prev.filter((_, i) => i !== index));
    };

    const handleUpload = async () => {
        if (files.length === 0) return;

        setUploading(true);
        setProgress(0);

        try {
            const result = await photosApi.uploadMultiplePhotos(projectId, files, {
                category,
                description: description || undefined
            });
            console.log(result);
            setProgress(100);
            onSuccess();
        } catch (err: any) {
            alert(err.response?.data?.message || 'Ошибка при загрузке');
        } finally {
            setUploading(false);
        }
    };

    return (
        <Modal
            isOpen={true}
            onClose={onClose}
            title="Загрузка фотографий"
            size="lg"
            footer={
                <>
                    <Button variant="secondary" onClick={onClose} disabled={uploading}>Отмена</Button>
                    <Button onClick={handleUpload} loading={uploading} disabled={files.length === 0}>
                        Загрузить ({files.length})
                    </Button>
                </>
            }
        >
            <div className="space-y-4">
                {/* Зона Drag & Drop */}
                <div
                    {...getRootProps()}
                    className={`border-2 border-dashed rounded-lg p-8 text-center cursor-pointer transition-colors ${isDragActive ? 'border-blue-500 bg-blue-50' : 'border-gray-300 hover:border-gray-400'
                        }`}
                >
                    <input {...getInputProps()} />
                    <svg className="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12" />
                    </svg>
                    <p className="mt-2 text-sm text-gray-600">
                        {isDragActive
                            ? 'Отпустите файлы здесь...'
                            : 'Перетащите файлы сюда или нажмите для выбора'}
                    </p>
                    <p className="text-xs text-gray-500 mt-1">
                        JPG, PNG, GIF, BMP, WebP до 20 MB
                    </p>
                </div>

                {/* Настройки */}
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Категория</label>
                        <select
                            value={category}
                            onChange={(e) => setCategory(e.target.value)}
                            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                        >
                            <option value="Before">До начала работ</option>
                            <option value="WorkProgress">Процесс работы</option>
                            <option value="After">После завершения</option>
                            <option value="Defect">Дефекты</option>
                            <option value="Materials">Материалы</option>
                            <option value="Documentation">Документация</option>
                            <option value="Other">Другое</option>
                        </select>
                    </div>
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Описание</label>
                        <input
                            type="text"
                            value={description}
                            onChange={(e) => setDescription(e.target.value)}
                            placeholder="Общее описание для всех фото"
                            className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                        />
                    </div>
                </div>

                {/* Список файлов */}
                {files.length > 0 && (
                    <div className="space-y-2 max-h-60 overflow-y-auto">
                        {files.map((file, index) => (
                            <div key={index} className="flex items-center justify-between bg-gray-50 rounded-lg p-3">
                                <div className="flex items-center space-x-3">
                                    <div className="w-10 h-10 bg-gray-200 rounded flex items-center justify-center">
                                        <svg className="w-5 h-5 text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                                        </svg>
                                    </div>
                                    <div>
                                        <p className="text-sm font-medium text-gray-700 truncate max-w-[300px]">{file.name}</p>
                                        <p className="text-xs text-gray-500">{(file.size / 1024 / 1024).toFixed(2)} MB</p>
                                    </div>
                                </div>
                                <button
                                    onClick={() => removeFile(index)}
                                    className="text-red-500 hover:text-red-700"
                                >
                                    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                                    </svg>
                                </button>
                            </div>
                        ))}
                    </div>
                )}

                {/* Прогресс загрузки */}
                {uploading && (
                    <div className="w-full bg-gray-200 rounded-full h-2">
                        <div
                            className="bg-blue-600 h-2 rounded-full transition-all duration-300"
                            style={{ width: `${progress}%` }}
                        />
                    </div>
                )}
            </div>
        </Modal>
    );
};