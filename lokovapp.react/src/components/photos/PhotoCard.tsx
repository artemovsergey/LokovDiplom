import React from 'react';
import type { Photo } from '../../types/photo';
import { photosApi } from '../../api/photos.api';

interface PhotoCardProps {
  photo: Photo;
  projectId: string;
  isSelected: boolean;
  onSelect: (id: string) => void;
  onDelete: (id: string) => void;
  onView: (photo: Photo) => void;
}

export const PhotoCard: React.FC<PhotoCardProps> = ({
  photo,
  projectId,
  isSelected,
  onSelect,
  onDelete,
  onView
}) => {
  return (
    <div
      className={`relative group bg-white rounded-lg overflow-hidden border-2 transition-colors ${
        isSelected ? 'border-blue-500' : 'border-gray-200 hover:border-gray-300'
      }`}
    >
      {/* Чекбокс выбора */}
      <div className="absolute top-2 left-2 z-10">
        <input
          type="checkbox"
          checked={isSelected}
          onChange={() => onSelect(photo.id)}
          className="w-5 h-5 rounded border-gray-300 text-blue-600 focus:ring-blue-500"
        />
      </div>

      {/* Изображение */}
      <div
        className="aspect-square cursor-pointer overflow-hidden"
        onClick={() => onView(photo)}
      >
        <img
          src={photosApi.getThumbnailUrl(projectId, photo.id)}
          alt={photo.description || photo.originalFileName}
          className="w-full h-full object-cover transition-transform group-hover:scale-105"
          loading="lazy"
        />
      </div>

      {/* Информация */}
      <div className="p-3">
        <p className="text-xs text-gray-500 truncate">{photo.originalFileName}</p>
        <div className="flex items-center justify-between mt-1">
          <span className="inline-flex items-center px-1.5 py-0.5 rounded text-xs font-medium bg-blue-100 text-blue-800">
            {photo.categoryDisplay}
          </span>
          <span className="text-xs text-gray-400">
            {(photo.fileSize / 1024 / 1024).toFixed(1)} MB
          </span>
        </div>
      </div>

      {/* Кнопка удаления (появляется при наведении) */}
      <button
        onClick={(e) => { e.stopPropagation(); onDelete(photo.id); }}
        className="absolute top-2 right-2 p-1.5 bg-red-600 text-white rounded-full opacity-0 group-hover:opacity-100 transition-opacity hover:bg-red-700"
      >
        <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
        </svg>
      </button>
    </div>
  );
};