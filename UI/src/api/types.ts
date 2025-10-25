interface InitiUploadRequest {
    fileName: string;
    totalChunks: number;

}

interface InitiUploadResponse {
    uploadId: string;
    message: string;
}

interface UploadChunkRequest {
    uploadId: string;
    chunkIndex: number;
    chunkData: string; // Base64 encoded string
}

interface UploadChunkResponse {
    message: string;
    uploadId: string;
    chunkIndex: number;
    totalChunks: number;
    isComplete: boolean;
}


interface CompleteUploadRequest {
    uploadId: string;
}

interface CompleteUploadResponse {
    message: string;
    fileName: string;
    totalChunks: number;
    fileUrl: string;
    sizeInMB: number;
    completedAt: string;
}
