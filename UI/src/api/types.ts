export interface InitiUploadRequest {
    fileName: string;
    totalChunks: number;

}
export interface ApiResponse<T = any> {
    success: boolean;
    message: string;
    data?: T;
}

export interface InitiUploadResponse {
    uploadId: string;
    message: string;
}

export interface UploadChunkRequest {
    uploadId: string;
    chunkIndex: number;
    chunk: File
}

export interface UploadChunkResponse {
    message: string;
    uploadId: string;
    chunkIndex: number;
    uploadedChunks: number;
    totalChunks: number;
    isComplete: boolean;
}

export interface CompleteUploadRequest {
    uploadId: string;
}

export interface CompleteUploadResponse {
    message: string;
    fileName: string;
    totalChunks: number;
    fileUrl: string;
    sizeInMB: number;
    completedAt: string;
}