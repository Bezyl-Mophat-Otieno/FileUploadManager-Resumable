import axios from "axios"
import type { ApiResponse, CompleteUploadRequest, CompleteUploadResponse, InitiUploadRequest, InitiUploadResponse, UploadChunkRequest, UploadChunkResponse } from "./types";
const axiosInstance = axios.create({
    baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5022',
});

const uploadInit = async (payload: InitiUploadRequest) => {
    const response = await  axiosInstance.post<ApiResponse<InitiUploadResponse>>('/api/FileUpload/upload/initiate', payload);
    return response.data;
}

const uploadChunk = async (paylod: UploadChunkRequest)=> {
    const formData = new FormData();
    formData.append('chunk', paylod.chunk);
    const response =  await axiosInstance.post<ApiResponse<UploadChunkResponse>>(`/api/FileUpload/upload/chunk?chunkIndex=${paylod.chunkIndex}&uploadId=${paylod.uploadId}`,
        formData, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    });
    return response.data;
}

const completeUpload = async (payload: CompleteUploadRequest)=> {
    const response =  await axiosInstance.post<ApiResponse<CompleteUploadResponse>>(`/api/FileUpload/upload/complete?uploadId=${payload.uploadId}`);
    return response.data;
}

export {
    uploadInit,
    uploadChunk,
    completeUpload
}