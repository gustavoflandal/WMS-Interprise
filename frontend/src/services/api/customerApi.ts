import httpClient from './httpClient';

// ============================================================================
// Types
// ============================================================================

export interface Customer {
  id: string;
  nome: string;
  tipo: string; // "PJ" ou "PF"
  numeroDocumento: string | null;
  email: string | null;
  telefone: string | null;
  status: number; // 1=Ativo, 2=Inativo, 3=Bloqueado
  statusDescricao: string;
  createdAt: string;
  updatedAt: string | null;
}

export interface CreateCustomerRequest {
  nome: string;
  tipo: string; // "PJ" ou "PF"
  numeroDocumento?: string;
  email?: string;
  telefone?: string;
}

export interface UpdateCustomerRequest {
  nome: string;
  tipo: string; // "PJ" ou "PF"
  numeroDocumento?: string;
  email?: string;
  telefone?: string;
  status: number; // 1=Ativo, 2=Inativo, 3=Bloqueado
}

// ============================================================================
// Serviço de Cliente
// ============================================================================

export const customerService = {
  /**
   * Buscar todos os clientes do tenant
   */
  getAll: async (): Promise<Customer[]> => {
    const response = await httpClient.get('/customer');
    return response.data;
  },

  /**
   * Buscar cliente por ID
   */
  getById: async (id: string): Promise<Customer> => {
    const response = await httpClient.get(`/customer/${id}`);
    return response.data;
  },

  /**
   * Criar novo cliente
   */
  create: async (request: CreateCustomerRequest): Promise<Customer> => {
    const response = await httpClient.post('/customer', request);
    return response.data;
  },

  /**
   * Atualizar cliente existente
   */
  update: async (id: string, request: UpdateCustomerRequest): Promise<Customer> => {
    const response = await httpClient.put(`/customer/${id}`, request);
    return response.data;
  },

  /**
   * Deletar cliente (soft delete)
   */
  delete: async (id: string): Promise<void> => {
    await httpClient.delete(`/customer/${id}`);
  },
};
