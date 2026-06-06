import React from 'react';

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
    label?: string;
    error?: string;
    helperText?: string;
    icon?: React.ReactNode;
}

export const Input: React.FC<InputProps> = ({
    label,
    error,
    helperText,
    icon,
    className = '',
    ...props
}) => {
    return (
        <div className="w-full">
            {label && (
                <label className="block text-sm font-medium text-gray-700 mb-1">
                    {label}
                </label>
            )}
            <div className="relative">
                {icon && (
                    <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none text-gray-400">
                        {icon}
                    </div>
                )}
                <input
                    className={`w-full px-3 py-2 border rounded-lg transition-colors focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 ${icon ? 'pl-10' : ''
                        } ${error ? 'border-red-300 bg-red-50' : 'border-gray-300'
                        } ${className}`}
                    {...props}
                />
            </div>
            {error && <p className="text-red-500 text-xs mt-1">{error}</p>}
            {helperText && !error && <p className="text-gray-500 text-xs mt-1">{helperText}</p>}
        </div>
    );
};