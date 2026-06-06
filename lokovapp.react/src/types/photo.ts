export interface Photo {
  id: string;
  projectId: string;
  projectNumber: string;
  originalFileName: string;
  contentType: string;
  fileSize: number;
  category: string;
  categoryDisplay: string;
  sortOrder: number;
  description?: string;
  stageId?: string;
  stageName?: string;
  url: string;
  thumbnailUrl: string;
  createdAt: string;
  uploadedByName: string;
  latitude?: number;
  longitude?: number;
  takenAt?: string;
}

export interface PhotoFilter {
  category?: string;
  stageId?: string;
  uploadedFrom?: string;
  uploadedTo?: string;
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortOrder?: string;
}