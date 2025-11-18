import httpClient from './httpClient';
import { Permission } from './permissionApi';

export interface Role {
  id: string;
  name: string;
  description: string;
  isSystemRole: boolean;
  userCount: number;
  permissions: Permission[];
  createdAt: string;
}

export interface CreateRoleRequest {
  name: string;
  description: string;
}

export interface UpdateRoleRequest {
  name: string;
  description: string;
}

export interface AssignPermissionsRequest {
  permissionIds: string[];
}

export interface AssignUsersToRoleRequest {
  userIds: string[];
}

class RoleService {
  private readonly baseURL = '/Roles';

  async getAll(): Promise<Role[]> {
    const response = await httpClient.get<Role[]>(this.baseURL);
    return response.data;
  }

  async getById(id: string): Promise<Role> {
    const response = await httpClient.get<Role>(`${this.baseURL}/${id}`);
    return response.data;
  }

  async create(data: CreateRoleRequest): Promise<Role> {
    const response = await httpClient.post<Role>(this.baseURL, data);
    return response.data;
  }

  async update(id: string, data: UpdateRoleRequest): Promise<Role> {
    const response = await httpClient.put<Role>(`${this.baseURL}/${id}`, data);
    return response.data;
  }

  async delete(id: string): Promise<void> {
    await httpClient.delete(`${this.baseURL}/${id}`);
  }

  async assignPermissions(id: string, data: AssignPermissionsRequest): Promise<void> {
    await httpClient.post(`${this.baseURL}/${id}/permissions`, data);
  }

  async assignUsers(id: string, data: AssignUsersToRoleRequest): Promise<void> {
    await httpClient.post(`${this.baseURL}/${id}/users`, data);
  }
}

export const roleService = new RoleService();
