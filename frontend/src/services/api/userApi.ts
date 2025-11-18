import httpClient from './httpClient';
import {
  UserResponse,
  CreateUserRequest,
  UpdateUserRequest,
} from '../../types/api';

// ============================================================================
// Serviço de Usuários
// ============================================================================

export const userService = {
  getAll: async (): Promise<UserResponse[]> => {
    const response = await httpClient.get('/users');
    return response.data;
  },

  getById: async (id: string): Promise<UserResponse> => {
    const response = await httpClient.get(`/users/${id}`);
    return response.data;
  },

  create: async (request: CreateUserRequest): Promise<UserResponse> => {
    const response = await httpClient.post('/users', request);
    return response.data;
  },

  update: async (id: string, request: UpdateUserRequest): Promise<UserResponse> => {
    const response = await httpClient.put(`/users/${id}`, request);
    return response.data;
  },

  delete: async (id: string): Promise<void> => {
    await httpClient.delete(`/users/${id}`);
  },

  restore: async (id: string): Promise<UserResponse> => {
    const response = await httpClient.patch(`/users/${id}/restore`);
    return response.data;
  },

  getDeleted: async (): Promise<UserResponse[]> => {
    const response = await httpClient.get('/users/deleted');
    return response.data;
  },

  assignRoles: async (userId: string, roleIds: string[]): Promise<void> => {
    await httpClient.post(`/users/${userId}/roles`, { roleIds });
  },

  getUserRoles: async (userId: string): Promise<any[]> => {
    const response = await httpClient.get(`/users/${userId}/roles`);
    return response.data;
  },

  assignRole: async (userId: string, roleId: string): Promise<void> => {
    await httpClient.post(`/users/${userId}/roles/${roleId}`);
  },

  removeRole: async (userId: string, roleId: string): Promise<void> => {
    await httpClient.delete(`/users/${userId}/roles/${roleId}`);
  },
};
