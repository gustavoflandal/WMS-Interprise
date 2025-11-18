import httpClient from './httpClient';

export interface Permission {
  id: string;
  name: string;
  resource: string;
  action: string;
  description: string;
  module: string;
  createdAt: string;
}

export interface PermissionsByModule {
  [module: string]: Permission[];
}

class PermissionService {
  private readonly baseURL = '/Permissions';

  async getAll(): Promise<Permission[]> {
    const response = await httpClient.get<Permission[]>(this.baseURL);
    return response.data;
  }

  async getByModule(): Promise<PermissionsByModule> {
    const response = await httpClient.get<PermissionsByModule>(`${this.baseURL}/by-module`);
    return response.data;
  }

  async getById(id: string): Promise<Permission> {
    const response = await httpClient.get<Permission>(`${this.baseURL}/${id}`);
    return response.data;
  }
}

export const permissionService = new PermissionService();
