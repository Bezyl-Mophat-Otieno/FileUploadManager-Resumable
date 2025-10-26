import axios from "axios"
import { ApiResponse, CompleteUploadRequest, CompleteUploadResponse, InitiUploadRequest, InitiUploadResponse, UploadChunkRequest, UploadChunkResponse } from "./types";

const uploadInit = async (payload: InitiUploadRequest) => {
    const response = await  axios.post<ApiResponse<InitiUploadResponse>>('/api/upload/initiate', payload);
    return response.data;
}

const uploadChunk = async (paylod: UploadChunkRequest)=> {
    const response =  await axios.post<ApiResponse<UploadChunkResponse>>('/api/upload/chunk', paylod);
    return response.data;
}

const completeUpload = async (payload: CompleteUploadRequest)=> {
    const response =  await axios.post<ApiResponse<CompleteUploadResponse>>('/api/upload/complete', payload);
    return response.data;
}

export {
    uploadInit,
    uploadChunk,
    completeUpload
}